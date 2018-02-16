using System;
using System.Diagnostics.Contracts;
using System.IO;

namespace Taurit.Toolkit.FileProcessors
{
    public abstract class FileProcessorBase : IFileProcessor
    {
        /// <inheritdoc />
        public void ProcessMatchingFiles(String directoryPath)
        {
            Contract.Assert(Directory.Exists(directoryPath), "Inbox directory must exist to enumerate files");

            String[] filesInDirectory = Directory.GetFiles(directoryPath);
            foreach (String filePath in filesInDirectory) ProcessMatchingFile(filePath);
        }

        public abstract void ProcessMatchingFile(String filePath);
    }
}