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
            var logs = new List<String>(plannedActions.Count + 1);
            foreach (TaskActionModel action in plannedActions)
            {
                String taskName = string.IsNullOrEmpty(action.NewName) ? action.Name : action.NewName;
                logs.Add(
                    $"Scheduling action for task #{action.TaskId} '{taskName}': set @{action.Label}, #{action.Project}, p{action.Priority}");

                Int64 taskId = action.TaskId;
                Int32? newPriority = action.Priority;
                Int64? newLabelId = action.Label?.id;
                Int64? newProjectId = action.Project?.id;
                String newName = action.NewName;

                _todoistCommandService.AddUpdateProjectCommand(taskId, newProjectId);
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