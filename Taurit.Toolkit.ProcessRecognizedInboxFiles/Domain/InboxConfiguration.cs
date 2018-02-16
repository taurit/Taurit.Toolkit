using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Taurit.Toolkit.FileProcessors.LocationProcessors;
using Taurit.Toolkit.ProcessRecognizedInboxFiles.Models;

namespace Taurit.Toolkit.ProcessRecognizedInboxFiles.Domain
{
    internal class InboxConfiguration
    {
        public InboxConfiguration([NotNull] String configJsonPath)
        {
            if (configJsonPath == null) throw new ArgumentNullException(nameof(configJsonPath));

            if (!File.Exists(configJsonPath)) throw new ArgumentException("Config file does not exist");

            var config = JsonConvert.DeserializeObject<InboxConfigFile>(File.ReadAllText(configJsonPath));
            InboxPath = config.InboxFolder;

            var rules = new List<ChangeLocationRule>();
            foreach (MoveToLocationRule rule in config.MoveToLocationRules)
            foreach (String pattern in rule.Patterns)
                rules.Add(new ChangeLocationRule(pattern, rule.TargetLocation));

            ChangeLocationRules = rules.AsReadOnly();
            ConvertToWebpRules = config.ConvertToWebpRules;
        }

        public IReadOnlyList<ChangeLocationRule> ChangeLocationRules { get; }
        public String InboxPath { get; }
        public IEnumerable<ConvertToWebpRule> ConvertToWebpRules { get; }
    }
}