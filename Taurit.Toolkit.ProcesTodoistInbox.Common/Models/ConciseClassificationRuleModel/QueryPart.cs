using System;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Taurit.Toolkit.ProcessTodoistInbox.Common.Models.ConciseClassificationRuleModel
{
    /// <summary>
    ///     Represents part of a rule string (in a concise format).
    /// </summary>
    /// <example>
    ///     Full rule:
    ///     * "if startsWith(anki|nauka|ang|learn|edu|experiment) and numLabelsIs(0) then setLabel(nauka)"
    ///     ---
    ///     The "if" part (<see cref="IfPart" />):
    ///     * if startsWith(anki|nauka|ang|learn|edu|experiment) and numLabelsIs(0)
    ///     ---
    ///     The "then" part (<see cref="ThenPart" />):
    ///     * setLabel(nauka)
    /// </example>
    internal class QueryPart
    {
        private readonly String[] _arraySplitCharacter = {"|"};
        [NotNull] private readonly String _userProvidedString;

        protected QueryPart([NotNull] String userProvidedString)
        {
            _userProvidedString = userProvidedString ?? throw new ArgumentNullException(nameof(userProvidedString));
        }

        [CanBeNull]
        protected String GetStringArgument([NotNull] String key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            var argumentForKeyRegex = new Regex($@"{key}\((?<argument>.*?)\)", RegexOptions.IgnoreCase);
            Match match = argumentForKeyRegex.Match(_userProvidedString);
            if (!match.Success) return null;
            return match.Groups["argument"]?.Value;
        }

        [CanBeNull]
        protected String[] GetStringArrayArgument([NotNull] String key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            String arraySerialized = GetStringArgument(key);
            if (arraySerialized == null) return null;

            String[] splitArray = arraySerialized.Split(_arraySplitCharacter, StringSplitOptions.RemoveEmptyEntries);
            return splitArray;
        }

        [CanBeNull]
        protected Int32? GetPriorityArgument([NotNull] String key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            String prioritySerialized = GetStringArgument(key);
            if (prioritySerialized == null) return null;

            switch (prioritySerialized.ToLowerInvariant())
            {
                case "undefined": return 1;
                case "low": return 2;
                case "medium": return 3;
                case "high": return 4;
                default: return Convert.ToInt32(prioritySerialized);
            }
        }

        [CanBeNull]
        protected Int32? GetIntArgument([NotNull] String key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            String prioritySerialized = GetStringArgument(key);
            if (prioritySerialized == null) return null;
            return Convert.ToInt32(prioritySerialized);
        }
    }
}