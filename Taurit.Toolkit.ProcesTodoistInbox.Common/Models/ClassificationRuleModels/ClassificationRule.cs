﻿using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Taurit.Toolkit.ProcessTodoistInbox.Common.Models.Classification
{
    public class ClassificationRule
    {
        [JsonConstructor]
        [Obsolete("Should only be used by deserializer")]
        public ClassificationRule()
        {
        }

        public ClassificationRule([NotNull] ClassificationRuleIf ifPart, [NotNull] ClassificationRuleThen thenPart)
        {
            If = ifPart;
            Then = thenPart;
        }

        [JsonProperty]
        public ClassificationRuleIf If { get; set; }

        [JsonProperty]
        public ClassificationRuleThen Then { get; set; }
    }
}