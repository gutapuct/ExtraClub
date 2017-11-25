using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace TonusClub.UIControls
{
    public class RedLetterDayConverter : IValueConverter
    {
        public static Dictionary<DateTime, string> Days =
                    new Dictionary<DateTime, string>();

        static RedLetterDayConverter()
        {
        }

        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            string text;
            if (!Days.TryGetValue((DateTime)value, out text))
                text = null;
            return text;
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}





