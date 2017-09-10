using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using JetBrains.Annotations;

namespace Taurit.Toolkit.ConvertToWebP
{
    class Program
    {
        static void Main(string[] args)
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

            if (args.Length != 3)
            {
                Console.WriteLine("Invalid number of arguments. Exiting.");
                Console.ReadKey();
                return;
            }

            var quality = Convert.ToInt32(args[0]); // 32 seems minimum, for documents that only need to retain readability, but not print quality
            bool removeOriginals = args[1] == "true";
            var directoryOrFilePath = args[2];
            bool isDirectory = Directory.Exists(directoryOrFilePath);
            bool isFile = File.Exists(directoryOrFilePath);
            Contract.Assert(!(isDirectory && isFile));
            if (!isDirectory && !isFile)
            {
                Console.WriteLine("Specified path doesn't exist in a filesystem. Exiting.");
                return;
            }
            string pathType = isDirectory ? "directoryOrFilePath" : "file";

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
            
            var filesInDirectory = GetFilesInDirectory(directoryOrFilePath);
            ConvertFiles(filesInDirectory, removeOriginals, quality);
        }

        private static void ConvertFiles([NotNull]List<string> filesInDirectory, bool removeOriginals, int quality)
        {
            Contract.Assert(filesInDirectory != null);

            foreach (string filePath in filesInDirectory)
            {
                var fileInfo = new FileInfo(filePath);
                string extension = fileInfo.Extension.ToLowerInvariant();
                var isConvertibleImageFormat = (extension == ".jpg" || extension == ".jpeg" || extension == ".png");
                if (isConvertibleImageFormat)
                {
                    var webPPath = filePath.Substring(0, filePath.Length - extension.Length) + ".webp";
                    if (!File.Exists(webPPath)) // do not replace existing jp2 files
                    {
                        RunConvert(filePath, webPPath, quality);
                        bool conversionsResultSeemsReasonable = (new FileInfo(webPPath).Length > 20000);
                        if (!conversionsResultSeemsReasonable)
                        {
                            RunConvert(filePath, webPPath, quality + 5);
                            var convertedFileSize = new FileInfo(webPPath).Length;
                            var originalFileSize = new FileInfo(filePath).Length;
                            conversionsResultSeemsReasonable = (new FileInfo(webPPath).Length > 20000)
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
        private static List<string> GetFilesInDirectory(string directoryOrFilePath)
        {
            if (File.Exists(directoryOrFilePath)) return new List<string>() {directoryOrFilePath};
            
            return Directory.GetFiles(directoryOrFilePath).ToList();
        }

        private static void RunConvert([NotNull] string inputFile, [NotNull] string outputFile, [AssertionCondition(AssertionConditionType.IS_TRUE)] int quality)
        {
            Contract.Assert(inputFile != null);
            Contract.Assert(outputFile != null);
            Contract.Assert(quality > 0);

            // this requires imagemagick to be installed and added to windows path
            Process magickProcess = new Process();
            magickProcess.StartInfo.FileName = "magick";
            //magickProcess.StartInfo.Arguments = $"convert \"{inputFile}\" -define jp2:quality={quality} \"{outputFile}\"";
            magickProcess.StartInfo.Arguments = $"convert \"{inputFile}\" -quality {quality} -define webp:lossless=false \"{outputFile}\"";
            // webpack at quality=1 proves better than jpeg at qualuty 32. Better readability with similar or better size.

            magickProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            magickProcess.Start();
            magickProcess.WaitForExit();
        }
    }
}