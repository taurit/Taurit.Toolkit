using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using JetBrains.Annotations;

namespace Taurit.Toolkit.CountdownToSleep
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow : INotifyPropertyChanged
    {
        private static readonly TimeSpan TimerInterval = TimeSpan.FromSeconds(1);
        private readonly DispatcherTimer _countdownTimer;

        public MainWindow()
        {
            Time = TimeSpan.FromMinutes(10);

            InitializeComponent();
            SetLocationToRightBottomCorner();

            Dispatcher currentDispatcher = Application.Current.Dispatcher;
            Contract.Assume(currentDispatcher != null);
            _countdownTimer = new DispatcherTimer(DispatcherPriority.Normal, currentDispatcher);
            _countdownTimer.Tick += CountdownTimerOnTick;
            _countdownTimer.Interval = new TimeSpan(0, 0, 1);
            _countdownTimer.Start();
        }

        public TimeSpan Time { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void SetLocationToRightBottomCorner()
        {
            const Int32 margin = 20;
            Rect desktopWorkingArea = SystemParameters.WorkArea;
            Left = desktopWorkingArea.Right - Width - margin;
            Top = desktopWorkingArea.Bottom - Height - margin;
        }

        private void CountdownTimerOnTick(Object sender, EventArgs e)
        {
            if (Time.TotalMilliseconds <= 0)
            {
                _countdownTimer.Stop();

                // Suspend (go to sleep mode)
                SetSuspendState(false, true, true);

                Application.Current.Shutdown();
            }
            else
            {
                Time = Time.Subtract(TimerInterval);
                OnPropertyChanged(nameof(Time));
            }
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] String propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [DllImport("Powrprof.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern Boolean
            SetSuspendState(Boolean hiberate, Boolean forceCritical, Boolean disableWakeEvent);

        private void MainWindow_OnMouseDown(Object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
    }
}