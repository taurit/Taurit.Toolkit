﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using JetBrains.Annotations;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Taurit.Toolkit.ProcessTodoistInbox.Stats.Models;
using Taurit.Toolkit.ProcessTodoistInbox.Stats.Services;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

namespace Taurit.Toolkit.ProcessTodoistInbox.Stats
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly StatsAppSettings _settings;
        private readonly SnapshotReader _snapshotReader;

        public MainWindow()
        {
            InitializeComponent();
            _settings = new StatsAppSettings();
            _snapshotReader = new SnapshotReader();
        }

        public Func<Double, String> XFormatter { get; } = value => new DateTime((Int64) value).ToString("yyyy-MM-dd");

        public SeriesCollection NumberOfTasks { get; set; } = new SeriesCollection();

        [NotNull]
        public ChartValues<DateTimePoint> NumberOfLowPriorityTasks { get; } = new ChartValues<DateTimePoint>();

        private void RadioButtonSetupChanged([CanBeNull] Object sender, [CanBeNull] RoutedEventArgs e)
        {
            if (!IsInitialized) return;
            NumberOfLowPriorityTasks.Clear();

            TimeSpan selectedTimePeriod = GetSelectedTimePeriod();
            List<SnapshotOnTimeline> snapshotsInSelectedTimePeriod =
                _snapshotReader.Read(_settings.SnapshotsRootFolderPath, DateTime.UtcNow, selectedTimePeriod);

            var undefinedPriorityTasks = new List<DateTimePoint>();
            var lowPriorityTasks = new List<DateTimePoint>();
            var mediumPriorityTasks = new List<DateTimePoint>();
            var highPriorityTasks = new List<DateTimePoint>();

            foreach (SnapshotOnTimeline snapshot in snapshotsInSelectedTimePeriod)
            {
                List<TodoTask> tasksWithNoDueDate = snapshot.AllTasks.Where(x => !x.HasDate).ToList();
                Int32 numUndefinedPriorityTasks = tasksWithNoDueDate.Count(x => x.priority == 1);
                Int32 numLowPriorityTasks = tasksWithNoDueDate.Count(x => x.priority == 2);
                Int32 numMediumPriorityTasks = tasksWithNoDueDate.Count(x => x.priority == 3);
                Int32 numHighPriorityTasks = tasksWithNoDueDate.Count(x => x.priority == 4);

                undefinedPriorityTasks.Add(new DateTimePoint(snapshot.Time, numUndefinedPriorityTasks));
                lowPriorityTasks.Add(new DateTimePoint(snapshot.Time, numLowPriorityTasks));
                mediumPriorityTasks.Add(new DateTimePoint(snapshot.Time, numMediumPriorityTasks));
                highPriorityTasks.Add(new DateTimePoint(snapshot.Time, numHighPriorityTasks));
            }

            var series = new[]
            {
                new StackedAreaSeries
                {
                    Title = "High priority",
                    Values = new ChartValues<DateTimePoint>(highPriorityTasks),
                    LineSmoothness = 0,
                    Fill = new SolidColorBrush(Color.FromRgb(224, 87, 85))
                    //Fill = new SolidColorBrush(Color.FromRgb(252, 237, 237)),
                },
                new StackedAreaSeries
                {
                    Title = "Medium priority",
                    Values = new ChartValues<DateTimePoint>(mediumPriorityTasks),
                    LineSmoothness = 0,
                    Fill = new SolidColorBrush(Color.FromRgb(255, 198, 149))
                    //Fill = new SolidColorBrush(Color.FromRgb(255, 246, 238)),
                },
                new StackedAreaSeries
                {
                    Title = "Low priority",
                    Values = new ChartValues<DateTimePoint>(lowPriorityTasks),
                    LineSmoothness = 0,
                    Fill = new SolidColorBrush(Color.FromRgb(255, 232, 174))
                    //Fill = new SolidColorBrush(Color.FromRgb(255, 251, 241)),
                },
                new StackedAreaSeries
                {
                    Title = "Undefined priority",
                    Values = new ChartValues<DateTimePoint>(undefinedPriorityTasks),
                    LineSmoothness = 0,
                    Fill = new SolidColorBrush(Color.FromRgb(235, 235, 235))
                    //Fill = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                }
            };
            NumberOfTasks.AddRange(series);
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