using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Taurit.Toolkit.Commons
{
    public class DirectoryHelper
    {
        public static IReadOnlyList<string> GetFilesInDirectory(string directory)
        {
            return Directory.GetFiles(directory).ToList();
        }
    }
}