using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Taurit.Toolkit.ProcessTodoistInbox.Stats
{
    internal class StatsAppSettings
    {
        public StatsAppSettings(String settingsFilePath)
        {
            if (settingsFilePath == null) throw new ArgumentNullException(nameof(settingsFilePath));
            if (!File.Exists(settingsFilePath))
                throw new ArgumentException("Settings file not found", nameof(settingsFilePath));

            String settingsFileContent = File.ReadAllText(settingsFilePath);
            var settingsFileModel = JsonConvert.DeserializeObject<SettingsFileModel>(settingsFileContent);

            if (settingsFileModel == null) throw new ArgumentException("Settings file was not in a valid format");

            if (settingsFileModel.SnapshotsRootFolderPath is null || !Directory.Exists(settingsFileModel.SnapshotsRootFolderPath))
                throw new ArgumentException("Settings file contains invalid root folder path");
            if (settingsFileModel.KindleMateDirectory is null || !Directory.Exists(settingsFileModel.KindleMateDirectory))
                throw new ArgumentException("Settings file contains invalid KindleMate directory path");
            if (settingsFileModel.ProjectsToIgnoreInStats == null)
                throw new ArgumentException("Settings file contains invalid list of ignored projects");
            if (settingsFileModel.ReductionRatio <= 0)
                throw new ArgumentException("Settings file contains invalid ReductionRatio");

            ProjectsToIgnoreInStats = new HashSet<String>(settingsFileModel.ProjectsToIgnoreInStats);
            SnapshotsRootFolderPath = settingsFileModel.SnapshotsRootFolderPath;
            KindleMateDirectory = settingsFileModel.KindleMateDirectory;
            ReductionRatio = settingsFileModel.ReductionRatio;
        }

        /// <summary>
        ///     Projects that should not be included when counting total estimated backlog time/size (eg. because items in those
        ///     projects are not refined, not estimated accurately enough or are on hold).
        /// </summary>
        public HashSet<String> ProjectsToIgnoreInStats { get; }

        public String SnapshotsRootFolderPath { get; }

        public String KindleMateDirectory { get; }

        /// <summary>
        ///     Only every N-th snapshot will be read, this property's value being N
        /// </summary>
        public Int32 ReductionRatio { get; }

        [DataContract]
        private class SettingsFileModel
        {
            [DataMember]
            public List<String>? ProjectsToIgnoreInStats { get; set; }

            [DataMember]
            public String? SnapshotsRootFolderPath { get; set; }

            [DataMember]
            public String? KindleMateDirectory { get; set; }

            [DataMember]
            public Int32 ReductionRatio { get; set; }
        }
    }
}