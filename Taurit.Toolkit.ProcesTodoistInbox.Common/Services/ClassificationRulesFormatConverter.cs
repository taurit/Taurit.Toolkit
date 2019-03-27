using System;
using System.Diagnostics;
using JetBrains.Annotations;
using Taurit.Toolkit.ProcessTodoistInbox.Common.Models.Classification;

namespace Taurit.Toolkit.ProcessTodoistInbox.Common.Services
{
    public class ClassificationRulesFormatConverter : IClassificationRulesFormatConverter
    {
        [NotNull]
        public ClassificationRule Convert([NotNull] String conciseRule)
        {
            if (conciseRule == null) throw new ArgumentNullException(nameof(conciseRule));

            String[] ruleSplitToConditionAndActionPart = conciseRule.Trim()
                .Split(new[] {" then "}, StringSplitOptions.RemoveEmptyEntries);
            if (ruleSplitToConditionAndActionPart.Length != 2)
            {
                throw new ArgumentException("conciseRule could not be split into 'if' and 'then' parts",
                    nameof(conciseRule));
            }

            if (ruleSplitToConditionAndActionPart[0] == "if")
                throw new ArgumentException("'if' clause must not be empty", nameof(conciseRule));
            Debug.Assert(ruleSplitToConditionAndActionPart[1] != null);

            var ifPart = new IfPart(ruleSplitToConditionAndActionPart[0]);
            var thenPart = new ThenPart(ruleSplitToConditionAndActionPart[1]);

            return new ClassificationRule(ifPart.ToClassificationRule(), thenPart.ToClassificationRule());
        }
    }
}