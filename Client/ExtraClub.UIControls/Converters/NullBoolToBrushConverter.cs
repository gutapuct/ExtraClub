using System;
using System.Globalization;
using System.Windows.Media;

namespace ExtraClub.UIControls.Converters
{
    public class NullBoolToBrushConverter : ConvertorBase<NullBoolToBrushConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Transparent);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
