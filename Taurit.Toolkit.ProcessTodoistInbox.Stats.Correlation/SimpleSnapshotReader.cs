using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Taurit.Toolkit.ProcessTodoistInbox.Common.Models;
using Taurit.Toolkit.TodoistInboxHelper;

namespace Taurit.Toolkit.ProcessTodoistInbox.Stats.Services
{
    /// <summary>
    ///     Non-optimized version of snapshot reader for slow but reliable results while keeping the implementation super
    ///     simple
    /// </summary>
    internal class SimpleSnapshotReader
    {
        public List<SnapshotOnTimeline> Read(String snapshotsRootFolderPath)
        {
            // get all available dates
            IEnumerable<String> subdirectories = Directory.EnumerateDirectories(snapshotsRootFolderPath);
            IEnumerable<DateTime> selectedDates =
                subdirectories.Select(x =>
                    DateTime.ParseExact(
                        x.Replace(snapshotsRootFolderPath, ""),
                        "yyyy-MM-dd",
                        CultureInfo.InvariantCulture
                    )
                );

            // list all the snapshots in those dates
            var snapshots = new List<SnapshotOnTimeline>();
            foreach (DateTime selectedDate in selectedDates)
            {
                String selectedDateSubfolder = Path.Combine(snapshotsRootFolderPath, $"{selectedDate:yyyy-MM-dd}");
                List<String> snapshotsInSubfolder =
                    Directory.EnumerateFiles(selectedDateSubfolder, "*.labels").ToList();

                IEnumerable<String> selectedSnapshots = snapshotsInSubfolder;

                foreach (String snapshot in selectedSnapshots)
                {
                    String timeStringPart = Path.GetFileNameWithoutExtension(snapshot).Replace("snapshot-", "");
                    DateTime exactSnapshotDateLocalTime = DateTime.ParseExact($"{selectedDate:yyyy-MM-dd} {timeStringPart}",
                        "yyyy-MM-dd HH-mm-ss", CultureInfo.InvariantCulture);
                    
                    // fix the datetime - it's in local time, but it's more useful in UTC time for analysis
                    var exactSnapshotDate = exactSnapshotDateLocalTime.AddHours(1); // bad idea... todo do this correctly, this can be 1h off depending on dst probably

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
    }
}