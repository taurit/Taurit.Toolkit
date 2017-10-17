using System;

namespace Taurit.Toolkit.DietOptimization.Models
{
    /// <summary>
    ///     Optimization target for diet optimization problem.
    ///     Represents ideal characteristics for a diet we're searching for.
    ///     Some properties depend on user's preferences (eg. caloric intake or protein/carbo/fat ratio).
    ///     Some other are constants based on literature (eg. minimum recommended amounts or maximum tolerable intakes for
    ///     micronutrients).
    /// </summary>
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
        public const Double MinDailyCholineMg = 550;
        public const Double MaxDailyCholineMg = 3500;

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

        /// <summary>
        ///     The RDA for zinc (8mg for women and 11mg for men) is likely too low to offset the abundance of copper in our diets
        ///     and environment, so supplementation with a good quality amino acid chelate, such as zinc orotate, is a good idea.
        /// </summary>
        public const Double MinDailyZincMg = 11;

        /// <summary>
        ///     The tolerable upper intake level, or UL, for adults is 40 mg, which means that you can take up to that dosage
        ///     without worries.
        /// </summary>
        public const Double MaxDailyZincMg = 40;

        /// <summary>
        ///     Margin for matronutrients in attempt to prevent optimization from falling into local minima (and not converging).
        ///     This value might need to be tuned or removed if it does not help.
        /// </summary>
        public const Double MacronutrientToleranceMarginG = 7;

        public const Double EnergyToleranceMarginKcal = 100;
        public const Double FiberToleranceMarginG = 5;

        /// <summary>
        ///     "No matter how many calories you eat per day, the amount of trans fat you should eat remains constant: as little as
        ///     possible"
        ///     "Since avoiding trans fats altogether is difficult, the American Heart Association recommends that no more than 1
        ///     percent of your daily calories come from trans fats (..)
        ///     this equals 2 grams of trans fat per day."
        /// </summary>
        public const Double MaxTransFatsG = 1;

        /// <summary>
        ///     Copper works with iron to help the body form red blood cells. It also helps keep the blood vessels, nerves, immune
        ///     system, and bones healthy. Copper also aids in iron absorption.
        ///     Oysters and other shellfish, whole grains, beans, nuts, potatoes, and organ meats (kidneys, liver) are good sources
        ///     of copper. Dark leafy greens, dried fruits such as prunes, cocoa, black pepper, and yeast are also sources of
        ///     copper in the diet.
        ///     https://medlineplus.gov/ency/article/002419.htm
        /// </summary>
        public const Double MinDailyCopperMg = 1; // 900mcg on one souce, 2000 mcg in other

        /// <summary>
        ///     "As little as 10 mg of copper can have a toxic effect"
        ///     "routine use of dietary copper supplements containing 2 mg or more per day could lead to accumulation of excess
        ///     copper,"​
        ///     "Seven mg of copper is too much but we don't yet have the data to elucidate the right level for supplementation,"​
        ///     Dr Turnlund said."
        ///     "A copper deficiency is basically unheard of."
        ///     https://www.nutraingredients.com/Article/2005/07/07/Copper-levels-in-supplements-should-be-reduced
        ///     "Copper is very abundant in the average Western diet and water supply."
        /// </summary>
        public const Double MaxDailyCopperMg = 2;

        /// <summary>
        ///     Approximately 37 percent of Americans may not get enough manganese in their diet,
        ///     The recommended dietary intake of manganese is 1.8 mg for adult women and 2.3 mg for adult men.
        ///     https://www.livestrong.com/article/520170-taking-too-much-manganese/
        /// </summary>
        public const Double MinDailyManganeseMg = 2.3;

        /// <summary>
        ///     The tolerable upper intake level of manganese is 11 mg a day for all adults.
        ///     people eating vegetarian diets may have a dietary intake as high as 10.9 mg a day, according to the Linus Pauling
        ///     Institute
        ///     https://www.livestrong.com/article/520170-taking-too-much-manganese/
        ///     http://lpi.oregonstate.edu/mic/minerals/manganese
        ///     exceeding it a bit should not be a big problem
        ///     https://discourse.soylent.com/t/have-you-asked-yourself-why-upper-limit-of-manganese-is-11mg-day/7660
        /// </summary>
        public const Double MaxDailyManganeseMg = 11;

        /// <summary>
        ///     35 µg by one source
        ///     55 µg by another
        /// </summary>
        public const Double MinDailySeleniumUg = 55;

        /// <summary>
        ///     Doses larger than 200 µg can be toxic (one source)
        ///     400 µg (another)
        /// </summary>
        public const Double MaxDailySeleniumUg = 200;

        /// <summary>
        ///     http://lpi.oregonstate.edu/mic/vitamins/thiamin
        /// </summary>
        public const Double MinDailyVitaminB1Mg = 1.2;

        /// <summary>
        ///     The Food and Nutrition Board did not set a tolerable upper intake level(UL) for thiamin because there are no
        ///     well-established toxic effects from consumption of excess thiamin in food or through long-term, oral
        ///     supplementation(up to 200 mg/day).
        /// </summary>
        public const Double MaxDailyVitaminB1Mg = 200;

        /// <summary>
        ///     pantothenic acid deficiency in humans is very rare and has been observed only in cases of severe malnutrition
        /// </summary>
        public const Double MinDailyVitaminB5Mg = 5;

        /// <summary>
        ///     http://lpi.oregonstate.edu/mic/vitamins/riboflavin
        ///     High-dose riboflavin therapy has been found to intensify urine color to a bright yellow (flavinuria), but this is a
        ///     harmless side effect. The Food and Nutrition Board did not establish a tolerable upper intake level (UL) when the
        ///     RDA was revised in 1998 (1).
        /// </summary>
        public const Double MinDailyVitaminB2Mg = 1.3;

        /// <summary>
        ///     Pantothenic acid is not known to be toxic in humans. The only adverse effect noted was diarrhea resulting from very
        ///     high intakes of 10 to 20 g/day of calcium D-pantothenate
        /// </summary>
        public const Double MaxDailyVitaminB5Mg = 10000;


        /// <summary>
        ///     http://lpi.oregonstate.edu/mic/vitamins/niacin
        /// </summary>
        public const Double MinDailyVitaminB3Mg = 16;

        /// <summary>
        ///     Flushing of the skin primarily on the face, arms, and chest is a common side effect of niacin and may occur
        ///     initially at doses as low as 30 mg/day. Although flushing from nicotinamide is rare, the Food and Nutrition Board
        ///     set the UL for niacin (nicotinic acid and nicotinamide) at 35 mg/day to avoid the adverse effect of flushing (25;
        ///     Table 3).
        ///     http://lpi.oregonstate.edu/mic/vitamins/niacin
        /// </summary>
        public const Double MaxDailyVitaminB3Mg = 30;

        /// <summary>
        ///     http://lpi.oregonstate.edu/mic/vitamins/vitamin-B6
        /// </summary>
        public const Double MinDailyVitaminB6Mg = 1.3;

        /// <summary>
        ///     http://lpi.oregonstate.edu/mic/vitamins/vitamin-K
        ///     No tolerable upper intake level (UL) has been established for vitamin K (42).
        /// </summary>
        public const Double MinDailyVitaminKUg = 120;

        /// <summary>
        ///     http://lpi.oregonstate.edu/mic/vitamins/vitamin-E
        /// </summary>
        public const Double MinDailyVitaminEMg = 15;

        /// <summary>
        ///     A tolerable upper intake level (UL) for any form of supplemental α-tocopherol (all possible stereoisomers)
        ///     http://lpi.oregonstate.edu/mic/vitamins/vitamin-E
        /// </summary>
        public const Double MaxDailyVitaminEMg = 1000; // 1000 milligrams


        /// <summary>
        ///     http://lpi.oregonstate.edu/mic/vitamins/vitamin-B6
        /// </summary>
        public const Double MaxDailyVitaminB6Mg = 100;

        /// <summary>
        ///     Because of the low toxicity of vitamin B12, no tolerable upper intake level (UL) has been set by the US Food and
        ///     Nutrition Board (17).
        ///     Doses as high as 2 mg (2,000 μg) daily by mouth or 1 mg monthly by intramuscular (IM) injection have been used to
        ///     treat pernicious anemia without significant side effects (84).
        ///     http://lpi.oregonstate.edu/mic/vitamins/vitamin-B12#toxicity
        /// </summary>
        public const Double MinDailyVitaminB12Mg = 0.0024; // 2.4 microgram. Strange that units are chosen this way

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

        public Double MaxGramsOfSaturatedFat => DietPlan.MaxAmountEnergyFromSaturatedFats * TotalKcalIntake / 9.0d;

        public Double TotalKcalIntake { get; }
        public Double MaxPrice { get; }

        public Double TotalProtein { get; }
        public Double TotalFat { get; }
        public Double TotalCarbs { get; }
    }
}