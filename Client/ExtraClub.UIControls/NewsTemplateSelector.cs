using System;
using System.Windows;
using System.Windows.Controls;
using ExtraClub.ServiceModel;

namespace ExtraClub.UIControls
{
    public class NewsTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SimpleTemplate { get; set; }
        public DataTemplate UrlTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item,
          DependencyObject container)
        {
            var param = (News)item;

            if (!String.IsNullOrEmpty(param.Url)) return UrlTemplate;
            return SimpleTemplate;
        }
    }
}
