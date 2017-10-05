using System;
using System.Diagnostics;

namespace Taurit.Toolkit.DietOptimization.Models
{
    [DebuggerDisplay("{Name}")]
    public class FoodProduct
    {
        public FoodProduct(String name,
            Double energyKcal,
            Double percentProtein,
            Double percentFat,
            Double percentCarb,
            Double fiberTotalDietaryGrams,
            Double vitaminAIu,
            Double vitaminCMg)
        {
            Name = name;
            EnergyKcal = energyKcal;
            PercentProtein = percentProtein;
            PercentFat = percentFat;
            PercentCarb = percentCarb;
            FiberTotalDietaryGrams = fiberTotalDietaryGrams;
            VitaminAIu = vitaminAIu;
            VitaminCMg = vitaminCMg;

        }

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

        public Double SugarGrams { get; set; }
        public Double FiberTotalDietaryGrams { get; set; }
        public Double CalciumMg { get; set; }
        public Double IronMg { get; set; }
        public Double MagnesiumMg { get; set; }
        public Double PhosphorusMg { get; set; }
        public Double PotassiumMg { get; set; }
        public Double SodiumMg { get; set; }
        public Double ZincMg { get; set; }
        public Double CopperMg { get; set; }
        public Double ManganeseMg { get; set; }
        public Double SeleniumUg { get; set; }
        public Double VitaminAIu { get; set; }
        public Double CaroteneBetaUg { get; set; }
        public Double VitaminEMg { get; set; }
        public Double VitaminDIu { get; set; }
        public Double VitaminCMg { get; set; }
    }
}