using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using JetBrains.Annotations;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Newtonsoft.Json;
using Taurit.Toolkit.ProcesTodoistInbox.Common.Models;
using Taurit.Toolkit.ProcesTodoistInbox.Common.Services;
using Taurit.Toolkit.TodoistInboxHelper;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace Taurit.Toolkit.ProcessTodoistInboxBackground
{
    public sealed class StartupTask : IBackgroundTask
    {
        [NotNull] private ChangeExecutor _changeExecutor;

        [NotNull] private FilteredTaskAccessor _filteredTaskAccessor;

        [NotNull] private TodoistCommandService _todoistCommandService;

        [NotNull] private TodoistQueryService _todoistQueryService;


        public void Run(IBackgroundTaskInstance taskInstance)
        {
            SettingsFileModel settings = LoadSettings().Result ??
                                         throw new InvalidOperationException("Settings could not be loaded");
            InitializeDependencies(settings);
            var telemetryClient = new TelemetryClient();

            while (true)
            {
                if (!IsCurrentHourSleepHour()) // no need to query API too much, eg. at night
                {
                    telemetryClient.TrackTrace("Starting the classification");
                    TryClassifyAllTasks(settings, telemetryClient);
                }
                else
                    telemetryClient.TrackTrace(
                        $"Skipping the classification due to night hours (it is {DateTime.Now})");

                Thread.Sleep(TimeSpan.FromHours(1));
            }
        }

        private static Boolean IsCurrentHourSleepHour()
        {
            Int32 currentHour = DateTime.Now.Hour;
            return currentHour > 22 || currentHour < 7;
        }

        private void InitializeDependencies([NotNull] SettingsFileModel settings)
        {
            _todoistQueryService = new TodoistQueryService(settings.TodoistApiKey);
            _todoistCommandService = new TodoistCommandService(settings.TodoistApiKey);
            _filteredTaskAccessor = new FilteredTaskAccessor();
            _changeExecutor = new ChangeExecutor(_todoistCommandService);

            // needed to avoid "'Cyrillic' is not a supported encoding name." error later in code where a trick is used to compare string in an accent-insensitive way 
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            TelemetryConfiguration.Active.InstrumentationKey = settings.ApplicationInsightsKey;
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
            telemetryClient.TrackMetric("NumberOfUndefinedPriorityTasksNoDate",
                allTasks.Count(x => !x.HasDate && x.priority == 4));

            // other metrics
            telemetryClient.TrackMetric("NumberOfLabels", allLabels.Count);
            telemetryClient.TrackMetric("NumberOfProjects", allProjects.Count);


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
    }
}