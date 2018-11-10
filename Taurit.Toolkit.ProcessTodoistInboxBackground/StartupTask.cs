using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using JetBrains.Annotations;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using NaturalLanguageTimespanParser;
using Newtonsoft.Json;
using Taurit.Toolkit.ProcessTodoistInbox.Common.Services;
using Taurit.Toolkit.ProcesTodoistInbox.Common.Models;
using Taurit.Toolkit.ProcesTodoistInbox.Common.Services;
using Taurit.Toolkit.TodoistInboxHelper;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace Taurit.Toolkit.ProcessTodoistInboxBackground
{
    public sealed class StartupTask : IBackgroundTask
    {
        [NotNull] private readonly ChangeExecutor _changeExecutor;
        [NotNull] private readonly FilteredTaskAccessor _filteredTaskAccessor;
        [NotNull] private readonly SettingsFileModel _settings;

        [NotNull] private readonly TodoistQueryService _todoistQueryService;

        [NotNull] private readonly MultiCultureTimespanParser _mctp = new MultiCultureTimespanParser(new[]
        {
            new CultureInfo("pl"),
            new CultureInfo("en")
        });

        public StartupTask()
        {
            _settings = LoadSettings().Result;
            if (_settings == null)
                throw new InvalidOperationException("Settings could not be loaded");

            _todoistQueryService = new TodoistQueryService(_settings.TodoistApiKey);
            var todoistCommandService = new TodoistCommandService(_settings.TodoistApiKey);
            _filteredTaskAccessor = new FilteredTaskAccessor();
            _changeExecutor = new ChangeExecutor(todoistCommandService);

            // needed to avoid "'Cyrillic' is not a supported encoding name." error later in code where a trick is used to compare string in an accent-insensitive way 
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            TelemetryConfiguration.Active.InstrumentationKey = _settings.ApplicationInsightsKey;
        }

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            var telemetryClient = new TelemetryClient();

            while (true)
                try
                {
                    if (!IsCurrentHourSleepHour()) // no need to query API too much, eg. at night
                    {
                        telemetryClient.TrackTrace("Starting the classification");
                        TryClassifyAllTasks(_settings, telemetryClient);
                    }
                    else
                    {
                        telemetryClient.TrackTrace(
                            $"Skipping the classification due to night hours (it is {DateTime.Now})");
                    }

                    Thread.Sleep(TimeSpan.FromHours(value: 1));
                }
                catch (Exception e)
                {
                    telemetryClient.TrackException(e);
                    Thread.Sleep(TimeSpan.FromHours(value: 1));
                }

            // ReSharper disable once FunctionNeverReturns
        }

        private static Boolean IsCurrentHourSleepHour()
        {
            Int32 currentHour = DateTime.Now.Hour;
            return currentHour > 22 || currentHour < 7;
        }


        private async Task<SettingsFileModel> LoadSettings()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile sampleFile = await localFolder.GetFileAsync("ProcessTodoistInboxSettings.json");
            String settingsFileContents = await FileIO.ReadTextAsync(sampleFile);
            var settingsDeserialized = JsonConvert.DeserializeObject<SettingsFileModel>(settingsFileContents);
            return settingsDeserialized;
        }

        private void TryClassifyAllTasks([NotNull] SettingsFileModel settings,
            [NotNull] TelemetryClient telemetryClient)
        {
            var plannedActions = new List<TaskActionModel>();
            IReadOnlyList<Project> allProjects = _todoistQueryService.GetAllProjects();
            IReadOnlyList<Label> allLabels = _todoistQueryService.GetAllLabels();
            IReadOnlyList<TodoTask> allTasks = _todoistQueryService.GetAllTasks(allProjects.ToLookup(x => x.id));

            // analysis: save for the future use
            var snapshotsFolder = Path.Combine(ApplicationData.Current.LocalFolder.Path, _settings.SnapshotsFolder);
            BacklogSnapshotCreator.CreateSnapshot(snapshotsFolder, DateTime.UtcNow, allTasks, allProjects, allLabels);

            // legacy metric: all tasks, categories combined
            telemetryClient.TrackMetric("NumberOfTasks", allTasks.Count);
            telemetryClient.TrackMetric("NumberOfHighPriorityTasks",
                allTasks.Count(x => x.priority == 4));
            telemetryClient.TrackMetric("NumberOfHighAndMediumPriorityTasks",
                allTasks.Count(x => x.priority == 4 || x.priority == 3));
            telemetryClient.TrackMetric("NumberOfHighMediumAndLowPriorityTasks",
                allTasks.Count(x => x.priority == 4 || x.priority == 3 || x.priority == 2));

            // new metrics: tasks without date => the ones that can be done today and for which Inbox Zero is expected
            telemetryClient.TrackMetric("NumberOfHighPriorityTasksNoDate",
                allTasks.Count(x => !x.HasDate && x.priority == 4));
            telemetryClient.TrackMetric("NumberOfMediumPriorityTasksNoDate",
                allTasks.Count(x => !x.HasDate && x.priority == 3));
            telemetryClient.TrackMetric("NumberOfLowPriorityTasksNoDate",
                allTasks.Count(x => !x.HasDate && x.priority == 2));
            telemetryClient.TrackMetric("NumberOfUndefinedPriorityTasksNoDateFixed",
                allTasks.Count(x => !x.HasDate && x.priority == 1));

            // other metrics
            telemetryClient.TrackMetric("NumberOfLabels", allLabels.Count);
            telemetryClient.TrackMetric("NumberOfProjects", allProjects.Count);
            TrackSprintBurndown(telemetryClient, allTasks, allProjects);
            TrackAllTasksBurndown(telemetryClient, allTasks, allProjects);


            IReadOnlyList<TodoTask> tasksThatNeedReview =
                _filteredTaskAccessor.GetNotReviewedTasks(allTasks);
            telemetryClient.TrackMetric("NumberOfTasksChosenForClassification", allProjects.Count);

            var taskClassifier = new TaskClassifier(
                settings.ClassificationRules,
                settings.ClassificationRulesConcise,
                allLabels,
                allProjects
            );
            (IReadOnlyList<TaskActionModel> actions, IReadOnlyList<TaskNoActionModel> noActions) =
                taskClassifier.Classify(tasksThatNeedReview);
            telemetryClient.TrackMetric("NumberOfActions", actions.Count);
            telemetryClient.TrackMetric("NumberOfSkippedTasks", noActions.Count);

            foreach (TaskActionModel action in actions.OrderByDescending(x => x.Priority))
                plannedActions.Add(action);


            // Apply actions
            _changeExecutor.ApplyPlan(plannedActions);
        }

        private void TrackAllTasksBurndown(
            [NotNull] TelemetryClient telemetryClient,
            [NotNull] IReadOnlyList<TodoTask> allTasks,
            [NotNull] IReadOnlyList<Project> allProjects)
        {
            if (telemetryClient == null) throw new ArgumentNullException(nameof(telemetryClient));
            if (allTasks == null) throw new ArgumentNullException(nameof(allTasks));
            if (allProjects == null) throw new ArgumentNullException(nameof(allProjects));

            var totalTimeInMinutes = 0;
            var totalTimeInMinutesHigh = 0;
            var totalTimeInMinutesMedium = 0;
            var totalTimeInMinutesLow = 0;
            var totalTimeInMinutesUndefined = 0;
            
            foreach (TodoTask task in allTasks.Where(x => x.is_archived == 0 && x.is_deleted == 0 && !x.HasDate))
            {
                var timespanParseResult = _mctp.Parse(task.content);
                Int32 taskTimeInMinutes = timespanParseResult.Success ? (Int32)timespanParseResult.Duration.TotalMinutes : 0;

                totalTimeInMinutes += taskTimeInMinutes;
                totalTimeInMinutesHigh += task.priority == 4 ? taskTimeInMinutes : 0;
                totalTimeInMinutesMedium += task.priority == 3 ? taskTimeInMinutes : 0;
                totalTimeInMinutesLow += task.priority == 2 ? taskTimeInMinutes : 0;
                totalTimeInMinutesUndefined += task.priority == 1 ? taskTimeInMinutes : 0;
            }

            Double totalTimeInHours = totalTimeInMinutes / 60d;
            Double totalTimeInHoursHigh = totalTimeInMinutesHigh / 60d;
            Double totalTimeInHoursMedium = totalTimeInMinutesMedium / 60d;
            Double totalTimeInHoursLow = totalTimeInMinutesLow / 60d;
            Double totalTimeInHoursUndefined = totalTimeInMinutesUndefined / 60d;

            telemetryClient.TrackMetric("WorkLeftInMinutes", totalTimeInMinutes);
            telemetryClient.TrackMetric("WorkLeftInHours", totalTimeInHours);

            telemetryClient.TrackMetric("WorkLeftInHoursHigh", totalTimeInHoursHigh);
            telemetryClient.TrackMetric("WorkLeftInHoursMedium", totalTimeInHoursMedium);
            telemetryClient.TrackMetric("WorkLeftInHoursLow", totalTimeInHoursLow);
            telemetryClient.TrackMetric("WorkLeftInHoursUndefined", totalTimeInHoursUndefined);
        }

        private void TrackSprintBurndown([NotNull] TelemetryClient telemetryClient,
            [NotNull] IReadOnlyList<TodoTask> allTasks,
            [NotNull] IReadOnlyList<Project> allProjects)
        {
            if (telemetryClient == null) throw new ArgumentNullException(nameof(telemetryClient));
            if (allTasks == null) throw new ArgumentNullException(nameof(allTasks));
            if (allProjects == null) throw new ArgumentNullException(nameof(allProjects));

            // convention: sprint projects are named like "Sprint yyyy-MM-dd (x h)"
            // where "yyyy-MM-dd" is the sprint's end date
            // and "x" is total time available in the sprint
            Project currentSprintProject = allProjects
                .Where(x => x.name.StartsWith("Sprint", StringComparison.InvariantCultureIgnoreCase))
                .Where(x => x.is_archived == 0)
                .Where(x => x.is_deleted == 0)
                .OrderBy(x => x.name)
                .FirstOrDefault();

            if (currentSprintProject == null) return; // if I give up on this idea, this program should keep working

            List<TodoTask> allTasksInSprint =
                allTasks.Where(x => x.project_name == currentSprintProject.name).ToList();
            var totalTimeInMinutes = 0;
            foreach (TodoTask task in allTasksInSprint.Where(x => x.is_archived == 0 && x.is_deleted == 0))
            {
                var timespanParseResult = _mctp.Parse(task.content);
                Int32 taskTimeInMinutes = timespanParseResult.Success ? (Int32)timespanParseResult.Duration.TotalMinutes : 0;
                totalTimeInMinutes += taskTimeInMinutes;
            }

            Double totalTimeInHours = totalTimeInMinutes / 60d;
            telemetryClient.TrackMetric("WorkLeftInTheCurrentSprintInMinutes", totalTimeInMinutes);
            telemetryClient.TrackMetric("WorkLeftInTheCurrentSprintInHours", totalTimeInHours);
        }
    }
}