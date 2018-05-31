using System;
using Taurit.Toolkit.TodoistInboxHelper;

namespace Taurit.Toolkit.ProcessTodoistInbox.Services
{
    public class TodoistFakeCommandService : ITodoistCommandService
    {
        /// <inheritdoc />
        public void AddUpdateTaskCommand(Int64 taskId, Int32 priority, Int64 labelId, Int64 project)
        {
        }

        /// <inheritdoc />
        public String ExecuteCommands()
        {
            return "well done!";
        }
    }
}