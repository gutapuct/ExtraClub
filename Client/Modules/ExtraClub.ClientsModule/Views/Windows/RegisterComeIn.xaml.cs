using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Telerik.Windows.Controls;
using ExtraClub.CashRegisterModule;
using ExtraClub.Infrastructure;
using ExtraClub.Infrastructure.Extensions;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls;
using ExtraClub.UIControls.Interfaces;
using ExtraClub.UIControls.Windows;

namespace ExtraClub.Clients.Views.Windows
{
    /// <summary>
    /// Interaction logic for RegisterComeIn.xaml
    /// </summary>
    public partial class RegisterComeIn
    {
        #region DataContext
        Customer _customer;
        public Customer Customer
        {
            get
            {
                return _customer;
            }
            set
            {
                _customer = value;
                if(Customer != null)
                {
                    _tickets = _context.GetTicketsForCustomer(Customer.Id);
                    var bs = _context.GetCustomerImage(Customer.Id);
                    if(bs != null)
                    {
                        var bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.StreamSource = new MemoryStream(bs);
                        bitmapImage.EndInit();
                        CustomerImage = bitmapImage;
                    }
                    else
                    {
                        CustomerImage = null;
                    }
                }
                else
                {
                    CustomerImage = null;
                }
                RefreshEvents();
                RefreshNotifications();
                OnPropertyChanged("Customer");
                OnPropertyChanged("CustomerImage");
            }
        }


        public BitmapSource CustomerImage { get; set; }

        List<Ticket> _tickets;

        private void RefreshNotifications()
        {
            if(_customer != null)
            {
                IsFirstVisit = _context.IsFirstVisitEnabled(_customer.Id);
                OnPropertyChanged("IsFirstVisit");

                HasNoTickets = _tickets != null && !_tickets.Any(i => i.Status == TicketStatus.Active || i.Status == TicketStatus.Available);
                OnPropertyChanged("HasNoTickets");

                var res = _context.GetGoodsReserve(_customer.Id);
                if(res.Any())
                {
                    HasReserve = true;
                    ReserveText = "У клиента в резерве есть товары: " + String.Join(", ", res.Select(i => i.GoodName).ToArray());
                }
                else
                {
                    HasReserve = false;
                }
                OnPropertyChanged("HasReserve");
                OnPropertyChanged("ReserveText");

            }
        }

        public bool IsFirstVisit { get; set; }
        public bool HasNoTickets { get; set; }


        public ICollectionView TreatmentEvents { get; private set; }
        private readonly List<TreatmentEvent> _treatmentEvents = new List<TreatmentEvent>();

        string _cardNumber;
        public string CardNumber
        {
            get
            {
                return _cardNumber;
            }
            set
            {
                if(String.IsNullOrWhiteSpace(value)) return;
                if(Customer == null || String.IsNullOrEmpty(_cardNumber))
                {
                    Customer = _context.GetCustomerByCard(int.Parse(value), true);
                    _cardNumber = value;
                }
            }
        }

        bool _post = true;
        public bool IsComeInEnabled
        {
            get
            {
                return _post;
            }
            set
            {
                _post = value;
                OnPropertyChanged("IsComeInEnabled");
            }
        }

        Visibility _programGridVisibility = Visibility.Collapsed;
        public Visibility ProgramGridVisibility
        {
            get
            {
                return _programGridVisibility;
            }
            set
            {
                _programGridVisibility = value;
                OnPropertyChanged("IsComeInEnabled");
            }
        }

        string _prog;
        public string ProgramName
        {
            get
            {
                return _prog;
            }
            set
            {
                ProgramGridVisibility = String.IsNullOrWhiteSpace(value) ? Visibility.Collapsed : Visibility.Visible;
                _prog = value;
                OnPropertyChanged("ProgramName");
            }
        }


        private int? _shelfNumber;
        public int? ShelfNumber
        {
            get
            {
                return _shelfNumber;
            }
            set
            {
                _shelfNumber = value;
                OnPropertyChanged("ShelfNumber");
            }
        }


        private int? _safeNumber;
        public int? SafeNumber
        {
            get
            {
                return _safeNumber;
            }
            set
            {
                _safeNumber = value;
                OnPropertyChanged("SafeNumber");
            }
        }

        public List<int> Shelves { get; set; }
        public List<int> Safes { get; set; }

        public bool HasReserve { get; set; }
        public string ReserveText { get; set; }

        #endregion

        readonly CashRegisterManager _cashMan;
        readonly IReportManager _repMan;

        public RegisterComeIn(ClientContext context, CashRegisterManager cashMan, Customer customer, IReportManager repMan)
            : base(context)
        {
            Shelves = _context.GetAvailableShelfNumbers(false);
            Safes = _context.GetAvailableShelfNumbers(true);

            if(customer.Id == Guid.Empty) customer = null;
            TreatmentEvents = CollectionViewSource.GetDefaultView(_treatmentEvents);
            Customer = customer;
            _cashMan = cashMan;
            _repMan = repMan;
            InitializeComponent();
            if(!context.CurrentDivision.HasChildren)
            {
                RegisterChild.Visibility = Visibility.Collapsed;
            }
            RefreshEvents();
            DataContext = this;
            if(String.IsNullOrWhiteSpace(context.CurrentDivision.ShelvesRepository))
            {
                ShelfAvail.Visibility = Visibility.Collapsed;
            }
            if(String.IsNullOrWhiteSpace(context.CurrentDivision.SafesRepository))
            {
                SafeAvail.Visibility = Visibility.Collapsed;
            }
            Owner = Application.Current.MainWindow;

            var pre = AppSettingsManager.GetSetting("PrereceiptSetting");
            if(String.IsNullOrEmpty(pre) || pre == "0")
            {
                PrintFR.IsChecked = true;
            }
            else if(pre == "1")
            {
                PrintPDF.IsChecked = true;
            }
            else
            {
                DoNotPrint.IsChecked = true;
            }
        }

        private void AddEventsButton_Click(object sender, RoutedEventArgs e)
        {
            if(Customer == null) return;
            NavigationManager.MakeScheduleRequest(new ScheduleRequestParams { Customer = Customer, OnClose = () => RefreshEvents() });
            RefreshEvents();
        }

        private void RefreshEvents()
        {
            if(Customer == null) return;
            ProgramName = "";
            _treatmentEvents.Clear();
            _treatmentEvents.AddRange(_context.GetCustomerEvents(Customer.Id, DateTime.Today, DateTime.Today.AddDays(1).AddMilliseconds(-1), false)
                .Where(i => i.VisitStatus == (short)TreatmentEventStatus.Planned).OrderBy(i => i.VisitDate));
            TreatmentEvents.Refresh();
            foreach(var e in _treatmentEvents)
            {
                if(e.ProgramId.HasValue)
                {
                    ProgramName = e.SerializedProgramName;
                }
                if(e.SerializedTicketNumber == UIControls.Localization.Resources.Reserve)
                {
                    ExtraWindow.Alert(new DialogParameters
                    {
                        Header = UIControls.Localization.Resources.Error,
                        Content = UIControls.Localization.Resources.ReserveError,
                        OkButtonContent = UIControls.Localization.Resources.Ok,
                        Owner = Application.Current.MainWindow
                    });
                    IsComeInEnabled = false;
                    break;
                }
                var t1 = _tickets.FirstOrDefault(t => t.Number == e.SerializedTicketNumber);
                if(t1 != null && t1.Status == TicketStatus.Freezed)
                {
                    AuthorizationManager.SetElementVisible(DefreezeDiv);
                }
                if(t1 != null && t1.Status == TicketStatus.Unpaid)
                {
                    ExtraWindow.Alert(new DialogParameters
                    {
                        Header = UIControls.Localization.Resources.Error,
                        Content = String.Format(UIControls.Localization.Resources.LoanError, e.SerializedTicketNumber),
                        OkButtonContent = UIControls.Localization.Resources.Ok,
                        Owner = Application.Current.MainWindow
                    });
                    IsComeInEnabled = false;
                    break;
                }
            }
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            if(Customer == null)
            {
                ExtraWindow.Alert(new DialogParameters
                {
                    Header = UIControls.Localization.Resources.Error,
                    Content = UIControls.Localization.Resources.NeedCustomerError,
                    OkButtonContent = UIControls.Localization.Resources.Ok,
                    Owner = this
                });
                return;
            }
            if(String.IsNullOrWhiteSpace(CardNumber) || CardNumber != Customer.ActiveCard.CardBarcode)
            {
                if(Customer.ActiveCard != null)
                {
                    ExtraWindow.Alert(new DialogParameters
                        {
                            Header = UIControls.Localization.Resources.Error,
                            Content = String.Format(UIControls.Localization.Resources.CardNeeded, Customer.ActiveCard.CardBarcode),
                            OkButtonContent = UIControls.Localization.Resources.Ok,
                            Owner = this
                        });
                }
                else
                {
                    ExtraWindow.Alert(new DialogParameters
                    {
                        Header = UIControls.Localization.Resources.Error,
                        Content = String.Format(UIControls.Localization.Resources.NoCardError),
                        OkButtonContent = UIControls.Localization.Resources.Ok,
                        Owner = this
                    });
                }
                return;
            }
            if((PrintFR.IsChecked ?? false) || (PrintPDF.IsChecked ?? false))
            {
#if !BEAUTINIKA
                var hasSmart = _tickets.Any(i => i.SerializedTicketType.IsSmart);
#else
                var hasSmart = false;
#endif

                var len = PrintFR.IsChecked ?? false ? 35 : 256;
                var ls = new List<string>
                {
                    UIControls.Localization.Resources.Visitor + ":",
                    Customer.FullName,
                    String.Format(UIControls.Localization.Resources.CardNumber + ": {0}",
                        Customer.ActiveCard.CardBarcode),
                    "-----------------------------------",
                    UIControls.Localization.Resources.TodaysSchedule
                };
                var n = 1;
                var sum = 0;
                foreach(var ev in _treatmentEvents.OrderBy(i => i.VisitDate))
                {
                    ls.AddRange(String.Format("{0}. {2}: {1}", n++,
                        ev.SerializedTreatmentTypeName,
                        UIControls.Localization.Resources.TreatmentType).SplitByLen(len));
                    ls.AddRange(String.Format("   {2}: {1:H:mm}, {3}: {0}", ev.SerializedTreatmentName,
                        ev.VisitDate, UIControls.Localization.Resources.Time, UIControls.Localization.Resources.Treatment).SplitByLen(len));
#if BEAUTINIKA
                    ls.AddRange(String.Format("   {0}: {1}", UIControls.Localization.Resources.Duration, ev.SerializedDuration).SplitByLen(len));
#else
                    ls.AddRange(String.Format(hasSmart ? "   {2}: {0}" : "   {2}: {0}, {3}: {1:n0}", ev.SerializedDuration,
                        ev.Price, UIControls.Localization.Resources.Duration, UIControls.Localization.Resources.Cost).SplitByLen(len));
#endif
                    ls.AddRange(String.Format("   {1}: {0}", ev.SerializedTicketNumber, UIControls.Localization.Resources.TicketNumber).SplitByLen(len));
                    sum += ev.SerializedCost;

#if BEAUTINIKA
                    ls.AddRange(String.Format("Эстетист: ______ {0}", ev.SerializedEmployeeName).SplitByLen(len));
#endif

                }

#if BEAUTINIKA
                ls.Add("");
                ls.AddRange(
                    String.Format("Количество процедур: {0:n0}", _treatmentEvents.Count)
                        .SplitByLen(len));
#else
                if(!hasSmart)
                {
                    ls.Add("");
                    ls.AddRange(
                        String.Format("{1}: {0:n0}", sum, UIControls.Localization.Resources.TotalTreCost)
                            .SplitByLen(len));
                }
#endif

                if(ShelfNumber.HasValue)
                {
                    ls.Add("");
                    ls.AddRange(String.Format("{1}: {0:n0}", ShelfNumber.Value, UIControls.Localization.Resources.GivenShelf).SplitByLen(len));
                }
                if(SafeNumber.HasValue)
                {
                    ls.Add("");
                    ls.AddRange(String.Format("{1}: {0:n0}", SafeNumber.Value, UIControls.Localization.Resources.GivenSafe).SplitByLen(len));
                }

                ls.Add("");
                ls.AddRange(String.Format("{1}: {0:n0}", Customer.BonusDepositValue, UIControls.Localization.Resources.BonusesAmount).SplitByLen(len));

                var so = _context.GetCurrentActions();
                if(so.Count > 0)
                {
                    ls.Add("");
                    ls.Add(UIControls.Localization.Resources.TodaysOffers);
                    foreach(var act in _context.GetCurrentActions())
                    {
                        ls.AddRange(act.ActionText.SplitByLen(len));
                    }
                }

#if !BEAUTINIKA
                int n1 = 1;
                ls.Add("");
                ls.AddRange("Активные и доступные абонементы:".SplitByLen(len));
                foreach(var t in _tickets.Where(i => i.Status == TicketStatus.Active || i.Status == TicketStatus.Available))
                {
                    ls.AddRange(
                        String.Format(
                            t.SerializedTicketType.IsSmart
                                ? UIControls.Localization.Resources.TicketInfoBulkSmart
                                : UIControls.Localization.Resources.TicketInfoBulk,
                                n1++, t.Number + " " + t.SerializedTicketType.Name,
                            t.FinishDate, t.UnitsLeft, t.GuestUnitsLeft,
                            t.SolariumMinutesLeft,
                            Math.Floor(t.UnitsLeft / 8)).SplitByLen(len));
                }
#endif


                ls.Add("-----------------------------------");
                if(PrintFR.IsChecked ?? false)
                {
                    if(!_cashMan.PrintText(ls))
                    {
                        return;
                    }
                }
                else
                {
                    _repMan.PrintTextToPdf(ls);
                }
            }
            try
            {
                _context.RegisterCustomerVisit(Customer.Id, ShelfNumber ?? -1, SafeNumber ?? -1);
            }
            catch(FaultException ex)
            {
                ExtraWindow.Alert(new DialogParameters
                {
                    Header = UIControls.Localization.Resources.Error,
                    Content = ex.Reason,
                    Owner = this
                });
            }
            if(RegisterChild.IsChecked ?? false)
            {
                NavigationManager.MakeNewChildRequest(Customer.Id);
            }

            string pre;
            if(PrintFR.IsChecked ?? false)
            {
                pre = "0";
            }
            else if(PrintPDF.IsChecked ?? false)
            {
                pre = "1";
            }
            else
            {
                pre = "2";
            }
            AppSettingsManager.SetSetting("PrereceiptSetting", pre);

            DialogResult = true;
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddParallel_Click(object sender, RoutedEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            if(frameworkElement != null)
            {
                var tId = ((TreatmentEvent)frameworkElement.DataContext).Id;
                NavigationManager.MakeParallelSigningRequest(tId, RefreshEvents);
            }
        }
    }
}
