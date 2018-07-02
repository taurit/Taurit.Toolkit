using System;
using JetBrains.Annotations;
using Taurit.Toolkit.ProcesTodoistInbox.Common.Models.Classification;

namespace Taurit.Toolkit.ProcesTodoistInbox.Common.Services
{
    public class ClassificationRulesFormatConverter : IClassificationRulesFormatConverter
    {
        [NotNull]
        public ClassificationRule Convert(String conciseRule)
        {
            return new ClassificationRule();
        }
    }
}