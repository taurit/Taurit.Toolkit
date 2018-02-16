using System;
using System.IO;
using JetBrains.Annotations;

namespace Taurit.Toolkit.FileProcessors
{
    public class SingleFile
    {
        [NotNull] private readonly IFileProcessor[] _fileProcessors;

        public SingleFile([NotNull] IFileProcessor[] fileProcessors)
        {
            _fileProcessors = fileProcessors ?? throw new ArgumentNullException(nameof(fileProcessors));
        }

        public void ProcessFile(String filePath)
        {
            if (!File.Exists(filePath)) throw new ArgumentException("Given file does not exist", nameof(filePath));

            foreach (IFileProcessor fileProcessor in _fileProcessors) fileProcessor.ProcessMatchingFile(filePath);
        }
    }
}