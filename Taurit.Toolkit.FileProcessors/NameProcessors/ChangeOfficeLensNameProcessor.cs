using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Taurit.Toolkit.FileProcessors.NameProcessors.NameFormatProviders;

namespace Taurit.Toolkit.FileProcessors.NameProcessors
{
    public class ChangeOfficeLensNameProcessor : FileProcessorBase
    {
        [NotNull] private static readonly Regex FileWithInvalidDateFormat =
            new Regex(@"(?<day>\d\d)\.(?<month>\d\d)\.(?<year>\d\d\d\d) \d\d \d\d (?<description>.*)",
                RegexOptions.Compiled);

        /// <summary>
        ///     New filename format introduced around June or July of 2018
        /// </summary>
        [NotNull] private static readonly Regex FileWithInvalidDateFormat2 =
            new Regex(@"(?<year>\d\d\d\d)_(?<month>\d\d)_(?<day>\d\d) \d\d_\d\d (?<description>.*)",
                RegexOptions.Compiled);

        /// <summary>
        ///     HP scanner's file format (would deserve another class, but to mae the change quick i place it here as a workaround).
        /// Example filename: 514316-080118 BUW nauka ASP NET Web Api notatki 1.jpg
        /// </summary>
        [NotNull] private static readonly Regex FileWithInvalidDateFormat3 =
            new Regex(@"\d{6}-(?<day>\d\d)(?<month>\d\d)(?<year>\d\d) (?<description>.*)",
                RegexOptions.Compiled);

        
        /// OnePlus camera filename format can also be improved
        /// Example filenames:
        /// * d:\Inbox\IMG_20200129_210243.jpg
        /// * d:\Inbox\IMG_20200129_210243__01.jpg // for edits
        [NotNull] private static readonly Regex FileWithInvalidDateFormat4 =
            new Regex(@"IMG_(?<year>\d\d\d\d)(?<month>\d\d)(?<day>\d\d)_\d{6}_*(?<description>.*)",
                RegexOptions.Compiled);

        [NotNull] private readonly IFileNameFormatProvider _fileNameFormatProvider;

        public ChangeOfficeLensNameProcessor([NotNull] IFileNameFormatProvider fileNameFormatProvider)
        {
            _fileNameFormatProvider = fileNameFormatProvider ??
                                      throw new ArgumentNullException(nameof(fileNameFormatProvider));
        }

        /// <inheritdoc />
        public override void ProcessMatchingFile(String file)
        {
            String directoryPath = new FileInfo(file).DirectoryName;
            Debug.Assert(directoryPath != null);
            Match match = GetMatch(file);

            if (match.Success)
            {
                String day = match.Groups["day"].Value;
                String month = match.Groups["month"].Value;
                String year = match.Groups["year"].Value;
                String description = match.Groups["description"].Value;

                String newFileName = _fileNameFormatProvider.FormatFileName(year, month, day, description);
                String newFilePath = Path.Combine(directoryPath, newFileName);
                var copyIndex = 2;
                while (File.Exists(newFilePath))
                    newFilePath = Path.Combine(directoryPath,
                        Path.GetFileNameWithoutExtension(newFileName) + $"({copyIndex++})" +
                        Path.GetExtension(newFileName));
                File.Move(file, newFilePath);
            }
        }

        [NotNull]
        public Match GetMatch([NotNull] String fileName)
        {
            Match match = FileWithInvalidDateFormat.Match(fileName);
            if (!match.Success) match = FileWithInvalidDateFormat2.Match(fileName);
            if (!match.Success) match = FileWithInvalidDateFormat3.Match(fileName);
            if (!match.Success) match = FileWithInvalidDateFormat4.Match(fileName);


            return match;
        }
    }
}