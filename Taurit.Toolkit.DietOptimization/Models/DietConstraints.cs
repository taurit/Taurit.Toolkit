using System.Diagnostics;
using JetBrains.Annotations;

namespace Taurit.Toolkit.DietOptimization.Models
{
    public class DietConstraints
    {
        public DietConstraints([NotNull] DietCharacteristics dietCharacteristics)
        {
            Debug.Assert(dietCharacteristics != null);
            DietCharacteristics = dietCharacteristics;
        }

        [NotNull]
        public DietCharacteristics DietCharacteristics { get; }
    }
}