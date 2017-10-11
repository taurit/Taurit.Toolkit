using System;
using System.Diagnostics;

namespace Taurit.Toolkit.DietOptimization.Models
{
    public class DietCharacteristics
    {
        public DietCharacteristics(Double totalKcalIntake,
            Double totalPrice,
            Int32 numIngredients,
            Double totalProtein,
            Double totalFat,
            Double totalCarbs,
            Double totalVitaminAiu,
            Double totalVitaminCMg,
            Double totalFiberGrams,
            Double totalIronMg,
            Double totalCalciumMg,
            Double totalMagnesiumMg,
            Double totalPhosphorusMg,
            Double totalPotassiumMg,
            Double totalSodiumMg,
            Double totalZincMg,
            Int32 totalGramsEaten)
        {
            Debug.Assert(totalKcalIntake >= 0);
            Debug.Assert(totalPrice >= 0);
            Debug.Assert(numIngredients >= 0);
            Debug.Assert(totalProtein >= 0);
            Debug.Assert(totalFat >= 0);
            Debug.Assert(totalCarbs >= 0);
            Debug.Assert(totalVitaminAiu >= 0);
            Debug.Assert(totalVitaminCMg >= 0);
            Debug.Assert(totalFiberGrams >= 0);
            Debug.Assert(totalIronMg >= 0);
            Debug.Assert(totalCalciumMg >= 0);
            Debug.Assert(totalMagnesiumMg >= 0);
            Debug.Assert(totalPhosphorusMg >= 0);
            Debug.Assert(totalPotassiumMg >= 0);
            Debug.Assert(totalSodiumMg >= 0);
            Debug.Assert(totalZincMg >= 0);
            Debug.Assert(totalGramsEaten >= 0);

            TotalKcalIntake = totalKcalIntake;
            TotalPrice = totalPrice;
            NumIngredients = numIngredients;
            TotalProtein = totalProtein;
            TotalFat = totalFat;
            TotalCarbs = totalCarbs;
            TotalVitaminAiu = totalVitaminAiu;
            TotalVitaminCMg = totalVitaminCMg;
            TotalFiberGrams = totalFiberGrams;
            TotalIronMg = totalIronMg;
            TotalCalciumMg = totalCalciumMg;
            TotalMagnesiumMg = totalMagnesiumMg;
            TotalPhosphorusMg = totalPhosphorusMg;
            TotalPotassiumMg = totalPotassiumMg;
            TotalSodiumMg = totalSodiumMg;
            TotalZincMg = totalZincMg;
            TotalGramsEaten = totalGramsEaten;
        }

        public Double TotalKcalIntake { get; }
        public Double TotalPrice { get; }
        public Int32 NumIngredients { get; }

        public Double TotalProtein { get; }
        public Double TotalFat { get; }
        public Double TotalCarbs { get; }
        public Double TotalVitaminAiu { get; }
        public Double TotalVitaminCMg { get; }
        public Double TotalFiberGrams { get; }
        public Double TotalIronMg { get; }
        public Double TotalCalciumMg { get; }
        public Double TotalMagnesiumMg { get; }
        public Double TotalPhosphorusMg { get; }
        public Double TotalPotassiumMg { get; }
        public Double TotalSodiumMg { get; }
        public Double TotalZincMg { get; }

        public Int32 TotalGramsEaten { get; }
    }
}