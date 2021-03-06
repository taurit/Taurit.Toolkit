﻿using System;
using JetBrains.Annotations;
using Taurit.Toolkit.ProcessTodoistInbox.Common.Models.ClassificationRuleModels;

namespace Taurit.Toolkit.ProcessTodoistInbox.Common.Models.ConciseClassificationRuleModel
{
    internal class IfPart : QueryPart
    {
        public IfPart([NotNull] String ifPart) : base(ifPart)
        {
        }

        [CanBeNull]
        private String[] ContainsWord => GetStringArrayArgument("containsWord");

        [CanBeNull]
        private String[] Contains => GetStringArrayArgument("contains");

        [CanBeNull]
        private String[] StartsWith => GetStringArrayArgument("startsWith");

        [CanBeNull]
        private String[] HasLabel => GetStringArrayArgument("hasLabel");

        [CanBeNull]
        private Int32? NumLabels => GetIntArgument("numLabelsIs");

        [CanBeNull]
        private String Project => GetStringArgument("projectIs");

        [CanBeNull]
        private String Duration => GetStringArgument("durationIs");

        [CanBeNull]
        private Int32? Priority => GetPriorityArgument("priorityIs");

        [NotNull]
        public ClassificationRuleIf ToClassificationRule()
        {
            return new ClassificationRuleIf(Contains, ContainsWord, StartsWith, Priority, Project, NumLabels, Duration,
                HasLabel);
        }
    }
}