using System;
using System.Diagnostics;
using JetBrains.Annotations;
// ReSharper disable MemberCanBePrivate.Global - deserializer uses setters.
// ReSharper disable NotNullMemberIsNotInitialized

namespace Taurit.Toolkit.DietOptimization.Models
{
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    // ReSharper disable once ClassNeverInstantiated.Global
    public class OptimizationMetadata
    {
        /// <summary>
        ///     Product name. It must match name of a product in USDA database, otherwise it will be ignored.
        /// </summary>
        [NotNull]
        public String Name { get; set; }

        /// <summary>
        ///     Price per kg in whatever currency user prefers to work with.
        /// </summary>
        public Double PricePerKg { get; set; }

        /// <summary>
        ///     If null, amount is an optimization variable. Otherwise, this particular value will be used in a diet plan.
        ///     This flag should not be overused, but it might be useful to force some tasty stuff into the diet (eg. small amounts
        ///     of chocolate) while it would not be necessary from purely optimizational standpoint.
        /// </summary>
        [CanBeNull]
        public Double? FixedNonNegotiableAmount { get; set; }
    }
}