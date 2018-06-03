using System;
using System.Collections.Generic;
using System.Linq;
using Taurit.Toolkit.TodoistInboxHelper;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

namespace Taurit.Toolkit.ProcessTodoistInbox.Services
{
    internal class TodoistFakeQueryService : ITodoistQueryService
    {
        private const Int32 InboxProjectId = 1;

        /// <inheritdoc />
        public IReadOnlyList<Label> GetAllLabels()
        {
            return new List<Label>
            {
                new Label
                {
                    is_deleted = 0,
                    id = 1,
                    name = "home"
                },
                new Label
                {
                    is_deleted = 0,
                    id = 2,
                    name = "work"
                },
                new Label
                {
                    is_deleted = 0,
                    id = 3,
                    name = "shop"
                },
                new Label
                {
                    is_deleted = 1,
                    id = 4,
                    name = "gym"
                }
            };
        }

        /// <inheritdoc />
        public IReadOnlyList<Project> GetAllProjects()
        {
            return new List<Project>
            {
                new Project
                {
                    is_deleted = 0,
                    id = InboxProjectId,
                    is_archived = 0,
                    name = "Inbox"
                },
                new Project
                {
                    is_deleted = 0,
                    id = 2,
                    is_archived = 0,
                    name = "Obowiązki"
                }
            };
        }

        /// <param name="allProjects"></param>
        /// <inheritdoc />
        public IReadOnlyList<TodoTask> GetAllTasks(ILookup<Int64, Project> allProjectsIndexedById)
        {
            return new List<TodoTask>
            {
                new TodoTask
                {
                    id = 123,
                    priority = 1,
                    content = "Buy milk",
                    is_deleted = 0,
                    labels = new List<Int64>(),
                    project_id = InboxProjectId
                },
                new TodoTask
                {
                    id = 123,
                    priority = 1,
                    content = "Learn WPF basics",
                    is_deleted = 0,
                    labels = new List<Int64>(),
                    project_id = InboxProjectId
                }
            };
        }
    }
}