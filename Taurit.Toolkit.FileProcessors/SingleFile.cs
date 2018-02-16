using System;
using System.IO;
using JetBrains.Annotations;

namespace Taurit.Toolkit.FileProcessors
{
    internal class SingleFile : IConversionSource
    {
        [NotNull] private readonly String _filePath;
        [NotNull] private readonly IFileProcessor[] _fileProcessors;

        public SingleFile([NotNull] IFileProcessor[] fileProcessors, [NotNull] String filePath)
        {
            if (!File.Exists(filePath)) throw new ArgumentException("Given file does not exist", nameof(filePath));
            _fileProcessors = fileProcessors ?? throw new ArgumentNullException(nameof(fileProcessors));
            _filePath = filePath;
        }

        public void Process()
        {
            foreach (IFileProcessor fileProcessor in _fileProcessors) fileProcessor.ProcessMatchingFile(_filePath);
        }
    }
}