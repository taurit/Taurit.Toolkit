using System;

namespace Taurit.Toolkit.DietOptimization.Models
{
    public class DietTarget
    {
        public DietTarget(Double totalKcalIntake,
            Double totalProtein,
            Double totalFat,
            Double totalCarbs)
        {
            TotalKcalIntake = totalKcalIntake;
            TotalProtein = totalProtein;
            TotalFat = totalFat;
            TotalCarbs = totalCarbs;
        }

        public Double TotalKcalIntake { get; }
        public Double TotalProtein { get; }
        public Double TotalFat { get; }
        public Double TotalCarbs { get; }
    }
}