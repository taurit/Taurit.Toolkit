using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Taurit.Toolkit.FixDateFormatInFilenames
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine(
                @"TauritToolkit.FixDateFormatInFilenames
---------------------------
This program replaces date format in a filename like the one from Office Lens:
'27.08.2017 15 54 wizytowka.jpg'
with a standard ISO format:
'2017-08-27 wizytowka.jpg'
(also dropping hour and minutes)

Arguments:
[0] path to directory containing files to rename. If no such files are recognized, nothing happens.
");

            if (args.Length != 1)
            {
                Console.WriteLine("Invalid number of arguments. Exiting.");
                Console.ReadKey();
                return;
            }


            var directory = args[0];
            var filesInDirectory = GetFilesInDirectory(directory);

            RenameFiles(filesInDirectory, directory);
        }

        private static void RenameFiles(List<string> filesInDirectory, string path)
        {
            var fileWithInvalidDateFormat = new Regex(@"(\d\d)\.(\d\d)\.(\d\d\d\d) \d\d \d\d (.*)");

            foreach (var file in filesInDirectory)
            {
                var m = fileWithInvalidDateFormat.Match(file);
                if (m.Success)
                {
                    var day = m.Groups[1].Value;
                    var month = m.Groups[2].Value;
                    var year = m.Groups[3].Value;
                    var description = m.Groups[4].Value;

                    var newFileName = string.Format("{0:4}-{1:2}-{2:2} {3}", year, month, day, description);
                    var newFilePath = Path.Combine(path, newFileName);
                    while (File.Exists(newFilePath))
                        newFilePath = Path.Combine(path,
                            Path.GetFileNameWithoutExtension(newFileName) + "(2)" + Path.GetExtension(newFileName));
                    File.Move(file, newFilePath);
                }
            }
        }

        private static List<string> GetFilesInDirectory(string directory)
        {
            return Directory.GetFiles(directory).ToList();
        }
    }
}