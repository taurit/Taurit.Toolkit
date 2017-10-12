using System;
using System.Diagnostics;

namespace Taurit.Toolkit.DietOptimization.Models
{
    [DebuggerDisplay("Acc. probability = {" + nameof(AccumulatedProbability) + "}")]
    public sealed class DietPlanWithProbability
    {
        public DietPlanWithProbability(DietPlan dietPlan, Double accumulatedProbability)
        {
            DietPlan = dietPlan;
            AccumulatedProbability = accumulatedProbability;
        }

        public DietPlan DietPlan { get; }
        public Double AccumulatedProbability { get; }
    }
}