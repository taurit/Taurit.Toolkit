﻿using System;
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
        internal void Display(DietPlan diet, DietTarget referenceValue)
        {
            DisplayHeader();
            DisplaySkippedProducts(diet);
            DisplaySeparator();
            DisplayChosenProducts(diet);
            DisplaySeparator();

            DisplayMainMetadata(diet, referenceValue);

            DisplayMacronutrientsMetadata(diet, referenceValue);
            DisplayVitaminAmounts(diet);
            DisplayMineralAmounts(diet);
            DisplayFatsAmounts(diet);

            DisplayTotalGramsEaten(diet);
        }

        private void DisplayTotalGramsEaten(DietPlan diet)
        {
            DisplayInColor("Total grams eaten*", $"{diet.Characteristics.TotalGramsEaten:0}", "g",
                null, ConsoleColor.White);
            Console.WriteLine(
                "* Average is about 1.8 kg across the globe, in the US it's about 2.7 kg, where as Somalia it's about 1 kg.");
        }

        private void DisplayMineralAmounts(DietPlan diet)
        {
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
        }

        private void DisplayFatsAmounts(DietPlan diet)
        {
            Display("FA Saturated", diet.Characteristics.TotalFattyAcidsSaturatedG, "g",
                0, 0); // todo range
            Display("FA Monoun.", diet.Characteristics.TotalFattyAcidsMonounsaturatedG, "g",
                0, 0);
            Display("FA Polyun.", diet.Characteristics.TotalFattyAcidsPolyunsaturatedG, "g",
                0, 0);
            Display("FA Trans", diet.Characteristics.TotalFattyAcidsTransG, "g",
                0, 0);
            Display("FA Cholesterol Mg", diet.Characteristics.TotalCholesterolMg, "g",
                0, 0);
        }

        private void DisplayVitaminAmounts(DietPlan diet)
        {
            Display("Total Vitamin A", diet.Characteristics.TotalVitaminAiu, "IU",
                DietTarget.MinDailyVitaminAiu);
            Display("Total Vitamin C", diet.Characteristics.TotalVitaminCMg, "Mg",
                DietTarget.MinDailyVitaminCMg, DietTarget.MaxDailyVitaminCMg);
        }

        private void DisplayMacronutrientsMetadata(DietPlan diet, DietTarget referenceValue)
        {
            Display("Total protein", diet.Characteristics.TotalProtein, "g",
                referenceValue.TotalProtein - DietTarget.MacronutrientToleranceMarginG,
                referenceValue.TotalProtein + DietTarget.MacronutrientToleranceMarginG);
            Display("Total carbohydrates", diet.Characteristics.TotalCarbs, "g",
                referenceValue.TotalCarbs - DietTarget.MacronutrientToleranceMarginG,
                referenceValue.TotalCarbs + DietTarget.MacronutrientToleranceMarginG);
            Display("Total fat", diet.Characteristics.TotalFat, "g",
                referenceValue.TotalFat - DietTarget.MacronutrientToleranceMarginG,
                referenceValue.TotalFat + DietTarget.MacronutrientToleranceMarginG);
            DisplayInColor("Total Fiber", $"{diet.Characteristics.TotalFiberGrams:0}", "g",
                $"14g for each 1000 kcal", ConsoleColor.White);
        }

        private void DisplayMainMetadata(DietPlan diet, DietTarget referenceValue)
        {
            DisplayInColor("Score (lower is better)", $"{diet.ScoreToTarget:0}", "", "0", ConsoleColor.Cyan);
            Int32 numSkippedProducts = diet.DietPlanItems.Count(x => Math.Abs(x.AmountGrams) < 0.1);
            DisplayInColor("Num skipped products", $"{numSkippedProducts:0}", "", "-", ConsoleColor.Gray);
            Display("Energy", diet.Characteristics.TotalKcalIntake, "kcal",
                referenceValue.TotalKcalIntake - DietTarget.EnergyToleranceMarginKcal,
                referenceValue.TotalKcalIntake + DietTarget.EnergyToleranceMarginKcal);
            DisplayInColor("Price", $"{diet.Characteristics.TotalPrice:0.00}", "PLN",
                $"{referenceValue.MaxPrice}", ConsoleColor.Green);
            DisplayInColor("Avg monthly price", $"{diet.Characteristics.TotalPrice * (365d / 12d):0.00}", "PLN",
                $"{referenceValue.MaxPrice * (365d / 12d):0.00}", ConsoleColor.Green);
        }

        private static void DisplayHeader()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Diet plan");
            DisplaySeparator();
        }

        private static void DisplaySeparator()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("------------------------------------------------");
        }

        private static void DisplayChosenProducts(DietPlan diet)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            foreach (DietPlanItem item in diet.DietPlanItems.Where(x => x.AmountGrams > 0)
                .OrderByDescending(x => x.AmountGrams)) // skip 0g entries
                Console.WriteLine($"{(item.AmountGrams.ToString("0.00") + "g").PadLeft(10)}: {item.FoodProduct.Name}");
        }

        private static void DisplaySkippedProducts(DietPlan diet)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (DietPlanItem item in diet.DietPlanItems.Where(x => Math.Abs(x.AmountGrams) < 0.1))
                Console.WriteLine($"{(item.AmountGrams.ToString("0.00") + "g").PadLeft(10)}: {item.FoodProduct.Name}");
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