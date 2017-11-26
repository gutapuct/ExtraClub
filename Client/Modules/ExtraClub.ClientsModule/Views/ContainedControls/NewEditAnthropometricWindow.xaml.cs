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
using ExtraClub.ServiceModel;
using ExtraClub.Infrastructure.Interfaces;

namespace ExtraClub.Clients.Views.ContainedControls
{
    /// <summary>
    /// Interaction logic for NewEditAnthropometricWindow.xaml
    /// </summary>
    public partial class NewEditAnthropometricWindow
    {
        public Anthropometric Anthropometric { get; set; }
        public Customer Customer { get; set; }

        public NewEditAnthropometricWindow(Customer customer, Anthropometric anthro)
        {
            InitializeComponent();

            Customer = customer;

            if (anthro == null || anthro.Id == Guid.Empty)
            {
                Anthropometric = new Anthropometric
                {
                    CreatedOn=DateTime.Today,
                    CustomerId = customer.Id
                };
            }
            else
            {
                Anthropometric = anthro;
            }
            Anthropometric.Customer = customer;
            this.DataContext = Anthropometric;
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            var res = _context.PostCustomerAnthropomentric(Anthropometric);

            DialogResult = true;
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
