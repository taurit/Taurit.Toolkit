using System;

namespace Taurit.Toolkit.FileProcessors.ConversionProcessors
{
    public interface IConvertedFileNamingStrategy
    {
        String GetConvertedFilePath(String originalPath);
    }
}