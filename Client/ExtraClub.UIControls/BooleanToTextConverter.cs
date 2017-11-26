using System;
using System.Windows.Data;

namespace ExtraClub.UIControls
{
    public class BooleanToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;
            if (!(value is bool)) return null;
            if ((bool)value) return UIControls.Localization.Resources.Yes;
            return UIControls.Localization.Resources.No;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
