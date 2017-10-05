﻿using System;
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

        // mock, for debugging/testing purposes only
        internal IReadOnlyList<FoodProduct> GetSampleProductList()
        {
            var products = new List<FoodProduct>();
            products.Add(new FoodProduct("Fish oil", 900, 0, 100, 0));
            products.Add(new FoodProduct("Meat", 400, 100, 0, 0));
            products.Add(new FoodProduct("Potatoes", 400, 0, 0, 100));
            return products;
        }

        public IReadOnlyCollection<FoodProduct> GetProductsFromUsdaDatabase(IImmutableSet<string> productNames)
        {
            var csv = new CsvReader(File.OpenText("usda-product-database.csv"));
            var usdaProducts = csv.GetRecords<UsdaProduct>().ToList();

            
            var filteredProducts = usdaProducts.Where(p => productNames.Contains(p.Name)).ToList();
            var products = new List<FoodProduct>(filteredProducts.Count);
            foreach (var usdaProduct in filteredProducts)
            {
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
                }
            }
            return products;
        }
    }
}