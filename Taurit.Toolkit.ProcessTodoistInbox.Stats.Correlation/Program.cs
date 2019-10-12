using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using JetBrains.Annotations;
using NaturalLanguageTimespanParser;
using Taurit.Toolkit.ProcessTodoistInbox.Common.Models;

namespace Taurit.Toolkit.ProcessTodoistInbox.Stats.Correlation
{
    internal class Program
    {
        [NotNull] private readonly MultiCultureTimespanParser _timespanParser = new MultiCultureTimespanParser(new[]
        {
            new CultureInfo("pl"),
            new CultureInfo("en")
        });

        /// <summary>
        ///     In order to generate meaningful charts in Excel I want output in the following CSV file:
        ///     [ Task id ] [ Estimated time (minutes) ] [ Time between task creation and completion (minutes) ]
        /// </summary>
        private static void Main()
        {
            var program = new Program();
            List<SnapshotOnTimeline> snapshots = program.LoadSnapshots();
            List<TaskWithSnapshotDate> allTasks = program.StoreSnapshotDateWithTasks(snapshots);
            ILookup<Int64, TaskWithSnapshotDate> allTasksGroupedById = program.GroupTasksById(allTasks);
            List<TaskStats> stats = program.ComputeStatsForTasks(allTasksGroupedById);
            program.ExportToCsv("d:/taskstats.csv", stats);
        }


        private void ExportToCsv(String fileName, List<TaskStats> stats)
        {
            using var writer = new StreamWriter(fileName);
            using var csv = new CsvWriter(writer);
            csv.WriteRecords(stats);
        }

        private List<TaskStats> ComputeStatsForTasks(ILookup<Int64, TaskWithSnapshotDate> allTasksGroupedById)
        {
            var stats = new List<TaskStats>(allTasksGroupedById.Count);
            foreach (IGrouping<Int64, TaskWithSnapshotDate> taskInTime in allTasksGroupedById)
            {
                Int64 taskId = taskInTime.Key;
                TaskWithSnapshotDate taskAtMostRecentPointInTime =
                    taskInTime.OrderByDescending(x => x.SnapshotDate).First();
                String dateAdded = taskAtMostRecentPointInTime.Task.date_added; // creation date
                if (dateAdded == null) continue; // i didn't collect it before some point in time
                String taskDescription = taskAtMostRecentPointInTime.Task.content;

                DateTime dateAddedAsDateTime = DateTime.Parse(dateAdded);
                DateTime lastDateTaskWasSeen = taskInTime.Max(x => x.SnapshotDate);

                Double taskLifetimeDays = lastDateTaskWasSeen.Subtract(dateAddedAsDateTime).TotalDays;

                if (taskLifetimeDays < 0) continue; // at least before i deal with the timezone differences
                Debug.Assert(taskLifetimeDays >= 0);

                TimespanParseResult estimatedTime = _timespanParser.Parse(taskAtMostRecentPointInTime.Task.content);
                Double? estimateInMinutes =
                    estimatedTime.Success ? estimatedTime.Duration.TotalMinutes : (Double?) null;
                Int32 priority = taskAtMostRecentPointInTime.Task.priority;

                var taskStats = new TaskStats(taskId, taskLifetimeDays, estimateInMinutes, taskDescription, priority);
                stats.Add(taskStats);
            }

            return stats;
        }

        private ILookup<Int64, TaskWithSnapshotDate> GroupTasksById(List<TaskWithSnapshotDate> allTasks)
        {
            ILookup<Int64, TaskWithSnapshotDate> allTasksGroupedById = allTasks.ToLookup(x => x.Task.id);
            return allTasksGroupedById;
        }

        private List<TaskWithSnapshotDate> StoreSnapshotDateWithTasks(List<SnapshotOnTimeline> snapshots)
        {
            DateTime lastSnapshotDate = snapshots.Max(x => x.Time);
            HashSet<Int64> tasksStillNotCompletedInMostRecentSnapshot = snapshots
                .Single(x => x.Time == lastSnapshotDate).AllTasks.Select(x => x.id).ToHashSet();

            var allTasks = new List<TaskWithSnapshotDate>(100000); // todo tune 
            foreach (SnapshotOnTimeline snapshot in snapshots)
            {
                IEnumerable<TaskWithSnapshotDate> taskWithSnapshotDates = snapshot.AllTasks
                    .Where(x => x.is_deleted == 0) // just to be sure
                    .Where(x => x.HasDate == false)
                    .Where(x => x.is_archived == 0)
                    .Where(x => x.project_name != "Someday/Maybe")
                    .Where(x => x.project_name != "Edukacja")
                    .Where(x => !tasksStillNotCompletedInMostRecentSnapshot.Contains(x.id))
                    .Select(x => new TaskWithSnapshotDate(snapshot.Time, x));
                allTasks.AddRange(taskWithSnapshotDates);
            }

            return allTasks;
        }

        private List<SnapshotOnTimeline> LoadSnapshots()
        {
            var snapshotReader = new SimpleSnapshotReader();
            List<SnapshotOnTimeline> snapshots =
                snapshotReader.Read("d:\\mirrors\\RaspberryPiWindows10\\BacklogSnapshots\\");
            return snapshots;
        }
    }
}