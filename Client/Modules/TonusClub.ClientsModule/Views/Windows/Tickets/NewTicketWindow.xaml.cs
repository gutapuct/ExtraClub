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
using TonusClub.CashRegisterModule;
using TonusClub.Infrastructure.Interfaces;
using TonusClub.ServiceModel;
using System.ComponentModel;
using TonusClub.Clients.Business;
using TonusClub.UIControls;
using TonusClub.UIControls.Windows;
using System.Threading;
using TonusClub.UIControls.Interfaces;

namespace TonusClub.Clients.Views
{
    public partial class NewTicketWindow : TonusClub.UIControls.WindowBase, INotifyPropertyChanged
    {
        public bool CanSale { get; set; }
        private IDictionaryManager _dictMan;
        private CashRegisterManager _cashMan;
        public ICollectionView TicketTypesView { get; set; }
        public ICollectionView InstalmentsView { get; set; }

        public CumulativeDiscountInfo CumInfo { get; set; }
        public bool Cashless { get; set; }

        public Dictionary<decimal, string> Discounts { get; set; }
        public Dictionary<decimal, string> DiscountsRub { get; set; }

        public decimal PaidAmt { get; set; }

        public List<decimal> VATs { get; set; }

        public DateTime CorrDate { get; set; }
        public int CorrAmount { get; set; }
        public int CorrGuest { get; set; }
        public int CorrSol { get; set; }

        bool _isPartialPayment;
        public bool IsPartialPayment
        {
            get
            {
                return _isPartialPayment;
            }
            set
            {
                _isPartialPayment = value;
                if (_isPartialPayment)
                {
                    Ticket.InstalmentId = null;
                }
                OnPropertyChanged("IsPartialPayment");
            }
        }

        public decimal PartialPayment { get; set; }

        public Visibility VatVisibility { get; set; }

        decimal? _discountRub;
        public decimal? DiscountRub
        {
            get
            {
                return _discountRub;
            }
            set
            {
                _discountRub = value;
                Ticket.DiscountPercent = 0;
                if (_ticketTypes.Where(i => i.Id == Ticket.TicketTypeId).Select(i => i.Price).FirstOrDefault() > (value ?? 0m))
                {
                    Ticket.Price = _ticketTypes.Where(i => i.Id == Ticket.TicketTypeId).Select(i => i.Price).FirstOrDefault() - (value ?? 0m);
                }
                OnPropertyChanged("DiscountRub");
            }
        }

        public bool IsNotGuest
        {
            get
            {
                return CurrentTicketType == null || !CurrentTicketType.IsGuest;
            }
        }

        public bool IsGuest
        {
            get
            {
                return !IsNotGuest;
            }
        }

        public int MaxGuestUnits { get; set; }

        private int _guestUnits;
        public int GuestUnits
        {
            get
            {
                return _guestUnits;
            }
            set
            {
                _guestUnits = value;
                OnPropertyChanged("GuestUnits");
            }
        }

        bool _hasCredit;

        public bool HasCredit
        {
            get
            {
                return _hasCredit;
            }
            set
            {
                _hasCredit = value;
                if(_hasCredit)
                {
                    Ticket.InstalmentId = null;
                }
                else
                {
                    Ticket.CreditInitialPayment = null;
                }
                OnPropertyChanged("HasCredit");
                OnPropertyChanged("HasNotCredit");
            }
        }

        public bool HasNotCredit
        {
            get
            {
                return !_hasCredit;
            }
        }

        public bool PrintPdf { get; set; }

        private IEnumerable<TicketType> _ticketTypes;
        private IReportManager _repMan;

        public Ticket Ticket { get; private set; }

        public TicketType CurrentTicketType { get; private set; }

        public NewTicketWindow(CashRegisterManager cashMan, IDictionaryManager dictMan,
            ClientContext context, Customer customer, IReportManager repMan)
            : base(context)
        {
            InitializeComponent();
            CumInfo = context.GetCumulativeDiscountInfo(customer.Id);
            VatVisibility = Thread.CurrentThread.CurrentUICulture.IetfLanguageTag == "ru-RU" ? Visibility.Collapsed : Visibility.Visible;
            CanSale = (context.CurrentDivision.PresellDate ?? DateTime.Today) < DateTime.Now;
            if (!CanSale)
            {
                TonusWindow.Alert(UIControls.Localization.Resources.SaleForbidden,
                    UIControls.Localization.Resources.SaleForbiddenMessage);
            }
            VATs = new List<decimal> { 0, 5, 8, 17 };
            PrintPdf = true;
            _repMan = repMan;
            this.DataContext = this;
            Owner = Application.Current.MainWindow;
            _cashMan = cashMan;

            Ticket = new Ticket
            {
                AuthorId = context.CurrentUser.UserId,
                CreatedOn = DateTime.Now,
                Customer = customer,
                CustomerId = customer.Id,
                DiscountPercent = CumInfo.DiscountPercent
            };

            _dictMan = dictMan;

            Ticket.DivisionId = context.CurrentDivision.Id;

            _ticketTypes = context.GetActiveTicketTypesForCustomer(customer.Id);
            TicketTypesView = CollectionViewSource.GetDefaultView(_ticketTypes);
            List<Instalment> lst = new List<Instalment>();
            lst.AddRange(context.GetCompanyInstalments(true));
            InstalmentsView = CollectionViewSource.GetDefaultView(lst);
            InstalmentsView.MoveCurrentToFirst();
            Ticket.PropertyChanged += new PropertyChangedEventHandler(Ticket_PropertyChanged);

            Discounts = context.GetDiscountsForCurrentUser(DiscountTypes.TicketSale);
            DiscountsRub = new Dictionary<decimal, string>();
            foreach (var d in Discounts.Where(i => i.Key < 0).ToArray())
            {
                DiscountsRub.Add(-d.Key, String.Format("{0:c0}", -d.Key));
                Discounts.Remove(d.Key);
            }
            if (!Discounts.ContainsKey(0))
            {
                Discounts.Add(0, "0%");
            }

            if(!Discounts.ContainsKey(0.07m) && customer.MarketingPassed)
            {
                Discounts.Add(0.07m, "7%");
            }


            if (!Discounts.ContainsKey(CumInfo.DiscountPercent) && CumInfo.DiscountPercent > 0)
            {
                Discounts.Add(CumInfo.DiscountPercent, "Накопительная скидка");
            }
            if (customer.InvitorId.HasValue)
            {
                MaxGuestUnits = context.GetMaxGuestUnits(context.CurrentDivision.Id, customer.InvitorId.Value);
            }
            CorrDate = DateTime.Today;
        }

        void Ticket_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "TicketTypeId")
            {
                if (Ticket.TicketTypeId != Guid.Empty)
                {
                    CurrentTicketType = _ticketTypes.First(tt => tt.Id == Ticket.TicketTypeId);
                    Ticket.UnitsAmount = CurrentTicketType.Units;
                    Ticket.GuestUnitsAmount = CurrentTicketType.GuestUnits;
                    Ticket.Length = CurrentTicketType.Length;
                    Ticket.Price = CurrentTicketType.Price;
                    if (DiscountRub.HasValue)
                    {
                        DiscountRub = DiscountRub;
                    }
                    OnPropertyChanged("CurrentTicketType");
                    OnPropertyChanged("IsGuest");
                    OnPropertyChanged("IsNotGuest");
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            TonusWindow.Confirm(UIControls.Localization.Resources.Cancel,
                 UIControls.Localization.Resources.NewTicketCancelWarning,
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
            if (Ticket.DivisionId == Guid.Empty) return;
            if (Ticket.TicketTypeId == Guid.Empty) return;
            if (CurrentTicketType.IsGuest && GuestUnits == 0) return;

            var evs = _context.GetCustomerEvents(Ticket.Customer.Id, DateTime.Today.AddMonths(-1), DateTime.Now.AddMonths(1), false);

            CheckTreatments(evs, () =>
            {
                if (IsGuest)
                {
                    Ticket.UnitsAmount = GuestUnits;
                }
                _cashMan.ProcessPayment(new TicketGood(Ticket, PaidAmt, IsPartialPayment ? (decimal?)PartialPayment : null), Ticket.Customer, pd =>
                {
                    if (pd.Success)
                    {
                        if (CorrBox.IsChecked ?? false)
                        {
                            _context.PostLastTicketCorrection(Ticket.Customer.Id, CorrDate, CorrAmount, CorrGuest, PaidAmt, CorrSol);
                        }

                        if (PrintPdf)
                        {
                            _repMan.ProcessPdfReport(() => _context.GenerateLastTicketSaleReport(Ticket.Customer.Id));
                        }

                        DialogResult = true;
                        Close();
                    }
                }, Cashless);
            });
        }

        //TODO: TEST STRONGLY!
        private void CheckTreatments(List<TreatmentEvent> evs, Action onOk)
        {
            var problem = evs.Where(ev => ev.VisitStatus == (short)TreatmentEventStatus.Planned && !ev.TicketId.HasValue && CurrentTicketType.SerializedTreatmentTypes.Any(tt => tt.Id == ev.SerializedTreatmentTypeId)).ToList();
            ThreadPool.QueueUserWorkItem(_ =>
            {
                var r = true;
                problem.ForEach(te =>
                {
                    if (r)
                    {
                        ConfirmWindow res = null;
                        Dispatcher.Invoke(new Action(() =>
                        {
                            res = TonusWindow.Confirm(UIControls.Localization.Resources.Warning,
                                 String.Format(UIControls.Localization.Resources.TreatmentPlanCancelWarning, te.SerializedTreatmentName),
                                e1 => { });
                        }));
                        while (res == null || !res.DialogResult.HasValue)
                        {
                            Thread.Sleep(500);
                        }
                        r = res.DialogResult.Value;
                    }
                });
                if (r)
                {
                    Dispatcher.Invoke(new Action(() => onOk()));
                }
            });

        }
        bool block;
        private void RadComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (block) return;
            var perc = Ticket.DiscountPercent;
            DiscountRub = null;
            block = true;
            Ticket.DiscountPercent = perc;
            Ticket.Price = _ticketTypes.Where(i => i.Id == Ticket.TicketTypeId).Select(i => i.Price).FirstOrDefault();
            block = false;
        }
    }
}
