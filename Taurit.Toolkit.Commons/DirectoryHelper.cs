using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Taurit.Toolkit.Commons
{
    public class DirectoryHelper
    {
        public static IReadOnlyList<String> GetFilesInDirectory(String directory)
        {
            return Directory.GetFiles(directory).ToList();
        }
    }
}