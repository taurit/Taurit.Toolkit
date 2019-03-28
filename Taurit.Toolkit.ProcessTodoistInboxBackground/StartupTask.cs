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
using Taurit.Toolkit.ProcessTodoistInbox.Common.Models;
using Taurit.Toolkit.ProcessTodoistInbox.Common.Services;
using Taurit.Toolkit.TodoistInboxHelper;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace Taurit.Toolkit.ProcessTodoistInboxBackground
{
    public sealed class StartupTask : IBackgroundTask
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

        public StartupTask()
        {
            _settings = LoadSettings().Result;
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
            return currentHour > 23 || currentHour < 7;
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
            _backlogSnapshotCreator.CreateSnapshot(snapshotsFolder, DateTime.UtcNow, allTasks, allProjects, allLabels);

            IReadOnlyList<TodoTask> tasksThatNeedReview =
                _filteredTaskAccessor.GetNotReviewedTasks(allTasks);

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
            _changeExecutor.ApplyPlan(plannedActions);
        }
    }
}