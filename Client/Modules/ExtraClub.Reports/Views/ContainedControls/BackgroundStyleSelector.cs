using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Data;
using System.Windows.Media;
using Telerik.Windows.Controls.GridView;

namespace ExtraClub.Reports.Views.ContainedControls
{
    public class BackgroundStyleSelector : StyleSelector
    {
        int Index { get; set; }
        decimal Red, Orange, Yellow;


        public BackgroundStyleSelector(int index, decimal red, decimal orange, decimal yellow)
        {
            Index = index;
            Red = red/100;
            Orange = orange/100;
            Yellow = yellow/100;
        }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            var row = item as DataRow;
            if (row == null) return new Style(typeof(GridViewCell));
            decimal curr, prev;
            if (Index > 1 && Decimal.TryParse((row[Index] ?? "").ToString(), out curr))
            {
                var currcol = row.Table.Columns[Index].ExtendedProperties.ContainsKey("MonthColumn");
                var prevcol = row.Table.Columns[Index - 1].ExtendedProperties.ContainsKey("MonthColumn");
                if (currcol == prevcol)
                {
                    if (Decimal.TryParse((row[Index - 1] ?? "").ToString(), out prev))
                    {
                        Brush br = null;
                        if (curr != 0)
                        {
                            var perc = prev / curr;
                            if ((perc >= 1 - Orange && perc <= 1 - Yellow) || (perc >= 1 + Yellow && perc < 1 + Orange)) br = Brushes.Yellow;
                            if ((perc >= 1 - Red && perc <= 1 - Orange) || (perc >= 1 + Orange && perc < 1 + Red)) br = Brushes.Orange;
                            if (perc <= 1 - Red || perc >= 1 + Red) br = Brushes.Salmon;
                        }
                        else
                        {
                            if (prev != 0) br = Brushes.Red;
                        }
                        if (br != null)
                        {
                            Style style = new Style(typeof(GridViewCell));
                            style.Setters.Add(new Setter(GridViewCell.BackgroundProperty, br));
                            return style;
                        }
                    }
                }
            }
            return new Style(typeof(GridViewCell));
        }
    }
}
