using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using NaturalLanguageTimespanParser;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

// ReSharper disable InconsistentNaming

// ReSharper disable ReplaceWithSingleAssignment.True

namespace Taurit.Toolkit.ProcessTodoistInbox.Common.Models.ClassificationRuleModels
{
    /// <summary>
    ///     Represents part of the classification rule that defines the condition
    /// </summary>
    [SuppressMessage("Microsoft.Design", "IDE1006", Justification = "Names are aligned with JSON property names")]
    [DataContract]
    public class ClassificationRuleIf
    {
        [NotNull] private static readonly MultiCultureTimespanParser mctp = new MultiCultureTimespanParser(new[]
        {
            new CultureInfo("pl"),
            new CultureInfo("en")
        });

        [Obsolete("Use only if you are JSON deserializer")]
        public ClassificationRuleIf()
        {
        }

        public ClassificationRuleIf(String[] contains,
            String[] containsWord,
            String[] startsWith,
            Int32? priority,
            String project,
            Int32? numLabels,
            String duration,
            String[] hasLabel)
        {
            this.contains = contains;
            this.containsWord = containsWord;
            this.startsWith = startsWith;
            this.priority = priority;
            this.project = project;
            this.numLabels = numLabels;
            this.duration = duration;
            this.hasLabel = hasLabel;
        }

        [CanBeNull]
        [DataMember]
        public String[] contains { get; set; }

        [CanBeNull]
        [DataMember]
        public String[] containsWord { get; set; }

        [CanBeNull]
        [DataMember]
        public String[] startsWith { get; set; }

        [CanBeNull]
        [DataMember]
        public Int32? priority { get; set; }

        [CanBeNull]
        [DataMember]
        public String project { get; set; }

        [CanBeNull]
        [DataMember]
        public Int32? numLabels { get; set; }

        [CanBeNull]
        [DataMember]
        public String[] hasLabel { get; set; }

        [CanBeNull]
        [DataMember]
        public String duration { get; set; }

        /// <summary>
        ///     This method is to support the "Alternative inboxes" feature, where a rule targeting "Inbox" project in the "If"
        ///     part will automatically be ran on "Alternative inboxes" as well
        /// </summary>
        public ClassificationRuleIf GetCopyWithChangedProjectName(String newProjectName)
        {
            return new ClassificationRuleIf(
                contains,
                containsWord,
                startsWith,
                priority,
                newProjectName,
                numLabels,
                duration,
                hasLabel
            );
        }

        public Boolean Matches(TodoTask task)
        {
            var match = true;
            match &= DoesTaskContainsMatch(task);
            match &= DoesTaskStartsWithWordMatch(task);
            match &= DoesTaskContainsWordMatch(task);
            match &= DoesTaskPriorityMatch(task);
            match &= DoesTaskProjectMatch(task);
            match &= DoesTaskLabelPresenceMatch(task);
            match &= DoesTaskDurationMatch(task);
            match &= DoesTaskLabelNameMatch(task);

            return match;
        }

        private Boolean DoesTaskDurationMatch(TodoTask task)
        {
            if (duration is null) return true;

            TimespanParseResult parseResultContent = ClassificationRuleIf.mctp.Parse(task.content);
            TimespanParseResult parseResultRule = ClassificationRuleIf.mctp.Parse(duration);

            if (parseResultRule.Success == false && parseResultContent.Success == false) return true;
            if (parseResultRule.Success && parseResultContent.Success == false) return false;
            if (parseResultRule.Success == false && parseResultContent.Success) return false;

            return parseResultContent.Duration == parseResultRule.Duration;
        }

        private Boolean DoesTaskLabelPresenceMatch(TodoTask task)
        {
            if (!numLabels.HasValue) return true;

            return task.labels.Count == numLabels;
        }

        private Boolean DoesTaskProjectMatch(TodoTask task)
        {
            if (project == null) return true;

            return string.Equals(task.project_name, project, StringComparison.InvariantCultureIgnoreCase);
        }

        private Boolean DoesTaskPriorityMatch(TodoTask task)
        {
            if (!priority.HasValue) return true;

            return task.priority == priority.Value;
        }

        private Boolean DoesTaskLabelNameMatch(TodoTask task)
        {
            if (hasLabel == null) return true;
            String[] labelsNamesInTask = task.labelsNames;
            Debug.Assert(labelsNamesInTask != null);

            return hasLabel.Any(labelToCheck => labelsNamesInTask.Contains(labelToCheck));
        }

        private Boolean DoesTaskStartsWithWordMatch(TodoTask task)
        {
            if (startsWith == null) return true;
            String[] contentWords = ClassificationRuleIf.SplitIntoWords(task.content);

            Debug.Assert(startsWith != null, nameof(ClassificationRuleIf.startsWith) + " != null");
            foreach (String keyword in startsWith)
            {
                String keywordWithoutDiacritics = keyword.RemoveDiacritics();
                String firstWord = contentWords.First().RemoveDiacritics();
                if (string.Equals(firstWord, keywordWithoutDiacritics, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }

            return false;
        }

        private Boolean DoesTaskContainsWordMatch(TodoTask task)
        {
            if (containsWord == null) return true;
            String[] contentWords = ClassificationRuleIf.SplitIntoWords(task.content);

            Debug.Assert(containsWord != null, nameof(ClassificationRuleIf.containsWord) + " != null");
            foreach (String keyword in containsWord)
            {
                String keywordWithoutDiacritics = keyword.RemoveDiacritics();
                if (contentWords.Any(x => string.Equals(x.RemoveDiacritics(), keywordWithoutDiacritics,
                    StringComparison.InvariantCultureIgnoreCase)))
                    return true;
            }

            return false;
        }

        private Boolean DoesTaskContainsMatch(TodoTask task)
        {
            if (contains == null) return true;

            foreach (String keyword in contains)
            {
                String taskContentWithoutDiacritics = task.content.RemoveDiacritics();
                String ruleKeywordWithoutDiacritics = keyword.RemoveDiacritics();

                Boolean keywordInRuleMatchesTask = taskContentWithoutDiacritics.Contains(ruleKeywordWithoutDiacritics,
                    StringComparison.InvariantCultureIgnoreCase);
                if (keywordInRuleMatchesTask) return true;
            }

            return false;
        }

        private static String[] SplitIntoWords(String phrase)
        {
            phrase = phrase ?? string.Empty;
            Char[] punctuation = phrase.Where(char.IsPunctuation).Distinct().ToArray();
            IEnumerable<String> words = phrase.Split().Select(x => x.Trim(punctuation));
            return words.ToArray();
        }
    }
}