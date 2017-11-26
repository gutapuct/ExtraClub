using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Telerik.Windows.Controls.GridView;
using Microsoft.Practices.Unity;
using ExtraClub.ServiceModel;
using ExtraClub.OrganizerModule.Views.Windows;
using ExtraClub.OrganizerModule.ViewModels;
using ExtraClub.OrganizerModule.Views.Tasks.Windows;
using ExtraClub.UIControls;
using ExtraClub.ServiceModel.Reports;
using TicketReturnWindow = ExtraClub.OrganizerModule.Views.Tasks.Windows.TicketReturnWindow;

namespace ExtraClub.OrganizerModule.Views.Tasks
{
    /// <summary>
    /// Interaction logic for TaskListControl.xaml
    /// </summary>
    public partial class TaskListControl
    {

        private OrganizerLargeViewModel Model
        {
            get
            {
                return DataContext as OrganizerLargeViewModel;
            }
        }

        public TaskListControl()
        {
            InitializeComponent();

            NavigationManager.TaskRequest += NavigationManager_TaskRequest;
        }

        void NavigationManager_TaskRequest(object sender, ObjectEventArgs e)
        {
            var oi = e.Object as OrganizerItem;
            Execute(oi, e.OnOkay);
        }

        private void OrganizerTasksGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e))
            {
                ExecuteButton_Click(null, null);
            }
        }
        private void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.SelectedOrganizerItem == null) return;
            Execute(Model.SelectedOrganizerItem as OrganizerItem, () => { });
        }

        private void Execute(OrganizerItem item, Action onOkay)
        {
            if(item.Data is CustomerTarget)
            {
                ProcessUserDialog<CustomerTargetWindow>(() =>
                {
                    Model.RefreshTasks();
                    Model.RefreshArchivedTasks();
                    onOkay();
                }, new ResolverOverride[] { new ParameterOverride("item", item.Data) });

            }
            else if(item.Data is Ticket)
            {
                ProcessUserDialog<TicketReturnWindow>(() =>
                {
                    Model.RefreshTasks();
                    Model.RefreshArchivedTasks();
                    onOkay();
                }, new ResolverOverride[] { new ParameterOverride("item", item.Data) });

            }
            else if(item.Data is DepositOut)
            {
                ProcessUserDialog<DepositOutWindow>(() =>
                {
                    Model.RefreshTasks();
                    Model.RefreshArchivedTasks();
                    onOkay();
                }, new ResolverOverride[] { new ParameterOverride("item", item.Data) });

            }
            else if(item.Data is BarOrder)
            {
                ProcessUserDialog<CashlessPaymentWindow>(() =>
                {
                    Model.RefreshTasks();
                    Model.RefreshArchivedTasks();
                    onOkay();
                }, new ResolverOverride[] { new ParameterOverride("item", item.Data) });

            }
            else if(item.Data is CustomerNotification)
            {
                ProcessUserDialog<CustomerNotificationTaskWindow>(() =>
                {
                    Model.RefreshTasks();
                    Model.RefreshArchivedTasks();
                    onOkay();
                }, new ResolverOverride[] { new ParameterOverride("item", item.Data) });

            }
            else if(item.Data is CallInfo)
            {
                var ci = item.Data as CallInfo;
                var notInfo = new CustomerNotification { CustomerId = ci.CustomerId, Id=ci.Id, Message = ci.Description};
                ProcessUserDialog<CustomerNotificationTaskWindow>(() =>
                {
                    Model.RefreshTasks();
                    Model.RefreshArchivedTasks();
                    onOkay();
                }, new ResolverOverride[] { new ParameterOverride("item", notInfo) });
            }
            else if(item.Data is Task)
            {
                //Header="{Binding Task.Subject}" 

                ProcessUserDialog<GenericTaskWindow>(w =>
                {
                    if (w.DialogResult ?? false)
                    {
                        Model.RefreshTasks();
                        Model.RefreshArchivedTasks();
                        onOkay();
                    }
                },
                p =>
                {
                    p.Header = (item.Data as Task).Subject;
                },
                new ResolverOverride[] { new ParameterOverride("item", item) });

            }
        }

        private void OrganizerTasksGrid_RowLoaded(object sender, RowLoadedEventArgs e)
        {
            if (e.Row is GridViewHeaderRow || e.Row is GridViewNewRow)
                return;

            Binding colorBinding = new Binding("Background");
            e.Row.SetBinding(GridViewRow.BackgroundProperty, colorBinding);
        }

        private void CardButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.SelectedOrganizerItem == null) return;
            var prop = Model.SelectedOrganizerItem.Data.GetType().GetProperty("CustomerId");
            if(prop==null) return;

            NavigationManager.MakeClientRequest((Guid)prop.GetValue(Model.SelectedOrganizerItem.Data, null));
        }
    }
}
