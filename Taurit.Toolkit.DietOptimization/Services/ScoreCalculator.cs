using System;
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

            // price constraint (multiplier will largely depend on the currency and time period!)
            // eg. multiplier=100 => every dollar beyond a threshold is treated as bad as 100 kcal miss
            score += PunishForDifferenceAbove(target.MaxPrice, diet.TotalPrice, 100.0); 

            // experimental: make sure there's not too many kilograms to eat ;)
            score += PunishForDifferenceAbove(2500, diet.TotalGramsEaten, 0.5);
            score += PunishForDifferenceAbove(3000, diet.TotalGramsEaten, 0.5);

            return score;
        }

        [Pure]
        private Double GetScoreForFiber(DietCharacteristics diet, Double targetTotalKcalIntake)
        {
            // Recommendations:
            // * children and adults should consume 14 grams of fiber for every 1,000 calories of food eaten.

            Double score = PunishForDiferrence(diet.TotalFiberGrams, 14 * (targetTotalKcalIntake / 1000), 20);
            return score;
        }

        [Pure]
        private Double GetScoreForVitaminC(DietCharacteristics diet)
        {
            // Recommendations:
            // * 90 is recommended for men in multiple sources, https://legionathletics.com/products/supplements/triumph/#vitamin-c
            // * If you smoke, add 35 mg
            // * 120-200 perceived as optimum by some other reasonable researchers.
            // * it doesn't seem to do any harm up to 2000mg/day, https://ods.od.nih.gov/factsheets/VitaminC-Consumer/#h2

            Double score = 0;
            score += PunishForDifferenceBelow(120, diet.TotalVitaminCMg, 5);
            score += PunishForDifferenceAbove(2000, diet.TotalVitaminCMg, 5);
            return score;
        }

        [Pure]
        private Double GetScoreForVitaminA(DietCharacteristics diet)
        {
            // Recommendations:
            // * U.S. recommended dietary allowance (RDA) for adults is as follows: 900 micrograms daily (3,000 IU) for men
            // * Upper tolerance 10 000 IU, https://ods.od.nih.gov/factsheets/VitaminA-HealthProfessional/


            Double score = 0;
            score += PunishForDifferenceBelow(3000, diet.TotalVitaminAiu, 1);
            score += PunishForDifferenceAbove(10000, diet.TotalVitaminAiu, 1);
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