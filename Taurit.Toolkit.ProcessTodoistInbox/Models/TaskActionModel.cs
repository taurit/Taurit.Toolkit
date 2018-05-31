using System;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

namespace Taurit.Toolkit.ProcessTodoistInbox.Models
{
    public class TaskActionModel
    {
        public TaskActionModel()
        {
            
        }
        public TaskActionModel(TodoTask task, String newLabel, Int32 newPriority, String newProject)
        {
            Name = task.content;
            Label = newLabel;
            Project = newProject;
            Priority = newPriority;
        }

        public String Name { get; set; }
        public String Project { get; set; }
        public String Label { get; set; }
        public Int32 Priority { get; set; }
    }
}