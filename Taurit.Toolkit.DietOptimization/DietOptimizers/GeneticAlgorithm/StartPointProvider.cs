using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Taurit.Toolkit.DietOptimization.Models;
using Taurit.Toolkit.DietOptimization.Services;

namespace Taurit.Toolkit.DietOptimization.DietOptimizers.GeneticAlgorithm
{
    public class StartPointProvider
    {
        [NotNull] private readonly DietCharacteristicsCalculator _dietCharacteristicsCalculator;
        [NotNull] private readonly ScoreCalculator _scoreCalculator;
        [NotNull] private readonly DietTarget _dietTarget;

        private readonly Random _randomNumberGenerator = new Random(123);

        public StartPointProvider([NotNull] DietCharacteristicsCalculator dietCharacteristicsCalculator,
            [NotNull] ScoreCalculator scoreCalculator,
            [NotNull] DietTarget dietTarget)
        {
            _dietCharacteristicsCalculator = dietCharacteristicsCalculator;
            _scoreCalculator = scoreCalculator;
            _dietTarget = dietTarget;
        }

        /// <summary>
        ///     Generates (pseudo)random diet plan that includes all products from <paramref name="availableProducts" /> in random
        ///     (but reasonable) quantities. might be used as a start point for optimization.
        /// </summary>
        /// <param name="availableProducts"></param>
        /// <returns></returns>
        internal DietPlan GetRandomDietPlan(IReadOnlyCollection<FoodProduct> availableProducts)
        {
            var dietPlanItems = new List<DietPlanItem>(availableProducts.Count);

            foreach (FoodProduct product in availableProducts)
            {
                Int32 randomAmount =
                    _randomNumberGenerator
                        .Next(100); // experimental
                Int32 amount = product.Metadata.FixedAmountG ?? randomAmount;
                dietPlanItems.Add(new DietPlanItem(product, amount));
                //dietPlanItems.Add(new DietPlanItem(product, 0)); // what if I start with 0
                // ^ better start point when there's a lot of product to choose from (start point closer to minimum), but approaching optimum seems slow and almost no product has 0 grams
            }

            // calculate score
            DietCharacteristics characteristics = _dietCharacteristicsCalculator.GetCharacteristics(dietPlanItems);
            Double score = _scoreCalculator.CalculateScore(characteristics, _dietTarget);

            return new DietPlan(dietPlanItems, characteristics, score);
        }
    }
}