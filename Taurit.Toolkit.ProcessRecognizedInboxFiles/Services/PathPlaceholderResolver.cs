using System;

namespace Taurit.Toolkit.ProcessRecognizedInboxFiles.Services
{
    /// <inheritdoc />
    public class PathPlaceholderResolver : IPathPlaceholderResolver
    {
        public String Resolve(String templatePath)
        {
            if (templatePath == null) throw new ArgumentNullException(nameof(templatePath));

            String path = templatePath
                    .Replace("%YEAR%", DateTime.UtcNow.Year.ToString())
                ;
            return path;
        }
    }
}