using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Ninject;
using Taurit.Toolkit.DietOptimization.DietOptimizers;
using Taurit.Toolkit.DietOptimization.Models;
using Taurit.Toolkit.DietOptimization.Services;
using Taurit.Toolkit.FindOptimumDiet.Mappings;

namespace Taurit.Toolkit.FindOptimumDiet
{
    
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "It is instantiated by Ninject")]
    internal sealed class FindOptimumDiet
    {
        private const Int32 NumOptimizationThreads = 2;
        private readonly DietCharacteristicsCalculator _dietCharacteristicsCalculator;
        private readonly ScoreCalculator _dietCharacteristicsDistanceCalculator;
        private readonly DietPlanPresenter _dietPlanPresenter;
        private readonly ProductLoader _productLoader;

        public FindOptimumDiet(DietCharacteristicsCalculator dietCharacteristicsCalculator,
            ScoreCalculator dietCharacteristicsDistanceCalculator,
            DietPlanPresenter dietPlanPresenter, ProductLoader productLoader)
        {
            _dietCharacteristicsCalculator = dietCharacteristicsCalculator;
            _dietCharacteristicsDistanceCalculator = dietCharacteristicsDistanceCalculator;
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
                3000, // 3000 kcal
                30, // 30 PLN a day = 900 PLN/month average
                203, // protein - target for building muscle
                100, // fat - a bit higher than recommended for diet maintainability
                323 //, // carbs for the rest of calories
            );

            // find suboptimal diet (as close to a target as feasible)
            var optimizationTasks = new List<Task<DietPlan>>(NumOptimizationThreads);
            for (var i = 0; i < NumOptimizationThreads; i++)
            {
                Task<DietPlan> optimumDiet = Task.Run(() => OptimizeDiet(products, dietTargets));
                optimizationTasks.Add(optimumDiet);
            }

            Task.WaitAll(optimizationTasks.Cast<Task>().ToArray(), new TimeSpan(0, 0, 1, 0));

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

        private DietPlan OptimizeDiet(IReadOnlyCollection<FoodProduct> products, DietTarget target)
        {
            IDietOptimizer dietOptimizer = new GeneticAlgorithmDietOptimizer(_dietCharacteristicsCalculator,
                _dietCharacteristicsDistanceCalculator, target);
            DietPlan optimumDiet = dietOptimizer.Optimize(products);

            return optimumDiet;
        }
    }
}