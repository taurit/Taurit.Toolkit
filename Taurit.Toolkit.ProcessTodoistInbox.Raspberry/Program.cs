﻿using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Taurit.Toolkit.ProcessTodoistInbox.Common.Models;
using Taurit.Toolkit.ProcessTodoistInboxBackground;

namespace Taurit.Toolkit.ProcessTodoistInbox.Raspberry
{
    internal class Program
    {
        private static async Task Main(String[] args)
        {
            var programDataDirectory = "/home/pi/ProcessTodoistInboxData";
            String configurationFileName = $"{programDataDirectory}/ProcessTodoistInboxSettings.json";

            SettingsFileModel settings = await LoadSettings(configurationFileName);
            var startupTask = new StartupTask(settings);
            startupTask.Run(programDataDirectory);
        }


        private static async Task<SettingsFileModel> LoadSettings(String fileName)
        {
            String settingsFileContents = await File.ReadAllTextAsync(fileName);
            var settingsDeserialized = JsonConvert.DeserializeObject<SettingsFileModel>(settingsFileContents);
            return settingsDeserialized;
        }
    }
}