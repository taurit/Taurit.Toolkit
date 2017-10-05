using System;
using Taurit.Toolkit.DietOptimization.Models;

namespace Taurit.Toolkit.DietOptimization.Services
{
    /// <summary>
    ///     Provides a metric for a "distance" between two diet characteristics.
    ///     This metric is a single numeric value ("score") that have th efollowing values:
    ///     * it is equal 0 when two diet characteristics are exactly the same
    ///     * its absolute value is greater for characteristics that are more distant
    ///     * the closer is it to 0, the better diet characteristics matches
    ///     * the score is weighted, so that diet characteristic's compounds represented with different units doen't have too
    ///     much or too little impact on overall score.
    /// </summary>
    public class ScoreCalculator
    {
        public Double CalculateScore(DietCharacteristics diet1, DietCharacteristics target)
        {
            Double score = 0;
            score += Math.Abs(diet1.TotalKcalIntake - target.TotalKcalIntake);

            score += Math.Abs(diet1.TotalProtein - target.TotalProtein);
            score += Math.Abs(diet1.TotalCarbs - target.TotalCarbs);
            score += Math.Abs(diet1.TotalFat - target.TotalFat);

            // multiplers: assuming that:
            // * 1000 means "1 component was completely ignored, unacceptable"
            // * 500 means "1 component was underdosed/overdosed by about 50%, unacceptable"
            score += Math.Abs(diet1.TotalVitaminAiu - target.TotalVitaminAiu);
            score += 20 * Math.Abs(diet1.TotalFiberGrams - target.TotalFiberGrams);
            score += 2 * Math.Abs(diet1.TotalVitaminCMg - target.TotalVitaminCMg);

            return score;
        }
    }
}