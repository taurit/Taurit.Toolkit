using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Taurit.Toolkit.DietOptimization.Models;

namespace Taurit.Toolkit.DietOptimization.Services
{
    /// <summary>
    ///     Provides a metric for a "distance" between two diet characteristics.
    ///     This metric is a single numeric value ("score") that have the following values:
    ///     * it is equal 0 when two diet characteristics are exactly the same
    ///     * its absolute value is greater for characteristics that are more distant
    ///     * the closer is it to 0, the better diet characteristics matches
    ///     * the score is weighted, so that diet characteristic's compounds represented with different units doen't have too
    ///     much or too little impact on overall score.
    ///     * recommendations used are for adult males (18+ years old)
    /// </summary>
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "It is instantiated by Ninject")]
    public class ScoreCalculator
    {
        /// <remarks>
        ///     This method is executed *a lot*, so it should be simple and fast
        /// </remarks>
        [Pure]
        public Double CalculateScore(DietCharacteristics diet, DietTarget target)
        {
            Double score = 0;
            score += PunishForDiffBelow(diet.TotalKcalIntake - DietTarget.EnergyToleranceMarginKcal, target.TotalKcalIntake,
                1.0);
            score += PunishForDiffAbove(diet.TotalKcalIntake + DietTarget.EnergyToleranceMarginKcal, target.TotalKcalIntake,
                1.0);

            // 20g miss in macronutrients like 100kcal miss in diet overall
            const Double macroMultplier = 1.0;
            score += PunishForDiffBelow(diet.TotalProtein, target.TotalProtein - DietTarget.MacronutrientToleranceMarginG,
                macroMultplier);
            score += PunishForDiffAbove(diet.TotalProtein, target.TotalProtein + DietTarget.MacronutrientToleranceMarginG,
                macroMultplier);
            score += PunishForDiffBelow(diet.TotalCarbs, target.TotalCarbs - DietTarget.MacronutrientToleranceMarginG,
                macroMultplier);
            score += PunishForDiffAbove(diet.TotalCarbs, target.TotalCarbs + DietTarget.MacronutrientToleranceMarginG,
                macroMultplier);
            score += PunishForDiffBelow(diet.TotalFat, target.TotalFat - DietTarget.MacronutrientToleranceMarginG,
                macroMultplier);
            score += PunishForDiffAbove(diet.TotalFat, target.TotalFat + DietTarget.MacronutrientToleranceMarginG,
                macroMultplier);

            // multiplers should be chosen, so the score increases by:
            // * 1000 if component was completely ignored
            // * 500 if component was underdosed/overdosed by about 50%
            score += GetScoreForVitaminC(diet);
            score += GetScoreForVitaminA(diet);
            score += GetScoreForFiber(diet, target.TotalKcalIntake);

            score += GetScoreForIron(diet);
            score += GetScoreForCalcium(diet);
            score += GetScoreForMagnesium(diet);
            score += GetScoreForPhosphorus(diet);
            score += GetScoreForPotassium(diet);
            score += GetScoreForSodium(diet);
            score += GetScoreForZinc(diet);

            // price constraint (multiplier will largely depend on the currency and time period!)
            // eg. multiplier=100 => every dollar beyond a threshold is treated as bad as 100 kcal miss
            // currently disabled to optimize other variables faster while debugging
            //score += PunishForDiffAbove(target.MaxPrice, diet.TotalPrice, 100.0);

            // experimental: make sure there's not too many kilograms to eat ;)
            // currently disabled in scoring function to reduce number of constraints and achieve better results
            //score += PunishForDiffAbove(2500, diet.TotalGramsEaten, 0.5);
            //score += PunishForDiffAbove(3000, diet.TotalGramsEaten, 0.5);

            // experimental: having diets with the same characteristics, prefer ones that have less ingredients (easier shopping)
            score += PunishForLargeNumberOfIngredients(diet);

            return score;
        }

        [Pure]
        private static Double PunishForLargeNumberOfIngredients(DietCharacteristics diet)
        {
            const Int32 preferredMaxIngredientCount = 15;
            Int32 numExcessiveIngredients = diet.NumIngredients - preferredMaxIngredientCount;
            if (numExcessiveIngredients < 0)
            {
                numExcessiveIngredients = 0;
            }
            Double score = numExcessiveIngredients * (100 / 10); // 10 extra ingredients is the same as 100 kcal miss
            return score;
        }

        [Pure]
        private Double GetScoreForIron([NotNull] DietCharacteristics diet)
        {
            Double score = 0;
            score += PunishForDiffBelow(DietTarget.MinDailyIronMg, diet.TotalIronMg, 100);
            score += PunishForDiffAbove(DietTarget.MaxDailyIronMg, diet.TotalIronMg, 100);
            return score;
        }

        [Pure]
        private Double GetScoreForCalcium([NotNull] DietCharacteristics diet)
        {
            Double score = 0;
            score += PunishForDiffBelow(DietTarget.MinDailyCalciumMg, diet.TotalCalciumMg, 1);
            score += PunishForDiffAbove(DietTarget.MaxDailyCalciumMg, diet.TotalCalciumMg, 1);
            return score;
        }

        [Pure]
        private Double GetScoreForMagnesium([NotNull] DietCharacteristics diet)
        {
            Double score = 0;
            score += PunishForDiffBelow(DietTarget.MinDailyMagnesiumMg, diet.TotalMagnesiumMg, 3);
            return score;
        }

        [Pure]
        private Double GetScoreForPhosphorus([NotNull] DietCharacteristics diet)
        {
            Double score = 0;
            score += PunishForDiffBelow(DietTarget.MinDailyPhosphorusMg, diet.TotalPhosphorusMg, 2);
            score += PunishForDiffAbove(DietTarget.MaxDailyPhosphorusMg, diet.TotalPhosphorusMg, 3);
            return score;
        }

        [Pure]
        private Double GetScoreForPotassium([NotNull] DietCharacteristics diet)
        {
            Double score = 0;
            score += PunishForDiffBelow(DietTarget.MinDailyPotassiumMg, diet.TotalPotassiumMg, 0.5);
            score += PunishForDiffAbove(DietTarget.MaxDailyPotassiumMg, diet.TotalPotassiumMg, 0.5);
            return score;
        }

        [Pure]
        private Double GetScoreForSodium([NotNull] DietCharacteristics diet)
        {
            Double score = 0;
            score += PunishForDiffBelow(DietTarget.MinDailySodiumMg, diet.TotalSodiumMg, 1);
            score += PunishForDiffAbove(DietTarget.MaxDailySodiumMg, diet.TotalSodiumMg, 1);
            return score;
        }


        [Pure]
        private Double GetScoreForZinc([NotNull] DietCharacteristics diet)
        {
            Double score = 0;
            score += PunishForDiffBelow(DietTarget.MinDailyZincMg, diet.TotalZincMg, 100);
            score += PunishForDiffAbove(DietTarget.MaxDailyZincMg, diet.TotalZincMg, 100);
            return score;
        }


        [Pure]
        private Double GetScoreForFiber([NotNull] DietCharacteristics diet, Double targetTotalKcalIntake)
        {
            // Recommendations:
            // * children and adults should consume 14 grams of fiber for every 1,000 calories of food eaten.

            var idealFiberAmountG = 14 * (targetTotalKcalIntake / 1000);
            Double score = PunishForDiffBelow(diet.TotalFiberGrams, idealFiberAmountG - DietTarget.FiberToleranceMarginG, 20);
            score += PunishForDiffAbove(diet.TotalFiberGrams, idealFiberAmountG + DietTarget.FiberToleranceMarginG, 20);
            return score;
        }

        [Pure]
        private Double GetScoreForVitaminC([NotNull] DietCharacteristics diet)
        {
            Double score = 0;
            score += PunishForDiffBelow(DietTarget.MinDailyVitaminCMg, diet.TotalVitaminCMg, 5);
            score += PunishForDiffAbove(DietTarget.MaxDailyVitaminCMg, diet.TotalVitaminCMg, 5);
            return score;
        }

        [Pure]
        private Double GetScoreForVitaminA([NotNull] DietCharacteristics diet)
        {
            Double score = 0;
            score += PunishForDiffBelow(DietTarget.MinDailyVitaminAiu, diet.TotalVitaminAiu, 1);
            return score;
        }

        [Pure]
        [Obsolete("Try avoid in attempt for optimization to not fall into local minima")]
        private Double PunishForDiff(Double actualValue, Double targetValue, Double multiplier)
        {
            return Math.Abs(actualValue - targetValue) * multiplier;
        }

        [Pure]
        private Double PunishForDiffAbove(Double threshold, Double actualValue, Double multiplier)
        {
            if (actualValue <= threshold)
            {
                return 0;
            }
            return Math.Abs(actualValue - threshold) * multiplier;
        }

        [Pure]
        private Double PunishForDiffBelow(Double threshold, Double actualValue, Double multiplier)
        {
            if (actualValue >= threshold)
            {
                return 0;
            }
            return Math.Abs(threshold - actualValue) * multiplier;
        }
    }
}