using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Taurit.Toolkit.ProcessTodoistInbox.Models
{
    internal class SettingsFileModel
    {
        [JsonProperty]
        public String TodoistApiKey { get; set; }

        [JsonProperty]
        public List<ClassificationRule> ClassificationRules { get; set; }
    }
}