using System;
using System.Collections.Generic;
using System.Text;

namespace Taurit.Toolkit.WeightMonitor.Common.Models
{
    public class MaintenancePeriod: TrainingPeriod
    {
        protected override Double MinimumDailyWeightIncreaseKg => 0;
        protected override Double OptimumDailyWeightIncreaseKg => 0;
        protected override Double MaximumDailyWeightIncreaseKg => 0;
    }
}
