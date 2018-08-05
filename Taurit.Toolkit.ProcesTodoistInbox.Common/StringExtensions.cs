using System;
using System.Text;

namespace Taurit.Toolkit.ProcesTodoistInbox.Common
{
    public static class StringExtensions
    {
        public static Boolean Contains(this String source, String toCheck, StringComparison comp)
        {
            return source?.IndexOf(toCheck, comp) >= 0;
        }

        public static String RemoveDiacritics(this String s)
        {
            String asciiEquivalents = Encoding.ASCII.GetString(
                Encoding.GetEncoding("Cyrillic").GetBytes(s)
            );

            return asciiEquivalents;
        }
    }
}