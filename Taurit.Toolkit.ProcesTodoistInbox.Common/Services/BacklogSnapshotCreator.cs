using System;
using System.Collections.Generic;
using System.IO;
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
        public static void CreateSnapshot(String snapshotsFolderPath, IReadOnlyList<TodoTask> allTasks,
            DateTime snapshotTime)
        {
            String subfolderName = $"{snapshotTime:yyyy-MM-dd}";
            String subfolderPath = Path.Combine(snapshotsFolderPath, subfolderName);
            Directory.CreateDirectory(subfolderPath);

            String fileName = $"snapshot-{snapshotTime:HH-mm-ss}.json";
            String fullPath = Path.Combine(subfolderPath, fileName);

            String allTasksAsJson = JsonConvert.SerializeObject(allTasks);

            File.WriteAllText(fullPath, allTasksAsJson);
        }
    }
}