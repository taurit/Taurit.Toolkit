using System;
using System.IO;
using System.Windows;

namespace Taurit.Toolkit.ProcessTodoistInbox.UI
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private void Application_Startup(Object sender, StartupEventArgs e)
        {
            if (e.Args.Length != 2)
            {
                MessageBox.Show(
                    "Path to the settings file should be passed as an argument 1, and path to the snapshots root directory as argument 2.");
                return;
            }

            String settingsFilePath = e.Args[0];
            if (!File.Exists(settingsFilePath))
            {
                MessageBox.Show($"Settings file not found under a given path '{settingsFilePath}'.");
                return;
            }

            String snapshotsRootDirectory = e.Args[1];

            var mainWindow = new MainWindow(settingsFilePath, snapshotsRootDirectory);
            mainWindow.Show();
        }
    }
}