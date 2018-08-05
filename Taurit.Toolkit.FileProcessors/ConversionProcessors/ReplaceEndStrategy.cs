using System;
using JetBrains.Annotations;

namespace Taurit.Toolkit.FileProcessors.ConversionProcessors
{
    public class ReplaceEndStrategy : IConvertedFileNamingStrategy
    {
        private readonly String _from;
        private readonly String _to;


        public ReplaceEndStrategy([NotNull] String from, [NotNull] String to)
        {
            _to = to;
            _from = from;
        }

        /// <inheritdoc />
        public String GetConvertedFilePath([NotNull] String originalPath)
        {
            if (originalPath == null) throw new ArgumentNullException(nameof(originalPath));
            if (!originalPath.EndsWith(_from))
                throw new ArgumentException("Path doesn't end with provided string", nameof(originalPath));

            return ReplaceLastOccurrence(originalPath, _from, _to);
        }

        private static String ReplaceLastOccurrence(String source, String find, String replace)
        {
            Int32 place = source.LastIndexOf(find, StringComparison.InvariantCultureIgnoreCase);
            String result = source.Remove(place, find.Length).Insert(place, replace);
            return result;
        }
    }
}