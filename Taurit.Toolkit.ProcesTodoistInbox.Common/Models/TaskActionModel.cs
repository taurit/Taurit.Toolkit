using System;
using JetBrains.Annotations;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

namespace Taurit.Toolkit.ProcessTodoistInbox.Common.Models
{
    /// <summary>
    ///     Represents a Todoist API action that needs to be performed on an item, based on one of the rules that was matched
    ///     for this task.
    /// </summary>
    public class TaskActionModel
    {
        public TaskActionModel()
        {
        }

        public TaskActionModel([NotNull] TodoTask task, [CanBeNull] Label newLabel, [CanBeNull] Int32? newPriority,
            [CanBeNull] Project newProject, [CanBeNull] String newName)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            if (newPriority < 1 || newPriority > 4)
                throw new ArgumentException("Priority value is outside of range 1 (default) to 4 (high)");
            NewName = newName;

            Name = task.content;
            Label = newLabel;
            Project = newProject;
            Priority = newPriority;
            TaskId = task.id;
            OldProjectId = task.project_id;
        }

        public String Name { get; set; }

        [CanBeNull]
        public String NewName { get; set; }

        [CanBeNull]
        public Project Project { get; set; }

        [CanBeNull]
        public Label Label { get; set; }

        [CanBeNull]
        public Int32? Priority { get; set; }

        public Int64 TaskId { get; set; }
        public Int64 OldProjectId { get; set; }
    }
}