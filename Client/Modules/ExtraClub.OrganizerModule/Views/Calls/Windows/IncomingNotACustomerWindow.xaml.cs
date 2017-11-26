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
using ExtraClub.OrganizerModule.Business;
using ExtraClub.ServiceModel;
using ExtraClub.Infrastructure.Interfaces;
using ExtraClub.UIControls;

namespace ExtraClub.OrganizerModule.Views.Calls.Windows
{
    /// <summary>
    /// Interaction logic for IncomingNotACustomerWindow.xaml
    /// </summary>
    public partial class IncomingNotACustomerWindow
    {
        public IncomingResult Result { get; set; }
        public Company Company { get; set; }
        public bool MakeTask { get; set; }

        public string Comments { get; set; }

        public IncomingNotACustomerWindow(ClientContext context)
        {
            InitializeComponent();
            Result = IncomingResult.Cancelled;
            Company = context.CurrentCompany;
            this.DataContext = this;
        }

        private void NewCustomerClick(object sender, RoutedEventArgs e)
        {
            Result = IncomingResult.NewCustomer;
            Close();
        }

        private void NewScrenarioClick(object sender, RoutedEventArgs e)
        {
            Result = IncomingResult.NewCustomerScrenario;
            Close();
        }

        private void OldCustomerClick(object sender, RoutedEventArgs e)
        {
            Result = IncomingResult.OldCustomer;
            Close();
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            Result = IncomingResult.SaveClicked;
            Close();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
