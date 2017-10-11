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
            score += PunishForDiferrence(diet.TotalKcalIntake, target.TotalKcalIntake, 1.0);
            score += PunishForDiferrence(diet.TotalProtein, target.TotalProtein, 1.0);
            score += PunishForDiferrence(diet.TotalCarbs, target.TotalCarbs, 1.0);
            score += PunishForDiferrence(diet.TotalFat, target.TotalFat, 1.0);

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
            score += PunishForDifferenceAbove(target.MaxPrice, diet.TotalPrice, 100.0); 

            // experimental: make sure there's not too many kilograms to eat ;)
            score += PunishForDifferenceAbove(2500, diet.TotalGramsEaten, 0.5);
            score += PunishForDifferenceAbove(3000, diet.TotalGramsEaten, 0.5);

            return score;
        }

        [Pure]
        private Double GetScoreForIron([NotNull]DietCharacteristics diet)
        {
            Double score = 0;
            score += PunishForDifferenceBelow(DietTarget.MinDailyIronMg, diet.TotalIronMg, 100);
            score += PunishForDifferenceAbove(DietTarget.MaxDailyIronMg, diet.TotalIronMg, 100);
            return score;
        }

        [Pure]
        private Double GetScoreForCalcium([NotNull]DietCharacteristics diet)
        {
            Double score = 0;
            score += PunishForDifferenceBelow(DietTarget.MinDailyCalciumMg, diet.TotalCalciumMg, 1);
            score += PunishForDifferenceAbove(DietTarget.MaxDailyCalciumMg, diet.TotalCalciumMg, 1);
            return score;
        }

        [Pure]
        private Double GetScoreForMagnesium([NotNull]DietCharacteristics diet)
        {
            Double score = 0;
            score += PunishForDifferenceBelow(DietTarget.MinDailyMagnesiumMg, diet.TotalMagnesiumMg, 3);
            return score;
        }

        [Pure]
        private Double GetScoreForPhosphorus([NotNull]DietCharacteristics diet)
        {
            Double score = 0;
            score += PunishForDifferenceBelow(DietTarget.MinDailyPhosphorusMg, diet.TotalPhosphorusMg, 2);
            score += PunishForDifferenceAbove(DietTarget.MaxDailyPhosphorusMg, diet.TotalPhosphorusMg, 3);
            return score;
        }

        [Pure]
        private Double GetScoreForPotassium([NotNull]DietCharacteristics diet)
        {
            Double score = 0;
            score += PunishForDifferenceBelow(DietTarget.MinDailyPotassiumMg, diet.TotalPotassiumMg, 0.5);
            score += PunishForDifferenceAbove(DietTarget.MaxDailyPotassiumMg, diet.TotalPotassiumMg, 0.5);
            return score;
        }
        
        [Pure]
        private Double GetScoreForSodium([NotNull]DietCharacteristics diet)
        {
            Double score = 0;
            score += PunishForDifferenceBelow(DietTarget.MinDailySodiumMg, diet.TotalSodiumMg, 1);
            score += PunishForDifferenceAbove(DietTarget.MaxDailySodiumMg, diet.TotalSodiumMg, 1);
            return score;
        }


        [Pure]
        private Double GetScoreForZinc([NotNull]DietCharacteristics diet)
        {
            Double score = 0;
            score += PunishForDifferenceBelow(DietTarget.MinDailyZincMg, diet.TotalZincMg, 100);
            score += PunishForDifferenceAbove(DietTarget.MaxDailyZincMg, diet.TotalZincMg, 100);
            return score;
        }


        [Pure]
        private Double GetScoreForFiber([NotNull]DietCharacteristics diet, Double targetTotalKcalIntake)
        {
            // Recommendations:
            // * children and adults should consume 14 grams of fiber for every 1,000 calories of food eaten.

            Double score = PunishForDiferrence(diet.TotalFiberGrams, 14 * (targetTotalKcalIntake / 1000), 20);
            return score;
        }

        [Pure]
        private Double GetScoreForVitaminC([NotNull]DietCharacteristics diet)
        {
            Double score = 0;
            score += PunishForDifferenceBelow(DietTarget.MinDailyVitaminCMg, diet.TotalVitaminCMg, 5);
            score += PunishForDifferenceAbove(DietTarget.MaxDailyVitaminCMg, diet.TotalVitaminCMg, 5);
            return score;
        }

        [Pure]
        private Double GetScoreForVitaminA([NotNull]DietCharacteristics diet)
        {
            Double score = 0;
            score += PunishForDifferenceBelow(DietTarget.MinDailyVitaminAiu, diet.TotalVitaminAiu, 1);
            score += PunishForDifferenceAbove(DietTarget.MaxDailyVitaminAiu, diet.TotalVitaminAiu, 1);
            return score;
        }

        [Pure]
        private Double PunishForDiferrence(Double actualValue, Double targetValue, Double multiplier)
        {
            return Math.Abs(actualValue - targetValue) * multiplier;
        }

        [Pure]
        private Double PunishForDifferenceAbove(Double threshold, Double actualValue, Double multiplier)
        {
            if (actualValue <= threshold)
            {
                return 0;
            }
            return Math.Abs(actualValue - threshold) * multiplier;
        }

        [Pure]
        private Double PunishForDifferenceBelow(Double threshold, Double actualValue, Double multiplier)
        {
            if (actualValue >= threshold)
            {
                return 0;
            }
            return Math.Abs(threshold - actualValue) * multiplier;
        }
    }
}