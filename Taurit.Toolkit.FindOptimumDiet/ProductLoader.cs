﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AutoMapper;
using CsvHelper;
using JetBrains.Annotations;
using Newtonsoft.Json;
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

        private IReadOnlyCollection<FoodProduct> GetProductsFromUsdaDatabase(
            [NotNull] IImmutableList<OptimizationMetadata> productsToConsider)
        {
            var csv = new CsvReader(File.OpenText("usda-product-database.csv"));
            Dictionary<String, UsdaProduct> usdaProductNameToProduct =
                csv.GetRecords<UsdaProduct>()
                    .GroupBy(p => p.Name, StringComparer.OrdinalIgnoreCase) // duplicates happen in data, trick to skip them
                    .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            var products = new List<FoodProduct>(productsToConsider.Count);
            foreach (OptimizationMetadata product in productsToConsider)
                try
                {
                    if (!usdaProductNameToProduct.ContainsKey(product.Name))
                    {
                        continue;
                    }

                    UsdaProduct usdaProduct = usdaProductNameToProduct[product.Name];
                    var foodProduct = _mapper.Map<FoodProduct>(usdaProduct);
                    Debug.Assert(foodProduct != null,
                        "Probably some change in FoodProduct's constructor parameters was not reflected in mappings in AutoMapperModule.");

                    foodProduct.Metadata = product;
                    products.Add(foodProduct);
                }
                catch (FormatException)
                {
                    // most likely some numeric data in null/empty in source data.
                    // currently I want to just skip such records, as optimization doesn't make sense when the value of any required parameters in unknown
                    Console.WriteLine($"Product {product.Name} had to be skipped due to invalid data.");
                }
            return products;
        }

        [NotNull]
        public IReadOnlyCollection<FoodProduct> GetProducts()
        {
            IImmutableList<OptimizationMetadata> productsToConsider =
                GetProductsMetadata("usda-product-database-metadata.json");
            return GetProductsFromUsdaDatabase(productsToConsider);
        }

        [NotNull]
        private IImmutableList<OptimizationMetadata> GetProductsMetadata([NotNull] String productMetadataFileName)
        {
            Debug.Assert(File.Exists(productMetadataFileName), "Metadata file does not exist");
            String json = File.ReadAllText(productMetadataFileName);

            var productsMetadata = JsonConvert.DeserializeObject<OptimizationMetadataCollection>(json);

            Debug.Assert(productsMetadata != null, "Deserialization failed");
            Debug.Assert(productsMetadata.Products != null, "Deserialization of product list failed");
            return productsMetadata.Products.ToImmutableList();
        }
    }
}