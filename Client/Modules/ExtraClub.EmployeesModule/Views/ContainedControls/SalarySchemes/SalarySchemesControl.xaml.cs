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
using System.Windows.Shapes;
using ExtraClub.EmployeesModule.ViewModels;
using ExtraClub.EmployeesModule.Views.ContainedControls.SalarySchemes.Windows;
using Microsoft.Practices.Unity;
using ExtraClub.Infrastructure.BaseClasses;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls.BaseClasses;

namespace ExtraClub.EmployeesModule.Views.ContainedControls.SalarySchemes
{
    public partial class SalarySchemesControl
    {
        private EmployeesLargeViewModel Model
        {
            get
            {
                return this.DataContext as EmployeesLargeViewModel;
            }
        }

        public SalarySchemesControl()
        {
            InitializeComponent();
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditSchemeWindow>(() => Model.RefreshSalarySchemes());
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.SalarySchemesView.CurrentItem != null)
            {
                ProcessUserDialog<NewEditSchemeWindow>(() => Model.RefreshSalarySchemes(),
                    new ParameterOverride("scheme", ViewModelBase.Clone<SalaryScheme>(Model.SalarySchemesView.CurrentItem)));
            }
        }

        private void JobsViewGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e))
            {
                if (!ClientContext.CheckPermission("SalarySchemesMgmtBtns")) return;
                EditButton_Click(null, null);
            }
        }
    }
}
