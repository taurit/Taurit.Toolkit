using System;
using System.Text;

namespace Taurit.Toolkit.ProcessTodoistInbox.Common
{
    public static class StringExtensions
    {
        public static Boolean Contains(this String source, String toCheck, StringComparison comp)
        {
            return source?.IndexOf(toCheck, comp) >= 0;
        }

        public static String RemoveDiacritics(this String s)
        {
            // this is needed for .NET Core clients.
            // The .NET Framework on the Windows desktop supports a large number of character encodings and code pages.
            // .NET Core, on the other hand, by default supports only a few.
            // CodePagesEncodingProvider.Instance returns an EncodingProvider object that makes the full set of encodings available on the desktop .NET Framework Class Library available to .NET Core applications.
            // https://docs.microsoft.com/en-us/dotnet/api/system.text.encodingprovider?redirectedfrom=MSDN&view=netframework-4.8
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            String asciiEquivalents = Encoding.ASCII.GetString(
                Encoding.GetEncoding("Cyrillic").GetBytes(s)
            );

            return asciiEquivalents;
        }
    }
}