using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Windows.ApplicationModel.Background;
using Taurit.Toolkit.ProcesTodoistInbox.Common.Models;
using Taurit.Toolkit.TodoistInboxHelper;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace Taurit.Toolkit.ProcessTodoistInboxBackground
{
    public sealed class StartupTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            while (true)
            {
                Console.WriteLine("yo world");
                //TryClassifyAllTasks();
                Thread.Sleep(1000);
            }
        }

        private void TryClassifyAllTasks()
        {
            //var _todoistQueryService = new TodoistQueryService("todo");
            //IReadOnlyList<Project> allProjects = _todoistQueryService.GetAllProjects();
            //IReadOnlyList<Label> allLabels = _todoistQueryService.GetAllLabels();
            //IReadOnlyList<TodoTask> tasksThatNeedReview = GetNotReviewedTasks(allProjects.ToLookup(x => x.id));

            //var taskClassifier = new TaskClassifier(UserSettings.ClassificationRules, allLabels, allProjects);
            //(IReadOnlyList<TaskActionModel> actions, IReadOnlyList<TaskNoActionModel> noActions) =
            //    taskClassifier.Classify(tasksThatNeedReview);

            //foreach (TaskActionModel action in actions.OrderByDescending(x => x.Priority))
            //    PlannedActions.Add(action);

            //foreach (TaskNoActionModel noAction in noActions)
            //    SkippedTasks.Add(noAction);
        }
    }
}