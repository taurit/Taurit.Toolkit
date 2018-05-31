using System;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

namespace Taurit.Toolkit.ProcessTodoistInbox.Models
{
    public class TaskNoActionModel
    {
        public TaskNoActionModel(TodoTask task)
        {
            Name = task.content;
        }

        public TaskNoActionModel()
        {
        }

        public String Name { get; set; }
    }
}