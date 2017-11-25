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
using TonusClub.EmployeesModule.ViewModels;
using TonusClub.EmployeesModule.Views.ContainedControls.Schedules.Windows;
using Microsoft.Practices.Unity;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using TonusClub.ServiceModel.Employees;
using TonusClub.EmployeesModule.Views.ContainedControls.Employees.Windows;

namespace TonusClub.EmployeesModule.Views.ContainedControls.Schedules
{
    /// <summary>
    /// Interaction logic for CurrentVacations.xaml
    /// </summary>
    public partial class CurrentVacations
    {
        private EmployeesLargeViewModel Model
        {
            get
            {
                return this.DataContext as EmployeesLargeViewModel;
            }
        }

        public CurrentVacations()
        {
            InitializeComponent();
        }

        private void NewScheduleClick(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewVacationListWindow>(() => Model.RefreshVacationHistory(),
                new ParameterOverride("list", Model.CurrentVacationsScheduleView.SourceCollection),
                new ParameterOverride("id", Guid.Empty));
        }

        private void RadContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            RadContextMenu menu = (RadContextMenu)sender;
            GridViewRow row = menu.GetClickedElement<GridViewRow>();
            if (row != null)
            {
                row.IsSelected = row.IsCurrent = true;
                var item = row.DataContext as EmployeeScheduleProposalElement;
                if (item != null)
                {
                    if (item.Start >= DateTime.Today)
                    {
                        return;
                    }
                }
            }
            menu.IsOpen = false;
        }

        private void RadContextMenu_ItemClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var menu = (RadContextMenu)sender;
            RadMenuItem clickedItem = e.OriginalSource as RadMenuItem;
            GridViewRow row = menu.GetClickedElement<GridViewRow>();
            var item = row.DataContext as EmployeeScheduleProposalElement;

            if (item.Start >= DateTime.Today)
            {
                ProcessUserDialog<VacationWindow>(() => Model.RefreshDocuments(), new ParameterOverride("schedule", item));
            }
        }
    }
}
