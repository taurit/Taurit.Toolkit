using System;
using System.Windows;

namespace Taurit.Toolkit.ProcessTodoistInbox.Stats
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private void Application_Startup(Object sender, StartupEventArgs e)
        {
            if (e.Args.Length != 1)
            {
                MessageBox.Show("Path to the settings file must be passed as an argument 1.");
                return;
            }

            String settingsFilePath = e.Args[0];

            var mainWindow = new MainWindow(settingsFilePath);
            mainWindow.Show();
        }
    }
}