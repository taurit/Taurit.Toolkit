using System;
using Taurit.Toolkit.FileProcessors.LocationProcessors;
using Taurit.Toolkit.FileProcessors.NameProcessors;
using Taurit.Toolkit.FileProcessors.NameProcessors.NameFormatProviders;
using Taurit.Toolkit.FixDateFormatInFilenames.Domain;
using Taurit.Toolkit.ProcessRecognizedInboxFiles.Domain;

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
            var workflowConfiguration = new InboxConfiguration("config.json");
            IFileProcessor[] fileProcessors = ReadConfig(workflowConfiguration);

            var inboxFolder = new Folder(fileProcessors);
            inboxFolder.ProcessAllFiles(workflowConfiguration.InboxPath);
        }

        private static IFileProcessor[] ReadConfig(InboxConfiguration config)
        {
            var fileProcessors = new IFileProcessor[]
            {
                new ChangeOfficeLensNameProcessor(new IsoDateFileNameFormatProvider()),
                new ChangeLocationProcessor(config.ChangeLocationRules)
            };
            return fileProcessors;
        }
    }
}