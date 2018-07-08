using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

namespace Taurit.Toolkit.ProcesTodoistInbox.Common.Services
{
    /// <summary>
    ///     Allows to access pre-filtered sets of Todoist tasks.
    /// </summary>
    public sealed class FilteredTaskAccessor
    {
        [NotNull]
        public IReadOnlyList<TodoTask> GetNotReviewedTasks([NotNull] IReadOnlyList<TodoTask> allTasks)
        {
            List<TodoTask> tasksThatNeedProcessing = allTasks
                .Where(x => x.@checked == 0 &&
                            x.is_deleted == 0 &&
                            x.labels != null).ToList();
            return tasksThatNeedProcessing;
        }
    }
}