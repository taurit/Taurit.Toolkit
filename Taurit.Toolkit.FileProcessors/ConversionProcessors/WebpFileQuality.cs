using System;

namespace Taurit.Toolkit.FileProcessors.ConversionProcessors
{
    public class WebpFileQuality
    {
        /// <param name="qualityNumeric">
        ///     1-100. 1 is minimum quality (still quire readable eg for receipts, 80 looks just like
        ///     original and can be considered high quality). WebP at quality=1 proves better than jpeg at quality 32.
        /// </param>
        public WebpFileQuality(Int32 qualityNumeric)
        {
            if (qualityNumeric < 1 || qualityNumeric > 100)
                throw new ArgumentOutOfRangeException(nameof(qualityNumeric));

            QualityNumeric = qualityNumeric;
        }

        public Int32 QualityNumeric { get; }

        public WebpFileQuality GetSlightlyBetterQuality()
        {
            Int32 newQuality = Math.Min(QualityNumeric + 5, 100);
            return new WebpFileQuality(newQuality);
        }
    }
}