using System;
using System.Collections.Generic;
using Taurit.Toolkit.ProcessTodoistInbox.Common.Models;
using Taurit.Toolkit.TodoistInboxHelper;

namespace Taurit.Toolkit.ProcessTodoistInbox.Common.Services
{
    public sealed class ChangeExecutor
    {
        private readonly ITodoistCommandService _todoistCommandService;

        public ChangeExecutor(ITodoistCommandService todoistCommandService)
        {
            _todoistCommandService = todoistCommandService;
        }

        public List<String> ApplyPlan(ICollection<TaskActionModel> plannedActions)
        {
            List<string> logs = new List<String>(plannedActions.Count + 1);
            foreach (TaskActionModel action in plannedActions)
            {
                logs.Add($"Scheduling action for task {action.NewName}: set @{action.Label}, #{action.Project}, p{action.Priority}");
                Int64 taskId = action.TaskId;
                Int32? newPriority = action.Priority;
                Int64? newLabelId = action.Label?.id;
                Int64 oldProjectId = action.OldProjectId;
                Int64? newProjectId = action.Project?.id;
                var newName = action.NewName;

                _todoistCommandService.AddUpdateProjectCommand(taskId, oldProjectId, newProjectId);
                _todoistCommandService.AddUpdateLabelCommand(taskId, newLabelId);
                _todoistCommandService.AddUpdatePriorityCommand(taskId, newPriority);
                _todoistCommandService.AddUpdateTextCommand(taskId, newName);
            }

            // ReSharper disable once UnusedVariable - useful for debugging
            String response = _todoistCommandService.ExecuteCommands();
            logs.Add($"API response: {response}");

            return logs;
        }
    }
}