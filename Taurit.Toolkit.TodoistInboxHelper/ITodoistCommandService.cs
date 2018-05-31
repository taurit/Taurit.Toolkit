using System;

namespace Taurit.Toolkit.TodoistInboxHelper
{
    public interface ITodoistCommandService
    {
        void AddUpdateTaskCommand(Int64 oldProjectId, Int64 taskId, Int32 priority, Int64 labelId, Int64 project);
        String ExecuteCommands();
    }
}