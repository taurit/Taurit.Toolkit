using System;
using JetBrains.Annotations;

namespace Taurit.Toolkit.FixDateFormatInFilenames.Domain
{
    public interface IFileProcessor
    {
        void ProcessMatchingFiles([NotNull] String directoryPath);
    }
}