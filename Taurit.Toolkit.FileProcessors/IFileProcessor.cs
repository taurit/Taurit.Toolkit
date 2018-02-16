using System;
using JetBrains.Annotations;

namespace Taurit.Toolkit.FileProcessors
{
    public interface IFileProcessor
    {
        void ProcessMatchingFiles([NotNull] String directoryPath);
    }
}