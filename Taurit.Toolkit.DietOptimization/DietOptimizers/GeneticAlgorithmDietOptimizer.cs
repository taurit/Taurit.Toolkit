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
        private const int NumPlansInGeneration = 100;

        private const int NumPlansSurivingGeneration = 50;
        private const int ChanceOfAmountMutationPercent = 10;
        private const int MaxGramsToAddDuringMutation = 10;

        /// <summary>
        ///     How many generations are created/analyzed by single run of <see cref="Optimize" />?
        /// </summary>
        private const int NumGenerations = 100_000;

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

            for (var i = 0; i < NumGenerations; i++)
            {
                currentGeneration = CreateNextGeneration(currentGeneration);
                LogGenerationsBestScore(i + 1, currentGeneration.First().ScoreToTarget);
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
                Console.WriteLine($"Generation #{generationNumber}: best score is {score}");
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
                        .Next(200); // avg of 100g of some product daily might be reasonable starting point
                dietPlanItems.Add(new DietPlanItem(product, randomAmount));
            }

            // calculate score
            var characteristics = _dietCharacteristicsCalculator.GetCharacteristics(dietPlanItems);
            var score = _scoreCalculator.CalculateScore(characteristics, _targets.DietCharacteristics);

            return new DietPlan(dietPlanItems, characteristics, score);
        }
    }
}