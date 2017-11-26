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
using Microsoft.Practices.Unity;
using ExtraClub.EmployeesModule.ViewModels;
using ExtraClub.EmployeesModule.Views.ContainedControls.Employees.Windows;

namespace ExtraClub.EmployeesModule.Views.ContainedControls.Employees
{
    /// <summary>
    /// Interaction logic for SalaryControl.xaml
    /// </summary>
    public partial class SalaryControl
    {
        private EmployeesLargeViewModel Model
        {
            get
            {
                return this.DataContext as EmployeesLargeViewModel;
            }
        }

        public SalaryControl()
        {
            InitializeComponent();
        }

        private void NewSheetClick(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditSalarySheetWindow>(() => Model.RefreshSalarySheets());
        }

        private void SalariesViewGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e))
            {
                if (Model.SalariesView.CurrentItem == null) return;
                if (!ClientContext.CheckPermission("ProcessSalarySheet")) return;
                ProcessUserDialog<NewEditSalarySheetWindow>(() => Model.RefreshEmployeeCashlow(), new ParameterOverride("sheet", Model.SalariesView.CurrentItem));
            }
        }
    }
}
