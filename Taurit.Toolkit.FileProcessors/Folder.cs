using System;
using System.IO;
using JetBrains.Annotations;

namespace Taurit.Toolkit.FileProcessors
{
    internal class Folder : IConversionSource
    {
        [NotNull] private readonly String _directory;
        [NotNull] private readonly IFileProcessor[] _fileProcessors;

        public Folder([NotNull] IFileProcessor[] fileProcessors, [NotNull] String directory)
        {
            if (!Directory.Exists(directory))
                throw new ArgumentException("Given directory does not exist", nameof(directory));

            _fileProcessors = fileProcessors ?? throw new ArgumentNullException(nameof(fileProcessors));
            _directory = directory;
        }

        public void Process()
        {
            foreach (IFileProcessor fileProcessor in _fileProcessors) fileProcessor.ProcessMatchingFiles(_directory);
        }
    }
}