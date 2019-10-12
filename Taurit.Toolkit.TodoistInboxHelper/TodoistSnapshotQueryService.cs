using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;
using Utf8Json;

namespace Taurit.Toolkit.TodoistInboxHelper
{
    public class TodoistSnapshotQueryService : ITodoistQueryService
    {
        private readonly List<Label> _allLabels;
        private readonly List<Project> _allProjects;
        private readonly List<TodoTask> _allTasks;

        public TodoistSnapshotQueryService(
            [NotNull] String tasksJson,
            [NotNull] String projectsJson,
            [NotNull] String labelsJson)
        {
            _allTasks = JsonSerializer.Deserialize<List<TodoTask>>(tasksJson);
            _allProjects = JsonSerializer.Deserialize<List<Project>>(projectsJson);
            _allLabels = JsonSerializer.Deserialize<List<Label>>(labelsJson);
        }

        public IReadOnlyList<Label> GetAllLabels()
        {
            return _allLabels;
        }

        public IReadOnlyList<Project> GetAllProjects()
        {
            return _allProjects;
        }


        public IReadOnlyList<TodoTask> GetAllTasks(ILookup<Int64, Project> allProjectsIndexedById,
            ILookup<Int64, Label> allLabelsIndexedById)
        {
            return _allTasks;
        }
    }
}