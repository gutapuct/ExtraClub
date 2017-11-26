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
using Telerik.Windows.Data;
using Telerik.Windows.Controls.GridView;
using ExtraClub.UIControls;

namespace ExtraClub.EmployeesModule.Views.ContainedControls.Employees
{
    public partial class EmployeeCashflowControl
    {
        public EmployeeCashflowControl()
        {
            InitializeComponent();
            NavigationManager.NavigateToCashFlow += NavigationNavigateToCashFlow;
        }

        void NavigationNavigateToCashFlow(object sender, StringEventArgs e)
        {
            EmployeeCashlowGrid.FilterDescriptors.Clear();
            var f = new ColumnFilterDescriptor((IDataFieldDescriptor)EmployeeCashlowGrid.Columns[1]);
            f.DistinctFilter.DistinctValues.Add(e.Value);
            EmployeeCashlowGrid.FilterDescriptors.Add(f);

        }
    }
}
