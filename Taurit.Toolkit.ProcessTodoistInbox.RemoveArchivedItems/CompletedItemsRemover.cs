using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Taurit.Toolkit.TodoistInboxHelper;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

namespace Taurit.Toolkit.ProcessTodoistInbox.RemoveArchivedItems
{
    internal class CompletedItemsRemover
    {
        private readonly String _apiKey;
        private readonly TodoistQueryService _todoistQueryService;
        private readonly TodoistCommandService _todoistCommandService;

        public CompletedItemsRemover([NotNull]String apiKey)
        {
            _apiKey = apiKey;
            _todoistQueryService = new TodoistQueryService(_apiKey);
            _todoistCommandService = new TodoistCommandService(_apiKey);
        }

        public void RemoveAllCompletedItems()
        {
            // get all completed tasks
            IReadOnlyList<TodoTask> completedTasks = this._todoistQueryService.GetAllCompletedTasks()
                .Take(1).ToList(); // todo remove limit when it's proved to work

            // list them
            foreach (TodoTask todoTask in completedTasks)
            {
                Console.WriteLine("To remove: " + todoTask.content);
            }

            // remove them
            _todoistCommandService.AddRemoveTasksCommands(completedTasks);
            String response = _todoistCommandService.ExecuteCommands();
            Console.WriteLine($"API response: {response}");

            Console.WriteLine("Task removal complete.");

            // this returns ok, but items are not removed from https://api.todoist.com/sync/v8/completed/get_all
            // they also cannot be "uncompleted", which leaves them in a weird "completed but nonexistent" state.
            // I might want to check in the future (maybe API v9 will properly remove completed/archived items?). Or maybe https://api.todoist.com/sync/v8/completed/get_all
            // is based on some cached data which indeed no longer exists in the database after removal.
        }
    }
}