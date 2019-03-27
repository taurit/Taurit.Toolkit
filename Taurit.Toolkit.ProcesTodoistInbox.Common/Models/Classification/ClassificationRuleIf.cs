using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using NaturalLanguageTimespanParser;
using Newtonsoft.Json;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

// ReSharper disable InconsistentNaming

// ReSharper disable ReplaceWithSingleAssignment.True

namespace Taurit.Toolkit.ProcessTodoistInbox.Common.Models.Classification
{
    /// <summary>
    /// Represents part of the classification rule that defines the condition
    /// </summary>
    public class ClassificationRuleIf
    {
        [NotNull] private static readonly MultiCultureTimespanParser mctp = new MultiCultureTimespanParser(new[]
        {
            new CultureInfo("pl"),
            new CultureInfo("en")
        });

        [JsonConstructor]
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
            String duration)
        {
            this.contains = contains;
            this.containsWord = containsWord;
            this.startsWith = startsWith;
            this.priority = priority;
            this.project = project;
            this.numLabels = numLabels;
            this.duration = duration;
        }

        [CanBeNull]
        [JsonProperty]
        public String[] contains { get; set; }

        [CanBeNull]
        [JsonProperty]
        public String[] containsWord { get; set; }

        [CanBeNull]
        [JsonProperty]
        public String[] startsWith { get; set; }

        [CanBeNull]
        [JsonProperty]
        public Int32? priority { get; set; }

        [CanBeNull]
        [JsonProperty]
        public String project { get; set; }

        [CanBeNull]
        [JsonProperty]
        public Int32? numLabels { get; set; }

        [CanBeNull]
        [JsonProperty]
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
                duration
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

            return match;
        }

        private Boolean DoesTaskDurationMatch(TodoTask task)
        {
            if (duration is null) return true;

            TimespanParseResult parseResultContent = mctp.Parse(task.content);
            TimespanParseResult parseResultRule = mctp.Parse(duration);

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

        private Boolean DoesTaskStartsWithWordMatch(TodoTask task)
        {
            if (startsWith == null) return true;
            String[] contentWords = SplitIntoWords(task.content);

            Debug.Assert(startsWith != null, nameof(startsWith) + " != null");
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
            String[] contentWords = SplitIntoWords(task.content);

            Debug.Assert(containsWord != null, nameof(containsWord) + " != null");
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

        internal String[] SplitIntoWords(String phrase)
        {
            phrase = phrase ?? string.Empty;
            Char[] punctuation = phrase.Where(char.IsPunctuation).Distinct().ToArray();
            IEnumerable<String> words = phrase.Split().Select(x => x.Trim(punctuation));
            return words.ToArray();
        }
    }
}