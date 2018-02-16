using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Taurit.Toolkit.FileProcessors.LocationProcessors;
using Taurit.Toolkit.ProcessRecognizedInboxFiles.Models;

namespace Taurit.Toolkit.ProcessRecognizedInboxFiles.Domain
{
    internal class InboxConfiguration
    {
        public InboxConfiguration(String configJsonPath)
        {
            if (!File.Exists(configJsonPath))
            {
                throw new ArgumentException("Config file does not exist");
            }

            var config = JsonConvert.DeserializeObject<InboxConfigFile>(File.ReadAllText(configJsonPath));
            InboxPath = config.InboxFolder;
            ChangeLocationRules = config.MoveToLocationRules
                .Select(x => new ChangeLocationRule(x.Pattern, x.TargetLocation)).ToList().AsReadOnly();
        }

        public IReadOnlyList<ChangeLocationRule> ChangeLocationRules { get; }
        public String InboxPath { get; }
    }
}