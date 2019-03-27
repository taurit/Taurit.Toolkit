using System;
using JetBrains.Annotations;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

namespace Taurit.Toolkit.ProcessTodoistInbox.Common.Models
{
    /// <summary>
    ///     Represents a task for which no rule was associated and no action is planned.
    ///     Such tasks can be displayed eg. in the UI to show items that might still benefit from automation if we add more
    ///     rules.
    /// </summary>
    public class TaskNoActionModel
    {
        public TaskNoActionModel([NotNull] TodoTask task, TimeSpan estimatedTime)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            EstimatedTime = estimatedTime == TimeSpan.Zero ? (TimeSpan?) null : estimatedTime;
            Name = task.content;
            Labels = string.Join(", ", task.labels);
            Project = task.project_name;
        }


        public TaskNoActionModel()
        {
        }

        public String Name { get; set; }
        public String Labels { get; set; }
        public String Project { get; set; }
        public TimeSpan? EstimatedTime { get; set; }
    }
}