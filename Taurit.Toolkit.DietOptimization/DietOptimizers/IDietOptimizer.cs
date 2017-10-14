using System.Collections.Generic;
using JetBrains.Annotations;
using Taurit.Toolkit.DietOptimization.Models;

namespace Taurit.Toolkit.DietOptimization.DietOptimizers
{
    /// <summary>
    ///     Interface to allow use different optimization methods for the same problem of diet optimization.
    /// </summary>
    public interface IDietOptimizer
    {
        [NotNull]
        DietPlan Optimize(IReadOnlyCollection<FoodProduct> availableProducts);
    }
}