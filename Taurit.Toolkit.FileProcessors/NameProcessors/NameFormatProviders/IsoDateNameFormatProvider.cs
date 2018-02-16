﻿using System;

namespace Taurit.Toolkit.FileProcessors.NameProcessors.NameFormatProviders
{
    public class IsoDateFileNameFormatProvider : IFileNameFormatProvider
    {
        public String FormatFileName(String year, String month, String day, String description)
        {
            Int32 yearParsed = Convert.ToInt32(year);
            Int32 monthParsed = Convert.ToInt32(month);
            Int32 dayParsed = Convert.ToInt32(day);

            String newFileName = $"{yearParsed}-{monthParsed:00}-{dayParsed:00} {description}";
            return newFileName;
        }
    }
}