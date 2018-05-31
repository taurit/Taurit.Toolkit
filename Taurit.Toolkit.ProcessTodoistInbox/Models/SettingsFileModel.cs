using System;
using Newtonsoft.Json;

namespace Taurit.Toolkit.ProcessTodoistInbox.Models
{
    internal class SettingsFileModel
    {
        [JsonProperty]
        public String TodoistApiKey { get; set; }
    }
}