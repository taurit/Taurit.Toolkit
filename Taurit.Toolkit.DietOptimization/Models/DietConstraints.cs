using System.Diagnostics;
using JetBrains.Annotations;

namespace Taurit.Toolkit.DietOptimization.Models
{
    public class DietConstraints
    {
        [NotNull]
        public DietCharacteristics DietCharacteristics { get; }

        public DietConstraints([NotNull]DietCharacteristics dietCharacteristics)
        {
            Debug.Assert(dietCharacteristics != null);
            DietCharacteristics = dietCharacteristics;
        }
    }
}