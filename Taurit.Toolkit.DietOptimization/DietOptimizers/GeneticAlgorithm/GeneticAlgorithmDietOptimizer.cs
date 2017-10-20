using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using Taurit.Toolkit.DietOptimization.Models;
using Taurit.Toolkit.DietOptimization.Services;

namespace Taurit.Toolkit.DietOptimization.DietOptimizers.GeneticAlgorithm
{
    /// <summary>
    ///     Allows to find the diet that is closest to user-provided optimization target.
    ///     Uses variation of Genetic Algorithm. I'm no expert in evolutionary algorithm so the implementation might be bad,
    ///     but it usually converges and does it rather quickly when a scale is ~50 products.
    /// </summary>
    public class GeneticAlgorithmDietOptimizer : IDietOptimizer
    {
        /// <summary>
        ///     How many diet plans a single generation of genetic algorithm contains?
        /// </summary>
        private const Int32 NumPlansInGeneration = 500;

        /// <summary>
        ///     How many generations are created/analyzed by single run of <see cref="Optimize" />?
        /// </summary>
        private const Int32 MaxNumGenerations = 1_000;

        private const Int32 AcceptableScore = 10;
        [NotNull] private readonly CrossoverHelper _crossoverHelper;

        [NotNull] private readonly MutationHelper _mutationHelper;

        private readonly Random _randomNumberGenerator;

        [NotNull] private readonly StartPointProvider _startPointProvider;
        private readonly Int32 _threadNumber;

        [CanBeNull] private DietPlan _bestPlanSoFar;

        public GeneticAlgorithmDietOptimizer([NotNull] DietCharacteristicsCalculator dietCharacteristicsCalculator,
            [NotNull] ScoreCalculator scoreCalculator, [NotNull] DietTarget dietTarget, Int32 threadNumber)
        {
            _crossoverHelper = new CrossoverHelper(dietCharacteristicsCalculator, scoreCalculator, dietTarget);
            _startPointProvider = new StartPointProvider(dietCharacteristicsCalculator, scoreCalculator, dietTarget);
            _mutationHelper = new MutationHelper(dietCharacteristicsCalculator, scoreCalculator, dietTarget);
            _threadNumber = threadNumber;
            _randomNumberGenerator = new Random(threadNumber); // for repeatable, but different results for each thread
        }

        public DietPlan Optimize(IReadOnlyCollection<FoodProduct> availableProducts, CancellationToken cancellationToken)
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
                if (bestInGeneration.ScoreToTarget < AcceptableScore || cancellationToken.IsCancellationRequested)
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

        /// <summary>
        ///     Experimental selection method
        ///     Source: http://aragorn.pb.bialystok.pl/~wkwedlo/EA2.pdf
        ///     also: http://www.k0pper.republika.pl/geny.htm
        /// </summary>
        private ImmutableList<DietPlan> CreateNextGeneration(ImmutableList<DietPlan> currentGeneration)
        {
            var newGeneration = new List<DietPlan>(NumPlansInGeneration);

            Double sumOfInvertedScores = currentGeneration.Sum(x => x.InvertedScoreToTarget);

            // select parents
            var planWithProbabilities = new List<DietPlanWithProbability>(currentGeneration.Count);
            var accumulatedProbability = 0d;
            foreach (DietPlan plan in currentGeneration)
            {
                // w maksymalizacji: Przystosowanie / Suma przystosowania wszystkich osobników

                Double probability = plan.InvertedScoreToTarget / sumOfInvertedScores; // for minimization
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
            if (generationNumber % 50 == 0)
            {
                Console.WriteLine(
                    $"Thread {_threadNumber}, generation #{generationNumber}: best score is {score:0.00}");
            }
        }
    }
}