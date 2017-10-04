using System;
using System.Collections.Generic;
using Taurit.Toolkit.DietOptimization.Models;
using Taurit.Toolkit.DietOptimization.Services;

namespace Taurit.Toolkit.DietOptimization.DietOptimizers
{
    public class GeneticAlgorithmDietOptimizer : IDietOptimizer
    {
        private readonly DietCharacteristicsCalculator _dietCharacteristicsCalculator;
        private readonly DietCharacteristicsDistanceCalculator _scoreCalculator;
        private readonly DietConstraints _targets;

        public GeneticAlgorithmDietOptimizer(DietCharacteristicsCalculator dietCharacteristicsCalculator,
            DietCharacteristicsDistanceCalculator scoreCalculator, DietConstraints targets)
        {
            _dietCharacteristicsCalculator = dietCharacteristicsCalculator;
            _scoreCalculator = scoreCalculator;
            _targets = targets;
        }

        public DietPlan Optimize(IReadOnlyCollection<FoodProduct> availableProducts)
        {
            var dietPlan = GetRandomDietPlan(availableProducts);
            return dietPlan;
        }

        /// <summary>
        ///     Generates (pseudo)random diet plan that includes all products from <paramref name="availableProducts" /> in random
        ///     (but reasonable) quantities. might be used as a start point for optimization.
        /// </summary>
        /// <param name="availableProducts"></param>
        /// <returns></returns>
        private DietPlan GetRandomDietPlan(IReadOnlyCollection<FoodProduct> availableProducts)
        {
            var randomGenerator = new Random();
            var dietPlanItems = new List<DietPlanItem>(availableProducts.Count);

            foreach (var product in availableProducts)
            {
                var randomAmount =
                    randomGenerator.Next(200); // avg of 100g of some product daily might be reasonable starting point
                dietPlanItems.Add(new DietPlanItem(product, randomAmount));
            }

            // calculate score
            var characteristics = _dietCharacteristicsCalculator.GetCharacteristics(dietPlanItems);
            var score = _scoreCalculator.CalculateScore(characteristics, _targets.DietCharacteristics);

            return new DietPlan(dietPlanItems, characteristics, score);
        }

    }
}