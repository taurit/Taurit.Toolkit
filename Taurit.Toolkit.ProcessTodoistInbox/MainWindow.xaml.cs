using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using Newtonsoft.Json;
using Taurit.Toolkit.ProcessTodoistInbox.Models;
using Taurit.Toolkit.TodoistInboxHelper;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

namespace Taurit.Toolkit.ProcessTodoistInbox
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TodoistQueryService _todoistQueryService;

        public MainWindow()
        {
            InitializeComponent();
            this._todoistQueryService = new TodoistQueryService(UserSettings.TodoistApiKey);
        }

        public ObservableCollection<TaskActionModel> PlannedActions { get; set; }
        public ObservableCollection<TaskNoActionModel> SkippedTasks { get; set; }

        private SettingsFileModel UserSettings { get; set; }

        private void Window_Activated(Object sender, EventArgs e)
        {
            IList<TodoTask> tasksThatNeedReview = GetNotReviewedTasks(_todoistQueryService);
        }

        private IList<TodoTask> GetNotReviewedTasks(TodoistQueryService queryService)
        {
            IList<Project> projects = queryService.GetAllProjects();
            Project inboxProject = projects.Single(x => x.name == "Inbox");
            IList<TodoTask> allTasks = queryService.GetAllTasks();
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
    }
}