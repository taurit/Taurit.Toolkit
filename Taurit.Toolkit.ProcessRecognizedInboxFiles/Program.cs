using System;
using Taurit.Toolkit.ProcessRecognizedInboxFiles.Domain;

namespace Taurit.Toolkit.ProcessRecognizedInboxFiles
{
    internal static class Program
    {
        /// <summary>
        ///     Go through the files in "file inbox", which consists of files coming from:
        ///     - Office Lens
        ///     and process them based on a set of conventions. For example:
        ///     - photos of mind maps drawn on paper should go to some directory Y,
        ///     - photos of receipts may need to go to some directory X and get compressed with low quality setting,
        ///     - etc.
        /// </summary>
        /// <param name="args">[0]: config file path (defaults to "config.json")</param>
        private static void Main(String[] args)
        {
            var workflowConfiguration = new InboxConfiguration(args.Length > 0 ? args[0] : "config.json");
            var inboxWorkflow = new InboxWorkflow(workflowConfiguration);
            inboxWorkflow.Start();

            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}