using System;
using System.IO;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Taurit.Toolkit.FileProcessors.ConversionProcessors
{
    public class ConvertToWebpProcessor : FileProcessorBase
    {
        [NotNull] private readonly IConversionStrategy _conversionStrategy;
        [NotNull] private readonly IConvertedFileNamingStrategy _convertedFileNamingStrategy;
        [NotNull] private readonly Regex _fileMatchRegex;
        [NotNull] private readonly ILoggingStrategy _loggingStrategy;

        /// <summary>
        ///     Safety mechanism. If result file is too small (0 bytes? 500 bytes?) it is likely that conversion went wrong or
        ///     chosen quality was too low, and
        ///     we might prefer to preserve original file for manual review.
        /// </summary>
        private readonly Int32 _preserveOriginalThresholdBytes;

        [NotNull] private readonly WebpFileQuality _quality;

        public ConvertToWebpProcessor([NotNull] [RegexPattern] String pattern,
            [NotNull] WebpFileQuality quality,
            Int32 preserveOriginalThresholdBytes,
            [NotNull] IConvertedFileNamingStrategy convertedFileNamingStrategy,
            [NotNull] IConversionStrategy conversionStrategy,
            [NotNull] ILoggingStrategy loggingStrategy)
        {
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));

            _quality = quality ?? throw new ArgumentNullException(nameof(quality));
            _preserveOriginalThresholdBytes = preserveOriginalThresholdBytes;
            _convertedFileNamingStrategy = convertedFileNamingStrategy ??
                                           throw new ArgumentNullException(nameof(convertedFileNamingStrategy));
            _conversionStrategy = conversionStrategy ?? throw new ArgumentNullException(nameof(conversionStrategy));
            _loggingStrategy = loggingStrategy;
            _fileMatchRegex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        /// <inheritdoc />
        public override void ProcessMatchingFile(String filePath)
        {
            if (!_fileMatchRegex.Match(filePath).Success) return;

            var fileInfo = new FileInfo(filePath);
            Int64 originalFileSize = fileInfo.Length;

            String webPPath = _convertedFileNamingStrategy.GetConvertedFilePath(filePath);

            // do not replace existing converted files.
            // assume that the output file was manually created and is expected not to be replaced.
            if (!File.Exists(webPPath))
            {
                _loggingStrategy.LogAction($"Converting {fileInfo.Name} to WebP ({_quality.QualityNumeric}%)");
                ImageMagickWrapper.ConvertToWebp(filePath, webPPath, _quality, _conversionStrategy);

                Int64 compressedFileSize = new FileInfo(webPPath).Length;
                // try convert with a slightly higher quality
                if (compressedFileSize < _preserveOriginalThresholdBytes)
                {
                    WebpFileQuality betterQuality = _quality.GetSlightlyBetterQuality();
                    _loggingStrategy.LogAction($"Converting {fileInfo.Name} to WebP ({betterQuality.QualityNumeric}%)");
                    ImageMagickWrapper.ConvertToWebp(filePath, webPPath, betterQuality, _conversionStrategy);
                }

                Int64 convertedFileSize = new FileInfo(webPPath).Length;
                if (File.Exists(webPPath) && convertedFileSize < originalFileSize &&
                    convertedFileSize > _preserveOriginalThresholdBytes)
                {
                    _loggingStrategy.LogAction($"Deleting {fileInfo.Name}");
                    File.Delete(filePath);
                }
            }
            else
            {
                _loggingStrategy.LogSuggestion(
                    $"Manual action required: ${webPPath} already exists, remove or rename source file");
            }
        }
    }
}