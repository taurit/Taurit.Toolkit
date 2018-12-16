using System;
using System.Globalization;
using JetBrains.Annotations;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

namespace Taurit.Toolkit.TodoistInboxHelper
{
    /// <summary>
    ///     This class is able to parse the date string provided by the Todoist API (found e.g. in <see cref="TodoTask" />)
    /// </summary>
    public class TaskDateParser
    {
        [CanBeNull]
        public DateTime? TryParse(String dateString)
        {
            if (string.IsNullOrEmpty(dateString))
                return null;

            // Mon 07 Aug 2006 12:34:56 +0000
            try
            {
                // timezone part is for now ignored, it is always +0000 in API results (the time is in UTC) and there seems to be no standard formatter for this
                DateTime date = DateTime.ParseExact(dateString.Substring(0, 24),
                    "ddd d MMM yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                return date;
            }
            catch (FormatException)
            {
                return null;
            }
        }
    }
}