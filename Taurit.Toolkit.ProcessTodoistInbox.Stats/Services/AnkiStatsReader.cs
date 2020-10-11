using System;
using System.Collections.Generic;
using System.Linq;

namespace Taurit.Toolkit.ProcessTodoistInbox.Stats.Services
{
    internal class AnkiStatsReader
    {
        private readonly SortedDictionary<DateTime, Int32> statsInTime = new SortedDictionary<DateTime, Int32>();

        public AnkiStatsReader(IEnumerable<String> lines)
        {
            IEnumerable<String> linesWithoutHeader = lines.Skip(1).Where(x => !string.IsNullOrWhiteSpace(x));
            foreach (String line in linesWithoutHeader)
            {
                // build some simple internal structure to avoid storing all data in memory (it might be a lot)

                String[] splitLine = line.Split(';');

                String dateString = splitLine[0];
                String numCardsString = splitLine[1];

                var numCards = Convert.ToInt32(numCardsString);

                // trim the time part - for now I assume that an estimate once a day is enough for me
                DateTime dateTime = DateTime.Parse(dateString);
                DateTime date = dateTime.Date; // by default, keep all entries for a given day

                if (!statsInTime.ContainsKey(date))
                    statsInTime.Add(date, numCards);
            }
        }

        public TimeSpan GetEstimatedTimeNeededToProcessCards(in DateTime date, double estimatedNumMinutesToProcessSingleItem)
        {
            Int32 numItems = AnkiStatsReader.GetNumberOfItems(statsInTime, date);
            return TimeSpan.FromMinutes(numItems * estimatedNumMinutesToProcessSingleItem);
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