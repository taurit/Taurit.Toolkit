using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Taurit.Toolkit.FileProcessors.Exceptions;
using Taurit.Toolkit.FileProcessors.LocationProcessors;
using Taurit.Toolkit.ProcessRecognizedInboxFiles.Models;
using Taurit.Toolkit.ProcessRecognizedInboxFiles.Services;

namespace Taurit.Toolkit.ProcessRecognizedInboxFiles.Domain
{
    public class InboxConfiguration
    {
        private readonly IPathPlaceholderResolver _pathPlaceholderResolver;

        public InboxConfiguration(IPathPlaceholderResolver pathPlaceholderResolver, Options options)
        {
            if (options.ConfigurationFilePath == null) throw new ArgumentNullException(nameof(options.ConfigurationFilePath));
            if (!File.Exists(options.ConfigurationFilePath)) throw new ArgumentException("Config file does not exist");
            
            _pathPlaceholderResolver = pathPlaceholderResolver ?? throw new ArgumentNullException(nameof(pathPlaceholderResolver));

            var config = JsonConvert.DeserializeObject<InboxConfigFile>(File.ReadAllText(options.ConfigurationFilePath));
            if (config.AlternativeInboxes is null) throw new ArgumentException("Config file requires 'AlternativeInboxes' to be properly defined");
            if (config.InboxFolder is null) throw new ArgumentException("Config file requires 'InboxFolder' to be properly defined");
            if (config.FilesToNeverMove is null) throw new ArgumentException("Config file requires 'FilesToNeverMove' to be properly defined");
            if (config.MoveToLocationRules is null) throw new ArgumentException("Config file requires 'MoveToLocationRules' to be properly defined");
            if (config.ConvertToWebpRules is null) throw new ArgumentException("Config file requires 'ConvertToWebPRules' to be properly defined");

            if (!Directory.Exists(config.InboxFolder))
                throw new ArgumentException($"Inbox folder {config.InboxFolder} doesn't exist");
            foreach (String alternativeInbox in config.AlternativeInboxes)
            {
                if (!Directory.Exists(alternativeInbox))
                    throw new InvalidConfigurationException(
                        $"Alternative Inbox folder '{alternativeInbox}' doesn't exist");
            }

            InboxPath = config.InboxFolder;
            AlternativeInboxes = config.AlternativeInboxes;
            FilesToNeverMove = new HashSet<String>(config.FilesToNeverMove);
            var rules = new List<ChangeLocationRule>();
            foreach (MoveToLocationRule rule in config.MoveToLocationRules)
            {
                if (rule.TargetLocation == null)
                    throw new InvalidConfigurationException(
                        "One of the rules doesn't have a TargetLocation defined, which is invalid.");

                if (rule.Patterns == null)
                    throw new InvalidConfigurationException(
                        $"A rule to move files to '{rule.TargetLocation}' does not specify any patterns, which is invalid.");

                String targetLocation = _pathPlaceholderResolver.Resolve(rule.TargetLocation);

                if (rule.Patterns != null)
                {
                    foreach (String pattern in rule.Patterns)
                        rules.Add(new ChangeLocationRule(pattern, targetLocation));
                }
            }
            
            ChangeLocationRules = rules.AsReadOnly();
            ConvertToWebPRules = config.ConvertToWebpRules;
        }

        public IReadOnlyList<ChangeLocationRule> ChangeLocationRules { get; }
        public String InboxPath { get; }
        public List<String> AlternativeInboxes { get; }
        public IEnumerable<ConvertToWebPRule> ConvertToWebPRules { get; }
        public ISet<String> FilesToNeverMove { get; }
    }
}