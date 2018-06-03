using Newtonsoft.Json;

namespace Taurit.Toolkit.ProcessTodoistInbox.Models.Classification
{
    public class ClassificationRule
    {
        [JsonProperty]
        public ClassificationRuleIf If { get; set; }

        [JsonProperty]
        public ClassificationRuleThen Then { get; set; }
    }
}