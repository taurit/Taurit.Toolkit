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
        public String taskContains { get; set; }

        [CanBeNull]
        [JsonProperty]
        public String[] taskStartsWithWord { get; set; }

        [CanBeNull]
        [JsonProperty]
        public Int32? taskHasPriority { get; set; }

        [CanBeNull]
        [JsonProperty]
        public String taskProject { get; set; }

        [CanBeNull]
        [JsonProperty]
        public Int32? taskHasLabels { get; set; }


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
            if (!taskHasLabels.HasValue) return true;

            return task.labels.Count == taskHasLabels;
        }

        private Boolean DoesTaskProjectMatch(TodoTask task)
        {
            if (taskProject == null) return true;

            return String.Equals(task.project_name, taskProject, StringComparison.InvariantCultureIgnoreCase);
        }

        private Boolean DoesTaskPriorityMatch(TodoTask task)
        {
            if (!taskHasPriority.HasValue) return true;

            return task.priority == taskHasPriority.Value;
        }

        private Boolean DoesTaskStartsWithWordMatch(TodoTask task, String[] contentWords)
        {
            if (taskStartsWithWord == null) return true;

            foreach (String keyword in taskStartsWithWord)
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
            if (taskContains == null) return true;

            String taskContentWithoutDiacritics = task.content.RemoveDiacritics();
            return taskContentWithoutDiacritics.Contains(taskContains.RemoveDiacritics(),
                StringComparison.InvariantCultureIgnoreCase);
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