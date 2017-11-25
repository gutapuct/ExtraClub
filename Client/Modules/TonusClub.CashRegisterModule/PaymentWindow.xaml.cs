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
using Telerik.Windows.Controls;
using TonusClub.ServiceModel;
using TonusClub.Infrastructure.Interfaces;
using TonusClub.Infrastructure;

namespace TonusClub.CashRegisterModule
{
    /// <summary>
    /// Interaction logic for PaymentWindow.xaml
    /// </summary>
    partial class PaymentWindow
    {
        PaymentWindowModel _model;

        private PaymentDetails _paymentDetails;
        public PaymentDetails PaymentDetails
        {
            get
            {
                return _paymentDetails;
            }
            set
            {
                _paymentDetails = value;
                _paymentDetails.CashPayment = _paymentDetails.RequestedAmountTotal;
                DepositRadio.IsEnabled = PaymentDetails.RequestedAmount <= _model.Customer.RurDepositValue;
                _model.PaymentDetails = _paymentDetails;
            }
        }
        internal PaymentWindow(PaymentWindowModel model)
        {
            var sects = Int32.Parse(AppSettingsManager.GetSetting("SectionsNumber"));
            if (sects > 4 || sects < 2)
            {
                model.SectionSelectionVisible = System.Windows.Visibility.Collapsed;
                model.Sections = new Dictionary<int, string> { { 1, "1" } };
            }
            else
            {
                model.SectionSelectionVisible = System.Windows.Visibility.Visible;
                model.Sections = new Dictionary<int, string>();
                for (int i = 1; i <= sects; i++)
                {
                    model.Sections.Add(i, i.ToString());
                }
            }
            _model = model;
            DataContext = _model;
            InitializeComponent();
            CardRadio.Visibility = model.Context.CurrentDivision.RBankCards ? Visibility.Visible : Visibility.Collapsed;
            this.Title = String.Format(UIControls.Localization.Resources.OrderPayment, model.PaymentDetails.OrderNumber);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CashRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (PaymentDetails == null) return;
            PaymentDetails.CashPayment = PaymentDetails.RequestedAmountTotal;
            PaymentDetails.DepositPayment = 0;
            PaymentDetails.CardPayment = 0;
        }

        private void DepositRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            PaymentDetails.CashPayment = 0;
            PaymentDetails.DepositPayment = PaymentDetails.RequestedAmountTotal;
            PaymentDetails.CardPayment = 0;
        }

        private void CardRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            PaymentDetails.CashPayment = 0;
            PaymentDetails.DepositPayment = 0;
            PaymentDetails.CardPayment = PaymentDetails.RequestedAmountTotal;
        }

    }
}
