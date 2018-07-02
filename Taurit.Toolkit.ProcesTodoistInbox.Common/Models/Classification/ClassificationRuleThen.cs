using System;
using Newtonsoft.Json;

namespace Taurit.Toolkit.ProcesTodoistInbox.Common.Models.Classification
{
    public class ClassificationRuleThen
    {
        [JsonProperty]
        public Int32? setPriority { get; set; }

        [JsonProperty]
        public String setLabel { get; set; }

        [JsonProperty]
        public String moveToProject { get; set; }
    }
}