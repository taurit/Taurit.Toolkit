using System;
using JetBrains.Annotations;
using Taurit.Toolkit.ProcesTodoistInbox.Common.Models.Classification;

namespace Taurit.Toolkit.ProcesTodoistInbox.Common.Services
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
            return new ClassificationRuleIf(Contains, ContainsWord, StartsWith, Priority, Project, NumLabels, Duration);
        }
    }
}