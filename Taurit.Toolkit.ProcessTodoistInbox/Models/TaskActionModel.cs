using System;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

namespace Taurit.Toolkit.ProcessTodoistInbox.Models
{
    public class TaskActionModel
    {
        public TaskActionModel()
        {
        }

        public TaskActionModel(TodoTask task, Label newLabel, Int32 newPriority, Project newProject)
        {
            if (newPriority < 1 || newPriority > 4)
                throw new ArgumentException("Priority value is outsde of range 1 (default) to 4 (high)");

            Name = task.content;
            Label = newLabel;
            Project = newProject;
            Priority = newPriority;
            TaskId = task.id;
        }

        public String Name { get; set; }
        public Project Project { get; set; }
        public Label Label { get; set; }
        public Int32 Priority { get; set; }
        public Int64 TaskId { get; set; }
    }
}