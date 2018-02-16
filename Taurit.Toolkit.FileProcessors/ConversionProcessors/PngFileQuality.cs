using System;

namespace Taurit.Toolkit.FileProcessors.ConversionProcessors
{
    public class PngFileQuality
    {
        /// <param name="qualityNumeric">
        ///     100 is recommended, setting it lower actually increases file size in tested cases
        /// </param>
        public PngFileQuality(Int32 qualityNumeric)
        {
            if (qualityNumeric < 1 || qualityNumeric > 100)
                throw new ArgumentOutOfRangeException(nameof(qualityNumeric));

            QualityNumeric = qualityNumeric;
        }

        public Int32 QualityNumeric { get; }
    }
}