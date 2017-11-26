using System;
using System.Windows.Data;

namespace ExtraClub.UIControls
{
    public class BoolToOppositeBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;
            return !(((bool?)value) ?? false);
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;
            return !(((bool?)value) ?? false);
        }

        #endregion
    }
}