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
using ExtraClub.CashRegisterModule;
using ExtraClub.ServiceModel;
using ExtraClub.Infrastructure.Interfaces;
using System.ComponentModel;
using ExtraClub.UIControls;
using ExtraClub.UIControls.Interfaces;
using ExtraClub.UIControls.Windows;

namespace ExtraClub.Clients.Views
{
    public partial class TicketFreezeWindow : ExtraClub.UIControls.WindowBase, INotifyPropertyChanged
    {

        public Customer Customer { get; set; }
        public Ticket Ticket { get; set; }
        public List<TicketFreezeReason> TicketFreezeReasons { get; set; }

        public string Comment { get; set; }

        private TicketFreezeReason _reason;
        public TicketFreezeReason TicketFreezeReason
        {
            get
            {
                return _reason;
            }
            set
            {
                _reason = value;
                AllowedStartSate = TicketFreezeReason.IsBackDayAllowed ? DateTime.MinValue : DateTime.Today;
                OnPropertyChanged("TicketFreezeReason");
                OnPropertyChanged("FreezeCost");
            }
        }


        private DateTime _startDate;
        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                OnPropertyChanged("StartDate");
                OnPropertyChanged("FreezeLength");
                OnPropertyChanged("FreezeCost");
            }
        }

        public decimal FreezeCost
        {
            get
            {
                return FreezeLength
                  * _context.CurrentCompany.FreezePrice
                  * (decimal)Customer.ActiveCard.SerializedCustomerCardType.FreezePriceCoeff
                  * (decimal)Ticket.SerializedTicketType.FreezePriceCoeff
                  * ((TicketFreezeReason == null) ? 1 : (decimal)TicketFreezeReason.K4);
            }
        }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                OnPropertyChanged("EndDate");
                OnPropertyChanged("FreezeLength");
                OnPropertyChanged("FreezeCost");
            }
        }

        private DateTime _allowedStartDate;
        public DateTime AllowedStartSate
        {
            get { return _allowedStartDate; }
            set
            {
                _allowedStartDate = value;
                if (_allowedStartDate < Ticket.CreatedOn) _allowedStartDate = Ticket.CreatedOn.Date;
                if (EndDate < AllowedStartSate) EndDate = AllowedStartSate;
                if (StartDate < AllowedStartSate) StartDate = AllowedStartSate;
                OnPropertyChanged("AllowedStartSate");
            }
        }

        public int FreezeLength
        {
            get
            {
                return (int)(EndDate - StartDate).TotalDays + 1;
            }
        }

        private CashRegisterManager _cashMan;
        private IReportManager _repMan;

        public TicketFreezeWindow(Customer customer, Ticket ticket, ClientContext context, CashRegisterManager cashMan, IReportManager repMan)
            : base(context)
        {
            InitializeComponent();
            _cashMan = cashMan;
            _repMan = repMan;
            this.Owner = Application.Current.MainWindow;
            Ticket = ticket;
            Customer = customer;
            StartDate = DateTime.Today.AddDays(1);
            EndDate = DateTime.Today.AddDays(4);
            AllowedStartSate = DateTime.Today;
            TicketFreezeReasons = context.GetTicketFreezeReasons().OrderBy(i => i.Name).ToList();
            DataContext = this;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ExtraWindow.Confirm(UIControls.Localization.Resources.Cancel,
                UIControls.Localization.Resources.FreezeCancelMessage,
                e1 =>
                {
                    if ((e1.DialogResult ?? false))
                    {
                        Ticket = null;
                        Close();
                    }
                });
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            var msg = String.Empty;
            if (TicketFreezeReason == null) msg = UIControls.Localization.Resources.ProvideFreezeCause;
            if (EndDate < StartDate) msg = UIControls.Localization.Resources.FreezeDatesMismatch;
            var f = (Ticket.SerializedTicketFreezes.FirstOrDefault(tf => tf.StartDate <= StartDate && tf.FinishDate >= StartDate)
                ?? Ticket.SerializedTicketFreezes.FirstOrDefault(tf => tf.StartDate <= EndDate && tf.FinishDate >= EndDate));
            if (f != null)
            {
                msg = String.Format(UIControls.Localization.Resources.AlreadyFreezedWarning, StartDate, EndDate);
            }
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

            _cashMan.ProcessPayment(new TicketFreezeGood(Ticket, StartDate, EndDate, TicketFreezeReason.Id, Comment, FreezeCost), Customer, pd =>
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
            if (TicketFreezeReason == null) return;
            _repMan.ProcessPdfReport(()=>_context.GenerateTicketFreezeStatementReport(Ticket.Id, new TicketFreeze
            {
                Comment = Comment,
                CreatedOn = DateTime.Now,
                FinishDate = EndDate,
                StartDate = StartDate,
                TicketFreezeReasonId = TicketFreezeReason.Id
            }));
        }

    }
}
