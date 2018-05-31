using System;
using System.Globalization;
using System.Windows.Data;

namespace Taurit.Toolkit.ProcessTodoistInbox.Converters
{
    public class PriorityToTextConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType,
            Object parameter, CultureInfo culture)
        {
            // Do the conversion from bool to visibility
            switch (value.ToString())
            {
                case "1": return "Not assigned";
                case "2": return "Low";
                case "3": return "High";
                case "4": return "High and urgent";
                default: return "Error / unknown";
            }
        }

        public Object ConvertBack(Object value, Type targetType,
            Object parameter, CultureInfo culture)
        {
            // Do the conversion from visibility to bool
            return null;
        }
    }
}