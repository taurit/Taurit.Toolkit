using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;
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
        private ChangeExecutor _changeExecutor;
        private FilteredTaskAccessor _filteredTaskAccessor;
        private TodoistCommandService _todoistCommandService;
        private TodoistQueryService _todoistQueryService;


        public void Run(IBackgroundTaskInstance taskInstance)
        {
            SettingsFileModel settings = LoadSettings().Result;
            InitializeDependencies(settings);

            while (true)
            {
                if (!IsCurrentHourSleepHour()) // no need to query API too much, eg. at night
                    TryClassifyAllTasks(settings);

                Thread.Sleep(TimeSpan.FromHours(1));
            }
        }

        private static Boolean IsCurrentHourSleepHour()
        {
            Int32 currentHour = DateTime.Now.Hour;
            return currentHour > 22 || currentHour < 7;
        }

        private void InitializeDependencies(SettingsFileModel settings)
        {
            _todoistQueryService = new TodoistQueryService(settings.TodoistApiKey);
            _todoistCommandService = new TodoistCommandService(settings.TodoistApiKey);
            _filteredTaskAccessor = new FilteredTaskAccessor(_todoistQueryService);
            _changeExecutor = new ChangeExecutor(_todoistCommandService);
        }

        private async Task<SettingsFileModel> LoadSettings()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile sampleFile = await localFolder.GetFileAsync("ProcessTodoistInboxSettings.json");
            String settingsFileContents = await FileIO.ReadTextAsync(sampleFile);
            var settingsDeserialized = JsonConvert.DeserializeObject<SettingsFileModel>(settingsFileContents);
            return settingsDeserialized;
        }

        private void TryClassifyAllTasks(SettingsFileModel settings)
        {
            var plannedActions = new List<TaskActionModel>();
            IReadOnlyList<Project> allProjects = _todoistQueryService.GetAllProjects();
            IReadOnlyList<Label> allLabels = _todoistQueryService.GetAllLabels();
            IReadOnlyList<TodoTask> tasksThatNeedReview =
                _filteredTaskAccessor.GetNotReviewedTasks(allProjects.ToLookup(x => x.id));

            var taskClassifier = new TaskClassifier(
                settings.ClassificationRules,
                settings.ClassificationRulesConcise,
                allLabels,
                allProjects
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