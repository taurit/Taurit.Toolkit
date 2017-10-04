using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Taurit.Toolkit.DietOptimization.Models
{
    [DebuggerDisplay("Score: {" + nameof(ScoreToTarget) + "}")]
    public class DietPlan
    {
        public DietPlan([NotNull] IReadOnlyList<DietPlanItem> dietPlanItems,
            [NotNull] DietCharacteristics characteristics, double scoreToTarget)
        {
            Debug.Assert(dietPlanItems != null);
            Debug.Assert(characteristics != null);
            Debug.Assert(scoreToTarget >= 0);

            DietPlanItems = dietPlanItems.ToImmutableList();
            Characteristics = characteristics;
            ScoreToTarget = scoreToTarget;
        }

        [NotNull]
        public DietCharacteristics Characteristics { get; }

        public double ScoreToTarget { get; }

        [NotNull]
        public ImmutableList<DietPlanItem> DietPlanItems { get; }
    }
}