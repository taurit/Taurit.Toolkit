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
        private const Int32 NumPlansInGeneration = 600;

        /// <summary>
        ///     Elitist selection
        ///     This number must be low to avoid generation degeneration, but might be useful to prevent discarding best solutions
        ///     found so far.
        /// </summary>
        private const Int32 NumPlansSurivingGeneration = 50;

        /// <summary>
        ///     How many generations are created/analyzed by single run of <see cref="Optimize" />?
        /// </summary>
        private const Int32 MaxNumGenerations = 1_000;

        private const Int32 AcceptableScore = 20;
        [NotNull] private readonly CrossoverHelper _crossoverHelper;

        [NotNull] private readonly MutationHelper _mutationHelper;

        private readonly Random _randomNumberGenerator;

        [NotNull] private readonly StartPointProvider _startPointProvider;

        [CanBeNull] private DietPlan _bestPlanSoFar;
        private readonly int _threadNumber;

        public GeneticAlgorithmDietOptimizer([NotNull] DietCharacteristicsCalculator dietCharacteristicsCalculator, [NotNull] ScoreCalculator scoreCalculator, [NotNull] DietTarget dietTarget, Int32 threadNumber)
        {
            _crossoverHelper = new CrossoverHelper(dietCharacteristicsCalculator, scoreCalculator, dietTarget);
            _startPointProvider = new StartPointProvider(dietCharacteristicsCalculator, scoreCalculator, dietTarget);
            _mutationHelper = new MutationHelper(dietCharacteristicsCalculator, scoreCalculator, dietTarget);
            _threadNumber = threadNumber;
            _randomNumberGenerator = new Random(threadNumber); // for repeatable, but different results for each thread
        }

        public DietPlan Optimize(IReadOnlyCollection<FoodProduct> availableProducts)
        {
            ImmutableList<DietPlan> currentGeneration = CreateFirstGeneration(availableProducts);
            LogGenerationsBestScore(0, currentGeneration.First().ScoreToTarget);
            _bestPlanSoFar = currentGeneration.First();
            
            for (var i = 0; i < MaxNumGenerations; i++)
            {
                currentGeneration = CreateNextGeneration(currentGeneration);
                DietPlan bestInGeneration = currentGeneration.First();
                LogGenerationsBestScore(i + 1, bestInGeneration.ScoreToTarget);

                Debug.Assert(_bestPlanSoFar != null);
                if (bestInGeneration.ScoreToTarget < _bestPlanSoFar.ScoreToTarget)
                {
                    _bestPlanSoFar = bestInGeneration;
                }
                if (bestInGeneration.ScoreToTarget < AcceptableScore)
                {
                    // result is good enough, we don't need to search further
                    break;
                }
            }

            Debug.Assert(_bestPlanSoFar != null);
            return _bestPlanSoFar;
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
        ///     Experimental selection method
        ///     Source: http://aragorn.pb.bialystok.pl/~wkwedlo/EA2.pdf
        ///     also: http://www.k0pper.republika.pl/geny.htm
        /// </summary>
        private ImmutableList<DietPlan> CreateNextGeneration(ImmutableList<DietPlan> currentGeneration)
        {
            var newGeneration = new List<DietPlan>(NumPlansInGeneration);

            Double sumOfInvertedScores = currentGeneration.Sum(x => 1 / x.ScoreToTarget);

            // select parents
            var planWithProbabilities = new List<DietPlanWithProbability>(currentGeneration.Count);
            var accumulatedProbability = 0d;
            foreach (DietPlan plan in currentGeneration)
            {
                // w maksymalizacji: Przystosowanie / Suma przystosowania wszystkich osobników

                Double probability = 1 / plan.ScoreToTarget / sumOfInvertedScores; // for minimization
                Debug.Assert(probability >= 0);
                Debug.Assert(probability <= 1);
                accumulatedProbability += probability;
                planWithProbabilities.Add(new DietPlanWithProbability(plan, accumulatedProbability));
            }

            // parents matching (crossover)
            for (var j = 0; j < NumPlansInGeneration; j++)
            {
                // probability might end at 99.99999... due to rounding errors, therefore fallback is provided
                DietPlan parent1 = null, parent2 = null;
                // avoid using parent, as it leads to elitist selection and domination of population by this parent
                while (parent1 == parent2)
                {
                    Double indexForParent1 = _randomNumberGenerator.NextDouble();
                    Double indexForParent2 = _randomNumberGenerator.NextDouble();

                    parent1 =
                        planWithProbabilities.FirstOrDefault(x => x.AccumulatedProbability > indexForParent1)
                            ?.DietPlan ?? planWithProbabilities.Last().DietPlan;
                    parent2 =
                        planWithProbabilities.FirstOrDefault(x => x.AccumulatedProbability > indexForParent2)
                            ?.DietPlan ??
                        planWithProbabilities.First().DietPlan; // first in fallback to avoid choosing parent1 again
                }

                DietPlan child = _crossoverHelper.GetChild(parent1, parent2);
                newGeneration.Add(child);
            }

            // mutation
            newGeneration = newGeneration.Select(x => _mutationHelper.GetMutationOf(x)).ToList();

            Debug.Assert(newGeneration.Count == NumPlansInGeneration);
            return newGeneration.OrderBy(x => x.ScoreToTarget).ToImmutableList();
        }


        private void LogGenerationsBestScore(Int32 generationNumber, Double score)
        {
            if (generationNumber % 100 == 0)
            {
                Console.WriteLine($"Thread {_threadNumber}, generation #{generationNumber}: best score is {score:0.00}");
            }
        }
    }
}