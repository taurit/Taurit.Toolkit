using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Taurit.Toolkit.ProcessTodoistInbox.Common.Models.ClassificationRuleModels
{
    [DataContract]
    public class ClassificationRule
    {
        [Obsolete("Should only be used by deserializer")]
        public ClassificationRule()
        {
        }

        public ClassificationRule([NotNull] ClassificationRuleIf ifPart, [NotNull] ClassificationRuleThen thenPart)
        {
            If = ifPart;
            Then = thenPart;
        }

        [DataMember]
        public ClassificationRuleIf If { get; set; }

        [DataMember]
        public ClassificationRuleThen Then { get; set; }
    }
}