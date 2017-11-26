using System;
using System.Windows;
using ExtraClub.ServiceModel;

namespace ExtraClub.TurnoverModule.Views.Windows
{
    /// <summary>
    /// Interaction logic for NewProviderPaymentWindow.xaml
    /// </summary>
    public partial class NewProviderPaymentWindow
    {
        #region DataContext
        public double Amount { get; set; }
        public string Comment { get; set; }
        public string PaymentType { get; set; }
        public DateTime Date { get; set; }
        public Consignment Order { get; set; }
        #endregion

        public NewProviderPaymentWindow(Consignment order)
        {
            Date = DateTime.Today;
            InitializeComponent();
            Order = order;
            DataContext = this;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ProcessButton_Click(object sender, RoutedEventArgs e)
        {
            if (Amount <= 0) return;

            _context.PostProviderPayment(Order.Id, Date, PaymentType, (decimal)Amount, Comment);
            DialogResult = true;
            Close();
        }
    }
}
