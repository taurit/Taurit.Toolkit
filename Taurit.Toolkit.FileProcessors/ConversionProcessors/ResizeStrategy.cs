using System;
using System.Diagnostics;

namespace Taurit.Toolkit.FileProcessors.ConversionProcessors
{
    public class ResizeStrategy : IConversionStrategy
    {
        private readonly Int32 _width;
        private readonly Int32 _height;

        public ResizeStrategy(Int32 width, Int32 height)
        {
            Debug.Assert(width > 0);
            Debug.Assert(height > 0);

            _width = width;
            _height = height;
        }

        /// <inheritdoc />
        public String GetAdditionalImageMagickArguments()
        {
            return $"-resize {_width}x{_height}";
        }
    }
}