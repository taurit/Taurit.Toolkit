using System;
using Taurit.Toolkit.ProcesTodoistInbox.Common.Models.Classification;

namespace Taurit.Toolkit.ProcesTodoistInbox.Common.Services
{
    public interface IClassificationRulesFormatConverter
    {
        ClassificationRule Convert(String conciseRule);
    }
}