using System;
using System.Collections.Generic;
using Taurit.Toolkit.ProcesTodoistInbox.Common.Models;
using Taurit.Toolkit.TodoistInboxHelper;

namespace Taurit.Toolkit.ProcesTodoistInbox.Common.Services
{
    public sealed class ChangeExecutor
    {
        private readonly ITodoistCommandService _todoistCommandService;

        public ChangeExecutor(ITodoistCommandService todoistCommandService)
        {
            _todoistCommandService = todoistCommandService;
        }

        public void ApplyPlan(ICollection<TaskActionModel> plannedActions)
        {
            foreach (TaskActionModel action in plannedActions)
            {
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
        }
    }
}