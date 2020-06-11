using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Taurit.Toolkit.RemoveJpgFilesWhenCr2FilesExist
{
    internal class Program
    {
        private static void Main(String[] args)
        {
            Console.WriteLine(args[0]);

            String directoryPath = args[0];
            IReadOnlyList<String> filesInDirectory = DirectoryHelper.GetFilesInDirectory(directoryPath);

            Program.RemoveRedundantFiles(filesInDirectory);
            Console.WriteLine("Finished.");
        }

        private static void RemoveRedundantFiles(IReadOnlyList<String> filesInDirectory)
        {
            var filesInDirectoryLower = new HashSet<String>(filesInDirectory.Select(x => x.ToLower()));

            foreach (String filePath in filesInDirectoryLower)
            {
                var fileInfo = new FileInfo(filePath);
                String extension = fileInfo.Extension.ToLowerInvariant();
                Boolean isJpeg = extension == ".jpg" || extension == ".jpeg";
                if (isJpeg)
                {
                    String cr2Path = filePath.Substring(0, filePath.Length - extension.Length) + ".cr2";
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