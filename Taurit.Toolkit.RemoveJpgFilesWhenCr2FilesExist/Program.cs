using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Taurit.Toolkit.Commons;

namespace Taurit.Toolkit.RemoveJpgFilesWhenCr2FilesExist
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine(args[0]);

            var directoryPath = args[0];
            var filesInDirectory = DirectoryHelper.GetFilesInDirectory(directoryPath);

            RemoveRedundantFiles(filesInDirectory);
            Console.WriteLine("Finished.");
        }

        private static void RemoveRedundantFiles(IReadOnlyList<string> filesInDirectory)
        {
            var filesInDirectoryLower = new HashSet<string>(filesInDirectory.Select(x => x.ToLower()));

            foreach (var filePath in filesInDirectoryLower)
            {
                var fileInfo = new FileInfo(filePath);
                var extension = fileInfo.Extension.ToLowerInvariant();
                var isJpeg = extension == ".jpg" || extension == ".jpeg";
                if (isJpeg)
                {
                    var cr2Path = filePath.Substring(0, filePath.Length - extension.Length) + ".cr2";
                    if (filesInDirectoryLower.Contains(cr2Path))
                    {
                        Console.WriteLine($"Deleting redundant {filePath}");
                        File.Delete(filePath);
                    }
                }
            }
        }
    }
}