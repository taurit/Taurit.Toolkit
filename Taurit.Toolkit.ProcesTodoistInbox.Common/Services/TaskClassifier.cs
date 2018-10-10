using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using NaturalLanguageTimespanParser;
using Taurit.Toolkit.ProcesTodoistInbox.Common.Models;
using Taurit.Toolkit.ProcesTodoistInbox.Common.Models.Classification;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

namespace Taurit.Toolkit.ProcesTodoistInbox.Common.Services
{
    public class TaskClassifier
    {
        [NotNull] private readonly List<ClassificationRule> _classificationRules;

        [NotNull] private readonly IReadOnlyList<Label> _labels;

        [NotNull] private readonly IReadOnlyList<Project> _projects;

        [NotNull] private readonly MultiCultureTimespanParser _mctp = new MultiCultureTimespanParser(new[]
        {
            new CultureInfo("pl"),
            new CultureInfo("en")
        });

        public TaskClassifier([NotNull] IReadOnlyList<ClassificationRule> classificationRules,
            [NotNull] IReadOnlyList<String> classificationRulesInConciseFormat,
            [NotNull] IReadOnlyList<Label> labels,
            [NotNull] IReadOnlyList<Project> projects)
        {
            if (classificationRules == null) throw new ArgumentNullException(nameof(classificationRules));
            if (classificationRulesInConciseFormat == null)
                throw new ArgumentNullException(nameof(classificationRulesInConciseFormat));

            IClassificationRulesFormatConverter formatConverter = new ClassificationRulesFormatConverter();
            _classificationRules = classificationRules.ToList();
            IEnumerable<ClassificationRule> convertedConciseRules =
                classificationRulesInConciseFormat.Select(x => formatConverter.Convert(x));
            _classificationRules.AddRange(convertedConciseRules);

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

                    string nameToSet = GetUpdatedName(task.content, matchingRule.Then._setDuration);

                    if (labelToSet != null || projectToSet != null || priorityToSet != null || nameToSet != null)
                    {
                        var action = new TaskActionModel(task, labelToSet, matchingRule.Then.setPriority,
                            projectToSet, nameToSet);
                        actions.Add(action);
                    }
                }

                // also, list tasks that seem like they need classification (they have the default priority, sit in inbox, or have no labels assigned)
                if (!matchFound && (task.labels.Count == 0 || task.priority == 1 || task.project_name == "Inbox"))
                    noActions.Add(new TaskNoActionModel(task));
            }

            return (actions, noActions);
        }

        private String GetUpdatedName([NotNull] string originalName, [CanBeNull] String durationInNaturalLanguage)
        {
            if (durationInNaturalLanguage == null)
                return null;

            var durationToSet = _mctp.Parse(durationInNaturalLanguage);

            if (!durationToSet.Success)
                throw new ArgumentException("Configuration contains invalid (not parseable) duration string");

            // if the task already has a duration set, do not set another one
            var durationAlreadySet = _mctp.Parse(originalName);
            if (durationAlreadySet.Success) return null;

            // set duration
            return originalName + $" ({(int) durationToSet.Duration.TotalMinutes} min)";
        }

        [NotNull]
        [ItemCanBeNull]
        private IReadOnlyList<ClassificationRule> FindMatchingRules(TodoTask task)
        {
            return _classificationRules.Where(rule => rule.If.Matches(task)).ToList();
        }
    }
}