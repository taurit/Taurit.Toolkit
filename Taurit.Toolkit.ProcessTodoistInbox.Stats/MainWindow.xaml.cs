using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using NaturalLanguageTimespanParser;
using Newtonsoft.Json;
using Taurit.Toolkit.ProcessTodoistInbox.Common.Models;
using Taurit.Toolkit.ProcessTodoistInbox.Common.Services;
using Taurit.Toolkit.ProcessTodoistInbox.Stats.Models;
using Taurit.Toolkit.ProcessTodoistInbox.Stats.Services;
using Taurit.Toolkit.TodoistInboxHelper;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

namespace Taurit.Toolkit.ProcessTodoistInbox.Stats
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly FileInboxesStatsReader _fileInboxesStatsReader;
        private readonly KindleMateStatsReader _kindleMateStatsReader;

        private readonly MultiCultureTimespanParser _mctp =
            new MultiCultureTimespanParser(new[]
            {
                new CultureInfo("pl"),
                new CultureInfo("en")
            });

        private readonly StatsAppSettings _settings;
        private readonly SnapshotReader _snapshotReader;
        private readonly TaskDateParser _taskDateParser;
        private readonly ITimeConverter _timeConverter;
        private readonly DateTime _tomorrowDateUtc;

        public MainWindow([JetBrains.Annotations.NotNull] String settingsFilePath)
        {
            WindowOpenedTime = DateTime.UtcNow;
            InitializeComponent();

            _settings = new StatsAppSettings(settingsFilePath);
            _snapshotReader = new SnapshotReader(_settings);
            _taskDateParser = new TaskDateParser();
            _tomorrowDateUtc = DateTime.UtcNow.Date.AddDays(1);
            _timeConverter = new TimeConverter();

            String kindleMateStatsPath = Path.Combine(_settings.KindleMateDirectory, "backlog-stats.csv");
            IEnumerable<String> kindleMateStatsCsvContent = File.ReadLines(kindleMateStatsPath);
            _kindleMateStatsReader = new KindleMateStatsReader(kindleMateStatsCsvContent);

            // todo un-hardcode path
            IEnumerable<String> fileInboxesStatsCsvContent =
                File.ReadLines("d:\\ProgramData\\ApplicationData\\TauritToolkit\\inboxFolderStats.csv");
            _fileInboxesStatsReader = new FileInboxesStatsReader(fileInboxesStatsCsvContent);
        }

        public Func<Double, String> XFormatter { get; } = value => new DateTime((Int64) value).ToString("yyyy-MM-dd");
        public Func<Double, String> YFormatter { get; } = value => $"{(Int64) value / 60d:0.00}";

        public SeriesCollection EstimatedTimeOfTasks { get; set; } = new SeriesCollection();

        public DateTime WindowOpenedTime { get; set; }
        public DateTime RenderFinishedTime { get; set; }

        private void RadioButtonSetupChanged([AllowNull] Object sender, [AllowNull] RoutedEventArgs e)
        {
            if (!IsInitialized) return;
            Boolean showFutureTasks = ShowFutureTasks.IsChecked ?? false;
            EstimatedTimeOfTasks.Clear();

            TimeSpan selectedTimePeriod = GetSelectedTimePeriod();
            var etUndefinedPriorityTasks = new List<DateTimePoint>();
            var etLowPriorityTasks = new List<DateTimePoint>();
            var etMediumPriorityTasks = new List<DateTimePoint>();
            var etHighPriorityTasks = new List<DateTimePoint>();
            var etKindleMateWords = new List<DateTimePoint>();
            var etKindleMateHighlights = new List<DateTimePoint>();
            var etAnkiFileInbox = new List<DateTimePoint>();
            var etWhitepapersFileInbox = new List<DateTimePoint>();
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

                TimeSpan kindleMateHighlightsEstimate =
                    _kindleMateStatsReader.GetEstimatedTimeNeededToProcessHighlight(snapshot.Time);
                TimeSpan kindleMateWordsEstimate =
                    _kindleMateStatsReader.GetEstimatedTimeNeededToProcessVocabularyWords(snapshot.Time);
                etKindleMateWords.Add(new DateTimePoint(snapshot.Time, kindleMateWordsEstimate.TotalMinutes));
                etKindleMateHighlights.Add(new DateTimePoint(snapshot.Time, kindleMateHighlightsEstimate.TotalMinutes));

                // todo: un-hardcode paths if problematic, maybe not worth the time...
                TimeSpan ankiFileInboxEstimate =
                    _fileInboxesStatsReader.GetEstimatedTimeNeededToProcessFolder("d:\\Inbox\\Do_anki\\", snapshot.Time,
                        30);
                TimeSpan whitepaperFileInboxEstimate =
                    _fileInboxesStatsReader.GetEstimatedTimeNeededToProcessFolder("d:\\Inbox\\Do_przeczytania\\",
                        snapshot.Time, 30);
                etAnkiFileInbox.Add(new DateTimePoint(snapshot.Time, ankiFileInboxEstimate.TotalMinutes));
                etWhitepapersFileInbox.Add(new DateTimePoint(snapshot.Time, whitepaperFileInboxEstimate.TotalMinutes));
            }

            StackedAreaSeries[] estimatedTimeOfTasksSeries = GetStackedSeries(
                etHighPriorityTasks,
                etMediumPriorityTasks,
                etLowPriorityTasks,
                etUndefinedPriorityTasks,
                etKindleMateWords,
                etKindleMateHighlights,
                etAnkiFileInbox,
                etWhitepapersFileInbox,
                etFutureTasks);
            EstimatedTimeOfTasks.AddRange(estimatedTimeOfTasksSeries);

            DateTime lastKnownDate = etLowPriorityTasks.Max(x => x.DateTime);

            // at what speed do I need to work NOW to achieve inbox zero at the end of quarter?
            DateTime firstDayOfQuarter = SnapshotOnTimeline.GetLastDayOfQuarter(lastKnownDate).AddDays(1).AddMonths(-3);
            DateTime lastDayOfQuarter = SnapshotOnTimeline.GetLastDayOfQuarter(firstDayOfQuarter);

            Int32 howManyDaysToEndOfQuarter = lastDayOfQuarter.Subtract(lastKnownDate).Days;
            Double totalMinutesNow = CountTotalMinutesAtDate(lastKnownDate,
                etLowPriorityTasks,
                etMediumPriorityTasks,
                etHighPriorityTasks,
                etKindleMateWords,
                etKindleMateHighlights,
                etAnkiFileInbox,
                etWhitepapersFileInbox);

            if (howManyDaysToEndOfQuarter == 0)
            {
                // fix division by zero on the last day of a quarter
                howManyDaysToEndOfQuarter = 1;
            }

            Double howManyMinutesNeedsToBeDoneInADayForCleanBacklog = totalMinutesNow / howManyDaysToEndOfQuarter;

            UpdateLabelsValues(totalMinutesNow, lastKnownDate, howManyMinutesNeedsToBeDoneInADayForCleanBacklog);

            // update the cache
            File.WriteAllText(cacheFileName, JsonConvert.SerializeObject(cache));
        }

        private void UpdateLabelsValues(
            Double mostRecentTotalBacklogTimeEstimateInMinutes,
            DateTime lastKnownDate,
            Double howManyMinutesNeedsToBeDoneInADayForCleanBacklog
        )
        {
            TimeSpan totalWorkLeft = TimeSpan.FromMinutes(mostRecentTotalBacklogTimeEstimateInMinutes);
            TimeSpan dailyWorkNeededToCleanBacklogThisQuarter =
                TimeSpan.FromMinutes(howManyMinutesNeedsToBeDoneInADayForCleanBacklog);

            const String unitOfWorkSymbol = "h";
            const Int32 unitOfWorkDurationInMinutes = 60;

            TotalWorkLeft.Text =
                $"{_timeConverter.ConvertToUnitsOfWorkAndCeil(totalWorkLeft, unitOfWorkDurationInMinutes)} {unitOfWorkSymbol}";
            MostRecentSnapshotTime.Text = lastKnownDate.ToString("yyyy-MM-dd HH:mm");
            BurndownSpeed.Text = _timeConverter.ConvertToUnitsOfWork(dailyWorkNeededToCleanBacklogThisQuarter)
                .ToString("0.00");
        }

        private static Double CountTotalMinutesAtDate(
            DateTime date,
            List<DateTimePoint> etLowPriorityTasks,
            List<DateTimePoint> etMediumPriorityTasks,
            List<DateTimePoint> etHighPriorityTasks,
            List<DateTimePoint> etKindleMateWords,
            List<DateTimePoint> etKindleMateHighlights,
            List<DateTimePoint> etAnkiFileInbox,
            List<DateTimePoint> etWhitepapersFileInbox
        )
        {
            Double lowTime = etLowPriorityTasks.Single(x => x.DateTime == date).Value;
            Double mediumTime = etMediumPriorityTasks.Single(x => x.DateTime == date).Value;
            Double highTime = etHighPriorityTasks.Single(x => x.DateTime == date).Value;
            Double wordsTime = etKindleMateWords.Single(x => x.DateTime == date).Value;
            Double highlightsTime = etKindleMateHighlights.Single(x => x.DateTime == date).Value;
            Double ankiTime = etAnkiFileInbox.Single(x => x.DateTime == date).Value;
            Double whitepapersTime = etWhitepapersFileInbox.Single(x => x.DateTime == date).Value;

            return lowTime + mediumTime + highTime + wordsTime + highlightsTime + ankiTime + whitepapersTime;
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
            DateTime? taskDate = GetTaskDate(todoTask);
            return taskDate.HasValue && taskDate >= snapshotDate && taskDate <= endOfQuarter;
        }

        private DateTime? GetTaskDate(TodoTask todoTask)
        {
            // for tasks up to API v7
#pragma warning disable 618
            DateTime? taskDate = _taskDateParser.TryParse(todoTask.due_date_utc);
#pragma warning restore 618

            if (taskDate == null)
            {
                // for tasks since API v8 - now the date is stored in Due object
                taskDate = _taskDateParser.TryParse(todoTask.due?.date);
            }

            return taskDate;
        }

        private Boolean IsScheduledForTodayOrOverdue(TodoTask todoTask)
        {
            DateTime? taskDate = GetTaskDate(todoTask);
            return taskDate.HasValue && taskDate < _tomorrowDateUtc;
        }

        private Double GetTimeInMinutes(String content)
        {
            // workaround: I sometimes use phrases like "Consider the 24h timeout", where 24h means a full day. I never estimate tasks so high, so I don't want such strings to be interpreted as estimates.

            var contentNormalied = content
                .Replace("24h", "")
                .Replace("24 h", "")
                .Replace("48h", "")
                .Replace("48 h", "")
                .Replace("72h", "")
                .Replace("72 h", "");

            TimespanParseResult parseResult = _mctp.Parse(contentNormalied);
            return parseResult.Success ? parseResult.Duration.TotalMinutes : 0d;
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        private static StackedAreaSeries[] GetStackedSeries(
            List<DateTimePoint> highPriorityTasks,
            List<DateTimePoint> mediumPriorityTasks,
            List<DateTimePoint> lowPriorityTasks,
            List<DateTimePoint> undefinedPriorityTasks,
            List<DateTimePoint> kindleMateWords,
            List<DateTimePoint> kindleMateHighlights,
            List<DateTimePoint> anki,
            List<DateTimePoint> whitepapers,
            List<DateTimePoint> futureTasks)
        {
            var series = new[]
            {
                new StackedAreaSeries
                {
                    Title = "Kindle Vocabulary",
                    Values = new ChartValues<DateTimePoint>(kindleMateWords),
                    LineSmoothness = 0,
                    Fill = new SolidColorBrush(Color.FromRgb(48, 92, 211))
                },
                new StackedAreaSeries
                {
                    Title = "Q&A input to put to Anki",
                    Values = new ChartValues<DateTimePoint>(anki),
                    LineSmoothness = 0,
                    Fill = new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FF3D67D3"))
                },
                new StackedAreaSeries
                {
                    Title = "Whitepaper files to read",
                    Values = new ChartValues<DateTimePoint>(whitepapers),
                    LineSmoothness = 0,
                    Fill = new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FF4073D6"))
                },
                new StackedAreaSeries
                {
                    Title = "Todoist: Least urgent",
                    Values = new ChartValues<DateTimePoint>(lowPriorityTasks),
                    LineSmoothness = 0,
                    Fill = new SolidColorBrush((Color) ColorConverter.ConvertFromString("#557ED1"))
                },
                new StackedAreaSeries
                {
                    Title = "Kindle Highlights",
                    Values = new ChartValues<DateTimePoint>(kindleMateHighlights),
                    LineSmoothness = 0,
                    Fill = new SolidColorBrush(Color.FromRgb(242, 184, 24))
                },
                new StackedAreaSeries
                {
                    Title = "Todoist: Relatively urgent",
                    Values = new ChartValues<DateTimePoint>(mediumPriorityTasks),
                    LineSmoothness = 0,
                    Fill = new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FFF49C18"))
                },
                new StackedAreaSeries
                {
                    Title = "Todoist: Urgent",
                    Values = new ChartValues<DateTimePoint>(highPriorityTasks),
                    LineSmoothness = 0,
                    Fill = new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FFDE4C4A"))
                },
                new StackedAreaSeries
                {
                    Title = "Future tasks",
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
                TimeAllTime,
                TimeLastQuarter,
                TimeLastWeek
            };
            RadioButton selectedTimeCheckbox = timePeriodCheckboxes.Single(x => x.IsChecked == true);
            var timeInDays = Convert.ToInt32((String) selectedTimeCheckbox.Tag);
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
            Debug.Assert(Dispatcher != null, nameof(Dispatcher) + " != null");
            Dispatcher.BeginInvoke(new Action(() =>
            {
                DateTime renderFinishedTime = DateTime.UtcNow;
                RenderFinishedTime = renderFinishedTime;

                // how long did it take to render the window completely?
                TimeSpan timeDifference = RenderFinishedTime.Subtract(WindowOpenedTime);

                Title += $" [Render time = {timeDifference.TotalMilliseconds} ms]";
            }), DispatcherPriority.ContextIdle, null);
        }
    }
}