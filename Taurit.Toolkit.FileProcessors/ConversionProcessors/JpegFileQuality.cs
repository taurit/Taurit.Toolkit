using System;

namespace Taurit.Toolkit.FileProcessors.ConversionProcessors
{
    public class JpegFileQuality
    {
        public JpegFileQuality(Int32 qualityNumeric)
        {
            if (qualityNumeric < 1 || qualityNumeric > 100)
                throw new ArgumentOutOfRangeException(nameof(qualityNumeric));

            QualityNumeric = qualityNumeric;
        }

        public Int32 QualityNumeric { get; }
    }
}