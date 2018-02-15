using System;
using Taurit.Toolkit.FileProcessors.FileNameProcessors.FileNameFormatProviders;
using Taurit.Toolkit.FixDateFormatInFilenames.Domain;

namespace Taurit.Toolkit.ProcessRecognizedInboxFiles
{
    internal class Program
    {
        /// <summary>
        ///     Go through the files in "file inbox", which consists of files coming from:
        ///     - Office Lens
        ///     and process them based on a set of conventions. For example:
        ///     - photos of mind maps drawn on paper should go to some directory Y,
        ///     - photos of receipts may need to go to some directory X and get compressed with low quality setting,
        ///     - etc.
        /// </summary>
        /// <param name="args"></param>
        private static void Main(String[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("Invalid parameters, expected: [inboxFolderPath]");
            }

            String inboxDirectoryPath = args[0];

            var fileProcessors = new IFileProcessor[]
            {
                new ChangeDateFormatFileProcessor(new IsoDateFileNameFormatProvider())
            };

            var inboxFolder = new Folder(fileProcessors);
            inboxFolder.ProcessAllFiles(inboxDirectoryPath);
        }
    }
}