using System.Diagnostics;

namespace Taurit.Toolkit.DietOptimization.Models
{
    [DebuggerDisplay("{Name}")]
    public class FoodProduct
    {
        public FoodProduct(string name, double energyKcal, double percentProtein, double percentFat, double percentCarb)
        {
            Name = name;
            EnergyKcal = energyKcal;
            PercentProtein = percentProtein;
            PercentFat = percentFat;
            PercentCarb = percentCarb;
        }

        /// <summary>
        /// Comes from LongDescription in FoodDescription
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// Energy in Kcal, comes from Nutrient definition->Nutrient data
        /// </summary>
        public double EnergyKcal { get; }

        public double PercentProtein { get; }
        public double PercentFat { get; }
        public double PercentCarb { get; }
    }
}