using System;

namespace Taurit.Toolkit.FileProcessors.NameProcessors.NameFormatProviders
{
    public class IsoDateFileNameFormatProvider : IFileNameFormatProvider
    {
        public String FormatFileName(String year, String month, String day, String description)
        {
            var yearParsed = Convert.ToInt32(year);
            var monthParsed = Convert.ToInt32(month);
            var dayParsed = Convert.ToInt32(day);

            // workaround to also support "yy" format (and not "yyyy" which is found in most regex patterns)
            if (yearParsed < 100) yearParsed = 2000 + yearParsed;

            String optionalDescription = string.IsNullOrWhiteSpace(description) ? string.Empty : $" {description}";
            String newFileName = $"{yearParsed}-{monthParsed:00}-{dayParsed:00}{optionalDescription}";
            return newFileName;
        }
    }
}