using System.Windows.Controls;
using System.Windows;
using Telerik.Windows.Controls.GridView;
using System.Windows.Media;

namespace TonusClub.UIControls
{

    public class ModifiedSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            try
            {
                var style = new Style(typeof(GridViewRow));

                var row = (GridViewRow)container;

                var obj = (bool)item.GetType().GetProperty("Modified").GetValue(item, null);

                if (obj)
                {
                    style.Setters.Add(new Setter(GridViewRow.BackgroundProperty, new SolidColorBrush(Colors.LightCoral)));
                }

                return style;
            }
            catch
            {
                return null;
            }
        }
    }
}
