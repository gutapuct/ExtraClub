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
using ExtraClub.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;
using Telerik.Windows.Controls;
using ExtraClub.Reports.Views.ContainedControls;
using ExtraClub.Reports.ViewModels;
using ExtraClub.Infrastructure.Events;
using ExtraClub.Reports.Views.ContainedControls.ReportDesigner;
using ExtraClub.ServiceModel.Reports;
using ExtraClub.UIControls;
using ExtraClub.UIControls.Windows;
using ExtraClub.Infrastructure;

namespace ExtraClub.Reports.Views
{
    /// <summary>
    /// Interaction logic for ReportsLargeView.xaml
    /// </summary>
    public partial class ReportsLargeView : ILargeSection
    {
        public ReportLargeViewModel Model { get; set; }

        public ReportsLargeView(ReportLargeViewModel model)
        {
            DataContext = Model = model;
            InitializeComponent();
            NavigationManager.TicketRemainReportRequest += NavigationManager_TicketRemainReportRequest;
            NavigationManager.UserActionsReportRequest += NavigationManager_UserActionsReportRequest;
            NavigationManager.AllCustomersReportRequest += NavigationManager_AllCustomersReportRequest;
            NavigationManager.AllTicketsReportRequest += NavigationManager_AllTicketsReportRequest;
        }

        void NavigationManager_AllTicketsReportRequest(object sender, EventArgs e)
        {
            Model.EnsureDataLoaded();
            var ex = ReportsTabPanel.Items.Cast<object>().Where(i => i.GetType() == typeof(TabReportItem)).Cast<TabReportItem>().FirstOrDefault(i => i.ReportKey == "GetAllTicketsEx");
            if (ex == null)
            {
                var item = ApplicationDispatcher.UnityContainer.Resolve<TabReportItem>(
                    new ParameterOverride("report", Model._reports.FirstOrDefault(i => i.Key == "GetAllTicketsEx")), new ParameterOverride("parameters", new object[] { new object() }));
                ReportsTabPanel.Items.Add(item);
                ReportsTabPanel.SelectedItem = item;
            }
            else
            {
                ReportsTabPanel.SelectedItem = ex;
            }
        }

        void NavigationManager_AllCustomersReportRequest(object sender, EventArgs e)
        {
            Model.EnsureDataLoaded();
            var ex = ReportsTabPanel.Items.Cast<object>().Where(i => i.GetType() == typeof(TabReportItem)).Cast<TabReportItem>().FirstOrDefault(i => i.ReportKey == "GetAllCustomersEx");
            if (ex == null)
            {
                var item = ApplicationDispatcher.UnityContainer.Resolve<TabReportItem>(
                    new ParameterOverride("report", Model._reports.FirstOrDefault(i => i.Key == "GetAllCustomersEx")), new ParameterOverride("parameters", new object[] { new object() }));
                ReportsTabPanel.Items.Add(item);
                ReportsTabPanel.SelectedItem = item;
            }
            else
            {
                ReportsTabPanel.SelectedItem = ex;
            }
        }

        void NavigationManager_UserActionsReportRequest(object sender, ObjectEventArgs e)
        {
            Model.EnsureDataLoaded();
            var item = ApplicationDispatcher.UnityContainer.Resolve<TabReportItem>(
                new ParameterOverride("report", Model._reports.FirstOrDefault(i => i.Key == "rep_GetLog")), new ParameterOverride("parameters", new object[] { e.Object, DateTime.Today.AddMonths(-1), DateTime.Today }));
            ReportsTabPanel.Items.Add(item);
            ReportsTabPanel.SelectedItem = item;
        }

        void NavigationManager_TicketRemainReportRequest(object sender, ObjectEventArgs e)
        {
            Model.EnsureDataLoaded();
            var item = ApplicationDispatcher.UnityContainer.Resolve<TabReportItem>(
                new ParameterOverride("report", Model._reports.FirstOrDefault(i => i.Key == "GetTicketRemainReport")), new ParameterOverride("parameters", new object[] { e.Object }));
            ReportsTabPanel.Items.Add(item);
            ReportsTabPanel.SelectedItem = item;
        }

        public object GetTransferDataForMinimize()
        {
            return null;
        }

        public object GetTransferDataForRestore()
        {
            return null;
        }

        public override void SetState(object data)
        {
            base.SetState(data);
            Model.EnsureDataLoading();
        }

        private void ReportsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e))
            {
                ProcessReportButton_Click(null, null);
            }
        }

        private void RemoveReportButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.ReportsView.CurrentItem == null) return;
            ExtraWindow.Confirm("Удаление отчета", "Удалить выделенный отчет?", (wnd) =>
            {
                if (wnd.DialogResult ?? false)
                {
                    var rep = Model.ReportsView.CurrentItem as ReportInfoInt;
                    ClientContext.DeleteReport(rep.Id, rep.Key, rep.Type);
                }
            });
        }

        private void EditReportButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.ReportsView.CurrentItem == null) return;
            var report = Model.ReportsView.CurrentItem as ReportInfoInt;
            Guid id;
            if (Guid.TryParse(report.Key, out id))
            {
                var rep = ClientContext.GetReportForEdit(id);
                if (rep != null)
                {
                    ProcessUserDialog<NewEditReportWindow>(() => Model.RefreshDataSync(), new ParameterOverride("report", rep));
                }
            }
        }

        private void NewReportButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditReportWindow>(() => Model.RefreshDataSync());
        }

        private void ProcessReportButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.ReportsView.CurrentItem != null)
            {
                var item = ApplicationDispatcher.UnityContainer.Resolve<TabReportItem>(
                    new ParameterOverride("report", Model.ReportsView.CurrentItem));
                ReportsTabPanel.Items.Add(item);
                ReportsTabPanel.SelectedItem = item;
            }
        }

        public void ProcessGoodDetailsReport(Guid divisionId, Guid goodId)
        {
            var item = ApplicationDispatcher.UnityContainer.Resolve<TabReportItem>(
                new ParameterOverride("report", Model._reports.FirstOrDefault(i => i.Key == "GetGoodDetails")), new ParameterOverride("parameters", new object[] { divisionId, goodId }));
            ReportsTabPanel.Items.Add(item);
            ReportsTabPanel.SelectedItem = item;
        }

        private void EmbedClick(object sender, RoutedEventArgs e)
        {
            Model.Button1Weight = FontWeights.DemiBold;

            NewReportButton.Visibility = System.Windows.Visibility.Collapsed;
            NewBasedReportButton.Visibility = System.Windows.Visibility.Collapsed;
            EditReportButton.Visibility = System.Windows.Visibility.Collapsed;
            RemoveReportButton.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void EmbedSaveClick(object sender, RoutedEventArgs e)
        {
            Model.Button2Weight = FontWeights.DemiBold;

            NewReportButton.Visibility = System.Windows.Visibility.Collapsed;
            EditReportButton.Visibility = System.Windows.Visibility.Collapsed;
            NewBasedReportButton.Visibility = System.Windows.Visibility.Collapsed;
            AuthorizationManager.SetElementVisible(RemoveReportButton);
        }

        private void ConstructorClick(object sender, RoutedEventArgs e)
        {
            Model.Button3Weight = FontWeights.DemiBold;

            AuthorizationManager.SetElementVisible(NewReportButton);
            AuthorizationManager.SetElementVisible(NewBasedReportButton);
            AuthorizationManager.SetElementVisible(EditReportButton);
            AuthorizationManager.SetElementVisible(RemoveReportButton);
        }

        private void ConstructorSaveClick(object sender, RoutedEventArgs e)
        {
            Model.Button4Weight = FontWeights.DemiBold;

            NewReportButton.Visibility = System.Windows.Visibility.Collapsed;
            EditReportButton.Visibility = System.Windows.Visibility.Collapsed;
            NewBasedReportButton.Visibility = System.Windows.Visibility.Collapsed;
            AuthorizationManager.SetElementVisible(RemoveReportButton);
        }

        private void NewBasedReportButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.ReportsView.CurrentItem == null) return;
            var report = Model.ReportsView.CurrentItem as ReportInfoInt;
            Guid id;
            if (Guid.TryParse(report.Key, out id))
            {
                var rep = ClientContext.GetReportForEdit(id);
                if (rep != null)
                {
                    rep.Key = Guid.Empty.ToString();
                    rep.Name += " - копия";
                    ProcessUserDialog<NewEditReportWindow>(() => Model.RefreshDataSync(), new ParameterOverride("report", rep));
                }
            }
        }
    }
}
