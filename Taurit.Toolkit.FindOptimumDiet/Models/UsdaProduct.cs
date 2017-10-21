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

        /// <summary>
        ///     Thiamine = Vitamin B1
        /// </summary>
        public String Thiamin_Mg { get; set; }

        /// <summary>
        ///     Riboflavin = Vitamin B2
        /// </summary>
        public String Riboflavin_Mg { get; set; }

        /// <summary>
        ///     Niacin = Vitamin B3
        /// </summary>
        public String Niacin_Mg { get; set; }

        /// <summary>
        ///     Pantothenic Acid = Vitamin B5
        /// </summary>
        public String PantothenicAcid_Mg { get; set; }

        /// <summary>
        /// Pyridoxine = Vitamin b6
        /// </summary>
        public String VitaminB6A_Mg { get; set; }

        public String VitaminB12_Ug { get; set; }
        public String CholineTotalMg { get; set; }
        public String VitaminK_Ug { get; set; }

        public String VitaminC_Mg { get; set; }
        public String VitaminE_Mg { get; set; }
        public String FattyAcidsTotalSaturated_Grams { get; set; }
        public String FattyAcidsTotalMonounsaturated_Grams { get; set; }
        public String FattyAcidsTotalPolyunsaturated_Grams { get; set; }
        public String Cholesterol_Mg { get; set; }
        public String FattyAcidsTotalTrans_Grams { get; set; }

        public Double Omega3Total => GetValue(F18D3_Grams) + GetValue(F20D5_Grams) + GetValue(F22D6_Grams) +
                                     GetValue(F22D5_Grams);

        public Double Omega6Total => GetValue(F18D2CN6_Grams) + GetValue(F18D3CN6_Grams) + GetValue(F20D4N6_Grams) +
                                     GetValue(F20D2CN6_Grams) + GetValue(F20D3N6_Grams);

        private Double GetValue(String s)
        {
            if (String.IsNullOrWhiteSpace(s))
            {
                return 0;
            }

            if (Double.TryParse(s, out Double res))
            {
                return res;
            }
            return 0;
        }

        #region omega-3 acids       

        /// <summary>
        ///     ALA
        /// </summary>
        public String F18D3_Grams { get; set; }

        /// <summary>
        ///     EPA
        /// </summary>
        public String F20D5_Grams { get; set; }

        /// <summary>
        ///     DHA
        /// </summary>
        public String F22D6_Grams { get; set; }

        /// <summary>
        ///     DPA
        /// </summary>
        public String F22D5_Grams { get; set; }

        #endregion

        #region omega-6 acids

        /// <summary>
        ///     LA
        /// </summary>
        public String F18D2CN6_Grams { get; set; }

        /// <summary>
        ///     GLA
        /// </summary>
        public String F18D3CN6_Grams { get; set; }

        /// <summary>
        ///     AA
        /// </summary>
        public String F20D4N6_Grams { get; set; }

        /// <summary>
        ///     AA
        /// </summary>
        public String F20D2CN6_Grams { get; set; }

        public String F20D3N6_Grams { get; set; }

        #endregion
    }
}