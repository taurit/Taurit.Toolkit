using System.Diagnostics;

namespace Taurit.Toolkit.DietOptimization.Models
{
    public class DietCharacteristics
    {
        public DietCharacteristics(double totalKcalIntake, double totalProtein, double totalFat, double totalCarbs,
            int totalGramsEaten)
        {
            Debug.Assert(totalKcalIntake >= 0);
            Debug.Assert(totalProtein >= 0);
            Debug.Assert(totalFat >= 0);
            Debug.Assert(totalCarbs >= 0);
            Debug.Assert(totalGramsEaten >= 0);

            TotalKcalIntake = totalKcalIntake;
            TotalProtein = totalProtein;
            TotalFat = totalFat;
            TotalCarbs = totalCarbs;
            TotalGramsEaten = totalGramsEaten;
        }

        public double TotalKcalIntake { get; }
        public double TotalProtein { get; }
        public double TotalFat { get; }
        public double TotalCarbs { get; }
        public int TotalGramsEaten { get; }
    }
}