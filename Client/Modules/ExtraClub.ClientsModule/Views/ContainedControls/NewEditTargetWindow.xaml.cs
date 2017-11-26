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
    /// Interaction logic for NewEditTargetWindow.xaml
    /// </summary>
    public partial class NewEditTargetWindow
    {
        public CustomerTarget CustomerTarget { get; set; }

        public object TargetTypes { get; set; }

        public NewEditTargetWindow(Customer customer, CustomerTarget target, IDictionaryManager dictMan)
        {
            InitializeComponent();
            TargetTypes = dictMan.GetViewSource("CustomerTargetTypes");

            if (target == null || target.Id == Guid.Empty)
            {
                CustomerTarget = new CustomerTarget
                {
                    CreatedOn=DateTime.Today,
                    TargetDate=DateTime.Today.AddDays(7),
                    CustomerId = customer.Id    
                };
            }
            else
            {
                CustomerTarget = target;
            }

            CustomerTarget.Customer = customer;

            TargetDateInput.SelectableDateStart = DateTime.Today;
            DataContext = this;
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            var res = _context.PostCustomerTarget(CustomerTarget);

            DialogResult = true;
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
