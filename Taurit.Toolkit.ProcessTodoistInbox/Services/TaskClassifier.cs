using System;
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
        [NotNull] private readonly IReadOnlyList<ClassificationRule> _classificationRules;

        [NotNull] private readonly IReadOnlyList<Label> _labels;

        [NotNull] private readonly IReadOnlyList<Project> _projects;

        public TaskClassifier([NotNull] IReadOnlyList<ClassificationRule> classificationRules,
            [NotNull] IReadOnlyList<Label> labels,
            [NotNull] IReadOnlyList<Project> projects)
        {
            _classificationRules = classificationRules ?? throw new ArgumentNullException(nameof(classificationRules));
            _labels = labels ?? throw new ArgumentNullException(nameof(labels));
            _projects = projects ?? throw new ArgumentNullException(nameof(projects));
        }

        public (IReadOnlyList<TaskActionModel>, IReadOnlyList<TaskNoActionModel>) Classify(
            IReadOnlyList<TodoTask> tasksToClassify)
        {
            var actions = new List<TaskActionModel>(tasksToClassify.Count);
            var noActions = new List<TaskNoActionModel>(tasksToClassify.Count);

            foreach (TodoTask task in tasksToClassify)
            {
                List<ClassificationRule> matchingRules = FindMatchingRules(task).Where(x => x != null).ToList();
                Boolean matchFound = matchingRules.Any();
                foreach (ClassificationRule matchingRule in matchingRules)
                {
                    Label labelToSet =
                        _labels.SingleOrDefault(x => x.name == matchingRule.Then.setLabel && x.is_deleted == 0);
                    Project projectToSet = _projects.SingleOrDefault(x =>
                        x.name == matchingRule.Then.moveToProject && x.is_deleted == 0 && x.is_archived == 0);
                    Int32? priorityToSet = matchingRule.Then.setPriority;

                    if (labelToSet != null || projectToSet != null || priorityToSet != null)
                    {
                        var action = new TaskActionModel(task, labelToSet, matchingRule.Then.setPriority,
                            projectToSet);
                        actions.Add(action);
                    }
                }

                // also, list tasks that seem like they need classification (they have the default priority, sit in inbox, or have no labels assigned)
                if (!matchFound && (task.labels.Count == 0 || task.priority == 1 || task.project_name == "Inbox"))
                    noActions.Add(new TaskNoActionModel(task));
            }

            return (actions, noActions);
        }

        [NotNull]
        [ItemCanBeNull]
        private IReadOnlyList<ClassificationRule> FindMatchingRules(TodoTask task)
        {
            return _classificationRules.Where(rule => rule.If.Matches(task)).ToList();
        }
    }
}