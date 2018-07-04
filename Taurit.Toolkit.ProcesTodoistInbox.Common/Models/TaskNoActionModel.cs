using System;
using JetBrains.Annotations;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

namespace Taurit.Toolkit.ProcesTodoistInbox.Common.Models
{
    public class TaskNoActionModel
    {
        public TaskNoActionModel([NotNull]TodoTask task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            Name = task.content;
        }

        public TaskNoActionModel()
        {
        }

        public String Name { get; set; }
    }
}