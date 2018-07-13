using System;

namespace Taurit.Toolkit.WeightMonitor.Common.Models
{
    public class BulkingPeriod : TrainingPeriod
    {
        /// <summary>
        ///     Optimum based on "gain 0.5 to 1 pound per week => 0.75 pound per week on average"
        ///     Source: https://www.muscleforlife.com/bulking-up/
        /// </summary>
        protected override Double OptimumDailyWeightIncreaseKg => 0.04859918257;
    }
}