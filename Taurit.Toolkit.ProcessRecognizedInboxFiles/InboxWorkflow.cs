using System;
using JetBrains.Annotations;
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
        [NotNull] private readonly IMergeInboxProcessor _mergeInboxProcessor;
        [NotNull] private readonly InboxConfiguration _workflowConfiguration;

        public InboxWorkflow([NotNull] InboxConfiguration workflowConfiguration,
            [NotNull] IMergeInboxProcessor mergeInboxProcessor)
        {
            _workflowConfiguration =
                workflowConfiguration ?? throw new ArgumentNullException(nameof(workflowConfiguration));
            _mergeInboxProcessor = mergeInboxProcessor ?? throw new ArgumentNullException(nameof(mergeInboxProcessor));
        }

        internal void Start()
        {
            MergeInbox();
            MoveFilesToSubfolders(_workflowConfiguration);
            CompressFiles(_workflowConfiguration);
        }

        private void MergeInbox()
        {
            _mergeInboxProcessor.MergeInbox(_workflowConfiguration.InboxPath,
                _workflowConfiguration.AlternativeInboxes, _workflowConfiguration.FilesToNeverMove);
        }

        private void CompressFiles(InboxConfiguration workflowConfiguration)
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

        private void MoveFilesToSubfolders(InboxConfiguration workflowConfiguration)
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