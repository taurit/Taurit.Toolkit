using System;
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
    ///     Loads list of available products from external files:
    ///     * usda-product-database.csv - contents of USDA SR26 database, denormalized into single table containing all
    ///     basic product nutrient information
    ///     * usda-product-database-metadata.json - file where products from the above database can be selected for
    ///     optimization and decorated with additional metadata, like price, maximum daily amount or minumum daily amount.
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
                csv.GetRecords<UsdaProduct>().ToDictionary(x => x.Name, y => y);

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
            IReadOnlyCollection<FoodProduct> productsFromUsdaDatabase = GetProductsFromUsdaDatabase(productsToConsider);
            List<FoodProduct> productFromBeyondUsdaDatbase = GetProductsThatDontExistInUsdaDatabase();
            return productsFromUsdaDatabase.Union(productFromBeyondUsdaDatbase).ToImmutableList();
        }

        /// <remarks>
        ///     It will be convenient to have it moved to json when optimization variable set gets more stable
        ///     currently it's easier to update strongly typed constructor call, as compiler instantly shows the problem
        /// </remarks>
        private static List<FoodProduct> GetProductsThatDontExistInUsdaDatabase()
        {
            FoodProduct kfdProteinSupplement = AddProductNonexistentInUsdaDatabase(new FoodProduct("KFD premium WPC 80",
                415, 79, 7, 9, 0, 0, 0, 0, 0, 0, 0, 0, 581.37, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0), 60, 2 * 40, 40, "recommended portion");
            FoodProduct kfdProteinDessertSupplement = AddProductNonexistentInUsdaDatabase(new FoodProduct(
                "KFD Protein Dessert",
                347, 80, 1.3, 3.7, 0, 0, 0, 0, 0, 0, 0, 0, 500, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0), 60, 40, 40, "recommended portion");
            FoodProduct kfdZincSupplement = AddProductNonexistentInUsdaDatabase(new FoodProduct("KFD Zinc tabs",
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 15, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0), 10, 1, 1, "pill");
            // https://sklep.kfd.pl/scitec-calcium-magnesium-100tabs-p-6338.html
            FoodProduct calciumMagnesiumSupplement = AddProductNonexistentInUsdaDatabase(new FoodProduct(
                "Calcium + Magnesium supplement",
                0, 0, 0, 0, 0, 0, 0, 0, 500, 250, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0), 29, 1, 1, "pill");
            var productFromBeyondUsdaDatbase = new List<FoodProduct>
            {
                kfdProteinSupplement,
                kfdProteinDessertSupplement,
                kfdZincSupplement,
                calciumMagnesiumSupplement
            };
            return productFromBeyondUsdaDatbase;
        }

        private static FoodProduct AddProductNonexistentInUsdaDatabase(FoodProduct product, Double pricePerKg,
            Double maxAmountG, Double oneItemWeight, string oneItemDescription)
        {
            product.Metadata =
                new OptimizationMetadata
                {
                    Name = product.Name,
                    PricePerKg = pricePerKg,
                    MaxAmountG = maxAmountG,
                    OneItemWeight = oneItemWeight,
                    OneItemDescription = oneItemDescription
                };
            return product;
        }

        [NotNull]
        private IImmutableList<OptimizationMetadata> GetProductsMetadata([NotNull] String productMetadataFileName)
        {
            Debug.Assert(File.Exists(productMetadataFileName), "Metadata file does not exist");
            String json = File.ReadAllText(productMetadataFileName);

            var productsMetadata = JsonConvert.DeserializeObject<OptimizationMetadataCollection>(json);

            Debug.Assert(productsMetadata != null, "Deserialization failed");
            Debug.Assert(productsMetadata.Products != null, "Deserialization of product list failed");
            Debug.Assert(productsMetadata.Products.All(x => !(x.FixedAmountG != null && x.MinAmountG != null)));
            Debug.Assert(productsMetadata.Products.All(x => !(x.FixedAmountG != null && x.MaxAmountG != null)));
            Debug.Assert(productsMetadata.Products.All(
                x => !(x.MinAmountG != null && x.MaxAmountG != null && x.MinAmountG >= x.MaxAmountG)));


            return productsMetadata.Products.ToImmutableList();
        }
    }
}