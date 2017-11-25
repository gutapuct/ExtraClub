using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using TonusClub.ServiceModel.Reports;

namespace TonusClub.Reports.Views.ContainedControls
{
    public class FilterTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DivisionTemplate { get; set; }
        public DataTemplate MultipleTemplate { get; set; }
        public DataTemplate DateTemplate { get; set; }
        public DataTemplate MonthTemplate { get; set; }
        public DataTemplate BooleanTemplate { get; set; }
        public DataTemplate StringTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item,
          DependencyObject container)
        {
            var param = (ReportParamInt)item;
            if (param.Type == ReportParameterType.Date) return DateTemplate;
            if (param.Type == ReportParameterType.Company) return DivisionTemplate;
            if (param.Type == ReportParameterType.Division) return DivisionTemplate;
            if (param.Type == ReportParameterType.Month) return MonthTemplate;
            if (param.Type == ReportParameterType.Boolean) return BooleanTemplate;
            if (param.Type == ReportParameterType.String) return StringTemplate;
            if (param.Type == ReportParameterType.CustomDropdown) return DivisionTemplate;
            if (param.Type == ReportParameterType.Good) return DivisionTemplate;
            if (param.Type == ReportParameterType.Employee) return DivisionTemplate;
            if (param.Type == ReportParameterType.Employees) return MultipleTemplate;
            return null;
        }
    }

}
