using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Taurit.Toolkit.ProcessTodoistInbox.Common.Models;
using Taurit.Toolkit.ProcessTodoistInbox.Common.Services;
using Taurit.Toolkit.TodoistInboxHelper;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace Taurit.Toolkit.ProcessTodoistInbox.Raspberry
{
    public sealed class StartupTask
    {
        private static readonly DateTime StartDateTime = DateTime.UtcNow;
        [NotNull] private readonly String _applicationInsightsKey;
        [NotNull] private readonly BacklogSnapshotCreator _backlogSnapshotCreator;
        [NotNull] private readonly ChangeExecutor _changeExecutor;
        [NotNull] private readonly FilteredTaskAccessor _filteredTaskAccessor;
        [NotNull] private readonly SettingsFileModel _settings;

        [NotNull] private readonly TodoistQueryService _todoistQueryService;

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

            _applicationInsightsKey = _settings.ApplicationInsightsKey;
        }

        public void Run(String outputDirectory)
        {
            String telemetrySessionId = $"{StartDateTime:yyyy-MM-dd-HH-mm-ss.fffffffzzz}";

            var telemetryClient = new TelemetryClient(new TelemetryConfiguration(_applicationInsightsKey));

            telemetryClient.Context.User.AuthenticatedUserId = "buli";
            telemetryClient.Context.User.UserAgent = "Taurit.Toolkit.ProcessTodoistInbox.Raspberry";
            telemetryClient.Context.Session.Id = telemetrySessionId;

            TrackAndWriteToConsole(telemetryClient, $"Starting the application. Session id = {telemetrySessionId}");
            WriteToConsole($"There are {_settings.ClassificationRulesConcise.Count} rules defined in settings");
            WriteToConsole(
                $"Projects treated as inboxes: {string.Join(", ", _settings.AlternativeInboxes.Select(x => $"'{x}'"))}");
            WriteToConsole($"API url used: {TodoistApiService.ApiUrl}");

            while (true)
                try
                {
                    if (!IsCurrentHourSleepHour()) // no need to query API too much, eg. at night
                    {
                        TrackAndWriteToConsole(telemetryClient, "Starting the classification");
                        var stopwatch = Stopwatch.StartNew();

                        TryClassifyAllTasks(_settings, telemetryClient, outputDirectory);

                        stopwatch.Stop();
                        TrackAndWriteToConsole(telemetryClient, "TotalClassificationTimeMs",
                            stopwatch.ElapsedMilliseconds);
                    }
                    else
                    {
                        TrackAndWriteToConsole(telemetryClient,
                            $"Skipping the classification due to night hours (it is {DateTime.Now})");
                    }

                    Thread.Sleep(TimeSpan.FromHours(1));
                }
                catch (Exception e)
                {
                    TrackAndWriteToConsole(telemetryClient, e);
                    Thread.Sleep(TimeSpan.FromHours(1));
                }

            // ReSharper disable once FunctionNeverReturns
        }

        private static Boolean IsCurrentHourSleepHour()
        {
            Int32 currentHour = DateTime.UtcNow.Hour;
            return
                currentHour > 20 ||
                currentHour < 5; // 20:00-5:00 utc == 22:00-7:00 CEST (good enough for winter as well)
        }

        private void TryClassifyAllTasks([NotNull] SettingsFileModel settings,
            [NotNull] TelemetryClient telemetryClient, [NotNull] String outputDirectory)
        {
            if (outputDirectory == null)
                throw new ArgumentNullException(nameof(outputDirectory));
            var plannedActions = new List<TaskActionModel>();

            var dataRetrievalStopwatch = Stopwatch.StartNew();
            IReadOnlyList<Project> allProjects = _todoistQueryService.GetAllProjects();
            IReadOnlyList<Label> allLabels = _todoistQueryService.GetAllLabels();
            IReadOnlyList<TodoTask> allTasks = _todoistQueryService.GetAllTasks(
                allProjects.ToLookup(x => x.id),
                allLabels.ToLookup(x => x.id)
            );
            dataRetrievalStopwatch.Stop();
            WriteToConsole(
                $"Retrieved {allProjects.Count} projects, {allLabels.Count} labels and {allTasks.Count} tasks");
            TrackAndWriteToConsole(telemetryClient, "TotalDataRetrievalTimeMs",
                dataRetrievalStopwatch.ElapsedMilliseconds);

            // analysis: save for the future use

            String snapshotsFolder = Path.Combine(outputDirectory, _settings.SnapshotsFolder);
            var snapshotCreatorStopwatch = Stopwatch.StartNew();
            String snapshotOutputFolder = _backlogSnapshotCreator.CreateSnapshot(snapshotsFolder, DateTime.UtcNow,
                allTasks, allProjects, allLabels);
            snapshotCreatorStopwatch.Stop();
            WriteToConsole($"Snapshot created in the folder '{snapshotOutputFolder}'");
            TrackAndWriteToConsole(telemetryClient, "TotalSnapshotCreationTimeMs",
                snapshotCreatorStopwatch.ElapsedMilliseconds);

            IReadOnlyList<TodoTask> tasksThatNeedReview =
                _filteredTaskAccessor.GetNotReviewedTasks(allTasks);

            WriteToConsole($"There are {tasksThatNeedReview.Count} tasks that need review");
            WriteToConsole("Performing the classification...");
            var taskClassifier = new TaskClassifier(
                settings.ClassificationRulesConcise,
                allLabels,
                allProjects,
                settings.AlternativeInboxes
            );
            (IReadOnlyList<TaskActionModel> actions, IReadOnlyList<TaskNoActionModel> _) =
                taskClassifier.Classify(tasksThatNeedReview);

            foreach (TaskActionModel action in actions.OrderByDescending(x => x.Priority))
                plannedActions.Add(action);

            // Apply actions
            List<String> logs = _changeExecutor.ApplyPlan(plannedActions);
            foreach (String log in logs) WriteToConsole(log);
        }

        private static void WriteToConsole(String message)
        {
            Console.WriteLine($"{DateTime.Now} {message}");
        }

        private static void TrackAndWriteToConsole(TelemetryClient telemetryClient, String message)
        {
            telemetryClient.TrackTrace(message);
            Console.WriteLine($"{DateTime.Now} {message}");
        }

        private static void TrackAndWriteToConsole(TelemetryClient telemetryClient, String key, Int64 metric)
        {
            telemetryClient.TrackMetric(key, metric);
            Console.WriteLine($"{DateTime.Now} {key}={metric}");
        }

        private static void TrackAndWriteToConsole(TelemetryClient telemetryClient, Exception e)
        {
            telemetryClient.TrackException(e);
            Console.WriteLine($"{DateTime.Now} {e}");
        }
    }
}