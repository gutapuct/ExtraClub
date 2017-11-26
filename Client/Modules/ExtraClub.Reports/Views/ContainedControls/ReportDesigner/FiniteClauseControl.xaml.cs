using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ExtraClub.ServiceModel.Reports;
using ExtraClub.ServiceModel;
using System.ComponentModel;

namespace ExtraClub.Reports.Views.ContainedControls.ReportDesigner
{
    /// <summary>
    /// Interaction logic for FiniteClauseControl.xaml
    /// </summary>
    public partial class FiniteClauseControl : UserControl
    {
        public FiniteClauseControl(Clause clause, dynamic context)
        {
            clause.SetContext(context);
            DataContext = clause;
            InitializeComponent();
        }

        private void ConvertOrButton_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as FiniteClause).Convert(new OrClause());
        }

        private void ConvertAndButton_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as FiniteClause).Convert(new AndClause());
        }

        private void AutoNameClick(object sender, RoutedEventArgs e)
        {
            var clause = (DataContext as FiniteClause);
            clause.ParameterName = ((DescriptionAttribute)clause.Parameter.GetCustomAttributes(typeof(DescriptionAttribute), false)[0]).Description+ " " + Helper.GetText(clause.Operator);
        }

    }
}
