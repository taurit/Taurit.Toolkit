using System.Diagnostics;
using Newtonsoft.Json;
using Taurit.Toolkit.DietOptimization.Models;

namespace Taurit.Toolkit.FindOptimumDiet.Models
{
    /// <summary>
    ///     Wrapper, so JSON contains object and not array, which allows to specify "$schema" on it and get some IntelliSense
    ///     in JSON file.
    /// </summary>
    [JsonObject]
    [DebuggerDisplay("{" + nameof(Products) + ".Count}")]
    public sealed class OptimizationMetadataCollection
    {
        [JsonProperty]
        public OptimizationMetadata[] Products { get; set; }
    }
}