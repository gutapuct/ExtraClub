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
using System.Windows.Shapes;
using TonusClub.CashRegisterModule;
using TonusClub.Infrastructure.Interfaces;
using TonusClub.ServiceModel;
using TonusClub.ServiceModel.Turnover;
using TonusClub.UIControls;
using System.Windows.Markup;
using System.Globalization;

namespace TonusClub.ScheduleModule.Views.Windows
{
    public partial class SolStartWindow
    {
        private Customer _Customer;
        public Customer Customer
        {
            get
            {
                return _Customer;
            }
            set
            {
                _Customer = value;
                OnPropertyChanged("Customer");
            }
        }

        private SolariumVisit _SolariumVisit;
        public SolariumVisit SolariumVisit
        {
            get
            {
                return _SolariumVisit;
            }
            set
            {
                _SolariumVisit = value;
                OnPropertyChanged("SolariumVisit");
            }
        }

        decimal _cost;
        public decimal Cost
        {
            get
            {
                return _cost;
            }
            set
            {
                _cost = value;
                OnPropertyChanged("Cost");
            }
        }

        decimal _costTicket;
        public decimal CostTicket
        {
            get
            {
                return _costTicket;
            }
            set
            {
                _costTicket = value;
                OnPropertyChanged("CostTicket");
            }
        }

        private List<Ticket> tickets;

        private CashRegisterManager _cashRegister;

        public SolStartWindow(ClientContext context, Guid visitId, Guid customerId, CashRegisterManager cashRegister)
            : base(context)
        {
            InitializeComponent();
            _cashRegister = cashRegister;
            Customer = _context.GetCustomer(customerId, true);
            SolariumVisit = _context.GetSolariumVisitById(visitId);

            tickets = _context.GetCustomerTickets(customerId);
            if (tickets.Any(i => i.Status == TicketStatus.Active && i.SolariumMinutesLeft >= SolariumVisit.Amount))
            {
            }
            else if (tickets.Any(i => i.Status == TicketStatus.Available && i.SolariumMinutesLeft >= SolariumVisit.Amount))
            {
                TicketButton.Content = UIControls.Localization.Resources.TicketActivation2;
            }
            else
            {
                TicketButton.Content = UIControls.Localization.Resources.NewTicket;
            }
            Cost = SolariumVisit.Amount * SolariumVisit.SerializedPrice;
            CostTicket = SolariumVisit.Amount * SolariumVisit.SerializedPriceCoeff;
            this.DataContext = this;
        }

        private void CashButton_Click(object sender, RoutedEventArgs e)
        {
            _cashRegister.ProcessPayment(new SolariumGood(_context.CurrentDivision, SolariumVisit), Customer, pd =>
            {
                if (pd.Success)
                {
                    DialogResult = true;
                    Close();
                }
            });
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TicketButton_Click(object sender, RoutedEventArgs e)
        {
            if (!tickets.Any(i => i.SolariumMinutesLeft >= SolariumVisit.Amount))
            {
                NavigationManager.MakeNewTicketRequest(Customer.Id, () =>
                {
                    tickets = _context.GetCustomerTickets(Customer.Id);
                    if (!tickets.Any(i => i.SolariumMinutesLeft >= SolariumVisit.Amount))
                    {
                        return;
                    }
                    else
                    {
                        _context.PostSolariumVisitStart(SolariumVisit.Id);
                        DialogResult = true;
                        Close();
                    }
                });
            }
            else
            {
                _context.PostSolariumVisitStart(SolariumVisit.Id);
                DialogResult = true;
                Close();
            }
        }
    }
}
