using System;
using System.IO;
using System.Windows;

namespace Taurit.Toolkit.ProcessTodoistInbox
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(Object sender, StartupEventArgs e)
        {
            var mainWindow = new MainWindow();
            if (e.Args.Length != 1)
            {
                MessageBox.Show("Path to the settings file should be passed as an argument");
                return;
            }

            String settingsFilePath = e.Args[0];
            if (!File.Exists(settingsFilePath))
            {
                MessageBox.Show($"Settings file not found under a given path '{settingsFilePath}'.");
                return;
            }

            mainWindow.LoadSettings(settingsFilePath);
            mainWindow.Show();
        }
    }
}