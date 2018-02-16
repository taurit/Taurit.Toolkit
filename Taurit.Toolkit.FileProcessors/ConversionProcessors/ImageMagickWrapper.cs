using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using JetBrains.Annotations;

namespace Taurit.Toolkit.FileProcessors.ConversionProcessors
{
    internal static class ImageMagickWrapper
    {
        internal static void ConvertToWebp([NotNull] String inputFile, [NotNull] String outputFile,
            WebpFileQuality quality)
        {
            Contract.Assert(inputFile != null);
            Contract.Assert(outputFile != null);

            // this requires ImageMagick to be installed and added to windows path
            var magickProcess = new Process();
            magickProcess.StartInfo.FileName = "magick";
            magickProcess.StartInfo.Arguments =
                $"convert \"{inputFile}\" -quality {quality.QualityNumeric} -define webp:lossless=false \"{outputFile}\"";

            magickProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            magickProcess.Start();
            magickProcess.WaitForExit();
        }
    }
}