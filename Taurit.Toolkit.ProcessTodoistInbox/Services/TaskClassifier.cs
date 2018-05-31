using System.Collections.Generic;
using System.Linq;
using Taurit.Toolkit.ProcessTodoistInbox.Models;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

namespace Taurit.Toolkit.ProcessTodoistInbox.Services
{
    public class TaskClassifier
    {
        private readonly IReadOnlyList<ClassificationRule> _classificationRules;
        private readonly IReadOnlyList<Label> _labels;
        private readonly IReadOnlyList<Project> _projects;

        public TaskClassifier(IReadOnlyList<ClassificationRule> classificationRules,
            IReadOnlyList<Label> labels,
            IReadOnlyList<Project> projects)
        {
            _classificationRules = classificationRules;
            _labels = labels;
            _projects = projects;
        }

        public (IReadOnlyList<TaskActionModel>, IReadOnlyList<TaskNoActionModel>) Classify(
            IReadOnlyList<TodoTask> tasksToClassify)
        {
            var actions = new List<TaskActionModel>(tasksToClassify.Count);
            var noActions = new List<TaskNoActionModel>(tasksToClassify.Count);

            foreach (TodoTask task in tasksToClassify)
            {
                ClassificationRule matchingRule = _classificationRules.SingleOrDefault(x => x.Matches(task));
                if (matchingRule != null)
                    actions.Add(new TaskActionModel(task, matchingRule.setLabel, matchingRule.setPriority,
                        matchingRule.moveToProject));
                else
                    noActions.Add(new TaskNoActionModel(task));
            }


            return (actions, noActions);
        }
    }
}