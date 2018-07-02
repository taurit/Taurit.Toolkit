using System;
using System.Collections.Generic;
using System.Linq;
using Taurit.Toolkit.TodoistInboxHelper;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

namespace Taurit.Toolkit.ProcesTodoistInbox.Common.Services
{
    /// <summary>
    ///     Allows to access pre-filtered sets of Todoist tasks.
    /// </summary>
    public sealed class FilteredTaskAccessor
    {
        private readonly ITodoistQueryService _todoistQueryService;

        public FilteredTaskAccessor(ITodoistQueryService todoistQueryService)
        {
            _todoistQueryService = todoistQueryService;
        }

        public IReadOnlyList<TodoTask> GetNotReviewedTasks(ILookup<Int64, Project> allProjectsIndexedById)
        {
            IReadOnlyList<TodoTask> allTasks = _todoistQueryService.GetAllTasks(allProjectsIndexedById);
            List<TodoTask> tasksThatNeedProcessing = allTasks
                .Where(x => x.@checked == 0 &&
                            x.is_deleted == 0 &&
                            x.labels != null).ToList();
            return tasksThatNeedProcessing;
        }
    }
}