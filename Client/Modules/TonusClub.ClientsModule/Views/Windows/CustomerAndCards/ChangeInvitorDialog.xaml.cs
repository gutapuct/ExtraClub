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
using TonusClub.ServiceModel;
using TonusClub.UIControls;

namespace TonusClub.Clients.Views.Windows.CustomerAndCards
{
    public partial class ChangeInvitorDialog
    {
        public Guid? CustomerResult { get; set; }
        public Customer Customer;

        public ChangeInvitorDialog(IUnityContainer cont, Customer customer, ClientContext context)
            : base(context)
        {
            Customer = customer;
            CustomerResult = customer.InvitorId;
            InitializeComponent();
            if(customer.InvitorId.HasValue)
            {
                CustomerSearch.SelectById(customer.InvitorId.Value);
            }
        }

        private void CustomerSearchControl_SelectedClientChanged(object sender, GuidEventArgs e)
        {
            CustomerResult = e.Guid;
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if(CustomerResult != null)
            {
                DialogResult = true;
                _context.PostCustomerInvitor(Customer.Id, CustomerResult);
                Close();
            }
        }
    }
}
