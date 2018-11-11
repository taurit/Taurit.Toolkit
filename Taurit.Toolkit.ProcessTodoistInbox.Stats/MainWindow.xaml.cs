using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using JetBrains.Annotations;
using LiveCharts;
using LiveCharts.Defaults;
using Taurit.Toolkit.ProcessTodoistInbox.Stats.Models;
using Taurit.Toolkit.ProcessTodoistInbox.Stats.Services;
using Taurit.Toolkit.TodoistInboxHelper;

namespace Taurit.Toolkit.ProcessTodoistInbox.Stats
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private StatsAppSettings _settings;
        private SnapshotReader _snapshotReader;

        public MainWindow()
        {
            InitializeComponent();
            _settings = new StatsAppSettings();
            _snapshotReader = new SnapshotReader();
        }

        public Func<Double, String> XFormatter { get; } = value =>
        {
            return new DateTime((Int64) value).ToString("yyyy-MM-dd");
        };

        [NotNull]
        public ChartValues<DateTimePoint> YData { get; } = new ChartValues<DateTimePoint>();

        private void RadioButtonSetupChanged([CanBeNull] Object sender, [CanBeNull] RoutedEventArgs e)
        {
            if (!IsInitialized) return;
            YData.Clear();

            TimeSpan selectedTimePeriod = GetSelectedTimePeriod();
            List<SnapshotOnTimeline> snapshotsInSelectedTimePeriod =
                _snapshotReader.Read(_settings.SnapshotsRootFolderPath, DateTime.UtcNow, selectedTimePeriod);

            foreach (var snapshot in snapshotsInSelectedTimePeriod)
            {
                YData.Add(new DateTimePoint(snapshot.Time, snapshot.AllTasks.Count));
            }

        }

        private TimeSpan GetSelectedTimePeriod()
        {
            RadioButton[] timePeriodCheckboxes =
            {
                Time_AllTime,
                Time_LastMonth,
                Time_LastWeek,
                Time_LastYear
            };
            RadioButton selectedTimeCheckbox = timePeriodCheckboxes.Single(x => x.IsChecked == true);
            Int32 timeInDays = Convert.ToInt32((String) selectedTimeCheckbox.Tag);
            return TimeSpan.FromDays(timeInDays);
        }


        private void MainWindow_OnContentRendered(Object sender, EventArgs e)
        {
            RadioButtonSetupChanged(null, null);
        }
    }
}