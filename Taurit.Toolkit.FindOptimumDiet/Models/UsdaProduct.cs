// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using System;
using System.Diagnostics;

namespace Taurit.Toolkit.FindOptimumDiet.Models
{
    /// <summary>
    ///     This class is used when deserializing data from USDA product database
    /// </summary>
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    [Serializable]
    internal sealed class UsdaProduct
    {
        public String Name { get; set; }
        public String Energy_Kcal { get; set; }
        public String Protein_Grams { get; set; }
        public String Fat_Grams { get; set; }
        public String Carbohydrate_Grams { get; set; }
    }
}