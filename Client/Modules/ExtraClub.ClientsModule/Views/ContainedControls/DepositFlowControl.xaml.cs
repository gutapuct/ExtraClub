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
using Microsoft.Practices.Unity;
using ExtraClub.Clients.ViewModels;
using ExtraClub.Clients.Views.Windows;

namespace ExtraClub.Clients.Views.ContainedControls
{
    /// <summary>
    /// Interaction logic for DepositFlowControl.xaml
    /// </summary>
    public partial class DepositFlowControl
    {
        private ClientLargeViewModel Model
        {
            get
            {
                return DataContext as ClientLargeViewModel;
            }
        }

        public DepositFlowControl()
        {
            InitializeComponent();
        }

        private void PayInClick(object sender, RoutedEventArgs e)
        {
            if (Model.CurrentCustomer != null)
            {
                ProcessUserDialog<AddDepositPayment>(() => Model.RefreshDeposit(), new ResolverOverride[] { new ParameterOverride("customer", Model.CurrentCustomer) });
            }
        }

        private void PayOutClick(object sender, RoutedEventArgs e)
        {
            if (Model.CurrentCustomer != null)
            {
                ProcessUserDialog<DepositOutWindow>(() => Model.RefreshDeposit(), new ResolverOverride[] { new ParameterOverride("customer", Model.CurrentCustomer) });
            }
        }
    }
}
