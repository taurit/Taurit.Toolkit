using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Taurit.Toolkit.FileProcessors.ConversionProcessors
{
    public class ConvertToWebpProcessor : IFileProcessor
    {
        [NotNull] private readonly IConvertedFileNamingStrategy _convertedFileNamingStrategy;
        [NotNull] private readonly Regex _fileMatchRegex;

        /// <summary>
        ///     Safety mechanism. If result file is too small (0 bytes? 500 bytes?) it is likely that conversion went wrong or
        ///     chosen quality was too low, and
        ///     we might prefer to preserve original file for manual review.
        /// </summary>
        private readonly Int32 _preserveOriginalThresholdBytes;

        [NotNull] private readonly WebpFileQuality _quality;

        public ConvertToWebpProcessor([NotNull] [RegexPattern] String pattern, [NotNull] WebpFileQuality quality,
            Int32 preserveOriginalThresholdBytes, [NotNull] IConvertedFileNamingStrategy convertedFileNamingStrategy)
        {
            if (pattern == null)
            {
                throw new ArgumentNullException(nameof(pattern));
            }

            _quality = quality ?? throw new ArgumentNullException(nameof(quality));
            _preserveOriginalThresholdBytes = preserveOriginalThresholdBytes;
            _convertedFileNamingStrategy = convertedFileNamingStrategy ??
                                           throw new ArgumentNullException(nameof(convertedFileNamingStrategy));
            _fileMatchRegex = new Regex(pattern, RegexOptions.Compiled);
        }

        /// <inheritdoc />
        public void ProcessMatchingFiles(String directoryPath)
        {
            Contract.Assert(Directory.Exists(directoryPath), "Inbox directory must exist to enumerate files");

            String[] filesInDirectory = Directory.GetFiles(directoryPath);
            foreach (String filePath in filesInDirectory)
            {
                String fileName = Path.GetFileName(filePath);
                Debug.Assert(fileName != null);

                if (_fileMatchRegex.Match(fileName).Success)
                {
                    ConvertSingleFile(filePath);
                }
            }
        }

        private void ConvertSingleFile(String filePath)
        {
            var fileInfo = new FileInfo(filePath);
            Int64 originalFileSize = fileInfo.Length;

            String webPPath = _convertedFileNamingStrategy.GetConvertedFilePath(filePath);

            // do not replace existing converted files.
            // assume that the output file was manually created and is expected not to be replaced.
            if (!File.Exists(webPPath))
            {
                Console.WriteLine($"Converting {fileInfo.Name} to WebP({_quality.QualityNumeric}%)");
                ImageMagickWrapper.ConvertToWebp(filePath, webPPath, _quality);

                Int64 compressedFileSize = new FileInfo(webPPath).Length;
                // try convert with a slightly higher quality
                if (compressedFileSize < _preserveOriginalThresholdBytes)
                {
                    var betterQuality = _quality.GetSlightlyBetterQuality();
                    Console.WriteLine($"Converting {fileInfo.Name} to WebP({betterQuality.QualityNumeric}%)");
                    ImageMagickWrapper.ConvertToWebp(filePath, webPPath, betterQuality);
                }

                Int64 convertedFileSize = new FileInfo(webPPath).Length;
                if (File.Exists(webPPath) && convertedFileSize < originalFileSize &&
                    convertedFileSize > _preserveOriginalThresholdBytes)
                {
                    Console.WriteLine($"Deleting {fileInfo.Name}");
                    File.Delete(filePath);
                }
            }
        }
    }
}