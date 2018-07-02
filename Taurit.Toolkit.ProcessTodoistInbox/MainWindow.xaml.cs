using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using Newtonsoft.Json;
using Taurit.Toolkit.ProcessTodoistInbox.Services;
using Taurit.Toolkit.ProcesTodoistInbox.Common.Models;
using Taurit.Toolkit.ProcesTodoistInbox.Common.Services;
using Taurit.Toolkit.TodoistInboxHelper;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

namespace Taurit.Toolkit.ProcessTodoistInbox
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ITodoistCommandService _todoistCommandService;
        private readonly ITodoistQueryService _todoistQueryService;
        private readonly FilteredTaskAccessor _filteredTaskAccessor;
        private readonly ChangeExecutor _changeExecutor;

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(String settingsFilePath)
        {
            String settingsFileContent = File.ReadAllText(settingsFilePath);
            UserSettings = JsonConvert.DeserializeObject<SettingsFileModel>(settingsFileContent);

#if DEBUG
            _todoistQueryService = new TodoistFakeQueryService();
            _todoistCommandService = new TodoistFakeCommandService();
#else
            _todoistQueryService = new TodoistQueryService(UserSettings.TodoistApiKey);
            _todoistCommandService = new TodoistCommandService(UserSettings.TodoistApiKey);
#endif

            _filteredTaskAccessor = new FilteredTaskAccessor(_todoistQueryService);
            _changeExecutor = new ChangeExecutor(_todoistCommandService);

            InitializeComponent();
        }

        public ObservableCollection<TaskActionModel> PlannedActions { get; set; } =
            new ObservableCollection<TaskActionModel>();

        public ObservableCollection<TaskNoActionModel> SkippedTasks { get; set; } =
            new ObservableCollection<TaskNoActionModel>();

        private SettingsFileModel UserSettings { get; }

        private void Initialize(Object sender, EventArgs e)
        {
            IReadOnlyList<Project> allProjects = _todoistQueryService.GetAllProjects();
            IReadOnlyList<Label> allLabels = _todoistQueryService.GetAllLabels();
            IReadOnlyList<TodoTask> tasksThatNeedReview = _filteredTaskAccessor.GetNotReviewedTasks(allProjects.ToLookup(x => x.id));

            var taskClassifier = new TaskClassifier(UserSettings.ClassificationRules, UserSettings.ClassificationRulesConcise, allLabels, allProjects);
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