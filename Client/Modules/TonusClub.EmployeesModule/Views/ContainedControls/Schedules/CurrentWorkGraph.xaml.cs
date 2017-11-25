using System.Windows;
using TonusClub.EmployeesModule.Views.ContainedControls.Schedules.Windows;
using TonusClub.EmployeesModule.ViewModels;
using TonusClub.ServiceModel;

namespace TonusClub.EmployeesModule.Views.ContainedControls.Schedules
{
    public partial class CurrentWorkGraph
    {
        private EmployeesLargeViewModel Model => DataContext as EmployeesLargeViewModel;

        public CurrentWorkGraph()
        {
            InitializeComponent();
        }

        private void NewScheduleClick(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewWorkScheduleWindow>(() => Model.RefreshWorkGraphs());
        }

        private void PrintClick(object sender, RoutedEventArgs e)
        {
            if (Model.WorkGraphsView.CurrentItem == null) return;
            ReportManager.ProcessPdfReport(() => ClientContext.GenerateEmployeeScheduleReport(((EmployeeWorkGraph)Model.WorkGraphsView.CurrentItem).Id));
        }
    }
}
