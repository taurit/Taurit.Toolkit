using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Ninject;
using Taurit.Toolkit.DietOptimization.DietOptimizers;
using Taurit.Toolkit.DietOptimization.DietOptimizers.GeneticAlgorithm;
using Taurit.Toolkit.DietOptimization.Models;
using Taurit.Toolkit.DietOptimization.Services;
using Taurit.Toolkit.FindOptimumDiet.Mappings;

namespace Taurit.Toolkit.FindOptimumDiet
{
    /// <summary>
    ///     Main program class. Defines workflow of the optimization process and presentation of results on a high level.
    /// </summary>
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "It is instantiated by Ninject")]
    internal sealed class FindOptimumDiet
    {
        private const Int32 NumOptimizationThreads = 1;
        [NotNull] private readonly DietCharacteristicsCalculator _dietCharacteristicsCalculator;
        [NotNull] private readonly DietPlanPresenter _dietPlanPresenter;
        [NotNull] private readonly ProductLoader _productLoader;
        [NotNull] private readonly ScoreCalculator _scoreCalculator;

        public FindOptimumDiet(DietCharacteristicsCalculator dietCharacteristicsCalculator,
            [NotNull] ScoreCalculator scoreCalculator,
            [NotNull] DietPlanPresenter dietPlanPresenter,
            [NotNull] ProductLoader productLoader)
        {
            _dietCharacteristicsCalculator = dietCharacteristicsCalculator;
            _scoreCalculator = scoreCalculator;
            _dietPlanPresenter = dietPlanPresenter;
            _productLoader = productLoader;
        }

        private static void Main()
        {
            var kernel = new StandardKernel();
            kernel.Load<AutoMapperModule>();
            kernel.Load<Bindings>();
            var program = kernel.Get<FindOptimumDiet>();

            program.Run();
        }

        /// <summary>
        ///     Finds optimum diet for a given constraints
        /// </summary>
        private void Run()
        {
            // get complete list of products that should be considered in a diet
            IReadOnlyCollection<FoodProduct> products = _productLoader.GetProducts();


            // specify target for the optimum diet
            var dietTargets = new DietTarget(
                2514, // kcal
                22, // 30 PLN a day = 900 PLN/month average
                200, // protein - target for building muscle
                57, // fat - a bit higher than recommended for diet maintainability
                300 // carbs for the rest of calories
            );

            // find suboptimal diet (as close to a target as feasible)
            var cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;
            var optimizationTasks = new List<Task<DietPlan>>(NumOptimizationThreads);
            for (var threadNumber = 0; threadNumber < NumOptimizationThreads; threadNumber++)
            {
                Int32 number = threadNumber;
                Task<DietPlan> optimumDiet =
                    Task.Run(() => OptimizeDiet(products, dietTargets, number, cancellationToken));
                optimizationTasks.Add(optimumDiet);
            }

            // allow manually cancel before stop conditions are true
            while (!Console.KeyAvailable && optimizationTasks.Cast<Task>().All(x => x.IsCompleted == false))
                Thread.Sleep(500);
            cancellationTokenSource.Cancel();

            Task.WaitAll(optimizationTasks.Cast<Task>().ToArray(), new TimeSpan(0, 0, 1, 0));
            // first NumOptimizationThreads lines are used to track progress of threads; print results below
            Console.SetCursorPosition(0,NumOptimizationThreads); 

            // display result
            DietPlan bestDietPlan = null;
            for (var index = 0; index < optimizationTasks.Count; index++)
            {
                Task<DietPlan> task = optimizationTasks[index];
                if (bestDietPlan == null)
                {
                    bestDietPlan = task.Result;
                }

                Console.WriteLine($"Final score for thread #{index}: {task.Result.ScoreToTarget}");
                if (task.Result.ScoreToTarget < bestDietPlan.ScoreToTarget)
                {
                    bestDietPlan = task.Result;
                }
            }

            _dietPlanPresenter.Display(bestDietPlan, dietTargets);
            Console.ReadLine();
        }

        private DietPlan OptimizeDiet(IReadOnlyCollection<FoodProduct> products, DietTarget target, Int32 threadNumber,
            CancellationToken cancellationToken)
        {
            Stopwatch optimizeTime = Stopwatch.StartNew();
            IDietOptimizer dietOptimizer = new GeneticAlgorithmDietOptimizer(_dietCharacteristicsCalculator,
                _scoreCalculator, target, threadNumber);
            DietPlan optimumDiet = dietOptimizer.Optimize(products, cancellationToken);

            optimizeTime.Stop();
            Console.WriteLine(
                $"Total optimization time for thread {threadNumber}: {optimizeTime.ElapsedMilliseconds} ms");

            return optimumDiet;
        }
    }
}