
using Microsoft.Practices.Unity;
using ExtraClub.UIControls;

namespace ExtraClub.EmployeesModule.Views
{
    public partial class VacanciesControl
    {
        public VacanciesControl()
        {
            InitializeComponent();
            NavigationManager.NavigateToJob += NavigationNavigateToJob;
        }

        private void NavigationNavigateToJob(object sender, GuidEventArgs e)
        {
            VacanciesTabControl.SelectedIndex = 0;
        }
    }
}
