using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Taurit.Toolkit.DietOptimization.Models;

namespace Taurit.Toolkit.DietOptimization.Services
{
    /// <summary>
    ///     Provides a metric for a "distance" between <see cref="DietCharacteristics" /> and <see cref="DietTarget" />.
    ///     This metric is a single numeric value ("score") that have the following values:
    ///     * it is equal 0 when diet characteristics fulfills the optimization target
    ///     * the closer is it to 0, the better diet characteristics matches target
    ///     * the greater score value, the more distant diet is from what user wanted
    ///     * the score is weighted, so that diet characteristic's compounds represented with different units doen't have too
    ///     much or too little impact on overall score.
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

            // theoretically energy can be derived from macronutient amounts, so it should not be a different variable,
            // but since margins are allowed for macro amounts, this might help to choose better solution
            score += PunishForCaloricIntakeDifference(diet, target);

            const Double macroMultplier = 5.0;
            score += PunishForDiffBelow(diet.TotalProtein,
                target.TotalProtein - DietTarget.MacronutrientToleranceMarginG,
                macroMultplier);
            score += PunishForDiffAbove(diet.TotalProtein,
                target.TotalProtein + DietTarget.MacronutrientToleranceMarginG,
                macroMultplier);
            score += PunishForDiffBelow(diet.TotalCarbs, target.TotalCarbs - DietTarget.MacronutrientToleranceMarginG,
                macroMultplier);
            score += PunishForDiffAbove(diet.TotalCarbs, target.TotalCarbs + DietTarget.MacronutrientToleranceMarginG,
                macroMultplier);
            score += PunishForDiffBelow(diet.TotalFat,
                target.TotalFat - DietTarget.MacronutrientToleranceMarginG, // test
                macroMultplier);
            score += PunishForDiffAbove(diet.TotalFat, target.TotalFat + DietTarget.MacronutrientToleranceMarginG,
                macroMultplier);

            // multiplers should be chosen, so the score increases by:
            // * 1000 if component was completely ignored
            // * 500 if component was underdosed/overdosed by about 50%
            score += GetScoreForVitaminA(diet);

            score += GetScoreForVitaminB(diet);
            score += GetScoreForVitaminC(diet);
            score += GetScoreForVitaminE(diet);
            score += GetScoreForVitaminK(diet);
            score += GetScoreForCholine(diet);

            score += GetScoreForFiber(diet, target.TotalKcalIntake);

            score += GetScoreForIron(diet);
            score += GetScoreForCalcium(diet);
            score += GetScoreForMagnesium(diet);
            score += GetScoreForPhosphorus(diet);
            score += GetScoreForPotassium(diet);
            score += GetScoreForSodium(diet);
            score += GetScoreForZinc(diet);

            score += GetScoreForFats(diet, target);

            // price constraint (multiplier will largely depend on the currency and time period!)
            // eg. multiplier=100 => every dollar beyond a threshold is treated as bad as 100 kcal miss
            // currently disabled to optimize other variables faster while debugging
            score += PunishForDiffAbove(diet.TotalPrice, target.MaxPrice, 100.0);

            // experimental: make sure there's not too many kilograms to eat ;)
            // currently disabled in scoring function to reduce number of constraints and achieve better results
            //score += PunishForDiffAbove(diet.TotalGramsEaten, 2500, 0.5);
            //score += PunishForDiffAbove(diet.TotalGramsEaten, 3000, 0.5);

            // experimental: having diets with the same characteristics, prefer ones that have less ingredients (easier shopping)
            score += PunishForLargeNumberOfIngredients(diet);

            return score;
        }

        private Double GetScoreForFats(DietCharacteristics diet, DietTarget target)
        {
            var score = 0d;

            score += PunishForDiffAbove(diet.TotalFattyAcidsSaturatedG, target.MaxGramsOfSaturatedFat, 50d);
            score += PunishForDiffAbove(diet.TotalFattyAcidsTransG, DietTarget.MaxTransFatsG, 100d);
            return score;
        }

        private Double PunishForCaloricIntakeDifference(DietCharacteristics diet, DietTarget target)
        {
            var score = 0d;
            score += PunishForDiffBelow(diet.TotalKcalIntake,
                target.TotalKcalIntake - DietTarget.EnergyToleranceMarginKcal,
                1.0);
            score += PunishForDiffAbove(diet.TotalKcalIntake,
                target.TotalKcalIntake + DietTarget.EnergyToleranceMarginKcal,
                1.0);
            return score;
        }

        [Pure]
        private static Double PunishForLargeNumberOfIngredients(DietCharacteristics diet)
        {
            const Int32 preferredMaxIngredientCount = 20; // I currently have 20 products on typical grocery shopping list
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
            score += PunishForDiffBelow(diet.TotalIronMg, DietTarget.MinDailyIronMg, 100);
            score += PunishForDiffAbove(diet.TotalIronMg, DietTarget.MaxDailyIronMg, 100);
            return score;
        }

        [Pure]
        private Double GetScoreForCalcium([NotNull] DietCharacteristics diet)
        {
            Double score = 0;
            score += PunishForDiffBelow(diet.TotalCalciumMg, DietTarget.MinDailyCalciumMg, 1);
            score += PunishForDiffAbove(diet.TotalCalciumMg, DietTarget.MaxDailyCalciumMg, 1);
            return score;
        }

        [Pure]
        private Double GetScoreForMagnesium([NotNull] DietCharacteristics diet)
        {
            Double score = 0;
            score += PunishForDiffBelow(diet.TotalMagnesiumMg, DietTarget.MinDailyMagnesiumMg, 3);
            return score;
        }

        [Pure]
        private Double GetScoreForPhosphorus([NotNull] DietCharacteristics diet)
        {
            Double score = 0;
            score += PunishForDiffBelow(diet.TotalPhosphorusMg, DietTarget.MinDailyPhosphorusMg, 2);
            score += PunishForDiffAbove(diet.TotalPhosphorusMg, DietTarget.MaxDailyPhosphorusMg, 3);
            return score;
        }

        [Pure]
        private Double GetScoreForPotassium([NotNull] DietCharacteristics diet)
        {
            Double score = 0;
            score += PunishForDiffBelow(diet.TotalPotassiumMg, DietTarget.MinDailyPotassiumMg, 0.5);
            score += PunishForDiffAbove(diet.TotalPotassiumMg, DietTarget.MaxDailyPotassiumMg, 0.5);
            return score;
        }

        [Pure]
        private Double GetScoreForSodium([NotNull] DietCharacteristics diet)
        {
            Double score = 0;
            score += PunishForDiffBelow(diet.TotalSodiumMg, DietTarget.MinDailySodiumMg, 1);
            score += PunishForDiffAbove(diet.TotalSodiumMg, DietTarget.MaxDailySodiumMg, 1);
            return score;
        }


        [Pure]
        private Double GetScoreForZinc([NotNull] DietCharacteristics diet)
        {
            Double score = 0;
            score += PunishForDiffBelow(diet.TotalZincMg, DietTarget.MinDailyZincMg, 100);
            score += PunishForDiffAbove(diet.TotalZincMg, DietTarget.MaxDailyZincMg, 100);
            return score;
        }


        [Pure]
        private Double GetScoreForFiber([NotNull] DietCharacteristics diet, Double targetTotalKcalIntake)
        {
            // Recommendations:
            // * children and adults should consume 14 grams of fiber for every 1,000 calories of food eaten.

            Double idealFiberAmountG = 14 * (targetTotalKcalIntake / 1000);
            Double score = PunishForDiffBelow(diet.TotalFiberGrams,
                idealFiberAmountG - DietTarget.FiberToleranceMarginG, 20);
            score += PunishForDiffAbove(diet.TotalFiberGrams, idealFiberAmountG + DietTarget.FiberToleranceMarginG, 20);
            return score;
        }
        
        [Pure]
        private Double GetScoreForVitaminA([NotNull] DietCharacteristics diet)
        {
            Double score = 0;
            score += PunishForDiffBelow(diet.TotalVitaminAiu, DietTarget.MinDailyVitaminAiu, 1);
            return score;
        }

        [Pure]
        private Double GetScoreForVitaminB([NotNull] DietCharacteristics diet)
        {
            Double score = 0;
            score += PunishForDiffBelow(diet.TotalVitaminB1Mg, DietTarget.MinDailyVitaminB1Mg, 100);
            score += PunishForDiffAbove(diet.TotalVitaminB1Mg, DietTarget.MaxDailyVitaminB1Mg, 100);

            score += PunishForDiffBelow(diet.TotalVitaminB2Mg, DietTarget.MinDailyVitaminB2Mg, 100);

            score += PunishForDiffBelow(diet.TotalVitaminB3Mg, DietTarget.MinDailyVitaminB3Mg, 100);
            score += PunishForDiffAbove(diet.TotalVitaminB3Mg, DietTarget.MaxDailyVitaminB3Mg, 15);

            score += PunishForDiffBelow(diet.TotalVitaminB5Mg, DietTarget.MinDailyVitaminB5Mg, 100);
            score += PunishForDiffAbove(diet.TotalVitaminB5Mg, DietTarget.MaxDailyVitaminB5Mg, 100);

            score += PunishForDiffBelow(diet.TotalVitaminB6Mg, DietTarget.MinDailyVitaminB6Mg, 100);
            score += PunishForDiffAbove(diet.TotalVitaminB6Mg, DietTarget.MaxDailyVitaminB6Mg, 100);

            score += PunishForDiffBelow(diet.TotalVitaminB12Mg, DietTarget.MinDailyVitaminB12Mg, 100);

            return score;
        }


        [Pure]
        private Double GetScoreForVitaminC([NotNull] DietCharacteristics diet)
        {
            Double score = 0;
            score += PunishForDiffBelow(diet.TotalVitaminCMg, DietTarget.MinDailyVitaminCMg, 5);
            score += PunishForDiffAbove(diet.TotalVitaminCMg, DietTarget.MaxDailyVitaminCMg, 5);
            return score;
        }

        [Pure]
        private Double GetScoreForVitaminE([NotNull] DietCharacteristics diet)
        {
            Double score = 0;
            score += PunishForDiffBelow(diet.TotalVitaminEMg, DietTarget.MinDailyVitaminEMg, 50);
            return score;
        }

        [Pure]
        private Double GetScoreForVitaminK([NotNull] DietCharacteristics diet)
        {
            Double score = 0;
            score += PunishForDiffBelow(diet.TotalVitaminKUg, DietTarget.MinDailyVitaminKUg, 10);
            return score;
        }

        [Pure]
        private Double GetScoreForCholine([NotNull] DietCharacteristics diet)
        {
            Double score = 0;
            score += PunishForDiffBelow(diet.TotalCholineMg, DietTarget.MinDailyCholineMg, 10);
            score += PunishForDiffAbove(diet.TotalCholineMg, DietTarget.MaxDailyCholineMg, 10);
            return score;
        }


        [Pure]
        private Double PunishForDiffAbove(Double actualValue, Double threshold, Double multiplier)
        {
            if (actualValue <= threshold)
            {
                return 0;
            }
            return Math.Abs(actualValue - threshold) * multiplier;
        }

        [Pure]
        private Double PunishForDiffBelow(Double actualValue, Double threshold, Double multiplier)
        {
            if (actualValue >= threshold)
            {
                return 0;
            }
            return Math.Abs(threshold - actualValue) * multiplier;
        }
    }
}