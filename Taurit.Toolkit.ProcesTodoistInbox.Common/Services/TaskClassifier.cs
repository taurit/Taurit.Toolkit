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
        [NotNull] private readonly List<String> _alternativeInboxes;
        [NotNull] private readonly List<ClassificationRule> _classificationRules;

        [NotNull] private readonly IReadOnlyList<Label> _labels;

        [NotNull] private readonly MultiCultureTimespanParser _mctp = new MultiCultureTimespanParser(new[]
        {
            new CultureInfo("pl"),
            new CultureInfo("en")
        });

        [NotNull] private readonly IReadOnlyList<Project> _projects;

        public TaskClassifier([NotNull] IReadOnlyList<ClassificationRule> classificationRules,
            [NotNull] IReadOnlyList<String> classificationRulesInConciseFormat,
            [NotNull] IReadOnlyList<Label> labels,
            [NotNull] IReadOnlyList<Project> projects,
            [NotNull] List<String> alternativeInboxes)
        {
            if (classificationRules == null) throw new ArgumentNullException(nameof(classificationRules));
            if (classificationRulesInConciseFormat == null)
                throw new ArgumentNullException(nameof(classificationRulesInConciseFormat));
            if (alternativeInboxes == null) throw new ArgumentNullException(nameof(alternativeInboxes));

            IClassificationRulesFormatConverter formatConverter = new ClassificationRulesFormatConverter();
            _classificationRules = classificationRules.ToList();
            IEnumerable<ClassificationRule> convertedConciseRules =
                classificationRulesInConciseFormat.Select(x => formatConverter.Convert(x));
            _classificationRules.AddRange(convertedConciseRules);

            // support for alternative inboxes
            List<ClassificationRule> rulesForInbox = _classificationRules.Where(x => x.If.project == "Inbox").ToList();
            foreach (String alternativeInbox in alternativeInboxes)
            {
                IEnumerable<ClassificationRule> rulesForAlternativeInbox = rulesForInbox.Select(x =>
                {
                    ClassificationRuleIf ifPartForAlternativeInbox =
                        x.If.GetCopyWithChangedProjectName(alternativeInbox);
                    return new ClassificationRule(ifPartForAlternativeInbox, x.Then);
                });

                _classificationRules.AddRange(rulesForAlternativeInbox);
            }

            _labels = labels ?? throw new ArgumentNullException(nameof(labels));
            _projects = projects ?? throw new ArgumentNullException(nameof(projects));
            _alternativeInboxes = alternativeInboxes;
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

                    String nameToSet = GetUpdatedName(task.content, matchingRule.Then._setDuration);

                    if (labelToSet != null || projectToSet != null || priorityToSet != null || nameToSet != null)
                    {
                        var action = new TaskActionModel(task, labelToSet, matchingRule.Then.setPriority,
                            projectToSet, nameToSet);
                        actions.Add(action);
                    }
                }

                // also, list tasks that seem like they need classification (they have the default priority, sit in inbox, or have no labels assigned, or have no estimated time assigned)
                TimespanParseResult estimatedTimeParseResult = _mctp.Parse(task.content);

                if (!matchFound &&
                    (task.labels.Count == 0 || task.priority == 1 || task.project_name == "Inbox" ||
                     !estimatedTimeParseResult.Success)
                )
                    noActions.Add(new TaskNoActionModel(task, estimatedTimeParseResult.Duration));
            }

            return (actions, noActions);
        }

        private String GetUpdatedName([NotNull] String originalName, [CanBeNull] String durationInNaturalLanguage)
        {
            if (durationInNaturalLanguage == null)
                return null;

            TimespanParseResult durationToSet = _mctp.Parse(durationInNaturalLanguage);

            if (!durationToSet.Success)
                throw new ArgumentException("Configuration contains invalid (not parseable) duration string");

            // if the task already has a duration set, do not set another one
            TimespanParseResult durationAlreadySet = _mctp.Parse(originalName);
            if (durationAlreadySet.Success) return null;

            // set duration
            return originalName + $" ({(Int32) durationToSet.Duration.TotalMinutes} min)";
        }

        [NotNull]
        [ItemCanBeNull]
        private IReadOnlyList<ClassificationRule> FindMatchingRules(TodoTask task)
        {
            return _classificationRules.Where(rule => rule.If.Matches(task)).ToList();
        }
    }
}