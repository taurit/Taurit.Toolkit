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
        private TodoistQueryService _todoistQueryService;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            SettingsFileModel settings = LoadSettings().Result;
            this._todoistQueryService = new TodoistQueryService(settings.TodoistApiKey);

            while (true)
            {
                TryClassifyAllTasks(settings);
                Thread.Sleep(TimeSpan.FromHours(1));
            }
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
            IReadOnlyList<TodoTask> tasksThatNeedReview = GetNotReviewedTasks(allProjects.ToLookup(x => x.id));

            var taskClassifier = new TaskClassifier(settings.ClassificationRules, allLabels, allProjects);
            (IReadOnlyList<TaskActionModel> actions, IReadOnlyList<TaskNoActionModel> noActions) =
                taskClassifier.Classify(tasksThatNeedReview);

            foreach (TaskActionModel action in actions.OrderByDescending(x => x.Priority))
                plannedActions.Add(action);

            // todo move "onbuttonclick" logic
        }

        private IReadOnlyList<TodoTask> GetNotReviewedTasks(ILookup<Int64, Project> allProjectsIndexedById)
        {
            // todo: this is duplicated in MainWindow.cs, move to a shared location in commons
            IReadOnlyList<TodoTask> allTasks = _todoistQueryService.GetAllTasks(allProjectsIndexedById);
            List<TodoTask> tasksThatNeedProcessing = allTasks
                .Where(x => x.@checked == 0 &&
                            x.is_deleted == 0 &&
                            x.labels != null).ToList();
            return tasksThatNeedProcessing;
        }
    }
}