using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Data;
using System.Windows.Media;
using Telerik.Windows.Controls.GridView;

namespace TonusClub.Reports.Views.ContainedControls
{
    class BackgroundStyleSelectorFinance : StyleSelector
    {
        int Index { get; set; }
        decimal Red, Orange, Yellow, LightGreen;


        public BackgroundStyleSelectorFinance(int index)
        {
            Index = index;
            Red = 50m;
            Orange = 70m;
            Yellow = 80m;
            LightGreen = 99m;
        }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            var row = item as DataRow;
            if (row[0] as int? != 0) return new Style(typeof(GridViewCell));

            decimal curr;
            if (Index > 4 && Decimal.TryParse((row[Index] ?? "").ToString(), out curr))
            {
                Brush br = null;
                if (curr <= Red) br = Brushes.Salmon;
                if (curr > Red && curr <= Orange) br = Brushes.Orange;
                if (curr > Orange && curr <= Yellow) br = Brushes.Yellow;
                if (curr > Yellow && curr <= LightGreen) br = new SolidColorBrush(Color.FromArgb(255, 205, 231, 202));
                if (curr > LightGreen) 
                    br = Brushes.LightGreen;
                if (br != null)
                    {
                        Style style = new Style(typeof(GridViewCell));
                        style.Setters.Add(new Setter(GridViewCell.BackgroundProperty, br));
                        return style;
                    }
                }
            
            return new Style(typeof(GridViewCell));
        }
    }
}