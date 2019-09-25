using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using NaturalLanguageTimespanParser;
using Newtonsoft.Json;
using Taurit.Toolkit.ProcessTodoistInbox.Common.Models;
using Taurit.Toolkit.ProcessTodoistInbox.Common.Services;
using Taurit.Toolkit.TodoistInboxHelper;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace Taurit.Toolkit.ProcessTodoistInboxBackground
{
    public sealed class StartupTask
    {
        [NotNull] private readonly ChangeExecutor _changeExecutor;
        [NotNull] private readonly FilteredTaskAccessor _filteredTaskAccessor;
        [NotNull] private readonly BacklogSnapshotCreator _backlogSnapshotCreator;
        [NotNull] private readonly SettingsFileModel _settings;

        [NotNull] private readonly TodoistQueryService _todoistQueryService;

        [NotNull] private readonly MultiCultureTimespanParser _mctp = new MultiCultureTimespanParser(new[]
        {
            new CultureInfo("pl"),
            new CultureInfo("en")
        });

        private static readonly DateTime StartDateTime = DateTime.UtcNow;

        public StartupTask(SettingsFileModel settings)
        {
            _settings = settings;
            if (_settings == null)
                throw new InvalidOperationException("Settings could not be loaded");

            _todoistQueryService = new TodoistQueryService(_settings.TodoistApiKey);
            var todoistCommandService = new TodoistCommandService(_settings.TodoistApiKey);
            _filteredTaskAccessor = new FilteredTaskAccessor();
            _backlogSnapshotCreator = new BacklogSnapshotCreator();
            _changeExecutor = new ChangeExecutor(todoistCommandService);

            // needed to avoid "'Cyrillic' is not a supported encoding name." error later in code where a trick is used to compare string in an accent-insensitive way 
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            TelemetryConfiguration.Active.InstrumentationKey = _settings.ApplicationInsightsKey;
        }

        public void Run(String outputDirectory)
        {
            String telemetrySessionId = $"{StartDateTime:yyyy-MM-dd-HH-mm-ss.fffffffzzz}";

            var telemetryClient = new TelemetryClient();
            telemetryClient.Context.User.AuthenticatedUserId = "buli";
            telemetryClient.Context.User.UserAgent = "Taurit.Toolkit.ProcessTodoistInbox.Raspberry";
            telemetryClient.Context.Session.Id = telemetrySessionId;

            TrackAndWriteToConsole(telemetryClient, $"Starting the application. Session id = {telemetrySessionId}");
            WriteToConsole($"There are {_settings.ClassificationRulesConcise.Count} rules defined in settings");
            WriteToConsole($"Projects treated as inboxes: {String.Join(", ", _settings.AlternativeInboxes.Select(x => $"'{x}'"))}");
            WriteToConsole($"API url used: {TodoistApiService.ApiUrl}");

            while (true)
                try
                {
                    if (!IsCurrentHourSleepHour()) // no need to query API too much, eg. at night
                    {
                        TrackAndWriteToConsole(telemetryClient, "Starting the classification");
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        
                        TryClassifyAllTasks(_settings, telemetryClient, outputDirectory);
                        
                        stopwatch.Stop();
                        TrackAndWriteToConsole(telemetryClient, "TotalClassificationTimeMs", stopwatch.ElapsedMilliseconds);
                    }
                    else
                    {
                        TrackAndWriteToConsole(telemetryClient,
                            $"Skipping the classification due to night hours (it is {DateTime.Now})");
                    }

                    Thread.Sleep(TimeSpan.FromHours(value: 1));
                }
                catch (Exception e)
                {
                    TrackAndWriteToConsole(telemetryClient, e);
                    Thread.Sleep(TimeSpan.FromHours(value: 1));
                }

            // ReSharper disable once FunctionNeverReturns
        }

        private static Boolean IsCurrentHourSleepHour()
        {
            Int32 currentHour = DateTime.UtcNow.Hour;
            return currentHour > 20 || currentHour < 5; // 20:00-5:00 utc == 22:00-7:00 CEST (good enough for winter as well)
        }

        private void TryClassifyAllTasks([NotNull] SettingsFileModel settings,
            [NotNull] TelemetryClient telemetryClient, [NotNull] String outputDirectory)
        {
            if (outputDirectory == null)
                throw new ArgumentNullException(nameof(outputDirectory));
            var plannedActions = new List<TaskActionModel>();

            Stopwatch dataRetrievalStopwatch = Stopwatch.StartNew();
            IReadOnlyList<Project> allProjects = _todoistQueryService.GetAllProjects();
            IReadOnlyList<Label> allLabels = _todoistQueryService.GetAllLabels();
            IReadOnlyList<TodoTask> allTasks = _todoistQueryService.GetAllTasks(
                allProjects.ToLookup(x => x.id),
                allLabels.ToLookup(x => x.id)
                );
            dataRetrievalStopwatch.Stop();
            WriteToConsole($"Retrieved {allProjects.Count} projects, {allLabels.Count} labels and {allTasks.Count} tasks");
            TrackAndWriteToConsole(telemetryClient, "TotalDataRetrievalTimeMs", dataRetrievalStopwatch.ElapsedMilliseconds);

            // analysis: save for the future use

            var snapshotsFolder = Path.Combine(outputDirectory, _settings.SnapshotsFolder);
            Stopwatch snapshotCreatorStopwatch = Stopwatch.StartNew();
            var snapshotOutputFolder = _backlogSnapshotCreator.CreateSnapshot(snapshotsFolder, DateTime.UtcNow, allTasks, allProjects, allLabels);
            snapshotCreatorStopwatch.Stop();
            WriteToConsole($"Snapshot created in the folder '{snapshotOutputFolder}'");
            TrackAndWriteToConsole(telemetryClient, "TotalSnapshotCreationTimeMs", snapshotCreatorStopwatch.ElapsedMilliseconds);

            IReadOnlyList<TodoTask> tasksThatNeedReview =
                _filteredTaskAccessor.GetNotReviewedTasks(allTasks);
            
            WriteToConsole($"There are {tasksThatNeedReview.Count} tasks that need review");
            WriteToConsole($"Performing the classification...");
            var taskClassifier = new TaskClassifier(
                settings.ClassificationRulesConcise,
                allLabels,
                allProjects,
                settings.AlternativeInboxes
            );
            (IReadOnlyList<TaskActionModel> actions, IReadOnlyList<TaskNoActionModel> noActions) =
                taskClassifier.Classify(tasksThatNeedReview);

            foreach (TaskActionModel action in actions.OrderByDescending(x => x.Priority))
                plannedActions.Add(action);

            // Apply actions
            List<String> logs = _changeExecutor.ApplyPlan(plannedActions);
            foreach (String log in logs)
            {
                WriteToConsole(log);
            }
        }

        private void WriteToConsole(string message)
        {
            Console.WriteLine($"{DateTime.Now} {message}");
        }

        private void TrackAndWriteToConsole(TelemetryClient telemetryClient, string message)
        {
            telemetryClient.TrackTrace(message);
            Console.WriteLine($"{DateTime.Now} {message}");
        }

        private void TrackAndWriteToConsole(TelemetryClient telemetryClient, string key, long metric)
        {
            telemetryClient.TrackMetric(key, metric);
            Console.WriteLine($"{DateTime.Now} {key}={metric}");
        }

        private void TrackAndWriteToConsole(TelemetryClient telemetryClient, Exception e)
        {
            telemetryClient.TrackException(e);
            Console.WriteLine($"{DateTime.Now} {e}");
        }
    }
}