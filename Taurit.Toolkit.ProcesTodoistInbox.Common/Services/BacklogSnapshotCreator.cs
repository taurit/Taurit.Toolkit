using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

namespace Taurit.Toolkit.ProcessTodoistInbox.Common.Services
{
    /// <summary>
    ///     This class allows to take snapshot of the tasks that are present in the backlog at a given moment and save them
    ///     into a file for later analysis.
    /// </summary>
    public class BacklogSnapshotCreator
    {
        public String CreateSnapshot(String snapshotsFolderPath, DateTime snapshotTime,
            IReadOnlyList<TodoTask> allTasks, IReadOnlyList<Project> allProjects, IReadOnlyList<Label> allLabels)
        {
            String subfolderName = $"{snapshotTime:yyyy-MM-dd}";
            String subfolderPath = Path.Combine(snapshotsFolderPath, subfolderName);
            Directory.CreateDirectory(subfolderPath);

            String allTasksAsJson = JsonConvert.SerializeObject(allTasks);
            String allProjectsAsJson = JsonConvert.SerializeObject(allProjects);
            String allLabelsAsJson = JsonConvert.SerializeObject(allLabels);

            String fileNameBase = $"snapshot-{snapshotTime:HH-mm-ss}";
            String fileNameTasks = $"{fileNameBase}.tasks";
            String fileNameProjects = $"{fileNameBase}.projects";
            String fileNameLabels = $"{fileNameBase}.labels";

            File.WriteAllText(Path.Combine(subfolderPath, fileNameTasks), allTasksAsJson);
            File.WriteAllText(Path.Combine(subfolderPath, fileNameProjects), allProjectsAsJson);
            File.WriteAllText(Path.Combine(subfolderPath, fileNameLabels), allLabelsAsJson);

            return subfolderPath;
        }

        public String GetNewestSnapshot(String snapshotsRootDirectory)
        {
            IEnumerable<String> subfolders = Directory.EnumerateDirectories(snapshotsRootDirectory);
            String newestSubfolder = subfolders.OrderByDescending(x => x).First();
            IEnumerable<String> files = Directory.EnumerateFiles(newestSubfolder);
            String newestFile = files.OrderByDescending(x => x).First();

            // remove file extension
            String newestFileWithoutExtension = newestFile
                    .Replace(".labels", "")
                    .Replace(".projects", "")
                    .Replace(".tasks", ""); // quick & dirty

            return newestFileWithoutExtension;
        }
    }
}