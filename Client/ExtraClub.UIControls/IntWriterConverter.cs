using System;
using System.Windows.Data;
using System.Globalization;

namespace ExtraClub.UIControls
{
    public class IntWriterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            var checkValue = value.ToString();
            var targetValue = parameter.ToString();
            return checkValue.Equals(targetValue,
                     StringComparison.InvariantCultureIgnoreCase);
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return null;
            if (!(bool)value) return null;

            return Int32.Parse(parameter.ToString());
        }
    }   
}
