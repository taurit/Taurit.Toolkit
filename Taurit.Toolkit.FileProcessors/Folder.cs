using System;
using JetBrains.Annotations;

namespace Taurit.Toolkit.FileProcessors
{
    public class Folder
    {
        [NotNull] private readonly IFileProcessor[] _fileProcessors;

        public Folder([NotNull] IFileProcessor[] fileProcessors)
        {
            _fileProcessors = fileProcessors ?? throw new ArgumentNullException(nameof(fileProcessors));
        }


        public void ProcessAllFiles(String directory)
        {
            foreach (IFileProcessor fileProcessor in _fileProcessors) fileProcessor.ProcessMatchingFiles(directory);
        }
    }
}