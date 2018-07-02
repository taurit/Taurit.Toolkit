using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using Newtonsoft.Json;
using Taurit.Toolkit.ProcessTodoistInbox.Models;
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
            IReadOnlyList<TodoTask> tasksThatNeedReview = GetNotReviewedTasks(allProjects.ToLookup(x => x.id));

            var taskClassifier = new TaskClassifier(UserSettings.ClassificationRules, allLabels, allProjects);
            (IReadOnlyList<TaskActionModel> actions, IReadOnlyList<TaskNoActionModel> noActions) =
                taskClassifier.Classify(tasksThatNeedReview);

            foreach (TaskActionModel action in actions.OrderByDescending(x => x.Priority))
                PlannedActions.Add(action);

            foreach (TaskNoActionModel noAction in noActions)
                SkippedTasks.Add(noAction);
        }

        private IReadOnlyList<TodoTask> GetNotReviewedTasks(ILookup<Int64, Project> allProjectsIndexedById)
        {
            IReadOnlyList<TodoTask> allTasks = _todoistQueryService.GetAllTasks(allProjectsIndexedById);
            List<TodoTask> tasksThatNeedProcessing = allTasks
                .Where(x => x.@checked == 0 &&
                            x.is_deleted == 0 &&
                            x.labels != null).ToList();
            return tasksThatNeedProcessing;
        }


        private void ProceedButton_Click(Object sender, RoutedEventArgs e)
        {
            foreach (TaskActionModel action in PlannedActions)
            {
                Int64 taskId = action.TaskId;
                Int32? newPriority = action.Priority;
                Int64? newLabelId = action.Label?.id;
                Int64 oldProjectId = action.OldProjectId;
                Int64? newProjectId = action.Project?.id;

                _todoistCommandService.AddUpdateProjectCommand(taskId, oldProjectId, newProjectId);
                _todoistCommandService.AddUpdateLabelCommand(taskId, newLabelId);
                _todoistCommandService.AddUpdatePriorityCommand(taskId, newPriority);

                //_todoistCommandService.AddUpdateTaskCommand(oldProjectId, taskId, priority, label, newProjectId);
            }

            String response = _todoistCommandService.ExecuteCommands();

            ProceedButton.IsEnabled = false;
        }
    }
}