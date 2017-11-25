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
using TonusClub.OrganizerModule.Business;
using TonusClub.OrganizerModule.ViewModels;
using Microsoft.Practices.Unity;
using TonusClub.OrganizerModule.Views.Calls.Windows;
using TonusClub.UIControls;

namespace TonusClub.OrganizerModule.Views.Calls
{
    /// <summary>
    /// Interaction logic for CallListControl.xaml
    /// </summary>
    public partial class CallListControl
    {
        private OrganizerLargeViewModel Model
        {
            get
            {
                return DataContext as OrganizerLargeViewModel;
            }
        }

        public CallListControl()
        {
            InitializeComponent();
            NavigationManager.NewOutgoingCallRequest += NavigationManager_NewOutgoingCallRequest;
        }

        void NavigationManager_NewOutgoingCallRequest(object sender, ClientEventArgs e)
        {
            ProcessUserDialog<OutgoingCallWindow>(() => e.OnSuccess(Guid.Empty), new ParameterOverride("customerId", e.ClientId));
        }

        private void NewCallButton_Click(object sender, RoutedEventArgs e)
        {
            new CallManager(Model);
        }

        private void CallsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e))
            {
                ProcessUserDialog<CallDetailsWindow>(() => { }, new ResolverOverride[] { new ParameterOverride("call", Model.CallsView.CurrentItem) });
            }
        }

        private void NewOutCallButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<OutgoingCallWindow>(() => Model.RefreshCalls(), new ParameterOverride("customerId", Guid.Empty));
        }
    }
}
