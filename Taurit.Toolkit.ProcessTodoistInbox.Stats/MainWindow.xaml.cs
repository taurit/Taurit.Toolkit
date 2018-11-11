using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using JetBrains.Annotations;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using NaturalLanguageTimespanParser;
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
        [NotNull] private readonly MultiCultureTimespanParser _mctp = new MultiCultureTimespanParser(new[]
        {
            new CultureInfo("pl"),
            new CultureInfo("en")
        });

        public MainWindow()
        {
            InitializeComponent();
            _settings = new StatsAppSettings();
            _snapshotReader = new SnapshotReader();
        }

        public Func<Double, String> XFormatter { get; } = value => new DateTime((Int64) value).ToString("yyyy-MM-dd");
        public Func<Double, String> YFormatter { get; } = value => $"{(Int64) value/60d:0.00}";
        
        public SeriesCollection EstimatedTimeOfTasks { get; set; } = new SeriesCollection();
        
        private void RadioButtonSetupChanged([CanBeNull] Object sender, [CanBeNull] RoutedEventArgs e)
        {
            if (!IsInitialized) return;
            EstimatedTimeOfTasks.Clear();

            TimeSpan selectedTimePeriod = GetSelectedTimePeriod();
            List<SnapshotOnTimeline> snapshotsInSelectedTimePeriod =
                _snapshotReader.Read(_settings.SnapshotsRootFolderPath, DateTime.UtcNow, selectedTimePeriod);


            var etUndefinedPriorityTasks = new List<DateTimePoint>();
            var etLowPriorityTasks = new List<DateTimePoint>();
            var etMediumPriorityTasks = new List<DateTimePoint>();
            var etHighPriorityTasks = new List<DateTimePoint>();

            foreach (SnapshotOnTimeline snapshot in snapshotsInSelectedTimePeriod)
            {
                List<TodoTask> tasksWithNoDueDate = snapshot.AllTasks.Where(x => !x.HasDate).ToList();
                IEnumerable<TodoTask> priority1Tasks = tasksWithNoDueDate.Where(x => x.priority == 1).ToList();
                IEnumerable<TodoTask> priority2Tasks = tasksWithNoDueDate.Where(x => x.priority == 2).ToList();
                IEnumerable<TodoTask> priority3Tasks = tasksWithNoDueDate.Where(x => x.priority == 3).ToList();
                IEnumerable<TodoTask> priority4Tasks = tasksWithNoDueDate.Where(x => x.priority == 4).ToList();
                
                double estTimeUndefinedPriorityTasks = priority1Tasks.Sum(x => GetTimeInMinutes(x.content));
                double estTimeLowPriorityTasks = priority2Tasks.Sum(x => GetTimeInMinutes(x.content));
                double estTimeMediumPriorityTasks = priority3Tasks.Sum(x => GetTimeInMinutes(x.content));
                double estTimeHighPriorityTasks = priority4Tasks.Sum(x => GetTimeInMinutes(x.content));
                
                etUndefinedPriorityTasks.Add(new DateTimePoint(snapshot.Time, estTimeUndefinedPriorityTasks));
                etLowPriorityTasks.Add(new DateTimePoint(snapshot.Time, estTimeLowPriorityTasks));
                etMediumPriorityTasks.Add(new DateTimePoint(snapshot.Time, estTimeMediumPriorityTasks));
                etHighPriorityTasks.Add(new DateTimePoint(snapshot.Time, estTimeHighPriorityTasks));
            }

            StackedAreaSeries[] estimatedTimeOfTasksSeries = GetStackedSeries(
                etHighPriorityTasks,
                etMediumPriorityTasks,
                etLowPriorityTasks,
                etUndefinedPriorityTasks);
            EstimatedTimeOfTasks.AddRange(estimatedTimeOfTasksSeries);
        }

        private double GetTimeInMinutes(String content)
        {
            var parseResult = _mctp.Parse(content);
            return parseResult.Success ? parseResult.Duration.TotalMinutes : 0d;
        }

        private static StackedAreaSeries[] GetStackedSeries(List<DateTimePoint> highPriorityTasks, List<DateTimePoint> mediumPriorityTasks,
            List<DateTimePoint> lowPriorityTasks, List<DateTimePoint> undefinedPriorityTasks)
        {
            var series = new[]
            {
                new StackedAreaSeries
                {
                    Title = "High priority",
                    Values = new ChartValues<DateTimePoint>(highPriorityTasks),
                    LineSmoothness = 0,
                    Fill = new SolidColorBrush(Color.FromRgb(224, 87, 85))
                },
                new StackedAreaSeries
                {
                    Title = "Medium priority",
                    Values = new ChartValues<DateTimePoint>(mediumPriorityTasks),
                    LineSmoothness = 0,
                    Fill = new SolidColorBrush(Color.FromRgb(255, 198, 149))
                },
                new StackedAreaSeries
                {
                    Title = "Low priority",
                    Values = new ChartValues<DateTimePoint>(lowPriorityTasks),
                    LineSmoothness = 0,
                    Fill = new SolidColorBrush(Color.FromRgb(227, 231, 231))
                },
                new StackedAreaSeries
                {
                    Title = "Undefined priority",
                    Values = new ChartValues<DateTimePoint>(undefinedPriorityTasks),
                    LineSmoothness = 0,
                    Fill = new SolidColorBrush(Color.FromRgb(235, 235, 235))
                }
            };
            return series;
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