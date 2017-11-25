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
using TonusClub.Clients.ViewModels;
using TonusClub.ServiceModel;
using TonusClub.UIControls.Windows;
using TonusClub.UIControls;
using Microsoft.Practices.Unity;
using TonusClub.Clients.Views.Windows;

namespace TonusClub.Clients.Views.ContainedControls
{
    public partial class CallsControl
    {
        private ClientLargeViewModel Model
        {
            get
            {
                return DataContext as ClientLargeViewModel;
            }
        }

        public CallsControl()
        {
            InitializeComponent();
        }

        private void NewCallClick(object sender, RoutedEventArgs e)
        {
            NavigationManager.MakeNewCustomerNotificationRequest(Model.CurrentCustomer, () => Model.RefreshCustomerEvents());
        }

        private void CallsViewGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e))
            {
                if (Model.CustomerEventsView.CurrentItem != null)
                {
                    var i = Model.CustomerEventsView.CurrentItem as CustomerEventView;
                    if (i.IsCall)
                    {
                        //TonusWindow.Alert(UIControls.Localization.Resources.CallReport, i.Comments);
                        NavigationManager.MakeCustomerNotificationTask(i.Id, () => Model.RefreshCustomerEvents());
                    }
                    else
                    {
                        ProcessUserDialog<NewEditCrmEventWindow>(() => Model.RefreshCustomerEvents(), new ParameterOverride("ev", i), new ParameterOverride("customer", Model.CurrentCustomer));
                    }
                }
            }
        }

        private void NewOutCallClick(object sender, RoutedEventArgs e)
        {
            NavigationManager.MakeNewOutgoingCallRequest(Model.CurrentCustomer, () => Model.RefreshCustomerEvents());
        }

        private void NewEventClick(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditCrmEventWindow>(() => Model.RefreshCustomerEvents(), new ParameterOverride("customer", Model.CurrentCustomer));
        }
    }
}
