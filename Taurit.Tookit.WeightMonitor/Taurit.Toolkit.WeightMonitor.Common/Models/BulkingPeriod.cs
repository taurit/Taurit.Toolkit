using System;

namespace Taurit.Toolkit.WeightMonitor.Common.Models
{
    public class BulkingPeriod : TrainingPeriod
    {
        public BulkingPeriod(DateTime start, DateTime end, Double startWeight) : base(start, end, startWeight)
        {
        }

        /// <summary>
        ///     Optimum based on "gain 0.5 to 1 pound per week => 0.75 pound per week on average"
        ///     Source: https://www.muscleforlife.com/bulking-up/
        /// </summary>
        protected override Double MaximumDailyWeightIncreaseKg => 0.06479891;

        protected override Double OptimumDailyWeightIncreaseKg => 0.04859918257;
        protected override Double MinimumDailyWeightIncreaseKg => 0.032399455;
    }
}