using System;
using CommandLine;
using Taurit.Toolkit.FileProcessors.Exceptions;
using Taurit.Toolkit.FileProcessors.LocationProcessors;
using Taurit.Toolkit.ProcessRecognizedInboxFiles.Domain;
using Taurit.Toolkit.ProcessRecognizedInboxFiles.Services;

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
        /// <param name="args">[0]: config file path (defaults to "config.json")</param>
        private static void Main(String[] args)
        {
            Parser.Default.ParseArguments<Args>(args).WithParsed(o => { new Program().Start(o); });

            Console.WriteLine("Done.");
            Console.ReadLine();
        }

        private void Start(Args args)
        {
            try
            {
                var pathPlaceholderResolver = new PathPlaceholderResolver();
                var workflowConfiguration = new InboxConfiguration(pathPlaceholderResolver, args.ConfigurationFilePath);
                var mergeInboxProcessor = new MergeInboxProcessor();

                var inboxWorkflow = new InboxWorkflow(workflowConfiguration, mergeInboxProcessor);
                inboxWorkflow.Start();
            }
            catch (InvalidConfigurationException e)
            {
                Console.WriteLine($"Configuration file is invalid: {e.Message}");
            }
        }
    }
}