using System;

namespace Taurit.Toolkit.FixDateFormatInFilenames.Domain
{
    public interface IFileNameFormatProvider
    {
        String FormatFileName(String year, String month, String day, String description);
    }
}