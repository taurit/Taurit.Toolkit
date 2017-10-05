using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AutoMapper;
using CsvHelper;
using Taurit.Toolkit.DietOptimization.Models;
using Taurit.Toolkit.FindOptimumDiet.Models;

namespace Taurit.Toolkit.FindOptimumDiet
{
    /// <summary>
    ///     Loads list of available products from external source
    /// </summary>
    internal sealed class ProductLoader
    {
        private readonly IMapper _mapper;

        public ProductLoader(IMapper mapper)
        {
            _mapper = mapper;
        }

        public IReadOnlyCollection<FoodProduct> GetProductsFromUsdaDatabase(IImmutableSet<String> productNames)
        {
            var csv = new CsvReader(File.OpenText("usda-product-database.csv"));
            List<UsdaProduct> usdaProducts = csv.GetRecords<UsdaProduct>().ToList();


            List<UsdaProduct> filteredProducts = usdaProducts.Where(p => productNames.Contains(p.Name)).ToList();
            var products = new List<FoodProduct>(filteredProducts.Count);
            foreach (UsdaProduct usdaProduct in filteredProducts)
                try
                {
                    var foodProduct = _mapper.Map<FoodProduct>(usdaProduct);
                    Debug.Assert(foodProduct != null,
                        "Probably some change in FoodProduct's constructor parameters was not reflected in mappings in AutoMapperModule.");
                    products.Add(foodProduct);
                }
                catch (FormatException)
                {
                    // most likely some numeric data in null/empty in source data.
                    // currently I want to just skip such records, as optimization doesn't make sense when the value of any required parameters in unknown
                    Console.WriteLine($"Product {usdaProduct.Name} had to be skipped due to invalid data.");
                }
            return products;
        }
    }
}