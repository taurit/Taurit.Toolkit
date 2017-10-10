using System;
using System.Collections.Generic;
using Taurit.Toolkit.DietOptimization.Models;

namespace Taurit.Toolkit.DietOptimization.Services
{
    public class DietCharacteristicsCalculator
    {
        public DietCharacteristics GetCharacteristics(IReadOnlyList<DietPlanItem> dietPlanItems)
        {
            Double totalKcal = 0;
            Double totalPrice = 0;
            Double totalProtein = 0;
            Double totalFat = 0;
            Double totalCarbs = 0;
            Double totalVitaminA = 0;
            Double totalVitaminC = 0;
            Double totalDietaryFiber = 0;

            var totalGrams = 0;
            foreach (DietPlanItem dietPlanItem in dietPlanItems)
            {
                Double amountMultiplier = dietPlanItem.AmountGrams / 100.0;

                totalKcal += dietPlanItem.FoodProduct.EnergyKcal * amountMultiplier;
                totalPrice += dietPlanItem.FoodProduct.Metadata.PricePerKg * amountMultiplier / 10.0;

                totalProtein += dietPlanItem.FoodProduct.PercentProtein * amountMultiplier;
                totalCarbs += dietPlanItem.FoodProduct.PercentCarb * amountMultiplier;
                totalFat += dietPlanItem.FoodProduct.PercentFat * amountMultiplier;

                totalVitaminA += dietPlanItem.FoodProduct.VitaminAIu * amountMultiplier;
                totalVitaminC += dietPlanItem.FoodProduct.VitaminCMg * amountMultiplier;
                totalDietaryFiber += dietPlanItem.FoodProduct.FiberTotalDietaryGrams * amountMultiplier;


                totalGrams += dietPlanItem.AmountGrams;
            }

            return new DietCharacteristics(totalKcal, totalPrice, totalProtein, totalFat, totalCarbs, totalVitaminA, totalVitaminC,
                totalDietaryFiber, totalGrams);
        }
    }
}