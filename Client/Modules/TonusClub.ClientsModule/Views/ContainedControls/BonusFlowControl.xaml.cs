using Microsoft.Practices.Unity;
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
using TonusClub.Clients.Views.Windows;
using TonusClub.Clients.Views.Windows.CustomerAndCards;
using TonusClub.UIControls.Windows;

namespace TonusClub.Clients.Views.ContainedControls
{
    public partial class BonusFlowControl
    {
        private ClientLargeViewModel Model
        {
            get
            {
                return DataContext as ClientLargeViewModel;
            }
        }

        public BonusFlowControl()
        {
            InitializeComponent();
        }

        private void AddBonus(object sender, RoutedEventArgs e)
        {
            if(Model.CurrentCustomer == null)
            {
                return;
            }

            ProcessUserDialog<AddBonusWindow>(() => Model.SelectClient(Model.CurrentCustomer.Id)
                , new ParameterOverride("customer", Model.CurrentCustomer)
                , new ParameterOverride("addBonus", true));
        }

        private void ChargeBonus(object sender, RoutedEventArgs e)
        {
            if(Model.CurrentCustomer == null)
            {
                return;
            }
            ProcessUserDialog<AddBonusWindow>(() => Model.SelectClient(Model.CurrentCustomer.Id)
                , new ParameterOverride("customer", Model.CurrentCustomer)
                , new ParameterOverride("addBonus", false));
        }

        private void CompensateClick(object sender, RoutedEventArgs e)
        {
            if(Model.CurrentCustomer == null)
            {
                return;
            }
            ProcessUserDialog<CompensateWindow>(() => Model.SelectClient(Model.CurrentCustomer.Id), new ParameterOverride("customer", Model.CurrentCustomer));
        }
    }
}
