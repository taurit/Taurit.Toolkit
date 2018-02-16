using System;

namespace Taurit.Toolkit.FileProcessors.ConversionProcessors
{
    public class EmptyConversionStrategy : IConversionStrategy
    {
        /// <inheritdoc />
        public String GetAdditionalImageMagickArguments()
        {
            return String.Empty;
        }
    }
}