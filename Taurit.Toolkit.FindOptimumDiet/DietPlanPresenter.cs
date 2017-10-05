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

        internal void Display(DietPlan diet)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Diet plan");
            Console.WriteLine("------------------------------------------------");

            Console.ForegroundColor = ConsoleColor.Green;
            Int32 numSkippedProducts = diet.DietPlanItems.Count(x => x.AmountGrams == 0);
            foreach (DietPlanItem item in diet.DietPlanItems.Where(x => x.AmountGrams > 0)) // skip 0g entries
                Console.WriteLine($"Product: {item.FoodProduct.Name}, quantity: {item.AmountGrams}g");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("------------------------------------------------");

            DisplayInColor("Score: ", $"{diet.ScoreToTarget} (lower is better)", ConsoleColor.Red);
            DisplayInColor("Num skipped products: ", $"{numSkippedProducts}", ConsoleColor.White);
            DisplayInColor("Energy: ", $"{diet.Characteristics.TotalKcalIntake} kcal", ConsoleColor.Gray);

            DisplayInColor("Total protein: ", $"{diet.Characteristics.TotalProtein} g", proteinColor);
            DisplayInColor("Total carbohydrates: ", $"{diet.Characteristics.TotalCarbs} g", carbsColor);
            DisplayInColor("Total fat: ", $"{diet.Characteristics.TotalFat} g", fatColor);

            DisplayInColor("Total Vitamin A: ", $"{diet.Characteristics.TotalVitaminAiu} IU", ConsoleColor.White);
            DisplayInColor("Total Vitamin C: ", $"{diet.Characteristics.TotalVitaminCMg} Mg", ConsoleColor.White);
            DisplayInColor("Total Fiber: ", $"{diet.Characteristics.TotalVitaminAiu} g", ConsoleColor.White);

            DisplayInColor("Total grams eaten*: ", $"{diet.Characteristics.TotalGramsEaten} g", fatColor);
            Console.WriteLine(
                "* Average is about 1.8 kg across the globe, in the US it's about 2.7 kg, where as Somalia it's about 1 kg.");
        }

        private void DisplayInColor(String label, String value, ConsoleColor valueColor)
        {
            ConsoleColor previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{label}: ");

            Console.ForegroundColor = valueColor;
            Console.WriteLine(value);

            Console.ForegroundColor = previousColor;
        }
    }
}