using System;
using System.Windows;
using ExtraClub.EmployeesModule.ViewModels;
using ExtraClub.EmployeesModule.Views.ContainedControls.Employees.Windows;
using Microsoft.Practices.Unity;
using Telerik.Windows.Controls;
using Telerik.Windows;
using Telerik.Windows.Controls.GridView;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls;

namespace ExtraClub.EmployeesModule.Views.ContainedControls.Employees
{
    /// <summary>
    /// Interaction logic for EmployeesControl.xaml
    /// </summary>
    public partial class EmployeesControl
    {
        private EmployeesLargeViewModel Model => DataContext as EmployeesLargeViewModel;

        public EmployeesControl()
        {
            InitializeComponent();
        }

        private void NewButtonClick(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditEmployeeWindow>(() =>
            {
                Model.RefreshEmployees();
                Model.RefreshDocuments();
            });
        }

        private void RadContextMenuItemClick(object sender, RadRoutedEventArgs e)
        {
            var menu = (RadContextMenu)sender;
            var clickedItem = e.OriginalSource as RadMenuItem;
            var row = menu.GetClickedElement<GridViewRow>();
            var employee = row.DataContext as Employee;
            if (clickedItem != null)
            {
                switch (clickedItem.Name)
                {
                    case "CardItem":
                        ProcessUserDialog<NewEditEmployeeWindow>(() => Model.RefreshEmployees(), new ParameterOverride("employee", row.DataContext));
                        break;
                    case "JobApplyItem":
                        ProcessUserDialog<JobApplyWindow>(() =>
                        {
                            Model.RefreshEmployees();
                            Model.RefreshDocuments();
                        }, new ParameterOverride("employee", row.DataContext));
                        break;
                    //case "PrintJobApplyItem":
                    //    IReportManager.ProcessPdfReport(Context.GenerateApplyOrder, employee.Id);
                    //    break;
                    //case "PrintJobChangeItem":
                    //    IReportManager.ProcessPdfReport(Context.GenerateJobChangeOrder, employee.Id);
                    //    break;
                    case "PrintJobAgreementItem":
                        if (employee != null)
                            ReportManager.ProcessPdfReport(()=>ClientContext.GenerateJobAgreement(employee.Id));
                        break;
                    case "PrintResonsibilityItem":
                        if (employee != null)
                            ReportManager.ProcessPdfReport(() => ClientContext.GenerateResponsibleAgreement(employee.Id));
                        break;
                    case "PrintSecurityItem":
                        if (employee != null)
                            ReportManager.ProcessPdfReport(() => ClientContext.GenerateSecretAgreement(employee.Id));
                        break;
                    case "JobDescriptionItem":
                        if (employee != null)
                            ReportManager.ProcessPdfReport(() => ClientContext.GenerateJobDescription(employee.Id));
                        break;
                    case "EmitCardItem":
                        ProcessUserDialog<EmitCardWindow>(() => Model.RefreshEmployees(), new ParameterOverride("employee", row.DataContext));
                        break;
                    case "NavigateToJobItem":
                        var jp = ((Employee)row.DataContext).SerializedJobPlacement;
                        if (jp != null)
                        {
                            NavigationManager.OnNavigateToJob(jp.JobId);
                        }
                        break;
                    case "ChangeJobItem":
                        ProcessUserDialog<JobChangeWindow>(() =>
                        {
                            Model.RefreshEmployees();
                            Model.RefreshDocuments();
                        }, new ParameterOverride("employee", row.DataContext));

                        break;
                    case "FireItem":
                        ProcessUserDialog<FireEmployeeWindow>(() =>
                        {
                            Model.RefreshEmployees();
                            Model.RefreshDocuments();
                        }, new ParameterOverride("employee", row.DataContext));

                        break;
                    case "VacationItem":
                        ProcessUserDialog<VacationWindow>(() => Model.RefreshDocuments(), new ParameterOverride("employee", row.DataContext));

                        break;
                    case "IllItem":
                        ProcessUserDialog<IllWindow>(() => Model.RefreshDocuments(), new ParameterOverride("employee", row.DataContext));
                        break;
                    case "MissItem":
                        ProcessUserDialog<MissWindow>(() => Model.RefreshDocuments(), new ParameterOverride("employee", row.DataContext));
                        break;
                    case "PreferencesItem":
                        ProcessUserDialog<EmployeePreferencesWindow>(() => { }, new ParameterOverride("employee", row.DataContext));
                        break;
                    case "Cashflow":
                        var emp = ((Employee)row.DataContext).SerializedCustomer.FullName;
                        NavigationManager.OnNavigateToCashFlow(emp);
                        return;
                    case "LoginInfo":
                        var empId = ((Employee)row.DataContext).Id;
                        NavigationManager.MakeActivateLoginsRequest(empId);
                        return;
                }
            }
        }

        private void RadContextMenuOpened(object sender, RoutedEventArgs e)
        {
            var menu = (RadContextMenu)sender;
            var row = menu.GetClickedElement<GridViewRow>();

            if (row != null)
            {
                row.IsSelected = row.IsCurrent = true;
                var cell = menu.GetClickedElement<GridViewCell>();
                if (cell != null)
                {
                    cell.IsCurrent = true;
                }
                var isFired = false;
                if (((Employee)row.DataContext).SerializedJobPlacement != null)
                {
                    isFired = ((Employee)row.DataContext).SerializedJobPlacement.FireDate.HasValue;
                }

                JobApplyItem.Visibility = ((((Employee)row.DataContext).SerializedJobPlacement == null || isFired) && ((Employee)row.DataContext).IsActive) ? Visibility.Visible : Visibility.Collapsed;
                IllItem.Visibility = MissItem.Visibility = FireItem.Visibility = VacationItem.Visibility = ChangeJobItem.Visibility = ChangeJobItem.Visibility = (((Employee)row.DataContext).SerializedJobPlacement != null) && !isFired ? Visibility.Visible : Visibility.Collapsed;

            }
            else
            {
                menu.IsOpen = false;
            }
        }

        private void TripButtonClick(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<TripWindow>(() => Model.RefreshDocuments());
        }

        private void CategoryButtonClick(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<CategoryWindow>(() => Model.RefreshDocuments(), new ParameterOverride("catId", Guid.Empty));
        }

        private void ActiveButtonClick(object sender, RoutedEventArgs e)
        {
            Model.SetEmployeeActive(true);
        }

        private void InactiveButtonClick(object sender, RoutedEventArgs e)
        {
            Model.SetEmployeeActive(false);
        }
    }
}
