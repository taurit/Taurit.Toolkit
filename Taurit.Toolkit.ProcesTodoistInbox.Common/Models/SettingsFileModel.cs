using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Taurit.Toolkit.ProcessTodoistInbox.Common.Models
{
    [DataContract]
    public class SettingsFileModel
    {
        [DataMember]
        public String TodoistApiKey { get; set; }

        [DataMember]
        public String SnapshotsFolder { get; set; }

        [DataMember]
        public List<String> AlternativeInboxes { get; set; }

        [DataMember]
        public List<String> ClassificationRulesConcise { get; set; }

        [DataMember]
        public String ApplicationInsightsKey { get; set; }
    }
}