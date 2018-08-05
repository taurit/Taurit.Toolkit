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
    internal class InboxWorkflow
    {
        private readonly InboxConfiguration _workflowConfiguration;

        internal InboxWorkflow([NotNull] InboxConfiguration workflowConfiguration)
        {
            _workflowConfiguration =
                workflowConfiguration ?? throw new ArgumentNullException(nameof(workflowConfiguration));
        }

        internal void Start()
        {
            MoveFilesToSubfolders(_workflowConfiguration);
            CompressFiles(_workflowConfiguration);
        }

        private void CompressFiles(InboxConfiguration workflowConfiguration)
        {
            foreach (ConvertToWebpRule rule in workflowConfiguration.ConvertToWebpRules)
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