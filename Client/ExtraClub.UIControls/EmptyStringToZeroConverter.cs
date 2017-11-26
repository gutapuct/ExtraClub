using System;
using System.Windows.Data;
using System.Globalization;

namespace ExtraClub.UIControls
{
    public class EmptyStringToZeroConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null || string.IsNullOrEmpty(value.ToString())
                ? 0
                : value;
        }

        #endregion
    }
}
