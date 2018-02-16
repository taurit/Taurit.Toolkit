using System;
using System.IO;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Taurit.Toolkit.FileProcessors.ConversionProcessors
{
    public class ConvertToJpegProcessor : FileProcessorBase
    {
        [NotNull] private readonly Regex _matchPattern;
        [NotNull] private readonly IConvertedFileNamingStrategy _namingStrategy;
        [NotNull] private readonly JpegFileQuality _quality;

        public ConvertToJpegProcessor([RegexPattern] [NotNull] String matchPattern, [NotNull] JpegFileQuality quality,
            [NotNull] IConvertedFileNamingStrategy namingStrategy)
        {
            if (matchPattern == null)
                throw new ArgumentNullException(nameof(matchPattern));

            _matchPattern = new Regex(matchPattern, RegexOptions.Compiled);
            _quality = quality ?? throw new ArgumentNullException(nameof(quality));
            _namingStrategy = namingStrategy;
        }

        /// <inheritdoc />
        public override void ProcessMatchingFile(String filePath)
        {
            if (_matchPattern.Match(filePath).Success)
            {
                var fileInfo = new FileInfo(filePath);
                String convertedFilePath = _namingStrategy.GetConvertedFilePath(filePath);

                // do not replace existing converted files
                if (!File.Exists(convertedFilePath))
                {
                    Console.WriteLine($"Converting {fileInfo.Name} to WebP({_quality.QualityNumeric}%)");
                    ImageMagickWrapper.ConvertToJpeg(filePath, convertedFilePath, _quality);
                }
            }
        }
    }
}