using System.ServiceModel;
using System.Windows;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls;

namespace ExtraClub.OrganizerModule.Views.Tasks.Windows
{
    /// <summary>
    /// Interaction logic for DepositOutWindow.xaml
    /// </summary>
    public partial class DepositOutWindow
    {
        public Customer Customer { get; set; }

        public DepositOut DepositOut { get; set; }

        public DepositOutWindow(ClientContext context, DepositOut item)
        {
            InitializeComponent();

            DepositOut = item;
            Customer = context.GetCustomer(DepositOut.CustomerId);

            DataContext = this;

        }

        private void MatchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _context.PostDepositOutDone(DepositOut.Id, DepositOut.Comment, true);
                DialogResult = true;
                Close();
            }
            catch (FaultException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UnmatchButton_Click(object sender, RoutedEventArgs e)
        {
            _context.PostDepositOutDone(DepositOut.Id, DepositOut.Comment, false);
            DialogResult = true;
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
