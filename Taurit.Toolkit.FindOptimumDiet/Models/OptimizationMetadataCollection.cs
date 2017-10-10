using System;
using System.Diagnostics;
using Taurit.Toolkit.DietOptimization.Models;
// ReSharper disable MemberCanBePrivate.Global - deserializer uses setters

namespace Taurit.Toolkit.FindOptimumDiet.Models
{
    /// <summary>
    ///     Wrapper, so JSON contains object and not array, which allows to specify "$schema" on it and get some IntelliSense
    ///     in JSON file.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("{" + nameof(Products) + ".Count}")]
    public sealed class OptimizationMetadataCollection
    {
        public OptimizationMetadata[] Products { get; set; }
    }
}