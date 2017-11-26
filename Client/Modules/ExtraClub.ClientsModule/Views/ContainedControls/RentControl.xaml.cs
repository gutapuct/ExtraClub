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
using ExtraClub.Clients.ViewModels;
using Microsoft.Practices.Unity;
using ExtraClub.Clients.Views.Windows;
using ExtraClub.ServiceModel;

namespace ExtraClub.Clients.Views.ContainedControls
{
    public partial class RentControl
    {
        private ClientLargeViewModel Model
        {
            get
            {
                return DataContext as ClientLargeViewModel;
            }
        }

        public RentControl()
        {
            InitializeComponent();
        }

        private void NewRentClick(object sender, RoutedEventArgs e)
        {
            if (Model.CurrentCustomer != null)
                ProcessUserDialog<NewRentWindow>(()=>Model.RefreshRent(), new ResolverOverride[] { new ParameterOverride("customer", Model.CurrentCustomer) });

        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.RentView.CurrentItem != null)
            {
                var rent = Model.RentView.CurrentItem as Rent;
                if (rent.FactReturnDate.HasValue) return;
                ProcessUserDialog<CloseRentWindow>(() => Model.RefreshRent(), new ResolverOverride[] { new ParameterOverride("customer", Model.CurrentCustomer), new ParameterOverride("rent", rent) });
            }
        }
    }
}
