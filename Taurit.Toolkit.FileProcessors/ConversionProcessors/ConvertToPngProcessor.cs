using System;
using System.IO;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Taurit.Toolkit.FileProcessors.ConversionProcessors
{
    public class ConvertToPngProcessor : FileProcessorBase
    {
        [NotNull] private readonly IConversionStrategy _conversionStrategy;
        [NotNull] private readonly Regex _matchPattern;
        [NotNull] private readonly IConvertedFileNamingStrategy _namingStrategy;
        [NotNull] private readonly PngFileQuality _quality;

        public ConvertToPngProcessor([RegexPattern] [NotNull] String matchPattern,
            [NotNull] PngFileQuality quality,
            [NotNull] IConvertedFileNamingStrategy namingStrategy,
            [NotNull] IConversionStrategy conversionStrategy)
        {
            if (matchPattern == null)
                throw new ArgumentNullException(nameof(matchPattern));

            _matchPattern = new Regex(matchPattern, RegexOptions.Compiled);
            _quality = quality ?? throw new ArgumentNullException(nameof(quality));
            _namingStrategy = namingStrategy;
            _conversionStrategy = conversionStrategy;
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
                    Console.WriteLine($"Converting {fileInfo.Name} to Png ({_quality.QualityNumeric}%)");
                    ImageMagickWrapper.ConvertToPng(filePath, convertedFilePath, _quality, _conversionStrategy);
                }
            }
        }
    }
}