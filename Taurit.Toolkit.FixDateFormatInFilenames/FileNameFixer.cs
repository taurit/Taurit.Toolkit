using System;

namespace Taurit.Toolkit.FixDateFormatInFilenames
{
    internal class FileNameFixer
    {
        public string GetProperFileName(string year, string month, string day, string description)
        {
            var yearParsed = Convert.ToInt32(year);
            var monthParsed = Convert.ToInt32(month);
            var dayParsed = Convert.ToInt32(day);

            var newFileName = $"{yearParsed}-{monthParsed:00}-{dayParsed:00} {description}";
            return newFileName;
        }
    }
}