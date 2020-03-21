using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using Taurit.Toolkit.ProcessTodoistInbox.Common.Models;
using Taurit.Toolkit.TodoistInboxHelper;

namespace Taurit.Toolkit.ProcessTodoistInbox.Stats.Services
{
    internal class SnapshotReader
    {
        private readonly StatsAppSettings _settings;

        public SnapshotReader(StatsAppSettings settings)
        {
            _settings = settings;
        }

        /// <param name="snapshotsToSkip">for those snapshots we just need a date, because the stats are already in the cache</param>
        [SuppressMessage("ReSharper", "InvalidXmlDocComment")]
        public List<SnapshotOnTimeline> Read(
            String snapshotsRootFolderPath,
            DateTime periodEnd,
            TimeSpan period,
            ISet<DateTime> snapshotsToSkip
        )
        {
            // get all available dates
            IEnumerable<String> subdirectories = Directory.EnumerateDirectories(snapshotsRootFolderPath);
            IEnumerable<DateTime> dates =
                subdirectories.Select(x =>
                    DateTime.ParseExact(
                        x.Replace(snapshotsRootFolderPath, ""),
                        "yyyy-MM-dd",
                        CultureInfo.InvariantCulture
                    )
                );

            // which dates overlap the requested period?
            DateTime periodStart = periodEnd.Subtract(period);
            List<DateTime> selectedDates = dates.Where(x => x >= periodStart && x <= periodEnd).ToList();

            // list all the snapshots in those dates
            DateTime detailedSnapshotsPeriodStart = periodEnd.AddDays(-3);
            var snapshots = new List<SnapshotOnTimeline>();
            foreach (DateTime selectedDate in selectedDates)
            {
                String selectedDateSubfolder = Path.Combine(snapshotsRootFolderPath, $"{selectedDate:yyyy-MM-dd}");
                List<String> snapshotsInSubfolder =
                    Directory.EnumerateFiles(selectedDateSubfolder, "*.labels").ToList();

                IEnumerable<String> selectedSnapshots =
                    selectedDate < detailedSnapshotsPeriodStart
                        ? GetNth(snapshotsInSubfolder, _settings.ReductionRatio)
                        : snapshotsInSubfolder; // show with maximum granularity of 1h for the last days, because this is data I'm most interested in

                foreach (String snapshot in selectedSnapshots)
                {
                    String timeStringPart = Path.GetFileNameWithoutExtension(snapshot).Replace("snapshot-", "");
                    DateTime exactSnapshotDate = DateTime.ParseExact($"{selectedDate:yyyy-MM-dd} {timeStringPart}",
                        "yyyy-MM-dd HH-mm-ss", CultureInfo.InvariantCulture);

                    if (snapshotsToSkip.Contains(exactSnapshotDate))
                    {
                        // skip for performance - stats are already in the cache
                        snapshots.Add(new SnapshotOnTimeline(exactSnapshotDate, null, null, null));
                        continue;
                    }

                    String snapshotPathWithoutExtension = snapshot.Replace(".labels", string.Empty);

                    String tasksSnapshotFileContent = File.ReadAllText($"{snapshotPathWithoutExtension}.tasks");
                    String projectsSnapshotFileContent = File.ReadAllText($"{snapshotPathWithoutExtension}.projects");
                    String labelsSnapshotFileContent = File.ReadAllText($"{snapshotPathWithoutExtension}.labels");

                    var queryService = new TodoistSnapshotQueryService(tasksSnapshotFileContent,
                        projectsSnapshotFileContent,
                        labelsSnapshotFileContent);


                    var snapshotOnTimeline = new SnapshotOnTimeline(
                        exactSnapshotDate,
                        queryService.GetAllTasks(null, null),
                        queryService.GetAllProjects(),
                        queryService.GetAllLabels()
                    );
                    snapshots.Add(snapshotOnTimeline);
                }
            }

            return snapshots;
        }

        private static IEnumerable<T> GetNth<T>(List<T> list, Int32 n)
        {
            for (Int32 i = n - 1; i < list.Count; i += n)
                yield return list[i];
        }
    }
}