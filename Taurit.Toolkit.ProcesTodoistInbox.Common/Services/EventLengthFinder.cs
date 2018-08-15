using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Taurit.Toolkit.ProcessTodoistInbox.Common.Services
{
    /// <summary>
    ///     Looks for fragments indicating event length in the string
    /// </summary>
    public class EventLengthFinder
    {
        /// <summary>
        ///     Regex to find patterns indicating event's length, for example: (1h 30 min)
        ///     This regular expression is covered by unit tests in EventLengthFinderTests class
        /// </summary>
        private static readonly Regex RegexFindTime = new Regex(
            @"(\[|\()?([\d,\.]+)(\s*?)(h|hour|minute|minut|min|m)([s\s]|\z|\)|\])((\s*?)([\d,\.]+)(\s*?)(minutes|minute|minut|min|m)(\)|\])?)?",
            RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.Compiled);

        /// <summary>
        ///     The unmodified string that was passed by the user
        /// </summary>
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly String _originalString;

        /// <summary>
        ///     The string that was passed by the user without the recognized regex part.
        ///     If no pattern was faound, it has a null value.
        /// </summary>
        private readonly String _stringWithoutPattern;

        /// <summary>
        ///     If time pattern was faound in provided string, this value is normalized time that it represents (in minutes).
        ///     Otherwise 0.
        /// </summary>
        private readonly Int32 _totalMinutes;

        /// <summary>
        ///     Examines task summery for fragments indicating event's length. If such fragments are found,
        ///     sets the property <see cref="PatternFound " /> to
        ///     <value>true</value>
        ///     and allows to get
        ///     them as a normalized TimeSpan object.
        /// </summary>
        /// <param name="taskSummary">string that may contain some form of event duration</param>
        public EventLengthFinder(String taskSummary)
        {
            Match match = RegexFindTime.Match(taskSummary);
            PatternFound = match.Success;
            _originalString = taskSummary;

            if (match.Success)
            {
                if (Decimal.TryParse(match.Groups[2].Value, NumberStyles.Any, CultureInfo.InvariantCulture,
                    out Decimal quantity))
                {
                    String unit = match.Groups[4].Value;

                    if (unit.StartsWith("h", StringComparison.InvariantCultureIgnoreCase))
                    {
                        // assume that unit is hours
                        _totalMinutes = (Int32) (quantity * 60m);

                        // there might be another part of the string specifying minutes in this case
                        if (Decimal.TryParse(match.Groups[8].Value, NumberStyles.Any, CultureInfo.InvariantCulture,
                            out Decimal quantityMinutesPart)) _totalMinutes += (Int32) quantityMinutesPart;
                    }
                    else
                    {
                        // assume that unit is minutes
                        _totalMinutes = (Int32) quantity;
                    }

                    _stringWithoutPattern = _originalString.Replace(match.Groups[0].Value, "").Replace("()", "")
                        .Replace("[]", "").Replace("  ", " ").Trim();
                }
            }
        }

        /// <summary>
        ///     Property indicating whether pattern looking like event's duration was found in a string
        /// </summary>
        public Boolean PatternFound { get; }

        /// <summary>
        ///     Duration found in string (in minutes). If this property is obtained and there was no match,
        ///     InvalidOperationException is thrown.
        /// </summary>
        public Int32 TotalMinutes
        {
            get
            {
                if (PatternFound == false)
                    throw new InvalidOperationException("Time duration pattern was not found in a given string");

                return _totalMinutes;
            }
        }

        /// <summary>
        ///     If pattern was found, returns a string without the found pattern. Otherwise returns original string.
        /// </summary>
        public String TaskSummaryWithoutPattern => _stringWithoutPattern;
    }
}