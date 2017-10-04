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
    public class DietCharacteristicsDistanceCalculator
    {
        public double CalculateScore(DietCharacteristics diet1, DietCharacteristics diet2)
        {
            double score = 0;
            score += Math.Abs(diet1.TotalKcalIntake - diet2.TotalKcalIntake);

            score += Math.Abs(diet1.TotalProtein - diet2.TotalProtein);
            score += Math.Abs(diet1.TotalCarbs - diet2.TotalCarbs);
            score += Math.Abs(diet1.TotalFat - diet2.TotalFat);

            return score;
        }
    }
}