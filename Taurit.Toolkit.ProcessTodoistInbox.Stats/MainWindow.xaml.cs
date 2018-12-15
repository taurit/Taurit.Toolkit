﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
        [NotNull] private readonly MultiCultureTimespanParser _mctp = new MultiCultureTimespanParser(new[]
        {
            new CultureInfo("pl"),
            new CultureInfo("en")
        });

        private readonly StatsAppSettings _settings;
        private readonly SnapshotReader _snapshotReader;

        public MainWindow([NotNull] String settingsFilePath)
        {
            InitializeComponent();

            _settings = new StatsAppSettings(settingsFilePath);
            _snapshotReader = new SnapshotReader(_settings);
        }

        public Func<Double, String> XFormatter { get; } = value => new DateTime((Int64) value).ToString("yyyy-MM-dd");
        public Func<Double, String> YFormatter { get; } = value => $"{(Int64) value / 60d:0.00}";

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
                List<TodoTask> taskToCount =
                    FilterOutTaskThatShouldNotBeCounted(snapshot.AllTasks, snapshot.AllProjects);

                IEnumerable<TodoTask> priority1Tasks = taskToCount.Where(x => x.priority == 1).ToList();
                IEnumerable<TodoTask> priority2Tasks = taskToCount.Where(x => x.priority == 2).ToList();
                IEnumerable<TodoTask> priority3Tasks = taskToCount.Where(x => x.priority == 3).ToList();
                IEnumerable<TodoTask> priority4Tasks = taskToCount.Where(x => x.priority == 4).ToList();

                Double estTimeUndefinedPriorityTasks = priority1Tasks.Sum(x => GetTimeInMinutes(x.content));
                Double estTimeLowPriorityTasks = priority2Tasks.Sum(x => GetTimeInMinutes(x.content));
                Double estTimeMediumPriorityTasks = priority3Tasks.Sum(x => GetTimeInMinutes(x.content));
                Double estTimeHighPriorityTasks = priority4Tasks.Sum(x => GetTimeInMinutes(x.content));

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

        private List<TodoTask> FilterOutTaskThatShouldNotBeCounted(IReadOnlyList<TodoTask> allTasks,
            IReadOnlyList<Project> allProjects)
        {
            // project names are trimmed because for some reason a non-breaking space ("A0" character) appears sometimes in the production data at the end
            var ignoredProjectsNames = new HashSet<String>(_settings.ProjectsToIgnoreInStats.Select(x => x.Trim()),
                StringComparer.InvariantCultureIgnoreCase);

            List<Project> ignoredProjects = allProjects
                .Where(x => ignoredProjectsNames.Contains(x.name.Trim()))
                .ToList();

            
            // also ignore sub-projects
            var ignoredSubProjects = new List<Project>();
            Int32 maxProjectOrder = allProjects.Max(x => x.item_order);
            var projectsByItemOrder = allProjects.ToLookup(x => x.item_order);
            foreach (Project ignoredProject in ignoredProjects)
            {
                Int32 ignoredProjectOrder = ignoredProject.item_order;
                for (int i = ignoredProjectOrder + 1; i <= maxProjectOrder; i++)
                {
                    var project = projectsByItemOrder[i].Single();
                    if (project.indent > ignoredProject.indent)
                    {
                        ignoredSubProjects.Add(project);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            // list projects to ignore
            ImmutableHashSet<Int64> ignoredProjectsIds = ignoredProjects.Select(x => x.id)
                    .Union(ignoredSubProjects.Select(x => x.id))
                    .ToImmutableHashSet();

            List<TodoTask> relevantTasks = allTasks
                .Where(x => !x.HasDate)
                .Where(x => !ignoredProjectsIds.Contains(x.project_id))
                .ToList();

            return relevantTasks;
        }

        private Double GetTimeInMinutes(String content)
        {
            TimespanParseResult parseResult = _mctp.Parse(content);
            return parseResult.Success ? parseResult.Duration.TotalMinutes : 0d;
        }

        private static StackedAreaSeries[] GetStackedSeries(List<DateTimePoint> highPriorityTasks,
            List<DateTimePoint> mediumPriorityTasks,
            List<DateTimePoint> lowPriorityTasks, List<DateTimePoint> undefinedPriorityTasks)
        {
            var series = new[]
            {
                new StackedAreaSeries
                {
                    Title = "High priority",
                    Values = new ChartValues<DateTimePoint>(highPriorityTasks),
                    LineSmoothness = 0,
                    Fill = new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FF95190c"))
                },
                new StackedAreaSeries
                {
                    Title = "Medium priority",
                    Values = new ChartValues<DateTimePoint>(mediumPriorityTasks),
                    LineSmoothness = 0,
                    Fill = new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FFe8bb68"))
                },
                new StackedAreaSeries
                {
                    Title = "Low priority",
                    Values = new ChartValues<DateTimePoint>(lowPriorityTasks),
                    LineSmoothness = 0,
                    Fill = new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FF7cb8b8"))
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