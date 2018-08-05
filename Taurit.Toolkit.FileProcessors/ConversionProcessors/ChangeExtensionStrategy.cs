using System;
using System.IO;
using JetBrains.Annotations;

namespace Taurit.Toolkit.FileProcessors.ConversionProcessors
{
    public class ChangeExtensionStrategy : IConvertedFileNamingStrategy
    {
        [NotNull] private readonly String _newExtension;

        public ChangeExtensionStrategy([NotNull] String newExtension)
        {
            _newExtension = newExtension;
        }

        /// <inheritdoc />
        public String GetConvertedFilePath([NotNull] String originalPath)
        {
            if (originalPath == null) throw new ArgumentNullException(nameof(originalPath));

            var fileInfo = new FileInfo(originalPath);
            String extension = fileInfo.Extension.ToLowerInvariant();
            String pathWithoutExtension = originalPath.Substring(0, originalPath.Length - extension.Length);
            return $"{pathWithoutExtension}.{_newExtension}";
        }
    }
}