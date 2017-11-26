using System;
using System.Windows;
using Microsoft.Practices.Unity;
using Telerik.Windows.Controls;
using ExtraClub.CashRegisterModule;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls;
using ExtraClub.UIControls.Interfaces;
using ExtraClub.UIControls.Windows;

namespace ExtraClub.Clients.Views
{
    public partial class TicketRebillWindow
    {
        readonly IReportManager _repMan;
        readonly CashRegisterManager _cashMan;

        public Ticket Ticket { get; set; }
        public Customer OldCustomer { get; set; }

        private Customer _newCustomer;
        public Customer NewCustomer
        {
            get
            {
                return _newCustomer;
            }
            set
            {
                _newCustomer = value;
                OnPropertyChanged("NewCustomer");
            }
        }

        public TicketRebillWindow(Customer customer, Ticket ticket, IReportManager repMan, CashRegisterManager cashMan)
        {
            _repMan = repMan;
            _cashMan = cashMan;
            Ticket = ticket;
            OldCustomer = customer;

            InitializeComponent();

            DataContext = this;
        }

        private void CustomerSearch_SelectedClientChanged(object sender, GuidEventArgs e)
        {
            NewCustomer = _context.GetCustomer(e.Guid, true);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ExtraWindow.Confirm("Отмена",
                 "Вы отменяете передачу абонемента.\nВведённые данные не будут сохранены.\nПродолжить?",
                    e1 =>
                    {
                        if ((e1.DialogResult ?? false))
                        {
                            Ticket = null;
                            Close();
                        }
                    });
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            var msg = String.Empty;
            if (NewCustomer == null) msg = "Выберите клиента-получателя!";
            if (NewCustomer != null && NewCustomer.Id == OldCustomer.Id) msg = "Текущий владелец и получатель не могут быть одним лицом!";
            if (NewCustomer != null && NewCustomer.ActiveCard == null)
            {
                msg = "У нового клиента нет карты клиента!";
            }
            if ((bool) !StatementReceived.IsChecked)
            {
                msg = "Клиент должен подписать заявление о передаче абонемента!";
            }
            if (!String.IsNullOrEmpty(msg))
            {
                ExtraWindow.Alert(new DialogParameters
                {
                    Header = "Ошибка",
                    Content = msg,
                    OkButtonContent = "ОК",
                    Owner = this
                });
                return;
            }

            if (NewCustomer != null)
                _cashMan.ProcessPayment(new TicketRebillGood(Ticket, NewCustomer.Id, _context.CurrentCompany.TicketRebillCommission), OldCustomer, pd =>
                {
                    if (pd.Success)
                    {
                        DialogResult = true;
                        Close();
                    }
                });
        }

        private void PrintStatementButton_Click(object sender, RoutedEventArgs e)
        {
            if (NewCustomer != null) _repMan.ProcessPdfReport(() => _context.GenerateTicketRebillStatementReport(Ticket.Id, NewCustomer.Id));
        }

    }
}
