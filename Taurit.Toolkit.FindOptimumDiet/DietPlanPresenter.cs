using System;
using System.Linq;
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

        internal void Display(DietPlan diet, DietCharacteristics referenceValue)
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
            DisplayInColor("Energy", $"{diet.Characteristics.TotalKcalIntake:0}", "kcal", $"{referenceValue.TotalKcalIntake}", ConsoleColor.Gray);

            DisplayInColor("Total protein", $"{diet.Characteristics.TotalProtein:0}", "g", $"{referenceValue.TotalProtein}", proteinColor);
            DisplayInColor("Total carbohydrates", $"{diet.Characteristics.TotalCarbs:0}", "g", $"{referenceValue.TotalCarbs}", carbsColor);
            DisplayInColor("Total fat", $"{diet.Characteristics.TotalFat:0}", "g", $"{referenceValue.TotalFat}", fatColor);

            DisplayInColor("Total Vitamin A", $"{diet.Characteristics.TotalVitaminAiu:0}", "IU", $"{referenceValue.TotalVitaminAiu}", ConsoleColor.White);
            DisplayInColor("Total Vitamin C", $"{diet.Characteristics.TotalVitaminCMg:0}", "Mg", $"{referenceValue.TotalVitaminCMg}", ConsoleColor.White);
            DisplayInColor("Total Fiber", $"{diet.Characteristics.TotalFiberGrams:0}", "g", $"{referenceValue.TotalFiberGrams}", ConsoleColor.White);

            DisplayInColor("Total grams eaten*", $"{diet.Characteristics.TotalGramsEaten:0}", "g", $"{referenceValue.TotalGramsEaten}",  fatColor);
            Console.WriteLine(
                "* Average is about 1.8 kg across the globe, in the US it's about 2.7 kg, where as Somalia it's about 1 kg.");
        }

        private void DisplayInColor(String label, String value, String unit, String referenceValue,
            ConsoleColor valueColor)
        {
            ConsoleColor previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{label}: ".PadRight(25));

            Console.ForegroundColor = valueColor;
            Console.Write((value + " " + unit).PadRight(20));
            Console.WriteLine("/ " + referenceValue + " " + unit);

            Console.ForegroundColor = previousColor;
        }
    }
}