using System;
using System.Diagnostics;
using System.IO;
using Taurit.Toolkit.ProcessRecognizedInboxFiles.Domain;

namespace Taurit.Toolkit.ProcessRecognizedInboxFiles
{
    internal class Program
    {
        /// <summary>
        ///     Go through the files in "file inbox", which consists of files coming from:
        ///     - Office Lens
        ///     and process them based on a set of conventions. For example:
        ///     - photos of mind maps drawn on paper should go to some directory Y,
        ///     - photos of receipts may need to go to some directory X and get compressed with low quality setting,
        ///     - etc.
        /// </summary>
        /// <param name="args"></param>
        private static void Main(String[] args)
        {
            Debug.Assert(args.Length == 2);

            String fileInboxPath = args[0];
            var inbox = new Inbox(fileInboxPath);

            // ...
        }


    }
}