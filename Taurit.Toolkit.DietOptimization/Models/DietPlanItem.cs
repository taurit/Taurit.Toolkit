using System.Diagnostics;

namespace Taurit.Toolkit.DietOptimization.Models
{
    [DebuggerDisplay("{FoodProduct.Name}, {AmountGrams}g")]
    public class DietPlanItem
    {
        public DietPlanItem(FoodProduct foodProduct, int amountGrams)
        {
            FoodProduct = foodProduct;
            AmountGrams = amountGrams;
        }

        public FoodProduct FoodProduct { get; }
        public int AmountGrams { get; }
    }
}