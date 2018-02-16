using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using JetBrains.Annotations;

namespace Taurit.Toolkit.FileProcessors.ConversionProcessors
{
    internal static class ImageMagickWrapper
    {
        internal static void ConvertToWebp([NotNull] String inputFile, [NotNull] String outputFile,
            [NotNull] WebpFileQuality quality, [NotNull] IConversionStrategy conversionStrategy)
        {
            Contract.Assert(inputFile != null);
            Contract.Assert(outputFile != null);
            Contract.Assert(quality != null);

            // this requires ImageMagick to be installed and added to windows path
            String additionalArguments = conversionStrategy.GetAdditionalImageMagickArguments();
            
            // this requires ImageMagick to be installed and added to windows path
            var magickProcess = new Process();
            magickProcess.StartInfo.FileName = "magick";
            magickProcess.StartInfo.Arguments =
                $"convert \"{inputFile}\" {additionalArguments} -quality {quality.QualityNumeric} -define webp:lossless=false \"{outputFile}\"";

            magickProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            magickProcess.Start();
            magickProcess.WaitForExit();
        }

        public static void ConvertToJpeg([NotNull] String inputFile, [NotNull] String outputFile,
            [NotNull] JpegFileQuality quality, [NotNull] IConversionStrategy conversionStrategy)
        {
            Contract.Assert(inputFile != null);
            Contract.Assert(outputFile != null);
            Contract.Assert(quality != null);

            // this requires ImageMagick to be installed and added to windows path
            String additionalArguments = conversionStrategy.GetAdditionalImageMagickArguments();

            var magickProcess = new Process();
            magickProcess.StartInfo.FileName = "magick";
            magickProcess.StartInfo.Arguments =
                $"convert \"{inputFile}\" {additionalArguments} -quality {quality.QualityNumeric}% \"{outputFile}\"";


            magickProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            magickProcess.Start();
            magickProcess.WaitForExit();
        }

        public static void ConvertToPng([NotNull] String inputFile, [NotNull] String outputFile,
            [NotNull] PngFileQuality quality, [NotNull] IConversionStrategy conversionStrategy)
        {
            Contract.Assert(inputFile != null);
            Contract.Assert(outputFile != null);
            Contract.Assert(quality != null);

            // this requires ImageMagick to be installed and added to windows path
            String additionalArguments = conversionStrategy.GetAdditionalImageMagickArguments();

            var magickProcess = new Process();
            magickProcess.StartInfo.FileName = "magick";
            magickProcess.StartInfo.Arguments =
                $"convert \"{inputFile}\" {additionalArguments} -quality {quality.QualityNumeric}% \"{outputFile}\"";
            
            magickProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            magickProcess.Start();
            magickProcess.WaitForExit();
        }
    }
}