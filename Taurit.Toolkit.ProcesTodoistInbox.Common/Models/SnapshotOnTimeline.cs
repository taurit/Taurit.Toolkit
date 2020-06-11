using System;
using System.Collections.Generic;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

namespace Taurit.Toolkit.ProcessTodoistInbox.Common.Models
{
    public class SnapshotOnTimeline
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

        public DateTime EndOfQuarter => SnapshotOnTimeline.GetLastDayOfQuarter(Time).AddDays(1);

        /// <summary>
        ///     https://dotnetcodr.com/2015/10/28/various-quarter-related-datetime-functions-in-c/
        /// </summary>
        /// <param name="originalDate"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfQuarter(DateTime originalDate) // todo: move to a separate class!
        {
            return SnapshotOnTimeline.AddQuarters(new DateTime(originalDate.Year, 1, 1), SnapshotOnTimeline.GetQuarter(originalDate)).AddDays(-1);
        }

        /// <summary>
        ///     https://dotnetcodr.com/2015/10/28/various-quarter-related-datetime-functions-in-c/
        /// </summary>
        /// <param name="fromDate"></param>
        /// <returns></returns>
        private static Int32 GetQuarter(DateTime fromDate)
        {
            Int32 month = fromDate.Month - 1;
            Int32 month2 = Math.Abs(month / 3) + 1;
            return month2;
        }

        /// <summary>
        ///     https://dotnetcodr.com/2015/10/28/various-quarter-related-datetime-functions-in-c/
        /// </summary>
        /// <param name="originalDate"></param>
        /// <param name="quarters"></param>
        /// <returns></returns>
        private static DateTime AddQuarters(DateTime originalDate, Int32 quarters)
        {
            return originalDate.AddMonths(quarters * 3);
        }
    }
}