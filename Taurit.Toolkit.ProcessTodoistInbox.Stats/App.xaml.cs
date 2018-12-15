using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Taurit.Toolkit.ProcessTodoistInbox.Stats
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
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
