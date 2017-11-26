using System;
using System.Linq;
using System.Windows.Data;

namespace ExtraClub.UIControls
{
    public class SpanStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
    System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;
            var val = (string)value;
            val = val.Replace(' ','0');
            if (!val.Contains(':')) val = val.Insert(2, ":");
            return TimeSpan.ParseExact(val, "g", null);
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;
            var val = (TimeSpan)value;
            return val.Hours.ToString("00") + val.Minutes.ToString("00");
        }
    }
}
