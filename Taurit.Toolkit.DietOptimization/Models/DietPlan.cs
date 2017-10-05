using System;
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
            [NotNull] DietCharacteristics characteristics, Double scoreToTarget)
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

        public Double ScoreToTarget { get; }

        [NotNull]
        public ImmutableList<DietPlanItem> DietPlanItems { get; }
    }
}