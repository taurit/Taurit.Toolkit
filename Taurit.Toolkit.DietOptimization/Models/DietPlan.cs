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
        /// <summary>
        ///     The American Heart Association recommends aiming for a dietary pattern that achieves 5 % to 6 % of calories from
        ///     saturated fat. Eg 0.05 means 5%.
        /// </summary>
        public const Double MaxAmountEnergyFromSaturatedFats = 0.05;

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