using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Taurit.Toolkit.ProcessTodoistInbox.Models;
using Taurit.Toolkit.ProcessTodoistInbox.Models.Classification;
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
                ClassificationRule matchingRule = FindMatchingRule(task);
                if (matchingRule != null)
                {
                    Label labelToSet =
                        _labels.SingleOrDefault(x => x.name == matchingRule.Then.setLabel && x.is_deleted == 0);
                    Project projectToSet = _projects.SingleOrDefault(x =>
                        x.name == matchingRule.Then.moveToProject && x.is_deleted == 0 && x.is_archived == 0);

                    if (labelToSet != null && projectToSet != null)
                    {
                        var action = new TaskActionModel(task, labelToSet, matchingRule.Then.setPriority, projectToSet);
                        actions.Add(action);
                    }
                    else
                        noActions.Add(new TaskNoActionModel(task));
                }

                else
                    noActions.Add(new TaskNoActionModel(task));
            }


            return (actions, noActions);
        }

        [CanBeNull]
        private ClassificationRule FindMatchingRule(TodoTask task)
        {
            return _classificationRules.FirstOrDefault(rule => rule.If.Matches(task));
        }
    }
}