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
using ExtraClub.UIControls;

namespace ExtraClub.Clients.Views.Windows.Tickets
{
    public partial class CreditPaymentDialog
    {
        public Customer Customer { get; set; }
        public Ticket Ticket { get; set; }

        private decimal _BankComissionRur = 0;
        public decimal BankComissionRur
        {
            get
            {
                return _BankComissionRur;
            }
            set
            {
                _BankComissionRur = value;
                OnPropertyChanged("BankComissionRur");
                OnPropertyChanged("BankComissionPercent");
            }
        }
        public decimal BankComissionPercent
        {
            get
            {
                return BankComissionRur / Ticket.Loan;
            }
        }

        public DateTime PaymentDate { get; set; }

        public CreditPaymentDialog(ClientContext context, Customer customer, Ticket ticket)
            : base(context)
        {
            Customer = customer;
            Ticket = ticket;
            InitializeComponent();
            DataContext = this;

            PaymentDate = DateTime.Today;
            if (Ticket.InitialInstallmentPercent <= 10) BankComissionRur = Ticket.Loan * 0.015m;
            if (Ticket.InitialInstallmentPercent > 10 && Ticket.InitialInstallmentPercent <= 20) BankComissionRur = Ticket.Loan * 0.01m;
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            _context.PostCreditTicketPayment(Ticket.Id, BankComissionRur, PaymentDate);
            DialogResult = true;
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
