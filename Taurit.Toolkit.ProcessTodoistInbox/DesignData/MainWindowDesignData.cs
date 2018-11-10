using System.Collections.ObjectModel;
using Taurit.Toolkit.ProcesTodoistInbox.Common.Models;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

#pragma warning disable 618

namespace Taurit.Toolkit.ProcessTodoistInbox.UI.DesignData
{
    public class MainWindowDesignData
    {
        public ObservableCollection<TaskActionModel> PlannedActions =>
            new ObservableCollection<TaskActionModel>
            {
                new TaskActionModel
                {
                    Name = "Task with a super long long long long long long long long name",
                    Project = new Project {name = "Obowiązki"},
                    Label = new Label {name = "anki"},
                    Priority = 2
                },
                new TaskActionModel
                {
                    Name = "Task with a typical name",
                    Project = new Project {name = "Obowiązki"},
                    Label = new Label {name = "anki"},
                    Priority = 2
                },
                new TaskActionModel
                {
                    Name = "Task with a typical name 2",
                    Project = new Project {name = "Praca"},
                    Label = new Label {name = "biuro"},
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