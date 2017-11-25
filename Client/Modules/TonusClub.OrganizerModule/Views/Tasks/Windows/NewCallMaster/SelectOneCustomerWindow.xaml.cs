using System.Windows;
using TonusClub.ServiceModel;
using TonusClub.UIControls;

namespace TonusClub.OrganizerModule.Views.Tasks.Windows.NewCallMaster
{
    public partial class SelectOneCustomerWindow
    {
        public Customer CustomerResult { get; set; }

        public SelectOneCustomerWindow(ClientContext context)
        {
            InitializeComponent();
        }

        private void CustomerSearchControl_SelectedClientChanged(object sender, GuidEventArgs e)
        {
            CustomerResult = _context.GetCustomer(e.Guid);
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (CustomerResult != null)
            {
                DialogResult = true;
                Close();
            }
        }

    }
}
