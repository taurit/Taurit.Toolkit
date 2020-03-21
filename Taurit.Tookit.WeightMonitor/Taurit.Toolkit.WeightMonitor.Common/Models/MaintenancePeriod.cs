using System;

namespace Taurit.Toolkit.WeightMonitor.Common.Models
{
    public class MaintenancePeriod : TrainingPeriod
    {
        public MaintenancePeriod(DateTime start, DateTime end, Double startWeight) : base(start, end, startWeight)
        {
        }

        protected override Double MinimumDailyWeightIncreaseKg => 0;
        protected override Double OptimumDailyWeightIncreaseKg => 0;
        protected override Double MaximumDailyWeightIncreaseKg => 0;
    }
}