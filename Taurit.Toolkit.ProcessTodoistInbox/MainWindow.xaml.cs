using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Taurit.Toolkit.ProcessTodoistInbox.Common.Services;
using Taurit.Toolkit.ProcessTodoistInbox.UI.Services;
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
        [NotNull] private readonly BacklogSnapshotCreator _backlogSnapshotCreator;

        [NotNull] private readonly FilteredTaskAccessor _filteredTaskAccessor;

        [NotNull] private readonly ITodoistQueryService _todoistQueryService;

        public MainWindow([NotNull] String settingsFilePath, [NotNull] String snapshotRootDirectory)
        {
            if (settingsFilePath == null) throw new ArgumentNullException(nameof(settingsFilePath));
            if (snapshotRootDirectory == null) throw new ArgumentNullException(nameof(snapshotRootDirectory));

            String settingsFileContent = File.ReadAllText(settingsFilePath);

            _backlogSnapshotCreator = new BacklogSnapshotCreator();
            String snapshotFilePath = _backlogSnapshotCreator.GetNewestSnapshot(snapshotRootDirectory);

            String tasksSnapshotFileContent = File.ReadAllText($"{snapshotFilePath}.tasks");
            String projectsSnapshotFileContent = File.ReadAllText($"{snapshotFilePath}.projects");
            String labelsSnapshotFileContent = File.ReadAllText($"{snapshotFilePath}.labels");

            UserSettings = JsonConvert.DeserializeObject<SettingsFileModel>(settingsFileContent) ??
                           throw new InvalidOperationException("Settings file as invalid contents");

            _todoistQueryService = new TodoistSnapshotQueryService(tasksSnapshotFileContent,
                projectsSnapshotFileContent, labelsSnapshotFileContent);

            _filteredTaskAccessor = new FilteredTaskAccessor();
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
            var alternativeInboxes = UserSettings.AlternativeInboxes;

            var taskClassifier = new TaskClassifier(UserSettings.ClassificationRules,
                UserSettings.ClassificationRulesConcise, allLabels, allProjects, alternativeInboxes);
            (IReadOnlyList<TaskActionModel> actions, IReadOnlyList<TaskNoActionModel> noActions) =
                taskClassifier.Classify(tasksThatNeedReview);

            foreach (TaskActionModel action in actions.OrderByDescending(x => x.Priority))
                PlannedActions.Add(action);

            foreach (TaskNoActionModel noAction in noActions)
                SkippedTasks.Add(noAction);
        }
    }
}