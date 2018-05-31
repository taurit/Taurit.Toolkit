using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using Newtonsoft.Json;
using Taurit.Toolkit.ProcessTodoistInbox.Models;
using Taurit.Toolkit.ProcessTodoistInbox.Services;
using Taurit.Toolkit.TodoistInboxHelper;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

namespace Taurit.Toolkit.ProcessTodoistInbox
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ITodoistQueryService _todoistQueryService;

        public MainWindow()
        {
            InitializeComponent();

#if DEBUG
            _todoistQueryService = new TodoistFakeQueryService();
#else
            _todoistQueryService = new TodoistQueryService(UserSettings.TodoistApiKey);
            #endif
        }

        public ObservableCollection<TaskActionModel> PlannedActions { get; set; } =
            new ObservableCollection<TaskActionModel>();

        public ObservableCollection<TaskNoActionModel> SkippedTasks { get; set; } =
            new ObservableCollection<TaskNoActionModel>();

        private SettingsFileModel UserSettings { get; set; }

        private void Window_Activated(Object sender, EventArgs e)
        {
            IReadOnlyList<Project> allProjects = _todoistQueryService.GetAllProjects();
            IReadOnlyList<Label> allLabels = _todoistQueryService.GetAllLabels();
            IReadOnlyList<TodoTask> tasksThatNeedReview = GetNotReviewedTasks(allProjects);

            var taskClassifier = new TaskClassifier(UserSettings.ClassificationRules, allLabels, allProjects);
            (IReadOnlyList<TaskActionModel> actions, IReadOnlyList<TaskNoActionModel> noActions) =
                taskClassifier.Classify(tasksThatNeedReview);

            foreach (TaskActionModel action in actions.OrderByDescending(x => x.Priority))
                PlannedActions.Add(action);

            foreach (TaskNoActionModel noAction in noActions)
                SkippedTasks.Add(noAction);
        }

        private IReadOnlyList<TodoTask> GetNotReviewedTasks(IReadOnlyList<Project> allProjects)
        {
            Project inboxProject = allProjects.Single(x => x.name == "Inbox");
            IReadOnlyList<TodoTask> allTasks = _todoistQueryService.GetAllTasks();
            List<TodoTask> tasksThatNeedProcessing = allTasks
                .Where(x => x.project_id == inboxProject.id &&
                            x.@checked == 0 &&
                            x.is_deleted == 0 &&
                            x.labels != null && x.labels.Count == 0 &&
                            x.priority == 1).ToList();
            return tasksThatNeedProcessing;
        }

        public void LoadSettings(String settingsFilePath)
        {
            String settingsFileContent = File.ReadAllText(settingsFilePath);
            UserSettings = JsonConvert.DeserializeObject<SettingsFileModel>(settingsFileContent);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}