using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

namespace Taurit.Toolkit.TodoistInboxHelper
{
    public interface ITodoistQueryService
    {
        [NotNull]
        IReadOnlyList<Label> GetAllLabels();

        [NotNull]
        IReadOnlyList<Project> GetAllProjects();

        [NotNull]
        [ItemNotNull]
        IReadOnlyList<TodoTask> GetAllTasks(
            ILookup<Int64, Project> allProjectsIndexedById,
            ILookup<Int64, Label> allLabelsIndexedById
        );
    }
}