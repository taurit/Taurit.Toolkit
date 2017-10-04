namespace Taurit.Toolkit.DietOptimization.Models
{
    public class DietConstraints
    {
        public DietCharacteristics DietCharacteristics { get; }

        public DietConstraints(DietCharacteristics dietCharacteristics)
        {
            DietCharacteristics = dietCharacteristics;
        }
    }
}