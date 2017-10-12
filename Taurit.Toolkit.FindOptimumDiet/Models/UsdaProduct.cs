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
        public String Sugar_Grams { get; set; }
        public String FiberTotalDietary_Grams { get; set; }
        public String Calcium_Mg { get; set; }
        public String Iron_Mg { get; set; }
        public String Magnesium_Mg { get; set; }
        public String Phosphorus_Mg { get; set; }
        public String Potassium_Mg { get; set; }
        public String Sodium_Mg { get; set; }
        public String Zinc_Mg { get; set; }
        public String Copper_Mg { get; set; }
        public String Manganese_Mg { get; set; }
        public String Selenium_Ug { get; set; }
        public String VitaminA_IU { get; set; }
        public String CaroteneBeta_Ug { get; set; }
        public String VitaminE_Mg { get; set; }
        public String VitaminD_IU { get; set; }
        public String VitaminC_Mg { get; set; }
        public String FattyAcidsTotalSaturated_Grams { get; set; }
        public String FattyAcidsTotalMonounsaturated_Grams { get; set; }
        public String FattyAcidsTotalPolyunsaturated_Grams { get; set; }
        public String Cholesterol_Mg { get; set; }
        public String FattyAcidsTotalTrans_Grams { get; set; }

    }
}