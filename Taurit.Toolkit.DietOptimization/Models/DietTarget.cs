using System;

namespace Taurit.Toolkit.DietOptimization.Models
{
    /// <remarks>
    ///     Constant values for micronutrients are currently set accordingly for ~30 years old male, exercising.
    /// </remarks>
    public class DietTarget
    {
        /// <summary>
        ///     * 90 is recommended for men in multiple sources,
        ///     https://legionathletics.com/products/supplements/triumph/#vitamin-c
        ///     * If you smoke, add 35 mg
        ///     * 120-200 perceived as optimum by some other reasonable researchers.
        /// </summary>
        public const Double MinDailyVitaminCMg = 120;

        /// <summary>
        ///     it doesn't seem to do any harm up to 2000mg/day, https://ods.od.nih.gov/factsheets/VitaminC-Consumer/#h2
        /// </summary>
        public const Double MaxDailyVitaminCMg = 2_000;

        /// <summary>
        ///     U.S. recommended dietary allowance (RDA) for adults is as follows: 900 micrograms daily (3,000 IU) for men
        /// </summary>
        public const Double MinDailyVitaminAiu = 3_000;

        /// <summary>
        ///     Upper tolerance 10 000 IU, https://ods.od.nih.gov/factsheets/VitaminA-HealthProfessional/
        ///     This limit refers to pre-formed Vitamin A, which can be found in foods and dietary supplements as palmitate,
        ///     acetate or fish liver oil, all of which are derived from animal sources.
        /// </summary>
        /// <summary>
        ///     https://ods.od.nih.gov/factsheets/Iron-Consumer/
        /// </summary>
        public const Double MinDailyIronMg = 8;

        public const Double MaxDailyIronMg = 45;

        /// <summary>
        ///     https://ods.od.nih.gov/factsheets/Calcium-Consumer/
        /// </summary>
        public const Double MinDailyCalciumMg = 1000;

        /// <summary>
        ///     https://ods.od.nih.gov/factsheets/Calcium-Consumer/
        /// </summary>
        public const Double MaxDailyCalciumMg = 2500;

        /// <summary>
        ///     https://ods.od.nih.gov/factsheets/Magnesium-Consumer/
        ///     Magnesium that is naturally present in food is not harmful and does not need to be limited. (therefore there's no
        ///     upper limit)
        /// </summary>
        public const Double MinDailyMagnesiumMg = 420;

        /// <summary>
        ///     https://medlineplus.gov/ency/article/002424.htm
        ///     9 to 18 years: 1,250 mg
        ///     Adults: 700 mg/day
        /// </summary>
        public const Double MinDailyPhosphorusMg = 700;

        /// <summary>
        ///     http://lpi.oregonstate.edu/mic/minerals/phosphorus
        ///     The tolerable upper intake level(UL) for phosphorus is 4,000 mg/day for generally healthy adults.
        ///     (...) daily phosphorus intakes more than twice the RDA (i.e., >1,400 mg/day) were significantly associated with an
        ///     increased risk of all-cause mortality
        /// </summary>
        public const Double MaxDailyPhosphorusMg = 1400;

        /// <summary>
        ///     Age 19 and older: 4.7 g/day
        /// </summary>
        public const Double MinDailyPotassiumMg = 4500;

        public const Double MaxDailyPotassiumMg = 4900;


        /// <summary>
        ///     https://sodiumbreakup.heart.org/how_much_sodium_should_i_eat
        ///     The body needs only a small amount of sodium (less than 500 milligrams per day) to function properly.
        /// </summary>
        public const Double MinDailySodiumMg = 500;

        /// <summary>
        ///     https://www.healthline.com/nutrition/how-much-sodium-per-day
        ///     United States Department of Agriculture(USDA): 2300 mg.
        ///     American Heart Association(AHA): 1500 mg(2).
        ///     Academy of Nutrition and Dietetics(AND): 1500 to 2300 mg.
        ///     American Diabetes Association(ADA): 1500 to 2300 mg.
        /// </summary>
        public const Double MaxDailySodiumMg = 1500;

        public const Double MinDailyZincMg = 11;
        public const Double MaxDailyZincMg = 40;

        /// <summary>
        ///     Margin for matronutrients in attempt to prevent optimization from falling into local minima (and not converging).
        ///     This value might need to be tuned or removed if it does not help.
        /// </summary>
        public const Double MacronutrientToleranceMarginG = 10;

        public const Double EnergyToleranceMarginKcal = 50;
        public const Double FiberToleranceMarginG = 5;

        public DietTarget(Double totalKcalIntake,
            Double maxPrice,
            Double totalProtein,
            Double totalFat,
            Double totalCarbs)
        {
            TotalKcalIntake = totalKcalIntake;
            MaxPrice = maxPrice;
            TotalProtein = totalProtein;
            TotalFat = totalFat;
            TotalCarbs = totalCarbs;
        }

        public Double TotalKcalIntake { get; }
        public Double MaxPrice { get; }

        public Double TotalProtein { get; }
        public Double TotalFat { get; }
        public Double TotalCarbs { get; }
    }
}