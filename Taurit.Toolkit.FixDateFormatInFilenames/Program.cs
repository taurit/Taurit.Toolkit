using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Taurit.Toolkit.FixDateFormatInFilenames
{
    internal class Program
    {
        private static void Main(String[] args)
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


            String directory = args[0];
            List<String> filesInDirectory = GetFilesInDirectory(directory);

            RenameFiles(new FileNameFixer(), filesInDirectory, directory);
        }

        private static void RenameFiles(FileNameFixer fileNameFixer, List<String> filesInDirectory, String path)
        {
            var fileWithInvalidDateFormat =
                new Regex(@"(?<day>\d\d)\.(?<month>\d\d)\.(?<year>\d\d\d\d) \d\d \d\d (.*)");


            foreach (String file in filesInDirectory)
            {
                Match m = fileWithInvalidDateFormat.Match(file);
                if (m.Success)
                {
                    String day = m.Groups["day"].Value;
                    String month = m.Groups["month"].Value;
                    String year = m.Groups["year"].Value;
                    String description = m.Groups[4].Value;

                    String newFileName = fileNameFixer.GetProperFileName(year, month, day, description);
                    String newFilePath = Path.Combine(path, newFileName);
                    while (File.Exists(newFilePath))
                        newFilePath = Path.Combine(path,
                            Path.GetFileNameWithoutExtension(newFileName) + "(2)" + Path.GetExtension(newFileName));
                    File.Move(file, newFilePath);
                }
            }
        }

        private static List<String> GetFilesInDirectory(String directory)
        {
            return Directory.GetFiles(directory).ToList();
        }
    }
}