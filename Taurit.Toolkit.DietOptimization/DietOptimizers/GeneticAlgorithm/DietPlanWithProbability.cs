using System;
using System.Diagnostics;
using Taurit.Toolkit.DietOptimization.Models;

namespace Taurit.Toolkit.DietOptimization.DietOptimizers.GeneticAlgorithm
{
    /// <summary>
    ///     Used in a context of genetic algorithm (GA) based optimization. Represents a diet plan in a single GA generation.
    ///     During selection phase of GA, diet plans are randomly chosen to be parents.
    ///     Diet plans that have characteristics closer to optimization target are more promising and are chosen with greater
    ///     probability.
    ///     Accumulated probability is stored instead of just probability as an implementation detail: it allows to easily map
    ///     random number from range [0..1] to concrete items.
    /// </summary>
    [DebuggerDisplay("Accumulated probability = {" + nameof(AccumulatedProbability) + "}")]
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