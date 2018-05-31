using System;

namespace Taurit.Toolkit.ProcessTodoistInbox
{
    public static class StringExtensions
    {
        public static Boolean Contains(this String source, String toCheck, StringComparison comp)
        {
            return source?.IndexOf(toCheck, comp) >= 0;
        }
    }
}