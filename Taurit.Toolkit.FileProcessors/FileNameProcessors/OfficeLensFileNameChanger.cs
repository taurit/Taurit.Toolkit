using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Taurit.Toolkit.FixDateFormatInFilenames.Domain
{
    public class ChangeDateFormatFileProcessor : IFileProcessor
    {
        [NotNull] private static readonly Regex FileWithInvalidDateFormat =
            new Regex(@"(?<day>\d\d)\.(?<month>\d\d)\.(?<year>\d\d\d\d) \d\d \d\d (?<description>.*)",
                RegexOptions.Compiled);

        [NotNull] private readonly IFileNameFormatProvider _fileNameFormatProvider;

        public ChangeDateFormatFileProcessor([NotNull] IFileNameFormatProvider fileNameFormatProvider)
        {
            _fileNameFormatProvider = fileNameFormatProvider ??
                                throw new ArgumentNullException(nameof(fileNameFormatProvider));
        }

        public void ProcessMatchingFiles([NotNull] String directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                throw new ArgumentException("Directory does not exist", nameof(directoryPath));
            }

            ReadOnlyCollection<String> allFilesInDirectory = Directory.GetFiles(directoryPath).ToList().AsReadOnly();

            foreach (String file in allFilesInDirectory)
            {
                Match match = FileWithInvalidDateFormat.Match(file);
                if (match.Success)
                {
                    String day = match.Groups["day"].Value;
                    String month = match.Groups["month"].Value;
                    String year = match.Groups["year"].Value;
                    String description = match.Groups["description"].Value;

                    String newFileName = _fileNameFormatProvider.FormatFileName(year, month, day, description);
                    String newFilePath = Path.Combine(directoryPath, newFileName);
                    while (File.Exists(newFilePath))
                        newFilePath = Path.Combine(directoryPath,
                            Path.GetFileNameWithoutExtension(newFileName) + "(2)" + Path.GetExtension(newFileName));
                    File.Move(file, newFilePath);
                }
            }
        }
    }
}