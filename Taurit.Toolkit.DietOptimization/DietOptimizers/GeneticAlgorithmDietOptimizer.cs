using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Taurit.Toolkit.DietOptimization.Models;
using Taurit.Toolkit.DietOptimization.Services;

namespace Taurit.Toolkit.DietOptimization.DietOptimizers
{
    public class GeneticAlgorithmDietOptimizer : IDietOptimizer
    {
        /// <summary>
        ///     How many diet plans a single generation of genetic algorithm contains?
        /// </summary>
        private const Int32 NumPlansInGeneration = 200;

        private const Int32 NumPlansSurivingGeneration = 100;
        private const Int32 ChanceOfAmountMutationPercent = 1;

        /// <summary>
        ///     How many generations are created/analyzed by single run of <see cref="Optimize" />?
        /// </summary>
        private const Int32 MaxNumGenerations = 30_000;

        private const Int32 AcceptableScore = 5;

        private readonly DietCharacteristicsCalculator _dietCharacteristicsCalculator;
#if DEBUG
        private readonly Random _randomNumberGenerator = new Random(12345678); // for results to be repeatable while developing/debugging
#else
        private readonly Random _randomNumberGenerator = new Random();
#endif
        private readonly ScoreCalculator _scoreCalculator;
        private readonly DietTarget _targets;

        /// <summary>
        ///     This parameters seems to be the key to control how quickly algorithm converges. Bigger values (50-80) work best at
        ///     the beginning, but lower (10-20) might be better for fine-tuning
        /// </summary>
        private Int32 _maxGramsToAddDuringMutation = 180;


        public GeneticAlgorithmDietOptimizer(DietCharacteristicsCalculator dietCharacteristicsCalculator,
            ScoreCalculator scoreCalculator, DietTarget targets)
        {
            _dietCharacteristicsCalculator = dietCharacteristicsCalculator;
            _scoreCalculator = scoreCalculator;
            _targets = targets;
        }

        public DietPlan Optimize(IReadOnlyCollection<FoodProduct> availableProducts)
        {
            ImmutableList<DietPlan> currentGeneration = CreateFirstGeneration(availableProducts);
            LogGenerationsBestScore(0, currentGeneration.First().ScoreToTarget);

            for (var i = 0; i < MaxNumGenerations; i++)
            {
                currentGeneration = CreateNextGeneration(currentGeneration);
                LogGenerationsBestScore(i + 1, currentGeneration.First().ScoreToTarget);
                if (currentGeneration.First().ScoreToTarget < AcceptableScore)
                {
                    // result is good enough, we don't need to search further
                    break;
                }

                // experimental: due to observation that fine-tuning requires smaller steps
                if (_maxGramsToAddDuringMutation > 10 && i % 1000 == 0)
                {
                    _maxGramsToAddDuringMutation -= 10;
                }
            }

            return currentGeneration.First(); // this is the best one we have found so far
        }

        private ImmutableList<DietPlan> CreateNextGeneration(ImmutableList<DietPlan> currentGeneration)
        {
            Debug.Assert(NumPlansSurivingGeneration < NumPlansInGeneration);
            Int32 numNewPlansToGenerate = NumPlansInGeneration - NumPlansSurivingGeneration;

            var newGeneration = new List<DietPlan>(NumPlansInGeneration);

            // mutated plans will fill the shoes of the ones that didn't survive
            List<DietPlan> plansToMutate = currentGeneration.Take(numNewPlansToGenerate).ToList();
            //var plansToMutate = Enumerable.Range(0, numNewPlansToGenerate).Select(x => currentGeneration.First()).ToList();

            newGeneration.AddRange(currentGeneration.Take(NumPlansSurivingGeneration));
            foreach (DietPlan planToMutate in plansToMutate)
            {
                DietPlan mutatedDietPlan = GetMutationOf(planToMutate);
                newGeneration.Add(mutatedDietPlan);
            }

            Debug.Assert(newGeneration.Count == NumPlansInGeneration);
            return newGeneration.OrderBy(x => x.ScoreToTarget).ToImmutableList();
        }

        private DietPlan GetMutationOf(DietPlan basePlan)
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
            Double score = _scoreCalculator.CalculateScore(characteristics, _targets);

            return new DietPlan(newDietPlanItems, characteristics, score);
        }

        private Boolean ReturnTrueWithChanceOf(Int32 chance)
        {
            return _randomNumberGenerator.Next(100) == chance;
        }

        private void LogGenerationsBestScore(Int32 generationNumber, Double score)
        {
            if (generationNumber % 1000 == 0)
            {
                Console.WriteLine($"Generation #{generationNumber}: best score is {score:0.00}");
            }
        }

        /// <summary>
        ///     Returns list of random diet plans, ordered by score (best first)
        /// </summary>
        /// <param name="availableProducts"></param>
        /// <returns></returns>
        private ImmutableList<DietPlan> CreateFirstGeneration(IReadOnlyCollection<FoodProduct> availableProducts)
        {
            var randomDietPlans = new List<DietPlan>(NumPlansInGeneration);
            for (var i = 0; i < NumPlansInGeneration; i++)
            {
                DietPlan randomDietPlan = GetRandomDietPlan(availableProducts);
                randomDietPlans.Add(randomDietPlan);
            }

            return randomDietPlans.OrderBy(x => x.ScoreToTarget).ToImmutableList();
        }

        /// <summary>
        ///     Generates (pseudo)random diet plan that includes all products from <paramref name="availableProducts" /> in random
        ///     (but reasonable) quantities. might be used as a start point for optimization.
        /// </summary>
        /// <param name="availableProducts"></param>
        /// <returns></returns>
        private DietPlan GetRandomDietPlan(IReadOnlyCollection<FoodProduct> availableProducts)
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
            Double score = _scoreCalculator.CalculateScore(characteristics, _targets);

            return new DietPlan(dietPlanItems, characteristics, score);
        }
    }
}