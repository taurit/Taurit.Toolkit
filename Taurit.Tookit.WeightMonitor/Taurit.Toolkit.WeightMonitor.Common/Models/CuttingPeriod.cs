using System;

namespace Taurit.Toolkit.WeightMonitor.Common.Models
{
    public class CuttingPeriod : TrainingPeriod
    {
        /// <summary>
        ///     Optimum based on "That works out to around 1 to 2 pounds of weight loss per week for most people, or 4 to 8 pounds per month.". Assuming avg of 6 pounds per month.
        ///     Source: https://www.muscleforlife.com/how-to-lose-weight-fast/
        /// </summary>
        protected override Double OptimumDailyWeightIncreaseKg => -0.090718474;
    }
}