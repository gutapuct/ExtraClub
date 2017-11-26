using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using Telerik.Windows.Controls;
using ExtraClub.CashRegisterModule;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls;
using ExtraClub.UIControls.Interfaces;
using ExtraClub.UIControls.Windows;

namespace ExtraClub.Clients.Views.Windows.Tickets
{
    /// <summary>
    /// Interaction logic for TicketChangeWindow.xaml
    /// </summary>
    public partial class TicketChangeWindow
    {

        #region DataContext
        public Ticket Ticket { get; set; }
        public Customer Customer { get; set; }
        public ICollectionView InstalmentsView { get; set; }

        public Instalment Instalment { get; set; }
        public Dictionary<decimal, string> Discounts { get; set; }

        private decimal? _discountPercent = 0;
        public decimal? DiscountPercent
        {
            get
            {
                return _discountPercent;
            }
            set
            {
                _discountPercent = value;
                OnPropertyChanged("Loan");
            }
        }

        public int? Bonuses
        {
            get
            {
                if (NewTicketType == null) return null;
                return (int)NewTicketType.Bonus - (int)Ticket.SerializedTicketType.Bonus;
            }
        }

        private TicketType _newTicketType;
        public TicketType NewTicketType
        {
            get
            {
                return _newTicketType;
            }
            set
            {
                _newTicketType = value;
                OnPropertyChanged("NewTicketType");
                OnPropertyChanged("Loan");
                OnPropertyChanged("FinishDate");
                OnPropertyChanged("UnitsLeft");
                OnPropertyChanged("Bonuses");
                OnPropertyChanged("GuestUnitsLeft");
            }
        }

        public ICollectionView TicketTypesView { get; set; }

        public decimal? Loan
        {
            get
            {
                if (NewTicketType == null) return null;
                //var oldTypeCost = _ticketTypes.Where(i => i.Id == Ticket.TicketTypeId).Select(i => i.Price).FirstOrDefault();
                //if (oldTypeCost == 0) oldTypeCost = Ticket.Cost;
                return (NewTicketType.Price * (1 - (DiscountPercent ?? 0)) - Ticket.Cost) + Ticket.Loan;
            }
        }

        public string FinishDate
        {
            get
            {
                if (NewTicketType == null) return null;
                var l = Ticket.LengthLeft + (NewTicketType.Length - Ticket.Length);
                return String.Format("{0}, " + UIControls.Localization.Resources.To + " {1:d}", l, DateTime.Today.AddDays(l));
            }
        }

        public decimal? UnitsLeft => Ticket.UnitsLeft + (NewTicketType?.Units - Ticket.UnitsAmount);

        public decimal? GuestUnitsLeft => Ticket.GuestUnitsLeft + (NewTicketType?.GuestUnits - Ticket.GuestUnitsAmount);

        #endregion

        private readonly IReportManager _repMan;
        private readonly CashRegisterManager _cashMan;

        public TicketChangeWindow(Ticket ticket, Customer customer, ClientContext context, IReportManager repMan, CashRegisterManager cashMan)
        {
            Ticket = ticket;
            Customer = customer;
            DataContext = this;
            _repMan = repMan;
            _cashMan = cashMan;
            Owner = Application.Current.MainWindow;

            var ticketTypes = context.GetActiveTicketTypesForCustomer(customer.Id);
            TicketTypesView = CollectionViewSource.GetDefaultView(ticketTypes);

            List<Instalment> lst = new List<Instalment>();
            lst.AddRange(context.GetCompanyInstalments(true));
            lst.Insert(0, new Instalment { Id = Guid.Empty, Name = UIControls.Localization.Resources.NoInstalment });
            InstalmentsView = CollectionViewSource.GetDefaultView(lst);
            InstalmentsView.MoveCurrentToFirst();
            Discounts = context.GetDiscountsForCurrentUser(DiscountTypes.TicketSale);

            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            var msg = String.Empty;
            if (NewTicketType == null) msg = UIControls.Localization.Resources.ProvideTicketType;
            if (NewTicketType != null && NewTicketType.Id == Ticket.SerializedTicketType.Id) msg = UIControls.Localization.Resources.TicketChangeWarning1;
            if (Loan.HasValue && Loan.Value < 0) msg = UIControls.Localization.Resources.TicketChangeWarning2;
            if (!String.IsNullOrEmpty(msg))
            {
                ExtraWindow.Alert(new DialogParameters
                {
                    Header = UIControls.Localization.Resources.Error,
                    Content = msg,
                    OkButtonContent = UIControls.Localization.Resources.Ok,
                    Owner = this
                });
                return;
            }

            //_context.PostTicketChange(Ticket.Id, NewTicketType.Id);
            //DialogResult = true;
            //Close();

            var pmt = new PaymentDetails(Customer.Id, Loan ?? 0, false);
            if (Instalment != null && Instalment.Id != Guid.Empty)
            {
                if (Instalment.ContribPercent.HasValue)
                {
                    pmt.RequestedAmount = Loan * Instalment.ContribPercent.Value ?? 0;
                }
                else
                {
                    pmt.RequestedAmount = Math.Min(Loan ?? 0, Instalment.ContribAmount ?? 0);
                }
            }


            _cashMan.ProcessPayment(pmt, new[] { new TicketChangeGood(Ticket.Id, NewTicketType, Instalment, pmt.RequestedAmount, DiscountPercent ?? 0) }, false, Customer, Guid.Empty, pd =>
            {
                if (pd.Success)
                {
                    if (PrintPdf.IsChecked ?? false)
                    {
                        _repMan.ProcessPdfReport(() => _context.GenerateLastTicketChangeReport(Customer.Id));
                    }

                    DialogResult = true;
                    Close();
                }
            });

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ExtraWindow.Confirm(UIControls.Localization.Resources.Cancel,
                 UIControls.Localization.Resources.TicketChangeCancelMessage,
                e1 =>
                {
                    if ((e1.DialogResult ?? false))
                    {
                        Ticket = null;
                        Close();
                    }
                });
        }
    }
}
