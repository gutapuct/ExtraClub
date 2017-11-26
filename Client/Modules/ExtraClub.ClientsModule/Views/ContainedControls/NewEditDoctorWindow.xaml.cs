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
using System.ComponentModel;

namespace ExtraClub.Clients.Views.ContainedControls
{
    /// <summary>
    /// Interaction logic for NewEditDoctorWindow.xaml
    /// </summary>
    public partial class NewEditDoctorWindow
    {
        public DoctorVisit DoctorVisit { get; set; }

        public Customer Customer { get; set; }

        private List<string> doctorTemplates = new List<string>();
        public ICollectionView DoctorTemplates { get; set; }

        public NewEditDoctorWindow(Customer customer, DoctorVisit doctor)
        {
            InitializeComponent();

            doctorTemplates = _context.GetGetDoctorTemplates();

            DoctorTemplates = CollectionViewSource.GetDefaultView(doctorTemplates);

            Customer = customer;

            if (doctor == null || doctor.Id == Guid.Empty)
            {
                DoctorVisit = new DoctorVisit
                {
                    CreatedOn = DateTime.Today,
                    CustomerId = customer.Id,
                    Date = DateTime.Now,
                };
            }
            else
            {
                DoctorVisit = doctor;
            }

            DoctorVisit.Customer = customer;
            this.DataContext = this;
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            var res = _context.PostCustomerDoctorVisit(DoctorVisit);

            DialogResult = true;
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
