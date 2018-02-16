using System;

namespace Taurit.Toolkit.FileProcessors.NameProcessors.NameFormatProviders
{
    public interface IFileNameFormatProvider
    {
        String FormatFileName(String year, String month, String day, String description);
    }
}