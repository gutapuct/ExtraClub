using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace ExtraClub.TurnoverModule.Views
{
    public class DivideConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(double))
                throw new InvalidOperationException("The target must be a double");

            double nominal = 200;

            value = (double)value - 10;

            if ((double)value / nominal<1)return value;
            return nominal; //+ ((double)value % nominal) / (Math.Floor((double)value / nominal))-1;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
