using System.Collections.ObjectModel;
using Taurit.Toolkit.ProcessTodoistInbox.Models;

namespace Taurit.Toolkit.ProcessTodoistInbox.DesignData
{
    public class MainWindowDesignData
    {
        public ObservableCollection<TaskActionModel> PlannedActions =>
            new ObservableCollection<TaskActionModel>
            {
                new TaskActionModel
                {
                    Name = "Task with a super long long long long long long long long name",
                    Project = "Obowiązki",
                    Label = "anki",
                    Priority = 2
                },
                new TaskActionModel
                {
                    Name = "Task with a typical name",
                    Project = "Obowiązki",
                    Label = "anki",
                    Priority = 2
                },
                new TaskActionModel
                {
                    Name = "Task with a typical name 2",
                    Project = "Praca",
                    Label = "biuro",
                    Priority = 4
                }
            };

        public ObservableCollection<TaskNoActionModel> SkippedTasks =>
            new ObservableCollection<TaskNoActionModel>
            {
                new TaskNoActionModel {Name = "Yo dawg"}
            };
    }
}