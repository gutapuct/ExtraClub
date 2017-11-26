using System.Linq;
using Microsoft.Practices.Unity;
using ExtraClub.EmployeesModule.ViewModels;
using ExtraClub.UIControls;

namespace ExtraClub.EmployeesModule.Views
{
    public partial class EmployeesControl
    {
        public EmployeesControl()
        {
            InitializeComponent();
            NavigationManager.EmployeeRequest += NavigationManagerEmployeeRequest;
            NavigationManager.NavigateToCashFlow += NavigationManagerNavigateToCashFlow;
        }

        void NavigationManagerNavigateToCashFlow(object sender, StringEventArgs e)
        {
            EmployeesTabPanel.SelectedIndex = 3;
        }

        void NavigationManagerEmployeeRequest(object sender, GuidEventArgs e)
        {
            EmployeesTabPanel.SelectedIndex = 0;
            var employeesLargeViewModel = DataContext as EmployeesLargeViewModel;
            if (employeesLargeViewModel != null)
            {
                var em = employeesLargeViewModel._EmployeesView.FirstOrDefault(i => i.Id == e.Guid);
                if (em != null)
                {
                    employeesLargeViewModel.EmployeesView.MoveCurrentTo(em);
                }
            }
        }

    }
}
