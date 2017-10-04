using System;
using Taurit.Toolkit.DietOptimization.Models;

namespace Taurit.Toolkit.FindOptimumDiet
{
    /// <summary>
    ///     Presents data about Diet Plan in a console
    /// </summary>
    internal class DietPlanPresenter
    {
        private ConsoleColor proteinColor = ConsoleColor.Cyan; // color convention same as in FitAtu app
        private ConsoleColor fatColor = ConsoleColor.Yellow;
        private ConsoleColor carbsColor = ConsoleColor.Green;

        internal void Display(DietPlan diet)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Diet plan");
            Console.WriteLine("------------------------------------------------");

            Console.ForegroundColor = ConsoleColor.Green;
            foreach (var item in diet.DietPlanItems)
                Console.WriteLine($"Product: {item.FoodProduct.Name}, quantity: {item.AmountGrams}g");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("------------------------------------------------");

            DisplayInColor("Score: ", $"{diet.ScoreToTarget} (lower is better)", ConsoleColor.Red);
            DisplayInColor("Energy: ", $"{diet.Characteristics.TotalKcalIntake} kcal", ConsoleColor.Gray);
            DisplayInColor("Total protein: ", $"{diet.Characteristics.TotalProtein} g", proteinColor);
            DisplayInColor("Total carbohydrates: ", $"{diet.Characteristics.TotalCarbs} g", carbsColor);
            DisplayInColor("Total fat: ", $"{diet.Characteristics.TotalFat} g", fatColor);
        }
        
        private void DisplayInColor(string label, string value, ConsoleColor valueColor)
        {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{label}: ");

            Console.ForegroundColor = valueColor;
            Console.WriteLine(value);
            
            Console.ForegroundColor = previousColor;
        }
    }
}