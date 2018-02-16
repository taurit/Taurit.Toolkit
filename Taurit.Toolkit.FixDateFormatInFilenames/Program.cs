using System;
using Taurit.Toolkit.FileProcessors;
using Taurit.Toolkit.FileProcessors.NameProcessors;
using Taurit.Toolkit.FileProcessors.NameProcessors.NameFormatProviders;

namespace Taurit.Toolkit.FixDateFormatInFilenames
{
    internal class Program
    {
        private static void Main(String[] args)
        {
            if (args.Length != 1)
            {
                PrintHelp();

                Console.WriteLine("Invalid number of arguments. Exiting.");
                Console.ReadKey();
                return;
            }

            String directory = args[0];

            var fileProcessors = new IFileProcessor[]
            {
                new ChangeOfficeLensNameProcessor(new IsoDateFileNameFormatProvider())
            };

            var inboxFolder = new Folder(fileProcessors);
            inboxFolder.ProcessAllFiles(directory);
        }

        private static void PrintHelp()
        {
            Console.WriteLine(
                @"TauritToolkit.ProcessMatchingFiles
---------------------------
This program replaces date format in a filename like the one from Office Lens:
'27.08.2017 15 54 wizytowka.jpg'
with a standard ISO format:
'2017-08-27 wizytowka.jpg'
(also dropping hour and minutes)

Arguments:
[0] path to directory containing files to rename. If no such files are recognized, nothing happens.
");
        }
    }
}