using System;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

namespace Taurit.Toolkit.ProcessTodoistInbox.Stats.Correlation
{
    internal class TaskWithSnapshotDate
    {
        public TaskWithSnapshotDate(DateTime snapshotDate, TodoTask task)
        {
            SnapshotDate = snapshotDate;
            Task = task;
        }

        public DateTime SnapshotDate { get; }
        public TodoTask Task { get; }
    }
}