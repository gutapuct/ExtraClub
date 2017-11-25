using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Practices.Unity;
using TonusClub.EmployeesModule.ViewModels;
using TonusClub.EmployeesModule.Views.ContainedControls.Schedules.Windows;
using TonusClub.ServiceModel;
using TonusClub.ServiceModel.Employees;

namespace TonusClub.EmployeesModule.Views.ContainedControls.Schedules
{
    public partial class ArchivedVacations
    {
        private EmployeesLargeViewModel Model => DataContext as EmployeesLargeViewModel;

        public ArchivedVacations()
        {
            InitializeComponent();
        }

        public void NewVacationList_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewVacationListWindow>(() => Model.RefreshVacationHistory(), new ParameterOverride("list", new List<EmployeeScheduleProposalElement>()),
                new ParameterOverride("id", Guid.Empty));
        }

        private void NewOnSelectedClick(object sender, RoutedEventArgs e)
        {
            var list = Model.VacationsHistoryView.CurrentItem as VacationList;
            if (list != null)
                ProcessUserDialog<NewVacationListWindow>(() => Model.RefreshVacationHistory(), new ParameterOverride("list", new List<EmployeeScheduleProposalElement>()), new ParameterOverride("id", list.Id));
        }

        private void PrintSelectedClick(object sender, RoutedEventArgs e)
        {
            if (Model.VacationsHistoryView.CurrentItem == null) return;
            var list = (VacationList)Model.VacationsHistoryView.CurrentItem;
            ReportManager.ProcessPdfReport(() => ClientContext.GenerateEmployeeVacationList(list.Id));
        }
    }
}
