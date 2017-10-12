using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Taurit.Toolkit.DietOptimization.Models;
using Taurit.Toolkit.DietOptimization.Services;

namespace Taurit.Toolkit.DietOptimization.DietOptimizers.GeneticAlgorithm
{
    public class CrossoverHelper
    {
        private readonly DietCharacteristicsCalculator _dietCharacteristicsCalculator;
        private readonly DietTarget _dietTarget;
        private readonly Random _randomNumberGenerator = new Random(123);
        private readonly ScoreCalculator _scoreCalculator;

        public CrossoverHelper(DietCharacteristicsCalculator dietCharacteristicsCalculator,
            ScoreCalculator scoreCalculator, DietTarget dietTarget)
        {
            _dietCharacteristicsCalculator = dietCharacteristicsCalculator;
            _scoreCalculator = scoreCalculator;
            _dietTarget = dietTarget;
        }

        public DietPlan GetChild(DietPlan parent1, DietPlan parent2)
        {
            Debug.Assert(parent1.DietPlanItems.Count == parent2.DietPlanItems.Count);

            Int32 numItems = parent1.DietPlanItems.Count;


            // single point crossover
            Int32 crossoverIndex = _randomNumberGenerator.Next(numItems);
            List<DietPlanItem> childItems = parent1.DietPlanItems.Take(crossoverIndex)
                .Union(parent2.DietPlanItems.Skip(crossoverIndex).Take(numItems - crossoverIndex)).ToList();

            // each value radomized individually
            //var childItems = new List<DietPlanItem>(numItems);
            //for (var i = 0; i < numItems; i++)
            //{
            //    DietPlanItem itemToAdd =
            //        SelectRandomlyOneOfTwoPlanItems(parent1.DietPlanItems[i], parent2.DietPlanItems[i]);
            //    childItems.Add(itemToAdd);
            //}

            Debug.Assert(childItems.Count == parent1.DietPlanItems.Count);

            DietCharacteristics dietCharacteristics = _dietCharacteristicsCalculator.GetCharacteristics(childItems);
            Double scoreToTarget = _scoreCalculator.CalculateScore(dietCharacteristics, _dietTarget);
            return new DietPlan(childItems, dietCharacteristics, scoreToTarget);
        }

        private DietPlanItem SelectRandomlyOneOfTwoPlanItems(DietPlanItem item1, DietPlanItem item2)
        {
            return _randomNumberGenerator.NextDouble() < 0.4
                ? item1
                : item2;
        }
    }
}