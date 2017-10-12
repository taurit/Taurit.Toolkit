using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using Taurit.Toolkit.DietOptimization.Models;
using Taurit.Toolkit.DietOptimization.Services;

namespace Taurit.Toolkit.DietOptimization.DietOptimizers.GeneticAlgorithm
{
    public sealed class MutationHelper
    {
        /// <summary>
        ///     Value smaller than 1 percent is recommended in most sources (eg. 0.001).
        ///     0.05 works best here so far
        /// </summary>
        private const Double ChanceOfAmountMutation = 0.04;

        [NotNull] private readonly DietCharacteristicsCalculator _dietCharacteristicsCalculator;
        [NotNull] private readonly DietTarget _dietTarget;

        /// <summary>
        ///     This parameters seems to be the key to control how quickly algorithm converges. Bigger values (50-80) work best at
        ///     the beginning, but lower (10-20) might be better for fine-tuning
        /// </summary>
        private readonly Int32 _maxGramsToAddDuringMutation = 140;

        private readonly Random _randomNumberGenerator = new Random(123);
        [NotNull] private readonly ScoreCalculator _scoreCalculator;

        public MutationHelper([NotNull] DietCharacteristicsCalculator dietCharacteristicsCalculator,
            [NotNull] ScoreCalculator scoreCalculator,
            [NotNull] DietTarget dietTarget
        )
        {
            _dietCharacteristicsCalculator = dietCharacteristicsCalculator;
            _scoreCalculator = scoreCalculator;
            _dietTarget = dietTarget;
        }


        internal DietPlan GetMutationOf(DietPlan basePlan)
        {
            var newDietPlanItems = new List<DietPlanItem>(basePlan.DietPlanItems.Count);
            foreach (DietPlanItem dietPlanItem in basePlan.DietPlanItems)
            {
                Double amount = dietPlanItem.AmountGrams;
                Boolean amountShouldBeModified = dietPlanItem.FoodProduct.Metadata.FixedAmountG == null &&
                                                 ReturnTrueWithChanceOf(ChanceOfAmountMutation);
                if (amountShouldBeModified)
                {
                    Int32 gramsToAdd = _randomNumberGenerator.Next(2 * _maxGramsToAddDuringMutation) -
                                       _maxGramsToAddDuringMutation / 2;
                    amount += gramsToAdd;
                    if (amount < 0)
                    {
                        amount = 0;
                    }

                    if (dietPlanItem.FoodProduct.Metadata.MaxAmountG.HasValue && amount > dietPlanItem.FoodProduct.Metadata.MaxAmountG)
                        amount = dietPlanItem.FoodProduct.Metadata.MaxAmountG.Value;
                    if (dietPlanItem.FoodProduct.Metadata.MinAmountG.HasValue && amount < dietPlanItem.FoodProduct.Metadata.MinAmountG)
                        amount = dietPlanItem.FoodProduct.Metadata.MinAmountG.Value;
                }

                var newDietPlanItem = new DietPlanItem(dietPlanItem.FoodProduct, amount);
                newDietPlanItems.Add(newDietPlanItem);
            }

            Debug.Assert(newDietPlanItems.Count == basePlan.DietPlanItems.Count);

            DietCharacteristics characteristics = _dietCharacteristicsCalculator.GetCharacteristics(newDietPlanItems);
            Double score = _scoreCalculator.CalculateScore(characteristics, _dietTarget);

            return new DietPlan(newDietPlanItems, characteristics, score);
        }

        /// <summary>
        ///     Returns true with probability of <paramref name="chance" />
        /// </summary>
        /// <param name="chance">value from range 0-1</param>
        /// <returns></returns>
        private Boolean ReturnTrueWithChanceOf(Double chance)
        {
            Debug.Assert(chance >= 0);
            Debug.Assert(chance <= 1);
            return _randomNumberGenerator.NextDouble() < chance;
        }
    }
}