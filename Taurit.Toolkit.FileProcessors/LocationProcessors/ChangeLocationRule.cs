using System;
using System.IO;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Taurit.Toolkit.FileProcessors.LocationProcessors
{
    public class ChangeLocationRule
    {
        [NotNull] private readonly Regex _fileNamePattern;
        [NotNull] public String TargetLocation { get; }
    
        public ChangeLocationRule([RegexPattern] [NotNull] String fileNamePattern, [NotNull] String targetLocation)
        {
            if (fileNamePattern == null)
            {
                throw new ArgumentNullException(nameof(fileNamePattern));
            }

            if (!Directory.Exists(targetLocation))
            {
                throw new ArgumentException("Target directory must exist", nameof(targetLocation));
            }

            _fileNamePattern = new Regex(fileNamePattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            TargetLocation = targetLocation ?? throw new ArgumentNullException(nameof(targetLocation));
        }

        public Boolean CanBeAppliedTo([NotNull] String fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            return this._fileNamePattern.Match(fileName).Success;
        }
    }
}