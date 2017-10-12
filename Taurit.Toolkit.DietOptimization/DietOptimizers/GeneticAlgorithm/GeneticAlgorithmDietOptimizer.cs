using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using Taurit.Toolkit.DietOptimization.Models;
using Taurit.Toolkit.DietOptimization.Services;

namespace Taurit.Toolkit.DietOptimization.DietOptimizers.GeneticAlgorithm
{
    public class GeneticAlgorithmDietOptimizer : IDietOptimizer
    {
        /// <summary>
        ///     How many diet plans a single generation of genetic algorithm contains?
        /// </summary>
        private const Int32 NumPlansInGeneration = 200;

        /// <summary>
        ///     Elitist selection
        ///     This number must be low to avoid generation degeneration, but might be useful to prevent discarding best solutions
        ///     found so far.
        /// </summary>
        private const Int32 NumPlansSurivingGeneration = 50;

        /// <summary>
        ///     How many generations are created/analyzed by single run of <see cref="Optimize" />?
        /// </summary>
        private const Int32 MaxNumGenerations = 50_000;

        private const Int32 AcceptableScore = 5;

        [NotNull] private readonly MutationHelper _mutationHelper;

        [NotNull] private readonly StartPointProvider _startPointProvider;

        public GeneticAlgorithmDietOptimizer([NotNull] DietCharacteristicsCalculator dietCharacteristicsCalculator,
            [NotNull] ScoreCalculator scoreCalculator,
            [NotNull] DietTarget dietTarget)
        {
            _startPointProvider = new StartPointProvider(dietCharacteristicsCalculator, scoreCalculator, dietTarget);
            _mutationHelper = new MutationHelper(dietCharacteristicsCalculator, scoreCalculator, dietTarget);
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
            }

            return currentGeneration.First(); // this is the best one we have found so far
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
                DietPlan randomDietPlan = _startPointProvider.GetRandomDietPlan(availableProducts);
                randomDietPlans.Add(randomDietPlan);
            }

            return randomDietPlans.OrderBy(x => x.ScoreToTarget).ToImmutableList();
        }

        /// <remarks>
        ///     "Elitism guarantees that the solution quality obtained by the GA will not decrease from one generation to the
        ///     next."
        ///     "The problem with elitism is that it causes the GA to converge on local maxima instead of the global maximum, so
        ///     pure elitism is just a race to the nearest local maximum and you'll get little improvement from there. "
        ///     This method quite quickly makes population degenerated (all diet plans are almost the same) and does not want to
        ///     go any further leaving us with some local minimum.
        /// </remarks>
        private ImmutableList<DietPlan> CreateNextGeneration_Elitist(ImmutableList<DietPlan> currentGeneration)
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
                DietPlan mutatedDietPlan = _mutationHelper.GetMutationOf(planToMutate);
                newGeneration.Add(mutatedDietPlan);
            }

            Debug.Assert(newGeneration.Count == NumPlansInGeneration);
            return newGeneration.OrderBy(x => x.ScoreToTarget).ToImmutableList();
        }

        /// <summary>
        /// Experimental selection method
        /// </summary>
        /// <param name="currentGeneration"></param>
        /// <returns></returns>
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
                DietPlan mutatedDietPlan = _mutationHelper.GetMutationOf(planToMutate);
                newGeneration.Add(mutatedDietPlan);
            }

            Debug.Assert(newGeneration.Count == NumPlansInGeneration);
            return newGeneration.OrderBy(x => x.ScoreToTarget).ToImmutableList();
        }


        private void LogGenerationsBestScore(Int32 generationNumber, Double score)
        {
            if (generationNumber % 1000 == 0)
            {
                Console.WriteLine($"Generation #{generationNumber}: best score is {score:0.00}");
            }
        }
    }
}