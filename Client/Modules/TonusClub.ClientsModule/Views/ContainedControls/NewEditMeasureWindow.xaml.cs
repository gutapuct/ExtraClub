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
using TonusClub.Infrastructure.Interfaces;
using System.ComponentModel;
using TonusClub.ServiceModel;

namespace TonusClub.Clients.Views.ContainedControls
{
    /// <summary>
    /// Interaction logic for NewEditMeasureWindow.xaml
    /// </summary>
    public partial class NewEditMeasureWindow
    {
        public CustomerMeasure CustomerMeasure { get; set; }
        public Customer Customer { get; set; }

        public NewEditMeasureWindow(Customer customer, CustomerMeasure measure)
        {
            InitializeComponent();

            var obj = _context.GetNutritionTemplates();

            Customer = customer;

            if (measure == null || measure.Id == Guid.Empty)
            {
                CustomerMeasure = new CustomerMeasure
                {
                    CreatedOn = DateTime.Today,
                    CustomerId = customer.Id,
                    Date = DateTime.Now,
                };
            }
            else
            {
                CustomerMeasure = measure;
            }

            Customer = customer;
            this.DataContext = this;
        }


        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            var res = _context.PostCustomerMeasure(CustomerMeasure);

            DialogResult = true;
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
