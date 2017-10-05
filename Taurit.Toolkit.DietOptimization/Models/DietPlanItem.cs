using System;
using System.Diagnostics;

namespace Taurit.Toolkit.DietOptimization.Models
{
    [DebuggerDisplay("{FoodProduct.Name}, {AmountGrams}g")]
    public class DietPlanItem
    {
        public DietPlanItem(FoodProduct foodProduct, Int32 amountGrams)
        {
            Debug.Assert(amountGrams >= 0);

            FoodProduct = foodProduct;
            AmountGrams = amountGrams;
        }

        public FoodProduct FoodProduct { get; }
        public Int32 AmountGrams { get; }
    }
}