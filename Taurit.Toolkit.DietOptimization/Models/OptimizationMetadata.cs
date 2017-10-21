﻿using System;
using System.Diagnostics;
using JetBrains.Annotations;
using Newtonsoft.Json;

// ReSharper disable NotNullMemberIsNotInitialized

namespace Taurit.Toolkit.DietOptimization.Models
{
    /// <summary>
    ///     User-provided optimization preferences related to a specific product.
    ///     While data in <see cref="FoodProduct" /> is rather universal and should not change in time,
    ///     <see cref="OptimizationMetadata" />  represents more of a users preference. Eg.:
    ///     * how much of a product does user want in a diet (minimum, maximum, fixed amount)?
    ///     * what is the local price for this product in user's location?
    /// </summary>
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    [JsonObject]
    public class OptimizationMetadata
    {
        /// <summary>
        ///     Product name. It must match name of a product in USDA database, otherwise it will be ignored.
        /// </summary>
        [NotNull]
        [JsonProperty]
        public String Name { get; set; }

        /// <summary>
        ///     Price per kg in whatever currency user prefers to work with.
        /// </summary>
        [JsonProperty]
        public Double PricePerKg { get; set; }

        /// <summary>
        ///     If null, amount is an optimization variable. Otherwise, this particular value will be used in a diet plan.
        ///     This flag should not be overused, but it might be useful to force some tasty stuff into the diet (eg. small amounts
        ///     of chocolate) while it would not be necessary from purely optimizational standpoint.
        /// </summary>
        [CanBeNull]
        [JsonProperty]
        public Double? FixedAmountG { get; set; }

        [CanBeNull]
        [JsonProperty]
        public Double? MinAmountG { get; set; }

        [CanBeNull]
        [JsonProperty]
        public Double? MaxAmountG { get; set; }

        /// <summary>
        ///     Hint for optimizer that defines start amount for this product.
        ///     This amount is likely to change during optimization, but will be used as a starting point in all plans in first
        ///     generation.
        /// </summary>
        [CanBeNull]
        [JsonProperty]
        public Double? StartAmountG { get; set; }

        [CanBeNull]
        [JsonProperty]
        public Double? OneItemWeight { get; set; }

        [CanBeNull]
        [JsonProperty]
        public String OneItemDescription { get; set; }
    }
}