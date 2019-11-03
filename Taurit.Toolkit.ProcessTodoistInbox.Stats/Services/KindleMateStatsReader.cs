using System;
using System.Collections.Generic;
using System.Linq;

namespace Taurit.Toolkit.ProcessTodoistInbox.Stats.Services
{
    public class KindleMateStatsReader
    {
        private const Int32 EstimatedTimeToProcessSingleHighlightMinutes = 4;
        private const Int32 EstimatedTimeToProcessSingleWordMinutes = 6;
        private readonly SortedDictionary<DateTime, Int32> _highlights = new SortedDictionary<DateTime, Int32>();
        private readonly SortedDictionary<DateTime, Int32> _vocabularyWords = new SortedDictionary<DateTime, Int32>();

        public KindleMateStatsReader(IEnumerable<String> lines)
        {
            IEnumerable<String> linesWithoutHeader = lines.Skip(1).Where(x => !String.IsNullOrWhiteSpace(x));
            foreach (String line in linesWithoutHeader)
            {
                // build some simple internal structure to avoid storing all data in memory (it might be a lot)
                String[] splitLine = line.Split(';');

                String dateString = splitLine[0];
                Int16 numHighlights = Convert.ToInt16(splitLine[1]);
                Int32 numVocabularyWords = Convert.ToInt32(splitLine[2]);

                // trim the time part - for now I assume that an estimate once a day is enough for me
                DateTime date = DateTime.Parse(dateString).Date;

                if (!_highlights.ContainsKey(date))
                {
                    _highlights.Add(date, numHighlights);
                    _vocabularyWords.Add(date, numVocabularyWords);
                }
            }
        }

        public TimeSpan GetEstimatedTimeNeededToProcessHighlight(DateTime date)
        {
            Int32 numItems = GetNumberOfItems(_highlights, date);
            return TimeSpan.FromMinutes(numItems * EstimatedTimeToProcessSingleHighlightMinutes);
        }


        public TimeSpan GetEstimatedTimeNeededToProcessVocabularyWords(DateTime date)
        {
            Int32 numItems = GetNumberOfItems(_vocabularyWords, date);
            return TimeSpan.FromMinutes(numItems * EstimatedTimeToProcessSingleWordMinutes);
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