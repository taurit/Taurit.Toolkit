using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

// ReSharper disable InconsistentNaming

// ReSharper disable ReplaceWithSingleAssignment.True

namespace Taurit.Toolkit.ProcessTodoistInbox.Models.Classification
{
    public class ClassificationRuleIf
    {
        [CanBeNull]
        [JsonProperty]
        public String[] contains { get; set; }

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
            String[] contentWords = SplitIntoWords(task.content);

            var match = true;
            match &= DoesTaskContainsMatch(task);
            match &= DoesTaskStartsWithWordMatch(task, contentWords);
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

        private Boolean DoesTaskStartsWithWordMatch(TodoTask task, String[] contentWords)
        {
            if (startsWith == null) return true;

            foreach (String keyword in startsWith)
            {
                String keywordWithoutDiacritics = keyword.RemoveDiacritics();
                String firstWord = contentWords.First().RemoveDiacritics();
                if (String.Equals(firstWord, keywordWithoutDiacritics, StringComparison.InvariantCultureIgnoreCase))
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