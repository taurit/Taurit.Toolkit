using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Taurit.Toolkit.ProcesTodoistInbox.Common.Models.Classification;

namespace Taurit.Toolkit.ProcesTodoistInbox.Common.Models
{
    public class SettingsFileModel
    {
        [JsonProperty]
        public String TodoistApiKey { get; set; }

        [JsonProperty]
        public String SnapshotsFolder { get; set; }

        [JsonProperty]
        public List<String> AlternativeInboxes { get; set; }

        [JsonProperty]
        public List<ClassificationRule> ClassificationRules { get; set; }

        [JsonProperty]
        public List<String> ClassificationRulesConcise { get; set; }

        [JsonProperty]
        public String ApplicationInsightsKey { get; set; }
    }
}