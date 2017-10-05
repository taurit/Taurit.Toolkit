// ReSharper disable InconsistentNaming

using System.Diagnostics;

namespace Taurit.Toolkit.FindOptimumDiet.Models
{
    [DebuggerDisplay("{Name}")]
    internal class UsdaProduct
    {
        public string Name { get; set; }
        public string Energy_Kcal { get; set; }
        public string Protein_Grams { get; set; }
        public string Fat_Grams { get; set; }
        public string Carbohydrate_Grams { get; set; }
    }
}