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
using ExtraClub.EmployeesModule.ViewModels;
using ExtraClub.UIControls.Windows;
//using Telerik.Windows.Controls;
using ExtraClub.ServiceModel;
using Microsoft.Practices.Unity;
using ExtraClub.EmployeesModule.Views.ContainedControls.Vacancies.Windows;
using ExtraClub.EmployeesModule.Views.ContainedControls.Employees.Windows;
using Telerik.Windows.Controls.GridView;
using ExtraClub.UIControls;
using ExtraClub.Infrastructure.BaseClasses;
using ExtraClub.UIControls.BaseClasses;

namespace ExtraClub.EmployeesModule.Views.ContainedControls.Vacancies
{
    public partial class JobsControl
    {
        private EmployeesLargeViewModel Model
        {
            get
            {
                return this.DataContext as EmployeesLargeViewModel;
            }
        }

        public JobsControl()
        {
            InitializeComponent();
            NavigationManager.NavigateToJob += new EventHandler<GuidEventArgs>(Navigation_NavigateToJob);
        }

        void Navigation_NavigateToJob(object sender, GuidEventArgs e)
        {
            var job = ((List<Job>)Model.JobsView.SourceCollection).SingleOrDefault(j => j.Id == e.Guid);
            if (job != null)
            {
                Model.JobsView.MoveCurrentTo(job);
            }
        }

        private void JobsViewGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e))
            {
                if (!ClientContext.CheckPermission("JobsManagementBtns")) return;
                EditButton_Click(null, null);
            }
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditJobWindow>(() => Model.RefreshJobs());
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.JobsView.CurrentItem != null)
            {
                var cnt = ClientContext.GetActiveEmployeesCountForJobId(((Job)Model.JobsView.CurrentItem).Id);
                if (cnt > 0)
                {
                    ExtraWindow.Confirm("Редактирование должности",
                        "В данной доложности работает " + cnt + " сотрудников. Изменить характеристики должности с сегодняшнего числа?",
                    wnd =>
                    {
                        ProcessUserDialog<NewEditJobWindow>(() => Model.RefreshJobs(), new ParameterOverride("job", ViewModelBase.Clone<Job>(Model.JobsView.CurrentItem)));
                    });
                }
                else
                {
                    ProcessUserDialog<NewEditJobWindow>(() => Model.RefreshJobs(), new ParameterOverride("job", ViewModelBase.Clone<Job>(Model.JobsView.CurrentItem)));
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.JobsView.CurrentItem == null) return;
            var job = (Job)Model.JobsView.CurrentItem;
            var cnt = ClientContext.GetActiveEmployeesCountForJobId(job.Id);
            if (cnt == 0)
            {

                ExtraWindow.Confirm("Удаление",
                     "Удалить должность \"" + ((Job)Model.JobsView.CurrentItem).Name + "\"?",
                    e1 =>
                    {
                        if ((e1.DialogResult ?? false))
                        {
                            ClientContext.HideEmployeeJobById(job.Id);
                            Model.RefreshJobs();
                        }
                    });
            }
            else
            {
                var employee = ((List<Employee>)Model.EmployeesView.SourceCollection).FirstOrDefault(i => i.SerializedJobPlacement != null && i.SerializedJobPlacement.JobId == job.Id);
                if (employee != null)
                {
                    ExtraWindow.Confirm("Удаление должности", "По крайней мере один сотрудник занимает удаляемую должность.\nНеобходимо перевести его на другую должность.\nСделать это сейчас?",
                        e1 =>
                        {
                            {
                                ProcessUserDialog<JobChangeWindow>(() =>
                                {
                                    Model.RefreshEmployees();
                                    DeleteButton_Click(sender, e);
                                }, new ResolverOverride[] { new ParameterOverride("employee", employee) });

                            }
                        });
                }
            }
        }

        private void BaselineButton_Click(object sender, RoutedEventArgs e)
        {
            ClientContext.BaselineJobs();
            Model.RefreshBaselineStatus();
        }

        private void PrintScheduleReport(object sender, RoutedEventArgs e)
        {
            ReportManager.ProcessPdfReport(ClientContext.GenerateStateScheduleReport);
        }

        //private void JobsViewGrid_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        //{
        //    var row = e.Row as GridViewRow;
        //    if (row != null)
        //    {
        //        ToolTip toolTip = new ToolTip()
        //        {
        //            Content = row.DataContext,
        //            ContentTemplate = (DataTemplate)JobsViewGrid.LayoutRoot.Resources["MyToolTip"]
        //        };
        //        ToolTipService.SetToolTip(row, toolTip);
        //    } 
        //}
    }
}
