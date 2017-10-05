using System;
using Ninject;
using Taurit.Toolkit.DietOptimization.DietOptimizers;
using Taurit.Toolkit.DietOptimization.Models;
using Taurit.Toolkit.DietOptimization.Services;
using Taurit.Toolkit.FindOptimumDiet.Mappings;

namespace Taurit.Toolkit.FindOptimumDiet
{
    internal sealed class FindOptimumDiet
    {
        private readonly DietCharacteristicsCalculator _dietCharacteristicsCalculator;
        private readonly DietCharacteristicsDistanceCalculator _dietCharacteristicsDistanceCalculator;
        private readonly DietPlanPresenter _dietPlanPresenter;
        private readonly ProductLoader _productLoader;

        public FindOptimumDiet(DietCharacteristicsCalculator dietCharacteristicsCalculator,
            DietCharacteristicsDistanceCalculator dietCharacteristicsDistanceCalculator,
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
            var products = _productLoader.GetProductsFromUsdaDatabase();

            // specify target for the optimum diet
            var targetDietCharacteristics = new DietCharacteristics(3000, 203, 100, 323);
            var target = new DietConstraints(targetDietCharacteristics);

            // find suboptimal diet (as close to a target as feasible)
            IDietOptimizer dietOptimizer = new GeneticAlgorithmDietOptimizer(_dietCharacteristicsCalculator,
                _dietCharacteristicsDistanceCalculator, target);
            var optimumDiet = dietOptimizer.Optimize(products);

            // display result
            _dietPlanPresenter.Display(optimumDiet);
            Console.ReadLine();
        }
    }
}