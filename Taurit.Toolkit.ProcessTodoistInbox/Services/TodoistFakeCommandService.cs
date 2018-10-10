﻿using System;
using Taurit.Toolkit.TodoistInboxHelper;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

namespace Taurit.Toolkit.ProcessTodoistInbox.Services
{
    public class TodoistFakeCommandService : ITodoistCommandService
    {
        /// <inheritdoc />
        public String ExecuteCommands()
        {
            return "well done!";
        }

        /// <inheritdoc />
        public void AddUpdateProjectCommand(Int64 taskId, Int64 oldProjectId, Int64? newProjectId)
        {
        }

        /// <inheritdoc />
        public void AddUpdateLabelCommand(Int64 taskId, Int64? newLabelId)
        {
        }

        /// <inheritdoc />
        public void AddUpdatePriorityCommand(Int64 taskId, Int32? newPriority)
        {
        }
        
        /// <inheritdoc />
        public void AddUpdateTextCommand(Int64 taskId, String newName)
        {
            
        }
    }
}