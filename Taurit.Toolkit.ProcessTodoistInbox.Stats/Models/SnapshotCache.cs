using System;
using System.Runtime.Serialization;

namespace Taurit.Toolkit.ProcessTodoistInbox.Stats.Models
{
    internal class SnapshotCache
    {
        public SnapshotCache(
            in Double estTimeUndefinedPriorityTasks,
            in Double estTimeLowPriorityTasks,
            in Double estTimeMediumPriorityTasks,
            in Double estTimeHighPriorityTasks,
            in Double estTimeFutureTasks)
        {
            EstTimeUndefinedPriorityTasks = estTimeUndefinedPriorityTasks;
            EstTimeLowPriorityTasks = estTimeLowPriorityTasks;
            EstTimeMediumPriorityTasks = estTimeMediumPriorityTasks;
            EstTimeHighPriorityTasks = estTimeHighPriorityTasks;
            EstTimeFutureTasks = estTimeFutureTasks;
        }

        [DataMember]
        public Double EstTimeUndefinedPriorityTasks { get; }

        [DataMember]
        public Double EstTimeLowPriorityTasks { get; }

        [DataMember]
        public Double EstTimeMediumPriorityTasks { get; }

        [DataMember]
        public Double EstTimeHighPriorityTasks { get; }

        [DataMember]
        public Double EstTimeFutureTasks { get; }
    }
}