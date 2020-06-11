using System;
using Microsoft.Extensions.Logging;
using Taurit.Toolkit.FileProcessors;
using Taurit.Toolkit.FileProcessors.ConversionProcessors;
using Taurit.Toolkit.FileProcessors.LocationProcessors;
using Taurit.Toolkit.FileProcessors.NameProcessors;
using Taurit.Toolkit.FileProcessors.NameProcessors.NameFormatProviders;
using Taurit.Toolkit.ProcessRecognizedInboxFiles.Domain;
using Taurit.Toolkit.ProcessRecognizedInboxFiles.Models;

namespace Taurit.Toolkit.ProcessRecognizedInboxFiles
{
    public class InboxWorkflow
    {
        private readonly ILogger<IMergeInboxProcessor> _logger;
        private readonly IMergeInboxProcessor _mergeInboxProcessor;
        private readonly InboxConfiguration _workflowConfiguration;

        public InboxWorkflow(InboxConfiguration workflowConfiguration, IMergeInboxProcessor mergeInboxProcessor, ILogger<IMergeInboxProcessor> logger)
        {
            _workflowConfiguration = workflowConfiguration ?? throw new ArgumentNullException(nameof(workflowConfiguration));
            _mergeInboxProcessor = mergeInboxProcessor ?? throw new ArgumentNullException(nameof(mergeInboxProcessor));
            _logger = logger;
        }

        internal void Start()
        {
            MergeInbox();
            InboxWorkflow.MoveFilesToSubfolders(_workflowConfiguration);
            InboxWorkflow.CompressFiles(_workflowConfiguration);
        }

        private void MergeInbox()
        {
            _mergeInboxProcessor.MergeInbox(
                _logger,
                _workflowConfiguration.InboxPath,
                _workflowConfiguration.AlternativeInboxes,
                _workflowConfiguration.FilesToNeverMove
            );
        }

        private static void CompressFiles(InboxConfiguration workflowConfiguration)
        {
            foreach (ConvertToWebPRule rule in workflowConfiguration.ConvertToWebPRules)
            {
                IConversionSource inboxFolder = ConversionSourceFactory.GetConversionSource(rule.Directory,
                    new IFileProcessor[]
                    {
                        new ConvertToWebpProcessor(rule.Pattern,
                            new WebpFileQuality(rule.Quality),
                            rule.PreserveOriginalThresholdBytes,
                            new ChangeExtensionStrategy("webp"),
                            new EmptyConversionStrategy(),
                            new ConsoleLoggingStrategy(ConsoleLoggingStrategy.LogLevel.ActionsAndSuggestions)
                        )
                    });
                inboxFolder.Process();
            }
        }

        private static void MoveFilesToSubfolders(InboxConfiguration workflowConfiguration)
        {
            var fileProcessors = new IFileProcessor[]
            {
                new ChangeOfficeLensNameProcessor(new IsoDateFileNameFormatProvider()),
                new ChangeLocationProcessor(workflowConfiguration.ChangeLocationRules)
            };
            IConversionSource inboxFolder =
                ConversionSourceFactory.GetConversionSource(workflowConfiguration.InboxPath, fileProcessors);
            inboxFolder.Process();
        }
    }
}