using System;
using System.Linq;
using JetBrains.Annotations;
using Taurit.Toolkit.DietOptimization.Models;

namespace Taurit.Toolkit.FindOptimumDiet
{
    /// <summary>
    ///     Presents data about Diet Plan in a console
    /// </summary>
    internal sealed class DietPlanPresenter
    {
        private readonly ConsoleColor carbsColor = ConsoleColor.Green;
        private readonly ConsoleColor fatColor = ConsoleColor.Yellow;
        private readonly ConsoleColor proteinColor = ConsoleColor.Cyan; // color convention same as in FitAtu app

        internal void Display(DietPlan diet, DietTarget referenceValue)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Diet plan");
            Console.WriteLine("------------------------------------------------");

            Console.ForegroundColor = ConsoleColor.Green;
            Int32 numSkippedProducts = diet.DietPlanItems.Count(x => x.AmountGrams == 0);
            foreach (DietPlanItem item in diet.DietPlanItems.Where(x => x.AmountGrams > 0)) // skip 0g entries
                Console.WriteLine($"{(item.AmountGrams + "g").PadLeft(6)}: {item.FoodProduct.Name}");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("------------------------------------------------");

            DisplayInColor("Score (lower is better)", $"{diet.ScoreToTarget:0}", "", "0", ConsoleColor.Red);
            DisplayInColor("Num skipped products", $"{numSkippedProducts:0}", "", "-", ConsoleColor.White);
            Display("Energy", diet.Characteristics.TotalKcalIntake, "kcal",
                referenceValue.TotalKcalIntake - DietTarget.EnergyToleranceMarginKcal,
                referenceValue.TotalKcalIntake + DietTarget.EnergyToleranceMarginKcal);
            DisplayInColor("Price", $"{diet.Characteristics.TotalPrice:0.00}", "PLN",
                $"{referenceValue.MaxPrice}", ConsoleColor.Gray);
            DisplayInColor("Avg monthly price", $"{diet.Characteristics.TotalPrice * (365d / 12d):0.00}", "PLN",
                $"{referenceValue.MaxPrice * (365d / 12d):0.00}", ConsoleColor.Gray);


            Display("Total protein", diet.Characteristics.TotalProtein, "g",
                referenceValue.TotalProtein - DietTarget.MacronutrientToleranceMarginG,
                referenceValue.TotalProtein + DietTarget.MacronutrientToleranceMarginG);
            Display("Total carbohydrates", diet.Characteristics.TotalCarbs, "g",
                referenceValue.TotalCarbs - DietTarget.MacronutrientToleranceMarginG,
                referenceValue.TotalCarbs + DietTarget.MacronutrientToleranceMarginG);
            Display("Total fat", diet.Characteristics.TotalFat, "g",
                referenceValue.TotalFat - DietTarget.MacronutrientToleranceMarginG,
                referenceValue.TotalFat + DietTarget.MacronutrientToleranceMarginG);

            Display("Total Vitamin A", diet.Characteristics.TotalVitaminAiu, "IU",
                DietTarget.MinDailyVitaminAiu);
            Display("Total Vitamin C", diet.Characteristics.TotalVitaminCMg, "Mg",
                DietTarget.MinDailyVitaminCMg, DietTarget.MaxDailyVitaminCMg);
            DisplayInColor("Total Fiber", $"{diet.Characteristics.TotalFiberGrams:0}", "g",
                $"14g for each 1000 kcal", ConsoleColor.White);

            Display("Total Iron", diet.Characteristics.TotalIronMg, "Mg",
                DietTarget.MinDailyIronMg, DietTarget.MaxDailyIronMg);
            Display("Total Calcium", diet.Characteristics.TotalCalciumMg, "Mg",
                DietTarget.MinDailyCalciumMg, DietTarget.MaxDailyCalciumMg);
            Display("Total Magnesium", diet.Characteristics.TotalMagnesiumMg, "Mg",
                DietTarget.MinDailyMagnesiumMg);
            Display("Total Phosphorus", diet.Characteristics.TotalPhosphorusMg, "Mg",
                DietTarget.MinDailyPhosphorusMg, DietTarget.MaxDailyPhosphorusMg);
            Display("Total Potassium", diet.Characteristics.TotalPotassiumMg, "Mg",
                DietTarget.MinDailyPotassiumMg, DietTarget.MaxDailyPotassiumMg);
            Display("Total Sodium", diet.Characteristics.TotalSodiumMg, "Mg",
                DietTarget.MinDailySodiumMg, DietTarget.MaxDailySodiumMg);
            Display("Total Zinc", diet.Characteristics.TotalZincMg, "Mg",
                DietTarget.MinDailyZincMg, DietTarget.MaxDailyZincMg);

            DisplayInColor("Total grams eaten*", $"{diet.Characteristics.TotalGramsEaten:0}", "g",
                null, fatColor);
            Console.WriteLine(
                "* Average is about 1.8 kg across the globe, in the US it's about 2.7 kg, where as Somalia it's about 1 kg.");
        }

        private void Display([NotNull] String label,
            Double value,
            [NotNull] String unit,
            Double minReferenceValue)
        {
            ConsoleColor valueColor = value >= minReferenceValue ? ConsoleColor.White : ConsoleColor.Red;
            DisplayInColor(label, $"{value:0}", unit, $"more than {minReferenceValue:0}", valueColor);
        }

        private void Display([NotNull] String label,
            Double value,
            [NotNull] String unit,
            Double minReferenceValue,
            Double maxReferenceValue)
        {
            ConsoleColor valueColor = value >= minReferenceValue && value <= maxReferenceValue
                ? ConsoleColor.White
                : ConsoleColor.Red;
            DisplayInColor(label, $"{value:0.0}", unit, $"{minReferenceValue:0.0} - {maxReferenceValue:0.0}",
                valueColor);
        }

        private void DisplayInColor([NotNull] String label, [NotNull] String value, [NotNull] String unit,
            [CanBeNull] String referenceValue,
            ConsoleColor valueColor)
        {
            ConsoleColor previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{label}: ".PadRight(25));

            Console.ForegroundColor = valueColor;
            Console.Write((value + " " + unit).PadRight(12));
            if (referenceValue != null)
            {
                Console.WriteLine("/ " + referenceValue + " " + unit);
            }
            else
            {
                Console.WriteLine("");
            }

            Console.ForegroundColor = previousColor;
        }
    }
}