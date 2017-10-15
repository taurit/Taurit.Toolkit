using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Taurit.Toolkit.DietOptimization.Models;

namespace Taurit.Toolkit.DietOptimization.Services
{
    /// <summary>
    ///     Calculate <see cref="DietCharacteristics" /> (statistics) for a <see cref="DietPlan" />.
    ///     Mainly, calculate total amounts of macronutrients and micronutrients for all positions in a diet (with their
    ///     specific amounts).
    /// </summary>
    /// ReSharper disable once ClassNeverInstantiated.Global - it is by Ninject
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
            Double totalIronMg = 0;
            Double totalCalciumMg = 0;
            Double totalMagnesiumMg = 0;
            Double totalPhosphorusMg = 0;
            Double totalPotassiumMg = 0;
            Double totalSodiumMg = 0;
            Double totalZincMg = 0;
            Double totalFattyAcidsSaturatedG = 0;
            Double totalFattyAcidsMonounsaturatedG = 0;
            Double totalFattyAcidsPolyunsaturatedG = 0;
            Double totalFattyAcidsTransG = 0;
            Double totalCholesterolMg = 0;
            Double totalOmega3 = 0;
            Double totalOmega6 = 0;
            Double totalCopperMg = 0;
            Double totalManganeseMg = 0;
            Double totalSeleniumUg = 0;

            var numIngredients = 0;

            var totalGrams = 0d;
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

                totalIronMg += dietPlanItem.FoodProduct.IronMg * amountMultiplier;
                totalCalciumMg += dietPlanItem.FoodProduct.CalciumMg * amountMultiplier;
                totalMagnesiumMg += dietPlanItem.FoodProduct.MagnesiumMg * amountMultiplier;
                totalPhosphorusMg += dietPlanItem.FoodProduct.PhosphorusMg * amountMultiplier;
                totalPotassiumMg += dietPlanItem.FoodProduct.PotassiumMg * amountMultiplier;
                totalSodiumMg += dietPlanItem.FoodProduct.SodiumMg * amountMultiplier;
                totalZincMg += dietPlanItem.FoodProduct.ZincMg * amountMultiplier;

                totalFattyAcidsSaturatedG += dietPlanItem.FoodProduct.FattyAcidsTotalSaturatedG * amountMultiplier;
                totalFattyAcidsMonounsaturatedG += dietPlanItem.FoodProduct.FattyAcidsTotalMonounsaturatedG *
                                                   amountMultiplier;
                totalFattyAcidsPolyunsaturatedG += dietPlanItem.FoodProduct.FattyAcidsTotalPolyunsaturatedG *
                                                   amountMultiplier;
                totalFattyAcidsTransG += dietPlanItem.FoodProduct.FattyAcidsTotalTransG * amountMultiplier;
                totalCholesterolMg += dietPlanItem.FoodProduct.CholesterolMg * amountMultiplier;
                totalOmega3 += dietPlanItem.FoodProduct.Omega3 * amountMultiplier;
                totalOmega6 += dietPlanItem.FoodProduct.Omega6 * amountMultiplier;

                totalCopperMg += dietPlanItem.FoodProduct.CopperMg * amountMultiplier;
                totalManganeseMg += dietPlanItem.FoodProduct.ManganeseMg * amountMultiplier;
                totalSeleniumUg += dietPlanItem.FoodProduct.SeleniumUg * amountMultiplier;

                totalGrams += dietPlanItem.AmountGrams;
                numIngredients += dietPlanItem.AmountGrams > 0 ? 1 : 0;
            }

            return new DietCharacteristics(totalKcal,
                totalPrice,
                numIngredients,
                totalProtein,
                totalFat,
                totalCarbs,
                totalVitaminA,
                totalVitaminC,
                totalDietaryFiber,
                totalIronMg,
                totalCalciumMg,
                totalMagnesiumMg,
                totalPhosphorusMg,
                totalPotassiumMg,
                totalSodiumMg,
                totalZincMg,
                totalGrams,
                totalFattyAcidsSaturatedG,
                totalFattyAcidsMonounsaturatedG,
                totalFattyAcidsPolyunsaturatedG,
                totalFattyAcidsTransG,
                totalCholesterolMg,
                totalOmega3,
                totalOmega6,
                totalCopperMg,
                totalManganeseMg, 
                totalSeleniumUg
            );
        }
    }
}