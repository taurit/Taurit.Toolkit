using System;
using Taurit.Toolkit.FileProcessors.Exceptions;
using Taurit.Toolkit.FileProcessors.LocationProcessors;
using Taurit.Toolkit.ProcessRecognizedInboxFiles.Domain;
using Taurit.Toolkit.ProcessRecognizedInboxFiles.Services;

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
            if (args.Length != 1)
            {
                Console.WriteLine("You need to pass a path to the configuration file as an argument.");
            }
            else
            {
                try
                {
                    var pathPlaceholderResolver = new PathPlaceholderResolver();
                    var workflowConfiguration = new InboxConfiguration(pathPlaceholderResolver, args[0]);
                    var mergeInboxProcessor = new MergeInboxProcessor();

                    var inboxWorkflow = new InboxWorkflow(workflowConfiguration, mergeInboxProcessor);
                    inboxWorkflow.Start();
                }
                catch (InvalidConfigurationException e)
                {
                    Console.WriteLine($"Configuration file is invalid: {e.Message}");
                }
            }

            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}