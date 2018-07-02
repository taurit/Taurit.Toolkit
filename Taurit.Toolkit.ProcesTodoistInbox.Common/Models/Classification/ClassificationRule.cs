using Newtonsoft.Json;

namespace Taurit.Toolkit.ProcesTodoistInbox.Common.Models.Classification
{
    public class ClassificationRule
    {
        [JsonProperty]
        public ClassificationRuleIf If { get; set; }

        [JsonProperty]
        public ClassificationRuleThen Then { get; set; }
    }
}