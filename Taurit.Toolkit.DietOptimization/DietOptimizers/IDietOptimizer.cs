using System.Collections.Generic;
using JetBrains.Annotations;
using Taurit.Toolkit.DietOptimization.Models;

namespace Taurit.Toolkit.DietOptimization.DietOptimizers
{
    public interface IDietOptimizer
    {
        [NotNull]
        DietPlan Optimize(IReadOnlyCollection<FoodProduct> availableProducts);
    }
}