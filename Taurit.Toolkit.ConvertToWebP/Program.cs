using System;
using System.Diagnostics.Contracts;
using System.IO;
using JetBrains.Annotations;
using Taurit.Toolkit.FileProcessors;
using Taurit.Toolkit.FileProcessors.ConversionProcessors;

namespace Taurit.Toolkit.ConvertToWebP
{
    internal static class Program
    {
        private static void Main([NotNull] String[] args)
        {
            if (args.Length != 2)
            {
                Program.DisplayHelp();

                Console.WriteLine("Invalid number of arguments. Exiting.");
                Console.ReadKey();
                return;
            }

            var quality = Convert.ToInt32(args[0]);
            String directoryOrFilePath = args[1];
            Boolean isDirectory = Directory.Exists(directoryOrFilePath);
            Boolean isFile = File.Exists(directoryOrFilePath);
            Contract.Assert(!(isDirectory && isFile));
            if (!isDirectory && !isFile)
            {
                Console.WriteLine("Specified path doesn't exist in a filesystem. Exiting.");
                return;
            }

            var conversionConfiguration = new IFileProcessor[]
            {
                new ConvertToWebpProcessor(".(jpg|png|jpeg)$", new WebpFileQuality(quality), 20000,
                    new ChangeExtensionStrategy("webp"), new EmptyConversionStrategy(),
                    new ConsoleLoggingStrategy(ConsoleLoggingStrategy.LogLevel.ActionsAndSuggestions))
            };

            IConversionSource filesSource =
                ConversionSourceFactory.GetConversionSource(directoryOrFilePath, conversionConfiguration);
            filesSource.Process();
        }

        private static void DisplayHelp()
        {
            Console.WriteLine(
                @"This tool converts images (jpg, jpeg, png) into WebP with a specified quality. ImageMagick is required.

Arguments:
[0] quality (int, 1-100). 1 is usually enough to retain readability of documents. 80-90 is good for ideal quality photos.
[1] path to the directory to convert or to a single image
");
        }
    }
}