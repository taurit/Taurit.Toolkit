﻿using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

namespace Taurit.Toolkit.WeightMonitor.GUI
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    internal partial class App
    {
        public App()
        {
            Debug.Assert(Dispatcher != null, nameof(DispatcherObject.Dispatcher) + " != null");
            Dispatcher.UnhandledException += App.OnDispatcherUnhandledException;
        }

        private static void OnDispatcherUnhandledException(Object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            String errorMessage = $"An unhandled exception occurred: {e.Exception.Message}";
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}