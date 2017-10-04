using System.Collections.Generic;
using Taurit.Toolkit.DietOptimization.Models;

namespace Taurit.Toolkit.DietOptimization.Services
{
    public class DietCharacteristicsCalculator
    {
        public DietCharacteristics GetCharacteristics(IReadOnlyList<DietPlanItem> dietPlanItems)
        {
            double totalKcal = 0;
            double totalProtein = 0;
            double totalFat = 0;
            double totalCarbs = 0;
            foreach (var dietPlanItem in dietPlanItems)
            {
                double amountMultiplier = (dietPlanItem.AmountGrams / 100.0);
                totalKcal += dietPlanItem.FoodProduct.EnergyKcal * amountMultiplier;
                totalProtein += dietPlanItem.FoodProduct.PercentProtein * amountMultiplier;
                totalCarbs += dietPlanItem.FoodProduct.PercentCarb * amountMultiplier;
                totalFat += dietPlanItem.FoodProduct.PercentFat * amountMultiplier;
                
            }

            return new DietCharacteristics(totalKcal, totalProtein, totalFat, totalCarbs);
        }
    }
}