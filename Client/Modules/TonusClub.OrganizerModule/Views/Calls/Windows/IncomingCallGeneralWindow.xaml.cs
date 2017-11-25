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
using System.Windows.Shapes;
using TonusClub.Infrastructure.Interfaces;
using TonusClub.OrganizerModule.Business;
using TonusClub.ServiceModel;
using TonusClub.UIControls;

namespace TonusClub.OrganizerModule.Views.Calls.Windows
{
    public partial class IncomingCallGeneralWindow
    {
        public IncomingResult Result { get; private set; }

        public Company Company { get; set; }

        public IncomingCallGeneralWindow(ClientContext context)
        {
            Company = context.CurrentCompany;
            DataContext = this;
            InitializeComponent();
            Result = IncomingResult.Cancelled;
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void NewCustomerClick(object sender, RoutedEventArgs e)
        {
            Result = IncomingResult.NewCustomer;
            Close();
        }

        private void NewScenarioClick(object sender, RoutedEventArgs e)
        {
            Result = IncomingResult.NewCustomerScrenario;
            Close();
        }

        private void OldCustomerClick(object sender, RoutedEventArgs e)
        {
            Result = IncomingResult.OldCustomer;
            Close();
        }

        private void NotACustomerClick(object sender, RoutedEventArgs e)
        {
            Result = IncomingResult.NotACustomer;
            Close();
        }
    }
}
