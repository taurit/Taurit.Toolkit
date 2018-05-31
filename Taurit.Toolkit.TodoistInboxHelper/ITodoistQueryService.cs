using System.Collections.Generic;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

namespace Taurit.Toolkit.TodoistInboxHelper
{
    public interface ITodoistQueryService
    {
        IReadOnlyList<Label> GetAllLabels();
        IReadOnlyList<Project> GetAllProjects();
        IReadOnlyList<TodoTask> GetAllTasks();
    }
}