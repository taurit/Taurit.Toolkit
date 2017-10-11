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
            DisplayInColor("Energy", $"{diet.Characteristics.TotalKcalIntake:0}", "kcal",
                $"{referenceValue.TotalKcalIntake}", ConsoleColor.Gray);
            DisplayInColor("Price", $"{diet.Characteristics.TotalPrice:0.00}", "PLN",
                $"{referenceValue.MaxPrice}", ConsoleColor.Gray);


            DisplayInColor("Total protein", $"{diet.Characteristics.TotalProtein:0}", "g",
                $"{referenceValue.TotalProtein}", proteinColor);
            DisplayInColor("Total carbohydrates", $"{diet.Characteristics.TotalCarbs:0}", "g",
                $"{referenceValue.TotalCarbs}", carbsColor);
            DisplayInColor("Total fat", $"{diet.Characteristics.TotalFat:0}", "g", $"{referenceValue.TotalFat}",
                fatColor);

            DisplayInColor("Total Vitamin A", $"{diet.Characteristics.TotalVitaminAiu:0}", "IU",
                DietTarget.MinDailyVitaminAiu, DietTarget.MaxDailyVitaminAiu, ConsoleColor.White);
            DisplayInColor("Total Vitamin C", $"{diet.Characteristics.TotalVitaminCMg:0}", "Mg",
                DietTarget.MinDailyVitaminCMg, DietTarget.MaxDailyVitaminCMg, ConsoleColor.White);
            DisplayInColor("Total Fiber", $"{diet.Characteristics.TotalFiberGrams:0}", "g",
                $"14g for each 1000 kcal", ConsoleColor.White);

            DisplayInColor("Total Iron", $"{diet.Characteristics.TotalIronMg:0}", "Mg",
                DietTarget.MinDailyIronMg, DietTarget.MaxDailyIronMg, ConsoleColor.White);
            DisplayInColor("Total Calcium", $"{diet.Characteristics.TotalCalciumMg:0}", "Mg",
                DietTarget.MinDailyCalciumMg, DietTarget.MaxDailyCalciumMg, ConsoleColor.White);
            DisplayInColor("Total Magnesium", $"{diet.Characteristics.TotalMagnesiumMg:0}", "Mg",
                DietTarget.MinDailyMagnesiumMg, ConsoleColor.White);
            DisplayInColor("Total Phosphorus", $"{diet.Characteristics.TotalPhosphorusMg:0}", "Mg",
                DietTarget.MinDailyPhosphorusMg, DietTarget.MaxDailyPhosphorusMg, ConsoleColor.White);
            DisplayInColor("Total Potassium", $"{diet.Characteristics.TotalPotassiumMg:0}", "Mg",
                DietTarget.MinDailyPotassiumMg, DietTarget.MaxDailyPotassiumMg, ConsoleColor.White);
            DisplayInColor("Total Sodium", $"{diet.Characteristics.TotalSodiumMg:0}", "Mg",
                DietTarget.MinDailySodiumMg, DietTarget.MaxDailySodiumMg, ConsoleColor.White);
            DisplayInColor("Total Zinc", $"{diet.Characteristics.TotalZincMg:0}", "Mg",
                DietTarget.MinDailyZincMg, DietTarget.MaxDailyZincMg, ConsoleColor.White);

            DisplayInColor("Total grams eaten*", $"{diet.Characteristics.TotalGramsEaten:0}", "g",
                null, fatColor);
            Console.WriteLine(
                "* Average is about 1.8 kg across the globe, in the US it's about 2.7 kg, where as Somalia it's about 1 kg.");
        }

        private void DisplayInColor([NotNull] String label,
            [NotNull] String value,
            [NotNull] String unit,
            Double minReferenceValue,
            ConsoleColor valueColor)
        {
            DisplayInColor(label, value, unit, $"more than {minReferenceValue:0}", valueColor);
        }

        private void DisplayInColor([NotNull] String label,
            [NotNull] String value,
            [NotNull] String unit,
            Double minReferenceValue,
            Double maxReferenceValue,
            ConsoleColor valueColor)
        {
            DisplayInColor(label, value, unit, $"{minReferenceValue:0} - {maxReferenceValue:0}", valueColor);
        }

        private void DisplayInColor([NotNull] String label, [NotNull] String value, [NotNull] String unit,
            [CanBeNull] String referenceValue,
            ConsoleColor valueColor)
        {
            ConsoleColor previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{label}: ".PadRight(25));

            Console.ForegroundColor = valueColor;
            Console.Write((value + " " + unit).PadRight(10));
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