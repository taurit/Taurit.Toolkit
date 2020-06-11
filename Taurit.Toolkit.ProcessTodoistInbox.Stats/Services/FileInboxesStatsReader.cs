using System;
using System.Collections.Generic;
using System.Linq;

namespace Taurit.Toolkit.ProcessTodoistInbox.Stats.Services
{
    internal class FileInboxesStatsReader
    {
        private readonly Dictionary<String, SortedDictionary<DateTime, Int32>> _folderPathToDateAndNumberOfFiles =
            new Dictionary<String, SortedDictionary<DateTime, Int32>>();

        public FileInboxesStatsReader(IEnumerable<String> lines)
        {
            IEnumerable<String> linesWithoutHeader = lines.Skip(1).Where(x => !string.IsNullOrWhiteSpace(x));
            foreach (String line in linesWithoutHeader)
            {
                // build some simple internal structure to avoid storing all data in memory (it might be a lot)

                // for now I'll assume no ';' characters in path - paths are in my control anyway
                String[] splitLine = line.Split(';');

                String dateString = splitLine[0];
                String folderPath = splitLine[1].Trim('\"');
                var numFiles = Convert.ToInt16(splitLine[2]);

                // trim the time part - for now I assume that an estimate once a day is enough for me
                DateTime dateTime = DateTime.Parse(dateString);
                DateTime date = dateTime; // by default, keep all entries for a given day

                if (!_folderPathToDateAndNumberOfFiles.ContainsKey(folderPath))
                    _folderPathToDateAndNumberOfFiles.Add(folderPath, new SortedDictionary<DateTime, Int32>());

                SortedDictionary<DateTime, Int32> folderStats = _folderPathToDateAndNumberOfFiles[folderPath];
                if (!folderStats.ContainsKey(date)) folderStats.Add(date, numFiles);
            }
        }

        public TimeSpan GetEstimatedTimeNeededToProcessFolder(String folderPath, DateTime date,
            Double estimatedTimeToProcessSingleFile)
        {
            SortedDictionary<DateTime, Int32> folderStats = _folderPathToDateAndNumberOfFiles[folderPath];
            Int32 numItems = FileInboxesStatsReader.GetNumberOfItems(folderStats, date);
            return TimeSpan.FromMinutes(numItems * estimatedTimeToProcessSingleFile);
        }

        // todo: duplicated with KindleMateStatsReader, I might want to move it to some base class
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