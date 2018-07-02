using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Newtonsoft.Json;
using Taurit.Toolkit.ProcessTodoistInbox.Models;
using Taurit.Toolkit.ProcesTodoistInbox.Common.Models;
using Taurit.Toolkit.ProcesTodoistInbox.Common.Services;
using Taurit.Toolkit.TodoistInboxHelper;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace Taurit.Toolkit.ProcessTodoistInboxBackground
{
    public sealed class StartupTask : IBackgroundTask
    {
        private FilteredTaskAccessor _filteredTaskAccessor;
        private TodoistQueryService _todoistQueryService;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            SettingsFileModel settings = LoadSettings().Result;
            InitializeDependencies(settings);

            while (true)
            {
                TryClassifyAllTasks(settings);
                Thread.Sleep(TimeSpan.FromHours(1));
            }
        }

        private void InitializeDependencies(SettingsFileModel settings)
        {
            _todoistQueryService = new TodoistQueryService(settings.TodoistApiKey);
            _filteredTaskAccessor = new FilteredTaskAccessor(_todoistQueryService);
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
            IReadOnlyList<TodoTask> tasksThatNeedReview = _filteredTaskAccessor.GetNotReviewedTasks(allProjects.ToLookup(x => x.id));

            var taskClassifier = new TaskClassifier(settings.ClassificationRules, allLabels, allProjects);
            (IReadOnlyList<TaskActionModel> actions, IReadOnlyList<TaskNoActionModel> noActions) =
                taskClassifier.Classify(tasksThatNeedReview);

            foreach (TaskActionModel action in actions.OrderByDescending(x => x.Priority))
                plannedActions.Add(action);

            // todo move "onbuttonclick" logic
        }
    }
}