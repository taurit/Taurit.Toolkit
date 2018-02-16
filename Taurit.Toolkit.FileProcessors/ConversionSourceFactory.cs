using System;
using System.IO;

namespace Taurit.Toolkit.FileProcessors
{
    public static class ConversionSourceFactory
    {
        public static IConversionSource GetConversionSource(String fileSystemPath, IFileProcessor[] conversionOptions)
        {
            if (Directory.Exists(fileSystemPath))
                return new Folder(conversionOptions, fileSystemPath);
            if (File.Exists(fileSystemPath))
                return new SingleFile(conversionOptions, fileSystemPath);

            throw new ArgumentException("Provided file system path is not valid", nameof(fileSystemPath));
        }
    }
}