using System;
using System.Diagnostics;
using JetBrains.Annotations;
using Newtonsoft.Json;

// ReSharper disable NotNullMemberIsNotInitialized

namespace Taurit.Toolkit.DietOptimization.Models
{
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
        public Int32? FixedAmountG { get; set; }
    }
}