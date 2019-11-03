using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Taurit.Toolkit.ProcessTodoistInbox.Common.Models;

namespace Taurit.Toolkit.ProcessTodoistInbox.Raspberry
{
    internal class Program
    {
        private static async Task Main(String[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("invalid number of arguments. Expected:");
                Console.WriteLine("[0] path to a data directory that contains ProcessTodoistInboxSettings.json");
                return;
            }

            String programDataDirectory = args[0];
            if (!Directory.Exists(programDataDirectory))
            {
                Console.WriteLine($"The `{programDataDirectory}` directory does not exist!");
                return;
            }

            String configurationFileName = $"{programDataDirectory}/ProcessTodoistInboxSettings.json";
            if (!File.Exists(configurationFileName))
            {
                Console.WriteLine(
                    $"The configuration file `{configurationFileName}` was not found in the `{programDataDirectory}` directory");
                return;
            }

            SettingsFileModel settings = await LoadSettings(configurationFileName);
            var startupTask = new StartupTask(settings);
            startupTask.Run(programDataDirectory);
        }


        private static async Task<SettingsFileModel> LoadSettings(String fileName)
        {
            String settingsFileContents = await File.ReadAllTextAsync(fileName);
            var settingsDeserialized = JsonSerializer.Deserialize<SettingsFileModel>(settingsFileContents);
            return settingsDeserialized;
        }
    }
}