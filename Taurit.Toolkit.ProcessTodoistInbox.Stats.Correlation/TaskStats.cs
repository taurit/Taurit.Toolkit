using System;
using System.Diagnostics;

namespace Taurit.Toolkit.ProcessTodoistInbox.Stats.Correlation
{
    [DebuggerDisplay("#{Id}: {TaskLifetimeDays}h")]
    internal class TaskStats
    {
        public TaskStats(Int64 id, Double taskLifetimeDays, Double? estimatedTaskTimeMinutes, String taskDescription, int priority)
        {
            Id = id;
            TaskLifetimeDays = taskLifetimeDays;
            EstimatedTaskTimeMinutes = estimatedTaskTimeMinutes;
            TaskDescription = taskDescription;
            Priority = priority;
        }

        public Int64 Id { get; }
        public Double TaskLifetimeDays { get; }
        public Double? EstimatedTaskTimeMinutes { get; }
        public String TaskDescription { get; }
        public Int32 Priority { get; }
    }
}