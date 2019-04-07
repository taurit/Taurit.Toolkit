using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using JetBrains.Annotations;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using NaturalLanguageTimespanParser;
using Newtonsoft.Json;
using Taurit.Toolkit.ProcessTodoistInbox.Stats.Models;
using Taurit.Toolkit.ProcessTodoistInbox.Stats.Services;
using Taurit.Toolkit.TodoistInboxHelper;
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
        private readonly TaskDateParser _taskDateParser;
        private readonly DateTime _tomorrowDateUtc;

        public MainWindow([NotNull] String settingsFilePath)
        {
            WindowOpenedTime = DateTime.UtcNow;
            InitializeComponent();

            _settings = new StatsAppSettings(settingsFilePath);
            _snapshotReader = new SnapshotReader(_settings);
            _taskDateParser = new TaskDateParser();
            _tomorrowDateUtc = DateTime.UtcNow.Date.AddDays(1);
        }

        public Func<Double, String> XFormatter { get; } = value => new DateTime((Int64) value).ToString("yyyy-MM-dd");
        public Func<Double, String> YFormatter { get; } = value => $"{(Int64) value / 60d:0.00}";

        public SeriesCollection EstimatedTimeOfTasks { get; set; } = new SeriesCollection();

        public DateTime WindowOpenedTime { get; set; }
        public DateTime RenderFinishedTime { get; set; }

        private void RadioButtonSetupChanged([CanBeNull] Object sender, [CanBeNull] RoutedEventArgs e)
        {
            if (!IsInitialized) return;
            Boolean showFutureTasks = ShowFutureTasks.IsChecked ?? false;
            EstimatedTimeOfTasks.Clear();

            TimeSpan selectedTimePeriod = GetSelectedTimePeriod();
            var etUndefinedPriorityTasks = new List<DateTimePoint>();
            var etLowPriorityTasks = new List<DateTimePoint>();
            var etMediumPriorityTasks = new List<DateTimePoint>();
            var etHighPriorityTasks = new List<DateTimePoint>();
            var etFutureTasks = new List<DateTimePoint>();

            var cacheFileName = "stats-cache.tmp.json";
            Dictionary<DateTime, SnapshotCache> cache = File.Exists(cacheFileName)
                ? JsonConvert.DeserializeObject<Dictionary<DateTime, SnapshotCache>>(File.ReadAllText(cacheFileName))
                : new Dictionary<DateTime, SnapshotCache>();

            List<SnapshotOnTimeline> snapshotsInSelectedTimePeriod =
                _snapshotReader.Read(_settings.SnapshotsRootFolderPath, DateTime.UtcNow, selectedTimePeriod,
                    cache.Select(x => x.Key).ToHashSet());

            foreach (SnapshotOnTimeline snapshot in snapshotsInSelectedTimePeriod)
            {
                if (!cache.ContainsKey(snapshot.Time))
                {
                    List<TodoTask> taskToCount =
                        FilterOutTaskThatShouldNotBeCounted(snapshot.AllTasks, snapshot.AllProjects, snapshot.Time,
                            snapshot.EndOfQuarter);

                    IEnumerable<TodoTask> priority1Tasks = taskToCount.Where(x => x.priority == 1).ToList();
                    IEnumerable<TodoTask> priority2Tasks = taskToCount.Where(x => x.priority == 2).ToList();
                    IEnumerable<TodoTask> priority3Tasks = taskToCount.Where(x => x.priority == 3).ToList();
                    IEnumerable<TodoTask> priority4Tasks = taskToCount.Where(x => x.priority == 4).ToList();
                    IEnumerable<TodoTask> futureTasks = taskToCount.Where(x => x.priority == 0).ToList();

                    Double estTimeUndefinedPriorityTasks = priority1Tasks.Sum(x => GetTimeInMinutes(x.content));
                    Double estTimeLowPriorityTasks = priority2Tasks.Sum(x => GetTimeInMinutes(x.content));
                    Double estTimeMediumPriorityTasks = priority3Tasks.Sum(x => GetTimeInMinutes(x.content));
                    Double estTimeHighPriorityTasks = priority4Tasks.Sum(x => GetTimeInMinutes(x.content));
                    Double estTimeFutureTasks = futureTasks.Sum(x => GetTimeInMinutes(x.content));

                    cache.Add(
                        snapshot.Time,
                        new SnapshotCache(
                            estTimeUndefinedPriorityTasks,
                            estTimeLowPriorityTasks,
                            estTimeMediumPriorityTasks,
                            estTimeHighPriorityTasks,
                            estTimeFutureTasks
                        )
                    );
                }

                SnapshotCache stats = cache[snapshot.Time];

                etUndefinedPriorityTasks.Add(new DateTimePoint(snapshot.Time, stats.EstTimeUndefinedPriorityTasks));
                etLowPriorityTasks.Add(new DateTimePoint(snapshot.Time, stats.EstTimeLowPriorityTasks));
                etMediumPriorityTasks.Add(new DateTimePoint(snapshot.Time, stats.EstTimeMediumPriorityTasks));
                etHighPriorityTasks.Add(new DateTimePoint(snapshot.Time, stats.EstTimeHighPriorityTasks));
                if (showFutureTasks) etFutureTasks.Add(new DateTimePoint(snapshot.Time, stats.EstTimeFutureTasks));
            }

            StackedAreaSeries[] estimatedTimeOfTasksSeries = GetStackedSeries(
                etHighPriorityTasks,
                etMediumPriorityTasks,
                etLowPriorityTasks,
                etUndefinedPriorityTasks,
                etFutureTasks);
            EstimatedTimeOfTasks.AddRange(estimatedTimeOfTasksSeries);

            // draw the optimum trend
            DateTime lastKnownDate = etLowPriorityTasks.Max(x => x.DateTime);
            var totalMinutes = etLowPriorityTasks.Single(x => x.DateTime == lastKnownDate).Value +
                               etMediumPriorityTasks.Single(x => x.DateTime == lastKnownDate).Value +
                               etHighPriorityTasks.Single(x => x.DateTime == lastKnownDate).Value;

            DateTime lastDayOfQuarter = SnapshotOnTimeline.GetLastDayOfQuarter(lastKnownDate);
            var howManyDaysToEndOfQuarter = lastDayOfQuarter.Subtract(lastKnownDate).Days;

            Double howManyMinutesNeedsToBeDoneInADayForCleanBacklog = totalMinutes / howManyDaysToEndOfQuarter;
            BurndownSpeed.Text = howManyMinutesNeedsToBeDoneInADayForCleanBacklog.ToString("0");
            Int32 howManyDaysToDraw = selectedTimePeriod.Days < 8 ? 3 : 7;

            var optimumTrendPoints = new ChartValues<DateTimePoint>();
            var currentHours = totalMinutes;
            var estimatedMinutes = currentHours;

            for (var day = 0; day < howManyDaysToDraw; day++)
            {
                optimumTrendPoints.Add(new DateTimePoint(lastKnownDate.AddDays(day), estimatedMinutes));
                estimatedMinutes = estimatedMinutes - howManyMinutesNeedsToBeDoneInADayForCleanBacklog;
            }

            var lineSeries = new LineSeries
            {
                Values = optimumTrendPoints,
                Fill = Brushes.Transparent,
                Stroke = Brushes.Gray,
                StrokeDashArray = new DoubleCollection {2},
                PointGeometry = DefaultGeometries.Cross,
                StrokeThickness = 1
                
            };
            EstimatedTimeOfTasks.Add(lineSeries);

            File.WriteAllText(cacheFileName, JsonConvert.SerializeObject(cache));
        }



        private List<TodoTask> FilterOutTaskThatShouldNotBeCounted(IReadOnlyList<TodoTask> allTasks,
            IReadOnlyList<Project> allProjects, DateTime snapshotTime, DateTime endOfQuarter)
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
            ILookup<Int32, Project> projectsByItemOrder = allProjects.ToLookup(x => x.item_order);
            foreach (Project ignoredProject in ignoredProjects)
            {
                Int32 ignoredProjectOrder = ignoredProject.item_order;
                for (Int32 i = ignoredProjectOrder + 1; i <= maxProjectOrder; i++)
                {
                    Project project = projectsByItemOrder[i].Single();
                    if (project.indent > ignoredProject.indent)
                        ignoredSubProjects.Add(project);
                    else
                        break;
                }
            }

            // list projects to ignore
            ImmutableHashSet<Int64> ignoredProjectsIds = ignoredProjects.Select(x => x.id)
                .Union(ignoredSubProjects.Select(x => x.id))
                .ToImmutableHashSet();

            // quick hack: mark future tasks as "priority 0"
            foreach (TodoTask task in allTasks.Where(x => IsFutureTaskWithDefinedDueDate(x, snapshotTime, endOfQuarter))
            )
                task.priority = 0;

            List<TodoTask> relevantTasks = allTasks
                .Where(x => !x.HasDate || x.priority == 0 || IsScheduledForTodayOrOverdue(x))
                .Where(x => !ignoredProjectsIds.Contains(x.project_id))
                .ToList();

            return relevantTasks;
        }

        private Boolean IsFutureTaskWithDefinedDueDate(TodoTask todoTask, DateTime snapshotDate, DateTime endOfQuarter)
        {
            DateTime? taskDate = _taskDateParser.TryParse(todoTask.due_date_utc);
            return taskDate.HasValue && taskDate >= snapshotDate && taskDate <= endOfQuarter;
        }

        private Boolean IsScheduledForTodayOrOverdue(TodoTask todoTask)
        {
            DateTime? taskDate = _taskDateParser.TryParse(todoTask.due_date_utc);
            return taskDate.HasValue && taskDate < _tomorrowDateUtc;
        }

        private Double GetTimeInMinutes(String content)
        {
            TimespanParseResult parseResult = _mctp.Parse(content);
            return parseResult.Success ? parseResult.Duration.TotalMinutes : 0d;
        }

        private static StackedAreaSeries[] GetStackedSeries(
            List<DateTimePoint> highPriorityTasks,
            List<DateTimePoint> mediumPriorityTasks,
            List<DateTimePoint> lowPriorityTasks,
            List<DateTimePoint> undefinedPriorityTasks,
            List<DateTimePoint> futureTasks)
        {
            var series = new[]
            {
                new StackedAreaSeries
                {
                    Title = "Low priority",
                    Values = new ChartValues<DateTimePoint>(lowPriorityTasks),
                    LineSmoothness = 0,
                    Fill = new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FF4073D6"))
                },
                new StackedAreaSeries
                {
                    Title = "Medium priority",
                    Values = new ChartValues<DateTimePoint>(mediumPriorityTasks),
                    LineSmoothness = 0,
                    Fill = new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FFF49C18"))
                },
                new StackedAreaSeries
                {
                    Title = "High priority",
                    Values = new ChartValues<DateTimePoint>(highPriorityTasks),
                    LineSmoothness = 0,
                    Fill = new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FFDE4C4A"))
                },
                new StackedAreaSeries
                {
                    Title = "Future",
                    Values = new ChartValues<DateTimePoint>(futureTasks),
                    LineSmoothness = 0,
                    Fill = new SolidColorBrush(Color.FromRgb(255, 204, 255))
                },
                new StackedAreaSeries
                {
                    Title = "Undefined priority",
                    Values = new ChartValues<DateTimePoint>(undefinedPriorityTasks),
                    LineSmoothness = 0,
                    Fill = new SolidColorBrush(Color.FromRgb(235, 235, 235))
                }
            };
            return series.Where(x => x.Values.Count > 0)
                .ToArray();
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

        private void ToggleButton_OnChecked(Object sender, RoutedEventArgs e)
        {
            RadioButtonSetupChanged(null, null);
        }

        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                DateTime renderFinishedTime = DateTime.UtcNow;
                RenderFinishedTime = renderFinishedTime;

                // how long did it take to render the window completely?
                TimeSpan timeDifference = RenderFinishedTime.Subtract(WindowOpenedTime);

                RenderTimeString.Text = $"{timeDifference.TotalMilliseconds} ms";
            }), DispatcherPriority.ContextIdle, null);
        }
    }
}