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
        [NotNull] private readonly DietCharacteristicsCalculator _dietCharacteristicsCalculator;
        [NotNull] private readonly ScoreCalculator _scoreCalculator;
        [NotNull] private readonly DietTarget _dietTarget;
#if DEBUG
        // constant seed might be used for results to be repeatable while developing/debugging
        private readonly Random _randomNumberGenerator = new Random(123);
#else
        private readonly Random _randomNumberGenerator = new Random();
#endif
        /// <summary>
        ///     This parameters seems to be the key to control how quickly algorithm converges. Bigger values (50-80) work best at
        ///     the beginning, but lower (10-20) might be better for fine-tuning
        /// </summary>
        private readonly Int32 _maxGramsToAddDuringMutation = 180;


        private const Int32 ChanceOfAmountMutationPercent = 1;

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
                Int32 amount = dietPlanItem.AmountGrams;
                Boolean amountShouldBeModified = dietPlanItem.FoodProduct.Metadata.FixedAmountG == null &&
                                                 ReturnTrueWithChanceOf(ChanceOfAmountMutationPercent);
                if (amountShouldBeModified)
                {
                    Int32 gramsToAdd = _randomNumberGenerator.Next(2 * _maxGramsToAddDuringMutation) -
                                       _maxGramsToAddDuringMutation / 2;
                    amount += gramsToAdd;
                    if (amount < 0)
                    {
                        amount = 0;
                    }
                }

                var newDietPlanItem = new DietPlanItem(dietPlanItem.FoodProduct, amount);
                newDietPlanItems.Add(newDietPlanItem);
            }

            Debug.Assert(newDietPlanItems.Count == basePlan.DietPlanItems.Count);

            DietCharacteristics characteristics = _dietCharacteristicsCalculator.GetCharacteristics(newDietPlanItems);
            Double score = _scoreCalculator.CalculateScore(characteristics, _dietTarget);

            return new DietPlan(newDietPlanItems, characteristics, score);
        }

        private Boolean ReturnTrueWithChanceOf(Int32 chance)
        {
            return _randomNumberGenerator.Next(100) == chance;
        }
    }
}