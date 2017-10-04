namespace Taurit.Toolkit.DietOptimization.Models
{
    public class DietCharacteristics
    {
        public DietCharacteristics(double totalKcalIntake, double totalProtein, double totalFat, double totalCarbs)
        {
            TotalKcalIntake = totalKcalIntake;
            TotalProtein = totalProtein;
            TotalFat = totalFat;
            TotalCarbs = totalCarbs;
        }

        public double TotalKcalIntake { get; }
        public double TotalProtein { get; }
        public double TotalFat { get; }
        public double TotalCarbs { get; }
    }
}