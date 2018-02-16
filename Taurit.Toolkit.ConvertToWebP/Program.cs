using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using JetBrains.Annotations;

namespace Taurit.Toolkit.ConvertToWebP
{
    internal class Program
    {
        private static void Main(String[] args)
        {
            if (args.Length != 3)
            {
                DisplayHelp();

                Console.WriteLine("Invalid number of arguments. Exiting.");
                Console.ReadKey();
                return;
            }

            Int32 quality =
                Convert.ToInt32(
                    args[0]); // 32 seems minimum, for documents that only need to retain readability, but not print quality
            Boolean removeOriginals = args[1] == "true";
            String directoryOrFilePath = args[2];
            Boolean isDirectory = Directory.Exists(directoryOrFilePath);
            Boolean isFile = File.Exists(directoryOrFilePath);
            Contract.Assert(!(isDirectory && isFile));
            if (!isDirectory && !isFile)
            {
                Console.WriteLine("Specified path doesn't exist in a filesystem. Exiting.");
                return;
            }
            String pathType = isDirectory ? "directoryOrFilePath" : "file";

            Console.WriteLine("The following arguments were found:");
            Console.WriteLine($"[0] quality: {quality}");
            Console.WriteLine($"[1] remove originals?: {removeOriginals}");
            Console.WriteLine($"[2] path to convert: {directoryOrFilePath} (type: {pathType})");

            //Console.WriteLine("Do you want to continue? (y/n)");
            //string answer = Console.ReadLine();
            //if (answer != "y")
            //{
            //    Console.WriteLine("You chose not to continue. Exiting.");
            //    Console.ReadKey();
            //    return;
            //}

            List<String> filesInDirectory = GetFilesInDirectory(directoryOrFilePath);
            ConvertFiles(filesInDirectory, removeOriginals, quality);
        }

        private static void DisplayHelp()
        {
            Console.WriteLine(
                @"TauritToolkit.ConvertToWebP
---------------------------
Converts images (jpg, jpeg, png) into WebP with a specified quality.

Remember, ImageMagick is required installed and available in PATH.

Arguments:
[0] quality (int). 1 is usually enough to retain readability of documents. 80-90 makes sense for high quality results.
[1] remove originals? if 'true' originals will be removed (only if the conversion succeeds)
[2] path to directoryOrFilePath with images to convert or path to a single image
");
        }

        private static void ConvertFiles([NotNull] List<String> filesInDirectory, Boolean removeOriginals,
            Int32 quality)
        {
            Contract.Assert(filesInDirectory != null);

            foreach (String filePath in filesInDirectory)
            {
                var fileInfo = new FileInfo(filePath);
                String extension = fileInfo.Extension.ToLowerInvariant();
                Boolean isConvertibleImageFormat = extension == ".jpg" || extension == ".jpeg" || extension == ".png";
                if (isConvertibleImageFormat)
                {
                    String webPPath = filePath.Substring(0, filePath.Length - extension.Length) + ".webp";
                    if (!File.Exists(webPPath)) // do not replace existing jp2 files
                    {
                        RunConvert(filePath, webPPath, quality);
                        Boolean conversionsResultSeemsReasonable = new FileInfo(webPPath).Length > 20000;
                        if (!conversionsResultSeemsReasonable)
                        {
                            RunConvert(filePath, webPPath, quality + 5);
                            Int64 convertedFileSize = new FileInfo(webPPath).Length;
                            Int64 originalFileSize = new FileInfo(filePath).Length;
                            conversionsResultSeemsReasonable = new FileInfo(webPPath).Length > 20000
                                                               && convertedFileSize < originalFileSize;
                        }

                        if (File.Exists(webPPath) // assuming success
                            &&
                            conversionsResultSeemsReasonable // arbitrary rules to avoid situations when compression was so big that the file lost its readability. This happens when image is bad quality, contrast is low and noise is high and conversion might produce very small, almost unreadable files
                            && removeOriginals)
                        {
                            File.Delete(filePath);
                        }
                    }
                }
            }
        }

        [JetBrains.Annotations.Pure]
        private static List<String> GetFilesInDirectory(String directoryOrFilePath)
        {
            if (File.Exists(directoryOrFilePath))
            {
                return new List<String> {directoryOrFilePath};
            }

            return Directory.GetFiles(directoryOrFilePath).ToList();
        }

        private static void RunConvert([NotNull] String inputFile, [NotNull] String outputFile,
            [AssertionCondition(AssertionConditionType.IS_TRUE)] Int32 quality)
        {
            Contract.Assert(inputFile != null);
            Contract.Assert(outputFile != null);
            Contract.Assert(quality > 0);

            // this requires imagemagick to be installed and added to windows path
            var magickProcess = new Process();
            magickProcess.StartInfo.FileName = "magick";
            //magickProcess.StartInfo.Arguments = $"convert \"{inputFile}\" -define jp2:quality={quality} \"{outputFile}\"";
            magickProcess.StartInfo.Arguments =
                $"convert \"{inputFile}\" -quality {quality} -define webp:lossless=false \"{outputFile}\"";
            // webpack at quality=1 proves better than jpeg at qualuty 32. Better readability with similar or better size.

            magickProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            magickProcess.Start();
            magickProcess.WaitForExit();
        }
    }
}