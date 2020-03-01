using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

namespace Taurit.Toolkit.WeightMonitor.GUI
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
            Debug.Assert(Dispatcher != null, nameof(Dispatcher) + " != null");
            Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }

        private void OnDispatcherUnhandledException(Object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            String errorMessage = $"An unhandled exception occurred: {e.Exception.Message}";
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}