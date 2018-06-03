using System;
using System.Collections.Generic;
using System.Linq;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

namespace Taurit.Toolkit.TodoistInboxHelper
{
    public interface ITodoistQueryService
    {
        IReadOnlyList<Label> GetAllLabels();
        IReadOnlyList<Project> GetAllProjects();
        IReadOnlyList<TodoTask> GetAllTasks(ILookup<Int64, Project> allProjectsIndexedById);
    }
}