using System;
using System.Diagnostics;

namespace Taurit.Toolkit.DietOptimization.Models
{
    /// <summary>
    ///     Represents a single food product (eg. tomato) and its quantity.
    ///     Usually used in a context of Diet Plan, where <see cref="DietPlan" /> is parent and multiple
    ///     <see cref="DietPlanItem" />s are children.
    /// </summary>
    [DebuggerDisplay("{FoodProduct.Name}, {AmountGrams}g")]
    public class DietPlanItem
    {
        public DietPlanItem(FoodProduct foodProduct, Double amountGrams)
        {
            Debug.Assert(amountGrams >= 0);

            FoodProduct = foodProduct;
            AmountGrams = amountGrams;
        }

        public FoodProduct FoodProduct { get; }
        public Double AmountGrams { get; }
    }
}