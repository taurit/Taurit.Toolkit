using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Taurit.Toolkit.ProcessTodoistInbox;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

// ReSharper disable InconsistentNaming

// ReSharper disable ReplaceWithSingleAssignment.True

namespace Taurit.Toolkit.ProcesTodoistInbox.Common.Models.Classification
{
    public class ClassificationRuleIf
    {
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
            Int32? numLabels)
        {
            this.contains = contains;
            this.containsWord = containsWord;
            this.startsWith = startsWith;
            this.priority = priority;
            this.project = project;
            this.numLabels = numLabels;
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


        public Boolean Matches(TodoTask task)
        {
            var match = true;
            match &= DoesTaskContainsMatch(task);
            match &= DoesTaskStartsWithWordMatch(task);
            match &= DoesTaskContainsWordMatch(task);
            match &= DoesTaskPriorityMatch(task);
            match &= DoesTaskProjectMatch(task);
            match &= DoesTaskLabelPresenceMatch(task);

            return match;
        }

        private Boolean DoesTaskLabelPresenceMatch(TodoTask task)
        {
            if (!numLabels.HasValue) return true;

            return task.labels.Count == numLabels;
        }

        private Boolean DoesTaskProjectMatch(TodoTask task)
        {
            if (project == null) return true;

            return String.Equals(task.project_name, project, StringComparison.InvariantCultureIgnoreCase);
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

            foreach (String keyword in startsWith)
            {
                String keywordWithoutDiacritics = keyword.RemoveDiacritics();
                String firstWord = contentWords.First().RemoveDiacritics();
                if (String.Equals(firstWord, keywordWithoutDiacritics, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }

            return false;
        }

        private Boolean DoesTaskContainsWordMatch(TodoTask task)
        {
            if (containsWord == null) return true;
            String[] contentWords = SplitIntoWords(task.content);

            foreach (String keyword in containsWord)
            {
                String keywordWithoutDiacritics = keyword.RemoveDiacritics();
                if (contentWords.Any(x => String.Equals(x.RemoveDiacritics(), keywordWithoutDiacritics,
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
            phrase = phrase ?? String.Empty;
            Char[] punctuation = phrase.Where(Char.IsPunctuation).Distinct().ToArray();
            IEnumerable<String> words = phrase.Split().Select(x => x.Trim(punctuation));
            return words.ToArray();
        }
    }
}