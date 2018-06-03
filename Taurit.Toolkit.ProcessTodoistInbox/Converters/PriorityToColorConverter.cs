using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Taurit.Toolkit.ProcessTodoistInbox.Converters
{
    public class PriorityToColorConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType,
            Object parameter, CultureInfo culture)
        {
            // Do the conversion from bool to visibility
            switch ((value ?? "priority won't be set").ToString())
            {
                case "2": return new SolidColorBrush(Color.FromRgb(255, 246, 225)); // Low
                case "3": return new SolidColorBrush(Color.FromRgb(255, 242, 232)); // High
                case "4": return new SolidColorBrush(Color.FromRgb(255, 229, 229)); // High and urgent
                default: return new SolidColorBrush(Color.FromRgb(255, 255, 255));
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