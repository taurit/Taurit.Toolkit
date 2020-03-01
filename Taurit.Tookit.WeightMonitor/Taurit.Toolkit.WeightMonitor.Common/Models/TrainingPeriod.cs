﻿using System;

namespace Taurit.Toolkit.WeightMonitor.Common.Models
{
    public abstract class TrainingPeriod : TimePeriod
    {
        protected abstract Double MinimumDailyWeightIncreaseKg { get; }
        protected abstract Double OptimumDailyWeightIncreaseKg { get; }
        protected abstract Double MaximumDailyWeightIncreaseKg { get; }

        public Double MinimumGainKg => DurationInDays * MinimumDailyWeightIncreaseKg;
        public Double OptimumGainKg => DurationInDays * OptimumDailyWeightIncreaseKg;
        public Double MaximumGainKg => DurationInDays * MaximumDailyWeightIncreaseKg;


        public Double StartWeight { get; set; }
        public Double ExpectedMinimumEndWeight => StartWeight + MinimumGainKg;
        public Double ExpectedOptimumEndWeight => StartWeight + OptimumGainKg;
        public Double ExpectedMaximumEndWeight => StartWeight + MaximumGainKg;

        public void Trim(DateTime newEndDate)
        {
            this.End = newEndDate;
        }

        protected TrainingPeriod(DateTime start, DateTime end, Double startWeight)
        {
            if (end <= start) throw new ArgumentOutOfRangeException(nameof(end), "End date must be greater than a start date");

            Start = start;
            End = end;
            StartWeight = startWeight;
        }
    }
}