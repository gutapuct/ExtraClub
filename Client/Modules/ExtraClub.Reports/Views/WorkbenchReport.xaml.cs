using System;
using System.Data;
using System.Windows;
using System.Windows.Input;
using ExtraClub.Reports.ViewModels;
using ExtraClub.ServiceModel;
using ExtraClub.ServiceModel.Reports;
using ExtraClub.UIControls;

namespace ExtraClub.Reports.Views
{
    public partial class WorkbenchReport
    {
        public WorkbenchReportViewModel Model { get; set; }

        public WorkbenchReport(WorkbenchReportViewModel model)
        {
            InitializeComponent();
            DataContext = Model = model;
        }

        private void Customer1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var id = (((FrameworkElement)sender).DataContext as VisitInfo).CustomerId;
            NavigationManager.MakeClientRequest(id);
        }

        private void Customer2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var id = (((FrameworkElement)sender).DataContext as CallInfo).CustomerId;
            NavigationManager.MakeClientRequest(id);
        }

        private void Task_Click(object sender, RoutedEventArgs e)
        {
            var item = ((FrameworkElement)sender).DataContext as OrganizerItem;
            NavigationManager.MakeTaskRequest(item, () => Model.RefreshDataAsync());
        }

        private void Call_Click(object sender, RoutedEventArgs e)
        {
            var item = ((FrameworkElement)sender).DataContext as CallInfo;
            NavigationManager.MakeTaskRequest(new OrganizerItem { Data = item }, () => Model.RefreshDataAsync());
        }

        private void CustomerIn_Click(object sender, RoutedEventArgs e)
        {
            var item = ((FrameworkElement)sender).DataContext as VisitInfo;
            NavigationManager.MakeClientInRequest(item.CustomerId, () => Model.RefreshDataAsync());
        }

        private void CustomerOut_Click(object sender, RoutedEventArgs e)
        {
            var item = ((FrameworkElement)sender).DataContext as VisitInfo;
            NavigationManager.MakeClientOutRequest(item.CustomerId, () => Model.RefreshDataAsync());
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt = new DataTable();
            DataColumn dc = new DataColumn("Заголовок", typeof(string));
            dt.Columns.Add(dc);
            dc = new DataColumn("Время события", typeof(string));
            dt.Columns.Add(dc);
            dc = new DataColumn("Клиент", typeof(string));
            dt.Columns.Add(dc);
            dc = new DataColumn("Доп. информация", typeof(string));
            dt.Columns.Add(dc);

            if (Model.WorkbenchInfo != null)
            {
                if (Model.WorkbenchInfo.CustomerVisits != null)
                {
                    dt.Rows.Add();
                    dt.Rows.Add("Запланированные на сегодня посещения", "", "", "");
                    dt.Rows.Add();
                    foreach (var item in Model.WorkbenchInfo.CustomerVisits)
                    {
                        dt.Rows.Add(item.TreatmnetNames, item.VisitTime, item.FullName, item.Phone);
                    }
                }
                if (Model.WorkbenchInfo.CustomerCalls != null)
                {
                    dt.Rows.Add();
                    dt.Rows.Add("Запланированные на сегодня звонки", "", "", "");
                    dt.Rows.Add();
                    foreach (var item in Model.WorkbenchInfo.CustomerCalls)
                    {
                        dt.Rows.Add(item.Description, String.Format("Позвонить до {0:HH.mm}", item.Deadline), item.FullName, item.Phone);
                    }
                }
                if (Model.WorkbenchInfo.CustomerTasks != null)
                {
                    dt.Rows.Add();
                    dt.Rows.Add("Задачи на сегодня", "", "", "");
                    dt.Rows.Add();
                    foreach (var item in Model.WorkbenchInfo.CustomerTasks)
                    {
                        dt.Rows.Add(item.Category, String.Format("Выполнить до {0:HH.mm}", item.AppearDate), item.Text, item.CompletionComment);
                    }
                }
            }
            ReportManager.ExportDataTableToExcel("План на текущий месяц", dt);
        }
    }
}
