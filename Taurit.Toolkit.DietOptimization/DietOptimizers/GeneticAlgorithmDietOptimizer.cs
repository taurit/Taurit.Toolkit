using System;
using System.Collections;
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
        private const int NumPlansInGeneration = 400;

        private const int NumPlansSurivingGeneration = 200;
        private const int ChanceOfAmountMutationPercent = 1;
        /// <summary>
        /// This parameters seems to be the key to control how quickly algorithm converges. Bigger values (50-80) work best at the beginning, but lower (10-20) might be better for fine-tuning
        /// </summary>
        private int MaxGramsToAddDuringMutation = 180;

        /// <summary>
        ///     How many generations are created/analyzed by single run of <see cref="Optimize" />?
        /// </summary>
        private const int MaxNumGenerations = 100_000;
        private const int AcceptableScore = 5;

        private readonly DietCharacteristicsCalculator _dietCharacteristicsCalculator;
        private readonly Random _randomNumberGenerator = new Random();
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
            var currentGeneration = CreateFirstGeneration(availableProducts);
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
                //if (MaxGramsToAddDuringMutation > 10 && i % 1000 == 0)
                //{
                //    MaxGramsToAddDuringMutation -= 10;
                //}
            }

            return currentGeneration.First(); // this is the best one we have found so far
        }

        private ImmutableList<DietPlan> CreateNextGeneration(ImmutableList<DietPlan> currentGeneration)
        {
            Debug.Assert(NumPlansSurivingGeneration < NumPlansInGeneration);
            var numNewPlansToGenerate = NumPlansInGeneration - NumPlansSurivingGeneration;

            var newGeneration = new List<DietPlan>(NumPlansInGeneration);

            // mutated plans will fill the shoes of the ones that didn't survive
            var plansToMutate = currentGeneration.Take(numNewPlansToGenerate).ToList();
            //var plansToMutate = Enumerable.Range(0, numNewPlansToGenerate).Select(x => currentGeneration.First()).ToList();

            newGeneration.AddRange(currentGeneration.Take(NumPlansSurivingGeneration));
            foreach (var planToMutate in plansToMutate)
            {
                var mutatedDietPlan = GetMutationOf(planToMutate);
                newGeneration.Add(mutatedDietPlan);
            }

            Debug.Assert(newGeneration.Count == NumPlansInGeneration);
            return newGeneration.OrderBy(x => x.ScoreToTarget).ToImmutableList();
        }

        private DietPlan GetMutationOf(DietPlan basePlan)
        {
            var newDietPlanItems = new List<DietPlanItem>(basePlan.DietPlanItems.Count);
            foreach (var dietPlanItem in basePlan.DietPlanItems)
            {
                var amount = dietPlanItem.AmountGrams;
                var amountShouldBeModified = ReturnTrueWithChanceOf(ChanceOfAmountMutationPercent);
                if (amountShouldBeModified)
                {
                    var gramsToAdd = _randomNumberGenerator.Next(2 * MaxGramsToAddDuringMutation) -
                                     MaxGramsToAddDuringMutation / 2;
                    amount += gramsToAdd;
                    if (amount < 0) amount = 0;
                }

                var newDietPlanItem = new DietPlanItem(dietPlanItem.FoodProduct, amount);
                newDietPlanItems.Add(newDietPlanItem);
            }

            Debug.Assert(newDietPlanItems.Count == basePlan.DietPlanItems.Count);

            var characteristics = _dietCharacteristicsCalculator.GetCharacteristics(newDietPlanItems);
            var score = _scoreCalculator.CalculateScore(characteristics, _targets.DietCharacteristics);

            return new DietPlan(newDietPlanItems, characteristics, score);
        }

        private bool ReturnTrueWithChanceOf(int chance)
        {
            return _randomNumberGenerator.Next(100) == chance;
        }

        private void LogGenerationsBestScore(int generationNumber, double score)
        {
            if (generationNumber % 1000 == 0)
                Console.WriteLine($"Generation #{generationNumber}: best score is {score:0.00}");
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
                var randomDietPlan = GetRandomDietPlan(availableProducts);
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

            foreach (var product in availableProducts)
            {
                var randomAmount =
                    _randomNumberGenerator
                        .Next(100); // experimental
                dietPlanItems.Add(new DietPlanItem(product, randomAmount));
                //dietPlanItems.Add(new DietPlanItem(product, 0)); // what if I start with 0
                // ^ better start point when there's a lot of product to choose from (start point closer to minimum), but approaching optimum seems slow and almost no product has 0 grams
            }

            // calculate score
            var characteristics = _dietCharacteristicsCalculator.GetCharacteristics(dietPlanItems);
            var score = _scoreCalculator.CalculateScore(characteristics, _targets.DietCharacteristics);

            return new DietPlan(dietPlanItems, characteristics, score);
        }
    }
}