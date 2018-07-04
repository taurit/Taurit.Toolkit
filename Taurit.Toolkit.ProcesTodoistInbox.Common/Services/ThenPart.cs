using System;
using JetBrains.Annotations;
using Taurit.Toolkit.ProcesTodoistInbox.Common.Models.Classification;

namespace Taurit.Toolkit.ProcesTodoistInbox.Common.Services
{
    internal class ThenPart : QueryPart
    {
        public ThenPart([NotNull] String thenPart) : base(thenPart)
        {
        }


        [CanBeNull]
        private String SetLabel => GetStringArgument("setLabel");

        [CanBeNull]
        private String MoveToProject => GetStringArgument("moveToProject");

        [CanBeNull]
        private Int32? SetPriority => GetIntArgument("setPriority");

        [NotNull]
        public ClassificationRuleThen ToClassificationRule()
        {
            return new ClassificationRuleThen(SetPriority, SetLabel, MoveToProject);
        }
    }
}