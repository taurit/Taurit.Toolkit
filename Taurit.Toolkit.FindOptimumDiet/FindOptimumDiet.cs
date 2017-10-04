using System;
using System.Collections.Generic;
using Taurit.Toolkit.DietOptimization.DietOptimizers;
using Taurit.Toolkit.DietOptimization.Models;
using Taurit.Toolkit.DietOptimization.Services;

namespace Taurit.Toolkit.FindOptimumDiet
{
    internal class FindOptimumDiet
    {
        private static void Main(string[] args)
        {
            var program = new FindOptimumDiet();
            program.Run(new DietCharacteristicsCalculator(), new DietCharacteristicsDistanceCalculator(), new DietPlanPresenter());
        }

        /// <summary>
        ///     Finds optimum diet for a given constraints
        /// </summary>
        private void Run(DietCharacteristicsCalculator dietCharacteristicsCalculator, DietCharacteristicsDistanceCalculator dietCharacteristicsDistanceCalculator, DietPlanPresenter dietPlanPresenter)
        {
            var products = new List<FoodProduct>();
            products.Add(new FoodProduct("Fish oil", 900, 0, 100, 0));
            products.Add(new FoodProduct("Meat", 400, 100, 0, 0));
            products.Add(new FoodProduct("Potatoes", 400, 0, 0, 100));

            var targetDietCharacteristics = new DietCharacteristics(3000, 203, 100, 323);
            var target = new DietConstraints(targetDietCharacteristics);


            IDietOptimizer dietOptimizer = new GeneticAlgorithmDietOptimizer(dietCharacteristicsCalculator,
                dietCharacteristicsDistanceCalculator, target);

            var optimumDiet = dietOptimizer.Optimize(products);

            // display result
            dietPlanPresenter.Display(optimumDiet);

            Console.ReadLine();
        }
    }
}