using System.Collections.Generic;
using Taurit.Toolkit.DietOptimization.Models;

namespace Taurit.Toolkit.FindOptimumDiet
{
    /// <summary>
    ///     Loads list of available products from external source 
    /// </summary>
    internal class ProductLoader
    {
        // mock, for debugging/testing purposes only
        internal IReadOnlyList<FoodProduct> GetSampleProductList()
        {
            var products = new List<FoodProduct>();
            products.Add(new FoodProduct("Fish oil", 900, 0, 100, 0));
            products.Add(new FoodProduct("Meat", 400, 100, 0, 0));
            products.Add(new FoodProduct("Potatoes", 400, 0, 0, 100));
            return products;
        }
    }
}