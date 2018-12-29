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

        public DateTime EndOfQuarter => GetLastDayOfQuarter(Time).AddDays(1);

        /// <summary>
        /// https://dotnetcodr.com/2015/10/28/various-quarter-related-datetime-functions-in-c/
        /// </summary>
        /// <param name="originalDate"></param>
        /// <returns></returns>
        private DateTime GetLastDayOfQuarter(DateTime originalDate)
        {
            return AddQuarters(new DateTime(originalDate.Year, 1, 1), GetQuarter(originalDate)).AddDays(-1);
        }

        /// <summary>
        /// https://dotnetcodr.com/2015/10/28/various-quarter-related-datetime-functions-in-c/
        /// </summary>
        /// <param name="fromDate"></param>
        /// <returns></returns>
        private Int32 GetQuarter(DateTime fromDate)
        {
            Int32 month = fromDate.Month - 1;
            Int32 month2 = Math.Abs(month / 3) + 1;
            return month2;
        }

        /// <summary>
        /// https://dotnetcodr.com/2015/10/28/various-quarter-related-datetime-functions-in-c/
        /// </summary>
        /// <param name="originalDate"></param>
        /// <param name="quarters"></param>
        /// <returns></returns>
        private DateTime AddQuarters(DateTime originalDate, Int32 quarters)
        {
            return originalDate.AddMonths(quarters * 3);
        }
    }
}