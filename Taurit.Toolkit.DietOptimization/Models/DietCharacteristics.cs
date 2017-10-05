using System;
using System.Diagnostics;

namespace Taurit.Toolkit.DietOptimization.Models
{
    public class DietCharacteristics
    {
        public DietCharacteristics(Double totalKcalIntake,
            Double totalProtein,
            Double totalFat,
            Double totalCarbs,
            Double totalVitaminAiu,
            Double totalVitaminCMg,
            Double totalFiberGrams,
            Int32 totalGramsEaten)
        {
            Debug.Assert(totalKcalIntake >= 0);
            Debug.Assert(totalProtein >= 0);
            Debug.Assert(totalFat >= 0);
            Debug.Assert(totalCarbs >= 0);
            Debug.Assert(totalVitaminAiu >= 0);
            Debug.Assert(totalVitaminCMg >= 0);
            Debug.Assert(totalFiberGrams >= 0);
            Debug.Assert(totalGramsEaten >= 0);

            TotalKcalIntake = totalKcalIntake;
            TotalProtein = totalProtein;
            TotalFat = totalFat;
            TotalCarbs = totalCarbs;
            TotalVitaminAiu = totalVitaminAiu;
            TotalVitaminCMg = totalVitaminCMg;
            TotalFiberGrams = totalFiberGrams;
            TotalGramsEaten = totalGramsEaten;
        }

        public Double TotalKcalIntake { get; }
        public Double TotalProtein { get; }
        public Double TotalFat { get; }
        public Double TotalCarbs { get; }
        public Double TotalVitaminAiu { get; }
        public Double TotalVitaminCMg { get; }
        public Double TotalFiberGrams { get; }


        public Int32 TotalGramsEaten { get; }
    }
}