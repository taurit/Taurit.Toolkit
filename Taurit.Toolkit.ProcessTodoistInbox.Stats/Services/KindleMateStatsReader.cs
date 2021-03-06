﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Taurit.Toolkit.ProcessTodoistInbox.Stats.Services
{
    public class KindleMateStatsReader
    {
        /// <remarks>
        ///     I measured that in 46 minutes I reduced the number of highlights in the backlog by 92.
        ///     This means that processing a single highlight takes 0.5 minute on average.
        ///     However, some of them were already processed before (due to syncing problems), so to have a margin I'll assume 1
        ///     minute on average, as before.
        /// </remarks>
        private const Int32 EstimatedTimeToProcessSingleHighlightMinutes = 1;

        /// <remarks>
        ///     I measured that I was able to process 20 vocabulary items in Kindle Mate in 46 minutes, in a focused work mode.
        ///     This gives average of 2.3 word / minute for Kindle Mate inbox.
        ///     This time might be larger for other inboxes - here I already have great example sentences from some text I know,
        ///     and the workflow is straightforward.
        ///     ---
        ///     Other measurement, 2020-10-17: 128 cards processed from Kindle Mate words inbox (2 examples each), resulting in 155 cards added. Took ~5 hours with needed breaks.
        ///     This means that the tempo is 25,6 words/hour, so time required for a single inbox item is 2,34375 - very similar to previous measurement, including breaks.
        /// </remarks>
        private const Double EstimatedTimeToProcessSingleWordMinutes = 2.34;

        private readonly SortedDictionary<DateTime, Int32> _highlights = new SortedDictionary<DateTime, Int32>();
        private readonly SortedDictionary<DateTime, Int32> _vocabularyWords = new SortedDictionary<DateTime, Int32>();

        public KindleMateStatsReader(IEnumerable<String> lines)
        {
            DateTime detailedPeriodStartDate = DateTime.UtcNow.AddDays(-7);

            IEnumerable<String> linesWithoutHeader = lines.Skip(1).Where(x => !string.IsNullOrWhiteSpace(x));
            foreach (String line in linesWithoutHeader)
            {
                // build some simple internal structure to avoid storing all data in memory (it might be a lot)
                String[] splitLine = line.Split(';');

                String dateString = splitLine[0];
                var numHighlights = Convert.ToInt16(splitLine[1]);
                var numVocabularyWords = Convert.ToInt32(splitLine[2]);

                // for performance, let's just see details for the recent 7 days. For the older dates, approximation is enough.

                // trim the time part - for now I assume that an estimate once a day is enough for me
                DateTime dateTime = DateTime.Parse(dateString);
                DateTime date = dateTime; // by default, keep all entries for a given day
                if (dateTime < detailedPeriodStartDate)
                    date = date.Date; // for older dates keep just one entry for each day

                if (!_highlights.ContainsKey(date))
                {
                    _highlights.Add(date, numHighlights);
                    _vocabularyWords.Add(date, numVocabularyWords);
                }
            }
        }

        public TimeSpan GetEstimatedTimeNeededToProcessHighlight(DateTime date)
        {
            Int32 numItems = KindleMateStatsReader.GetNumberOfItems(_highlights, date);
            return TimeSpan.FromMinutes(numItems * KindleMateStatsReader.EstimatedTimeToProcessSingleHighlightMinutes);
        }

        public TimeSpan GetEstimatedTimeNeededToProcessVocabularyWords(DateTime date)
        {
            Int32 numItems = KindleMateStatsReader.GetNumberOfItems(_vocabularyWords, date);
            return TimeSpan.FromMinutes(numItems * KindleMateStatsReader.EstimatedTimeToProcessSingleWordMinutes);
        }


        private static Int32 GetNumberOfItems(SortedDictionary<DateTime, Int32> collection, DateTime date)
        {
            // get the first item after the date
            foreach (DateTime keyDate in collection.Keys)
            {
                if (keyDate > date)
                    return collection[keyDate];
            }

            // fallback - use the most current time
            return collection.Last().Value;
        }
    }
}