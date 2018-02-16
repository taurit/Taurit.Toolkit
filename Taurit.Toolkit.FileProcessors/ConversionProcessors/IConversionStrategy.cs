using System;

namespace Taurit.Toolkit.FileProcessors.ConversionProcessors
{
    public interface IConversionStrategy
    {
        String GetAdditionalImageMagickArguments();
    }
}