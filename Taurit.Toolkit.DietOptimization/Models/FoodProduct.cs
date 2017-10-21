using System;
using System.Diagnostics;

namespace Taurit.Toolkit.DietOptimization.Models
{
    /// <summary>
    ///     Represents a food type.
    /// </summary>
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    // ReSharper disable once ClassNeverInstantiated.Global - it IS instantiated by AutoMapper
    public class FoodProduct
    {
        public FoodProduct(String name,
            Double energyKcal,
            Double percentProtein,
            Double percentFat,
            Double percentCarb,
            Double fiberTotalDietaryGrams,
            Double vitaminAIu,
            Double vitaminCMg,
            Double ironMg,
            Double calciumMg,
            Double magnesiumMg,
            Double phosphorusMg,
            Double potassiumMg,
            Double sodiumMg,
            Double zincMg,
            Double fattyAcidsTotalSaturatedG,
            Double fattyAcidsTotalMonounsaturatedG,
            Double fattyAcidsTotalPolyunsaturatedG,
            Double fattyAcidsTotalTransG,
            Double cholesterolMg,
            Double omega3,
            Double omega6,
            Double copperMg,
            Double manganeseMg,
            Double seleniumUg,
            Double vitaminB1Mg,
            Double vitaminB2Mg,
            Double vitaminB3Mg,
            Double vitaminB5Mg,
            Double vitaminB6Mg,
            Double vitaminB12Ug,
            Double cholineMg,
            Double vitaminEMg,
            Double vitaminKUg
        )
        {
            Name = name;
            EnergyKcal = energyKcal;
            PercentProtein = percentProtein;
            PercentFat = percentFat;
            PercentCarb = percentCarb;
            FiberTotalDietaryGrams = fiberTotalDietaryGrams;
            VitaminAIu = vitaminAIu;
            VitaminCMg = vitaminCMg;
            IronMg = ironMg;
            CalciumMg = calciumMg;
            MagnesiumMg = magnesiumMg;
            PhosphorusMg = phosphorusMg;
            PotassiumMg = potassiumMg;
            SodiumMg = sodiumMg;
            ZincMg = zincMg;
            FattyAcidsTotalSaturatedG = fattyAcidsTotalSaturatedG;
            FattyAcidsTotalMonounsaturatedG = fattyAcidsTotalMonounsaturatedG;
            FattyAcidsTotalPolyunsaturatedG = fattyAcidsTotalPolyunsaturatedG;
            FattyAcidsTotalTransG = fattyAcidsTotalTransG;
            CholesterolMg = cholesterolMg;
            Omega3 = omega3;
            Omega6 = omega6;
            CopperMg = copperMg;
            ManganeseMg = manganeseMg;
            SeleniumUg = seleniumUg;
            VitaminB1Mg = vitaminB1Mg;
            VitaminB2Mg = vitaminB2Mg;
            VitaminB3Mg = vitaminB3Mg;
            VitaminB5Mg = vitaminB5Mg;
            VitaminB6Mg = vitaminB6Mg;
            VitaminB12Ug = vitaminB12Ug;
            CholineMg = cholineMg;
            VitaminEMg = vitaminEMg;
            VitaminKUg = vitaminKUg;
        }

        public OptimizationMetadata Metadata { get; set; }

        /// <summary>
        ///     Comes from LongDescription in FoodDescription
        /// </summary>
        public String Name { get; }

        /// <summary>
        ///     Energy in Kcal, comes from Nutrient definition->Nutrient data
        /// </summary>
        public Double EnergyKcal { get; }

        public Double PercentProtein { get; }
        public Double PercentFat { get; }
        public Double PercentCarb { get; }

        public Double FiberTotalDietaryGrams { get; }
        public Double CalciumMg { get; }
        public Double IronMg { get; }
        public Double MagnesiumMg { get; }
        public Double PhosphorusMg { get; }
        public Double PotassiumMg { get; }
        public Double SodiumMg { get; }
        public Double ZincMg { get; }
        public Double FattyAcidsTotalSaturatedG { get; }
        public Double FattyAcidsTotalMonounsaturatedG { get; }
        public Double FattyAcidsTotalPolyunsaturatedG { get; }
        public Double FattyAcidsTotalTransG { get; }
        public Double CholesterolMg { get; }
        public Double Omega3 { get; }
        public Double Omega6 { get; }
        public Double CopperMg { get; }
        public Double ManganeseMg { get; }
        public Double SeleniumUg { get; }
        public Double VitaminAIu { get; }
        public Double VitaminB1Mg { get; }
        public Double VitaminB2Mg { get; }
        public Double VitaminB3Mg { get; }
        public Double VitaminB5Mg { get; }
        public Double VitaminB6Mg { get; }
        public Double VitaminB12Ug { get; }
        public Double VitaminCMg { get; }
        public Double VitaminEMg { get; }
        public Double VitaminKUg { get; }
        public Double CholineMg { get; }


    }
}