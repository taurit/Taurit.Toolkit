using System;
using Newtonsoft.Json;

// ReSharper disable InconsistentNaming

namespace Taurit.Toolkit.ProcesTodoistInbox.Common.Models.Classification
{
    public class ClassificationRuleThen
    {
        [JsonConstructor]
        [Obsolete("Use only if you are JSON deserializer")]
        public ClassificationRuleThen()
        {
        }

        public ClassificationRuleThen(Int32? setPriority, String setLabel, String moveToProject)
        {
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
    }
}