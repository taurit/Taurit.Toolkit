using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Taurit.Toolkit.DietOptimization.Models;
using Taurit.Toolkit.DietOptimization.Services;

namespace Taurit.Toolkit.DietOptimization.DietOptimizers.GeneticAlgorithm
{
    /// <summary>
    ///     Provides start point for optimization.
    ///     Because only product amounts are optimized, this just assigns random amounts to all the products.
    ///     From what I remember initializing with (pseudo)random numbers is probably best strategy.
    ///     The only exception are products, for which user requested "fixed amount". Those amounts are not optimized and are
    ///     initialized with their expected values.
    /// </summary>
    public class StartPointProvider
    {
        [NotNull] private readonly DietCharacteristicsCalculator _dietCharacteristicsCalculator;
        [NotNull] private readonly DietTarget _dietTarget;

        private readonly Random _randomNumberGenerator = new Random(123);
        [NotNull] private readonly ScoreCalculator _scoreCalculator;

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
                Double randomAmount =
                    _randomNumberGenerator
                        .NextDouble() * 100; // experimental
                Double amount = product.Metadata.FixedAmountG ?? product.Metadata.StartAmountG ??
                                product.Metadata.MinAmountG ??
                                product.Metadata.MaxAmountG ?? randomAmount;
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