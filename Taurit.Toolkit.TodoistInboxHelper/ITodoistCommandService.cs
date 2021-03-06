﻿using System;

namespace Taurit.Toolkit.TodoistInboxHelper
{
    public interface ITodoistCommandService
    {
        String ExecuteCommands();
        void AddUpdateProjectCommand(Int64 taskId, Int64? newProjectId);
        void AddUpdateLabelCommand(Int64 taskId, Int64? newLabelId);
        void AddUpdatePriorityCommand(Int64 taskId, Int32? newPriority);
        void AddUpdateTextCommand(Int64 taskId, String newName);
    }
}