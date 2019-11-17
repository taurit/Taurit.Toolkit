using System;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
            // parse args
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(options =>
                {
                    var program = new Program();
                    program.Start(options);
                });

            Console.WriteLine("Done.");
            Console.ReadLine();
        }

        private ServiceProvider ConfigureDependencyProvider(Options options)
        {
            var services = new ServiceCollection();

            services.AddLogging(configure => configure.AddConsole());
            services.AddSingleton(options);
            services.AddSingleton<IPathPlaceholderResolver, PathPlaceholderResolver>();
            services.AddSingleton<IMergeInboxProcessor, MergeInboxProcessor>();
            services.AddSingleton<InboxConfiguration>();
            services.AddSingleton<InboxWorkflow>();

            ServiceProvider serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }

        private void Start(Options options)
        {
            ServiceProvider dependencyProvider = ConfigureDependencyProvider(options);
            try
            {
                var inboxWorkflow = dependencyProvider.GetService<InboxWorkflow>();
                inboxWorkflow.Start();
            }
            catch (InvalidConfigurationException e)
            {
                Console.WriteLine($"Configuration file is invalid: {e.Message}");
            }
        }
    }
}