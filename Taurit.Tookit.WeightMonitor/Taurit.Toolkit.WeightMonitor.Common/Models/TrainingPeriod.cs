using System;

namespace Taurit.Toolkit.WeightMonitor.Common.Models
{
    public abstract class TrainingPeriod : TimePeriod
    {
        protected abstract Double OptimumDailyWeightIncreaseKg { get; }

        public Double OptimumGainKg => DurationInDays * OptimumDailyWeightIncreaseKg;
        public Double StartWeight { get; set; }
        public Double ExpectedEndWeight => StartWeight + OptimumGainKg;
    }
}