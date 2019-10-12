using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

// ReSharper disable InconsistentNaming

namespace Taurit.Toolkit.ProcessTodoistInbox.Common.Models.ClassificationRuleModels
{
    /// <summary>
    /// Represents part of the classification rule that defines the action
    /// </summary>
    [SuppressMessage("Microsoft.Design", "IDE1006", Justification = "Names are aligned with JSON property names")]
    public class ClassificationRuleThen
    {
        [JsonConstructor]
        [Obsolete("Use only if you are JSON deserializer")]
        public ClassificationRuleThen()
        {
        }

        public ClassificationRuleThen(Int32? setPriority, String setLabel, String moveToProject, String setDuration)
        {
            this.setDuration = setDuration;
            this.setPriority = setPriority;
            this.setLabel = setLabel;
            this.moveToProject = moveToProject;
        }

        [JsonProperty]
        public Int32? setPriority { get; set; }

        [JsonProperty]
        public String setLabel { get; set; }

        [JsonProperty]
        public String moveToProject { get; set; }

        [JsonProperty]
        public String setDuration { get; set; }
    }
}