using System;
using System.IO;
using JetBrains.Annotations;

namespace Taurit.Toolkit.ProcessRecognizedInboxFiles.Domain
{
    internal class Inbox
    {
        private String fileInboxPath;

        public Inbox([NotNull] String fileInboxPath)
        {
            if (fileInboxPath == null)
            {
                throw new ArgumentNullException(nameof(fileInboxPath));
            }

            if (!Directory.Exists(fileInboxPath))
            {
                throw new ArgumentException("Inbox directory does not exist", nameof(fileInboxPath));
            }

            this.fileInboxPath = fileInboxPath;
        }
    }
}