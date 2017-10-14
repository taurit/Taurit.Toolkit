using System;
using System.Diagnostics;
using Taurit.Toolkit.DietOptimization.Services;

namespace Taurit.Toolkit.DietOptimization.Models
{
    /// <summary>
    ///     Quantitive description of a single diet plan. It might be used to:
    ///     * display characteristics of a diet plan to the user,
    ///     * measure how close a diet plan is to a <see cref="DietTarget" /> specified by the user. This measurement is done
    ///     by <see cref="ScoreCalculator" />.
    /// </summary>
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
            Double totalGramsEaten,
            Double totalFattyAcidsSaturatedG,
            Double totalFattyAcidsMonounsaturatedG,
            Double totalFattyAcidsPolyunsaturatedG,
            Double totalFattyAcidsTransG,
            Double totalCholesterolMg,
            Double totalOmega3,
            Double totalOmega6
        )
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
            Debug.Assert(totalFattyAcidsMonounsaturatedG >= 0);
            Debug.Assert(totalFattyAcidsPolyunsaturatedG >= 0);
            Debug.Assert(totalFattyAcidsSaturatedG >= 0);
            Debug.Assert(totalFattyAcidsTransG >= 0);
            Debug.Assert(totalCholesterolMg >= 0);
            Debug.Assert(totalOmega3 >= 0);
            Debug.Assert(totalOmega6 >= 0);

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
            TotalFattyAcidsSaturatedG = totalFattyAcidsSaturatedG;
            TotalFattyAcidsMonounsaturatedG = totalFattyAcidsMonounsaturatedG;
            TotalFattyAcidsPolyunsaturatedG = totalFattyAcidsPolyunsaturatedG;
            TotalFattyAcidsTransG = totalFattyAcidsTransG;
            TotalCholesterolMg = totalCholesterolMg;
            TotalOmega3 = totalOmega3;
            TotalOmega6 = totalOmega6;
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

        public Double TotalGramsEaten { get; }
        public Double TotalFattyAcidsSaturatedG { get; }
        public Double TotalFattyAcidsMonounsaturatedG { get; }
        public Double TotalFattyAcidsPolyunsaturatedG { get; }
        public Double TotalFattyAcidsTransG { get; }
        public Double TotalCholesterolMg { get; }
        private Double TotalOmega3 { get; }
        private Double TotalOmega6 { get; }

        public String Omega3To6Ratio => TotalOmega6 > TotalOmega3
            ? $"1:{TotalOmega6 / TotalOmega3:0.00}"
            : $"{TotalOmega3 / TotalOmega6:0.00}:1";
    }
}