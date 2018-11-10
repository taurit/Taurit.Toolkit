using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Taurit.Toolkit.ProcesTodoistInbox.Common.Models;
using Taurit.Toolkit.ProcesTodoistInbox.Common.Services;
using Taurit.Toolkit.TodoistInboxHelper;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

namespace Taurit.Toolkit.ProcessTodoistInbox.UI
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [NotNull] private readonly ChangeExecutor _changeExecutor;

        [NotNull] private readonly FilteredTaskAccessor _filteredTaskAccessor;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        [NotNull] private readonly ITodoistCommandService _todoistCommandService;

        [NotNull] private readonly ITodoistQueryService _todoistQueryService;

        public MainWindow([NotNull] String settingsFilePath)
        {
            if (settingsFilePath == null) throw new ArgumentNullException(nameof(settingsFilePath));

            String settingsFileContent = File.ReadAllText(settingsFilePath);
            UserSettings = JsonConvert.DeserializeObject<SettingsFileModel>(settingsFileContent) ??
                           throw new InvalidOperationException("Settings file as invalid contents");

#if !DEBUG
            _todoistQueryService = new TodoistFakeQueryService();
            _todoistCommandService = new TodoistFakeCommandService();
#else
            _todoistQueryService = new TodoistQueryService(UserSettings.TodoistApiKey);
            _todoistCommandService = new TodoistCommandService(UserSettings.TodoistApiKey);
#endif

            _filteredTaskAccessor = new FilteredTaskAccessor();
            _changeExecutor = new ChangeExecutor(_todoistCommandService);

            InitializeComponent();
        }

        [NotNull]
        public ObservableCollection<TaskActionModel> PlannedActions { get; set; } =
            new ObservableCollection<TaskActionModel>();

        [NotNull]
        public ObservableCollection<TaskNoActionModel> SkippedTasks { get; set; } =
            new ObservableCollection<TaskNoActionModel>();

        [NotNull]
        private SettingsFileModel UserSettings { get; }

        private void Initialize(Object sender, EventArgs e)
        {
            IReadOnlyList<Project> allProjects = _todoistQueryService.GetAllProjects();
            IReadOnlyList<Label> allLabels = _todoistQueryService.GetAllLabels();
            IReadOnlyList<TodoTask> allTasks = _todoistQueryService.GetAllTasks(allProjects.ToLookup(x => x.id));
            IReadOnlyList<TodoTask> tasksThatNeedReview = _filteredTaskAccessor.GetNotReviewedTasks(allTasks);

            var taskClassifier = new TaskClassifier(UserSettings.ClassificationRules,
                UserSettings.ClassificationRulesConcise, allLabels, allProjects);
            (IReadOnlyList<TaskActionModel> actions, IReadOnlyList<TaskNoActionModel> noActions) =
                taskClassifier.Classify(tasksThatNeedReview);

            foreach (TaskActionModel action in actions.OrderByDescending(x => x.Priority))
                PlannedActions.Add(action);

            foreach (TaskNoActionModel noAction in noActions)
                SkippedTasks.Add(noAction);
        }

        private void ProceedButton_Click(Object sender, RoutedEventArgs e)
        {
            _changeExecutor.ApplyPlan(PlannedActions);
            ProceedButton.IsEnabled = false;
        }
    }
}