using System;
using JetBrains.Annotations;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

namespace Taurit.Toolkit.ProcesTodoistInbox.Common.Models
{
    public class TaskActionModel
    {
        public TaskActionModel()
        {
        }

        public TaskActionModel([NotNull] TodoTask task, [CanBeNull] Label newLabel, [CanBeNull] Int32? newPriority,
            [CanBeNull] Project newProject)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            if (newPriority < 1 || newPriority > 4)
                throw new ArgumentException("Priority value is outsde of range 1 (default) to 4 (high)");

            Name = task.content;
            Label = newLabel;
            Project = newProject;
            Priority = newPriority;
            TaskId = task.id;
            OldProjectId = task.project_id;
        }

        public String Name { get; set; }

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