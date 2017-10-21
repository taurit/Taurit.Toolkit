﻿using System;
using System.Linq;
using JetBrains.Annotations;
using Taurit.Toolkit.DietOptimization.Models;

namespace Taurit.Toolkit.FindOptimumDiet
{
    /// <summary>
    ///     Presents data about Diet Plan in a console window
    /// </summary>
    internal sealed class DietPlanPresenter
    {
        internal void Display(DietPlan diet, DietTarget referenceValue)
        {
            DisplayHeader();
            //DisplaySkippedProducts(diet);
            //DisplaySeparator();
            DisplayChosenProducts(diet);
            DisplaySeparator();

            DisplayMainMetadata(diet, referenceValue);

            DisplayMacronutrientsMetadata(diet, referenceValue);
            DisplayVitaminAmounts(diet);
            DisplayMineralAmounts(diet);
            DisplayFatsAmounts(diet, referenceValue);

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
            Display("Total Copper", diet.Characteristics.TotalCopperMg, "Mg",
                DietTarget.MinDailyCopperMg, DietTarget.MaxDailyCopperMg);
            Display("Total Manganese", diet.Characteristics.TotalManganeseMg, "Mg",
                DietTarget.MinDailyManganeseMg, DietTarget.MaxDailyManganeseMg);
            Display("Total Selenium", diet.Characteristics.TotalSeleniumUg, "Ug",
                DietTarget.MinDailySeleniumUg, DietTarget.MaxDailySeleniumUg);
        }

        private void DisplayFatsAmounts(DietPlan diet, DietTarget referenceValue)
        {
            Display("FA Saturated", diet.Characteristics.TotalFattyAcidsSaturatedG, "g",
                0, referenceValue.MaxGramsOfSaturatedFat);
            Display("FA Monoun.", diet.Characteristics.TotalFattyAcidsMonounsaturatedG, "g");
            Display("FA Polyun.", diet.Characteristics.TotalFattyAcidsPolyunsaturatedG, "g");
            Display("FA Trans", diet.Characteristics.TotalFattyAcidsTransG, "g",
                0, DietTarget.MaxTransFatsG);
            Display("FA Cholesterol Mg", diet.Characteristics.TotalCholesterolMg, "Mg");
            DisplayInColor("Omega3:Omega6 ratio", diet.Characteristics.Omega3To6Ratio, "", "1:1 is best",
                ConsoleColor.Gray);
        }

        private void Display(String label, Double value, String unit)
        {
            DisplayInColor(label, $"{value:0.00}", unit, "not limited", ConsoleColor.White);
        }

        private void DisplayVitaminAmounts(DietPlan diet)
        {
            Display("Total Vitamin A", diet.Characteristics.TotalVitaminAiu, "IU",
                DietTarget.MinDailyVitaminAiu);
            Display("Total Vitamin B1", diet.Characteristics.TotalVitaminB1Mg, "Mg", DietTarget.MinDailyVitaminB1Mg,
                DietTarget.MaxDailyVitaminB1Mg);
            Display("Total Vitamin B2", diet.Characteristics.TotalVitaminB2Mg, "Mg", DietTarget.MinDailyVitaminB2Mg);
            Display("Total Vitamin B3", diet.Characteristics.TotalVitaminB3Mg, "Mg", DietTarget.MinDailyVitaminB3Mg,
                DietTarget.MaxDailyVitaminB3Mg);
            Display("Total Vitamin B5", diet.Characteristics.TotalVitaminB5Mg, "Mg", DietTarget.MinDailyVitaminB5Mg,
                DietTarget.MaxDailyVitaminB5Mg);
            Display("Total Vitamin B6", diet.Characteristics.TotalVitaminB6Mg, "Mg", DietTarget.MinDailyVitaminB6Mg,
                DietTarget.MaxDailyVitaminB6Mg);
            Display("Total Vitamin B12", diet.Characteristics.TotalVitaminB12Ug, "Ug", DietTarget.MinDailyVitaminB12Ug);
            Display("Total Vitamin E", diet.Characteristics.TotalVitaminEMg, "Mg", DietTarget.MinDailyVitaminEMg,
                DietTarget.MaxDailyVitaminEMg);
            Display("Total Vitamin K", diet.Characteristics.TotalVitaminKUg, "Ug", DietTarget.MinDailyVitaminKUg);
            Display("Total Vitamin C", diet.Characteristics.TotalVitaminCMg, "Mg", DietTarget.MinDailyVitaminCMg,
                DietTarget.MaxDailyVitaminCMg);
            Display("Total Choline", diet.Characteristics.TotalCholineMg, "Mg", DietTarget.MinDailyCholineMg,
                DietTarget.MaxDailyCholineMg);
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
            Int32 numUsedProducts = diet.DietPlanItems.Count(x => Math.Abs(x.AmountGrams) >= 0.1);
            DisplayInColor("Num used products", $"{numUsedProducts:0}", "", $"{diet.DietPlanItems.Count:0}",
                ConsoleColor.Gray);
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
            foreach (DietPlanItem item in diet.DietPlanItems.Where(x => x.AmountGrams > 0)
                .OrderByDescending(x => x.AmountGrams)) // skip 0g entries
            {
                Double price = (item.AmountGrams/1000d) * item.FoodProduct.Metadata.PricePerKg;
                Double? numItems = CalculateNumItems(item);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(
                    $"{(item.AmountGrams.ToString("0.00") + "g").PadLeft(10)}: {item.FoodProduct.Name}");

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write($" {price:0.00} PLN/day");

                if (numItems.HasValue)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write($" {numItems.Value:0.0} items");

                    if (!String.IsNullOrEmpty(item.FoodProduct.Metadata.OneItemDescription))
                    {
                        Console.Write($" (1 item = {item.FoodProduct.Metadata.OneItemDescription})");
                    }
                }
                Console.WriteLine("");
            }
        }

        private static Double? CalculateNumItems(DietPlanItem item)
        {
            return item.FoodProduct.Metadata.OneItemWeight.HasValue
                ? item.AmountGrams / item.FoodProduct.Metadata.OneItemWeight.Value
                : (Double?) null;
        }

        private void Display([NotNull] String label,
            Double value,
            [NotNull] String unit,
            Double minReferenceValue)
        {
            ConsoleColor valueColor = value >= minReferenceValue ? ConsoleColor.White : ConsoleColor.Red;
            DisplayInColor(label, $"{value:0.0}", unit, $"more than {minReferenceValue:0}", valueColor);
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