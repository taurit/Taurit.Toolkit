using System;
using System.Collections.Generic;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

namespace Taurit.Toolkit.ProcessTodoistInbox.Stats.Models
{
    internal class SnapshotOnTimeline
    {
        public SnapshotOnTimeline(
            DateTime time,
            IReadOnlyList<TodoTask> allTasks,
            IReadOnlyList<Project> allProjects,
            IReadOnlyList<Label> allLabels)
        {
            Time = time;
            AllTasks = allTasks;
            AllProjects = allProjects;
            AllLabels = allLabels;
        }

        public DateTime Time { get; }
        public IReadOnlyList<TodoTask> AllTasks { get; }
        public IReadOnlyList<Project> AllProjects { get; }
        public IReadOnlyList<Label> AllLabels { get; }
    }
}