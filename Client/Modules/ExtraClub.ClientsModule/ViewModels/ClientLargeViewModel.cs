using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExtraClub.CashRegisterModule;
using ExtraClub.Infrastructure.BaseClasses;
using ExtraClub.Infrastructure.Interfaces;
using ExtraClub.ServiceModel;
using System.ComponentModel;
using System.Windows.Data;
using Microsoft.Practices.Unity;
using ExtraClub.UIControls;
using System.Windows;
using System.Threading;
using ExtraClub.UIControls.Windows;
using Telerik.Windows.Controls;
using System.Windows.Controls;
using ExtraClub.Infrastructure;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using ViewModelBase = ExtraClub.UIControls.BaseClasses.ViewModelBase;

namespace ExtraClub.Clients.ViewModels
{
    public class ClientLargeViewModel : ViewModelBase
    {
        public string PassportMask { get; set; }
        public MaskType PassportMaskType { get; set; }
        public string PhoneMask { get; set; }

        public CashRegisterManager _cashMan;

        public ICollectionView AdvertTypesView { get; set; }
        public ICollectionView ManagersView { get; set; }
        public List<AdvertType> AdvertTypes { get; set; }
        public List<Employee> Managers { get; set; }

        public List<string> cities, streets, metros, workplaces, positions;
        public ICollectionView Cities { get; set; }
        public ICollectionView Streets { get; set; }
        public ICollectionView Metros { get; set; }
        public ICollectionView WorkPlaces { get; set; }
        public ICollectionView Positions { get; set; }

        public ICollectionView CurrentContras { get; set; }
        private List<ContraView> _currentContras = new List<ContraView>();

        public ICollectionView TicketsView { get; set; }
        public List<Ticket> Tickets = new List<Ticket>();

        public ICollectionView CurrentStatuses { get; set; }
        private List<StatusView> _currentStatuses = new List<StatusView>();

        public ICollectionView AnthrosView { get; set; }
        public List<Anthropometric> _Anthros = new List<Anthropometric>();

        public ICollectionView TargetsView { get; set; }
        private List<CustomerTarget> _targets = new List<CustomerTarget>();
        public CustomerTarget SelectedTarget { get; set; }

        public ICollectionView DoctorVisitsView { get; set; }
        private List<DoctorVisit> _doctors = new List<DoctorVisit>();
        public DoctorVisit SelectedDoctor { get; set; }

        public ICollectionView NutritionsView { get; set; }
        private List<Nutrition> _nutritions = new List<Nutrition>();
        public Nutrition SelectedNutrition { get; set; }

        public ICollectionView MeasuresView { get; set; }
        private List<CustomerMeasure> _measures = new List<CustomerMeasure>();
        public CustomerMeasure SelectedMeasure { get; set; }

        public ICollectionView ChildrenView { get; set; }
        private List<ChildrenRoom> _children = new List<ChildrenRoom>();

        public ICollectionView GoodsReserveView { get; set; }
        private List<GoodReserve> _GoodsReserveView = new List<GoodReserve>();

        public ICollectionView EventsView { get; set; }
        public List<TreatmentEvent> _events = new List<TreatmentEvent>();

        public ICollectionView CustomerShelvesView { get; set; }
        public List<CustomerShelf> _CustomerShelves = new List<CustomerShelf>();

        public ICollectionView CustomerSafesView { get; set; }
        public List<CustomerShelf> _CustomerSafes = new List<CustomerShelf>();

        public ICollectionView SolariumView { get; set; }
        public List<SolariumVisit> _SolariumView = new List<SolariumVisit>();

        public ICollectionView SalesView { get; set; }
        public List<GoodSale> _SalesView = new List<GoodSale>();

        public ICollectionView CustomerEventsView { get; set; }
        public List<CustomerEventView> _CustomerEventsView = new List<CustomerEventView>();

        public ICollectionView DepositView { get; set; }
        public List<DepositAccount> _DepositView = new List<DepositAccount>();

        public ICollectionView BonusView { get; set; }
        public List<BonusAccount> _BonusView = new List<BonusAccount>();

        public ICollectionView RentView { get; set; }
        public List<Rent> _RentView = new List<Rent>();

        public ICollectionView VisitsView { get; set; }
        public List<CustomerVisit> _VisitsView = new List<CustomerVisit>();
#if BEAUTINIKA
        public ICollectionView RecommendationsView { get; set; }
        public List<Recommendation> _RecommendationsView = new List<Recommendation>();

        public bool CanChargeEvent
        {
            get
            {
                return EventsView != null && EventsView.CurrentItem is TreatmentEvent 
                    && (EventsView.CurrentItem as TreatmentEvent).VisitStatus == 2
                       && String.IsNullOrEmpty((EventsView.CurrentItem as TreatmentEvent).HasGoodCharges);
            }
        }

        public bool CanChangeEvent
        {
            get
            {
                return EventsView != null && EventsView.CurrentItem is TreatmentEvent /*(EventsView.CurrentItem as TreatmentEvent).VisitStatus == 0*/;
            }
        }
#endif
        public Visibility VatVisibility { get; set; }

        public bool NurseryEnabled { get; set; }

        DateTime _svd = DateTime.Today;
#if BEAUTINIKA
        public bool IsRecommendationPlanAllowed
        {
            get
            {
                var r = RecommendationsView.CurrentItem as Recommendation;
                return !(r == null || r.Status != "Не запланирована");
            }
        }
#endif
        public List<string> CurrentContrasBlocking
        {
            get
            {
                if(_currentContras == null) return null;
                var res = new List<string>();
                foreach(var c in _currentContras.Where(i => i.HasContra))
                {
                    c.Indication.SerializedTreatmentTypes.Where(i => !res.Contains(i.Name)).ToList().ForEach(i => res.Add(i.Name));
                }
                return res.Intersect(_availTreatmentTypes).ToList();
            }
        }

        public BitmapSource CustomerImage
        {
            get
            {
                if(CurrentCustomer == null || CurrentCustomer.Image == null || CurrentCustomer.Image.Length == 0) return null;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = new MemoryStream(CurrentCustomer.Image);
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }

        public DateTime StatisticsVisitDate
        {
            get
            {
                return _svd;

            }
            set
            {
                _svd = value;
                OnPropertyChanged("StatisticsVisitDate");
                OnPropertyChanged("StatisticsTreatmentEvents");
            }
        }


        private DateTime _EventsViewStart = DateTime.Today.AddDays(-7);
        public DateTime EventsViewStart
        {
            get
            {
                return _EventsViewStart;
            }
            set
            {
                if(_EventsViewStart != value)
                {
                    _EventsViewStart = value;
                    if(_EventsViewStart > _EventsViewEnd)
                    {
                        _EventsViewStart = _EventsViewEnd;
                    }
                    OnPropertyChanged("EventsViewStart");
                    RefreshEvents();
                }
            }
        }

        private DateTime _EventsViewEnd = DateTime.Today.AddMonths(1);
        public DateTime EventsViewEnd
        {
            get
            {
                return _EventsViewEnd;
            }
            set
            {
                if(_EventsViewEnd != value)
                {
                    _EventsViewEnd = value;
                    if(_EventsViewStart > _EventsViewEnd)
                    {
                        _EventsViewEnd = _EventsViewStart;
                    }
                    OnPropertyChanged("EventsViewEnd");
                    RefreshEvents();
                }
            }
        }

        private DateTime depositStart = DateTime.Today.AddDays(-14);
        public DateTime DepositStart
        {
            get
            {
                return depositStart;
            }
            set
            {
                depositStart = value;
                if(depositStart > DepositEnd)
                {
                    depositStart = depositEnd;
                }
                OnPropertyChanged("DepositStart");
                RefreshDeposit();
            }
        }

        private DateTime depositEnd = DateTime.Today;
        public DateTime DepositEnd
        {
            get
            {
                return depositEnd;
            }
            set
            {
                depositEnd = value;
                if(DepositStart > depositEnd)
                {
                    depositEnd = depositStart;
                }
                OnPropertyChanged("DepositEnd");
                RefreshDeposit();
            }
        }

        private DateTime barSalesStart = DateTime.Today.AddDays(-3);
        public DateTime BarSalesStart
        {
            get
            {
                return barSalesStart;
            }
            set
            {
                if(barSalesStart != value)
                {
                    barSalesStart = value;
                    if(barSalesStart > BarSalesEnd)
                    {
                        barSalesStart = BarSalesEnd;
                    }
                    OnPropertyChanged("BarSalesStart");
                    RefreshSales();
                }
            }
        }

        private DateTime barSalesEnd = DateTime.Today;
        public DateTime BarSalesEnd
        {
            get
            {
                return barSalesEnd;
            }
            set
            {
                if(barSalesEnd != value)
                {
                    barSalesEnd = value;
                    if(BarSalesStart > barSalesEnd)
                    {
                        barSalesEnd = barSalesStart;
                    }
                    OnPropertyChanged("BarSalesEnd");
                    RefreshSales();
                }
            }
        }

        private DateTime bonusStart = DateTime.Today.AddDays(-14);
        public DateTime BonusStart
        {
            get
            {
                return bonusStart;
            }
            set
            {
                bonusStart = value;
                if(bonusStart > BonusEnd)
                {
                    bonusStart = bonusEnd;
                }
                OnPropertyChanged("BonusStart");
                RefreshBonus();
            }
        }

        private DateTime bonusEnd = DateTime.Today;
        public DateTime BonusEnd
        {
            get
            {
                return bonusEnd;
            }
            set
            {
                bonusEnd = value;
                if(BonusStart > bonusEnd)
                {
                    bonusEnd = bonusStart;
                }
                OnPropertyChanged("BonusEnd");
                RefreshBonus();
            }
        }

        public List<TreatmentEvent> StatisticsTreatmentEvents
        {
            get
            {
                if(CurrentCustomer == null) return new List<TreatmentEvent>();
                return _events.Where(i => i.VisitDate.Date == _svd).ToList();
            }
        }

        private List<ContraIndication> AllContras = new List<ContraIndication>();
        private List<CustomerStatus> AllStatuses = new List<CustomerStatus>();

        private Customer _customer;
        public Customer CurrentCustomer
        {
            get { return _customer; }
            set
            {
                if(_customer != value)
                {
                    if(_customer != null)
                    {
                        _customer.PropertyChanged -= new PropertyChangedEventHandler(_customer_PropertyChanged);
                    }
                    _customer = value;
                    if(_customer != null)
                    {
                        _customer.PropertyChanged += new PropertyChangedEventHandler(_customer_PropertyChanged);
                    }
                    OnPropertyChanged("CurrentCustomer");
                    OnPropertyChanged("IsCustomerSelected");
                    OnPropertyChanged("PrivatePrintAllowed");
                    OnPropertyChanged("SaveFormAllowed");
                    OnPropertyChanged("IsCardSellEnabled");
                    OnPropertyChanged("IsCardOperationsEnabled");
                    OnPropertyChanged("NullContraIndications");
                    OnPropertyChanged("NoContraIndications");
                    OnPropertyChanged("ContrasChanged");
                    OnPropertyChanged("CurrentContrasBlocking");
                    OnPropertyChanged("StatusesChanged");
                    OnPropertyChanged("CustomerImage");
                }
            }
        }

        void _customer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("SaveFormAllowed");
        }

        public bool IsCardSellEnabled
        {
            get
            {
                return CurrentCustomer != null && !CurrentCustomer.SerializedCustomerCards.Any(i => i.IsActive);
            }
        }

        public bool IsCardOperationsEnabled
        {
            get
            {
                return CurrentCustomer != null && CurrentCustomer.SerializedCustomerCards.Count > 0;
            }
        }

        public bool PrivatePrintAllowed
        {
            get
            {
                return CurrentCustomer != null;
            }
        }

        public bool SaveFormAllowed
        {
            get
            {
                return CurrentCustomer != null && CurrentCustomer.Modified && String.IsNullOrEmpty(CurrentCustomer.Error);
            }
        }

        public bool IsCustomerSelected
        {
            get
            {
                return CurrentCustomer != null;
            }
        }

        public bool NullContraIndications
        {
            get
            {
                if(CurrentCustomer == null) return false;
                return !CurrentCustomer.NoContraIndications.HasValue;
            }
        }

        public bool NoContraIndications
        {
            get
            {
                if(CurrentCustomer == null) return false;
                return CurrentCustomer.NoContraIndications ?? false;
            }
        }

        public bool StatusesChanged
        {
            get
            {
                return _currentStatuses.Any(c => c.IsChanged);
            }
        }

        public bool ContrasChanged
        {
            get
            {
                return _currentContras.Any(c => c.IsChanged);
            }
        }

        List<string> _availTreatmentTypes;

        public ClientLargeViewModel(IUnityContainer container, CashRegisterManager cashRegister)
            : base()
        {
            var tts = ClientContext.GetAllTreatmentTypes().ToDictionary(i => i.Id, i => i.Name);
            var ts = ClientContext.GetAllTreatments();
            _availTreatmentTypes = new List<string>();
            ts.ForEach(i =>
            {
                if(tts.ContainsKey(i.TreatmentTypeId))
                    _availTreatmentTypes.Add(tts[i.TreatmentTypeId]);
            });
            _availTreatmentTypes = _availTreatmentTypes.Distinct().ToList();

            if(AppSettingsManager.GetSetting("Language") == "3")
            {
                PassportMask = "0000000";
                PassportMaskType = MaskType.Standard;
                PhoneMask = "+000 (00) 000 00 00";
            }
            else if(AppSettingsManager.GetSetting("Language") == "2")
            {
                PassportMask = "";
                PassportMaskType = MaskType.None;
                PhoneMask = "+000 00 00 00 00";
            }
            else
            {
                PassportMask = "0000 000 000";
                PassportMaskType = MaskType.Standard;
                PhoneMask = "+0 (000) 000-00-00";
            }


            var w = System.Diagnostics.Stopwatch.StartNew();
            System.Diagnostics.Debug.WriteLine("Start creating ClientLargeViewModel");

            VatVisibility = Thread.CurrentThread.CurrentUICulture.IetfLanguageTag == "ru-RU" ? Visibility.Collapsed : Visibility.Visible;

            NavigationManager.ClientRequest += new EventHandler<ClientEventArgs>(navMan_ClientRequest);
            //NavigationManager.CustomerTargetRequest += new EventHandler<GuidEventArgs>(navMan_CustomerTargetRequest);
            _cashMan = cashRegister;

            AdvertTypes = new List<AdvertType>();
            AdvertTypesView = CollectionViewSource.GetDefaultView(AdvertTypes);

            Managers = new List<Employee>();
            ManagersView = CollectionViewSource.GetDefaultView(Managers);

            metros = new List<string>();
            Metros = CollectionViewSource.GetDefaultView(metros);
            streets = new List<string>();
            Streets = CollectionViewSource.GetDefaultView(streets);
            cities = new List<string>();
            Cities = CollectionViewSource.GetDefaultView(cities);
            workplaces = new List<string>();
            WorkPlaces = CollectionViewSource.GetDefaultView(workplaces);
            positions = new List<string>();
            Positions = CollectionViewSource.GetDefaultView(positions);
            CurrentContras = CollectionViewSource.GetDefaultView(_currentContras);
            CurrentStatuses = CollectionViewSource.GetDefaultView(_currentStatuses);
            TargetsView = CollectionViewSource.GetDefaultView(_targets);
            AnthrosView = CollectionViewSource.GetDefaultView(_Anthros);
            DoctorVisitsView = CollectionViewSource.GetDefaultView(_doctors);
            NutritionsView = CollectionViewSource.GetDefaultView(_nutritions);
            MeasuresView = CollectionViewSource.GetDefaultView(_measures);
            EventsView = CollectionViewSource.GetDefaultView(_events);
            ChildrenView = CollectionViewSource.GetDefaultView(_children);
            CustomerShelvesView = CollectionViewSource.GetDefaultView(_CustomerShelves);
            CustomerSafesView = CollectionViewSource.GetDefaultView(_CustomerSafes);
            SolariumView = CollectionViewSource.GetDefaultView(_SolariumView);
            SalesView = CollectionViewSource.GetDefaultView(_SalesView);
            RentView = CollectionViewSource.GetDefaultView(_RentView);
            DepositView = CollectionViewSource.GetDefaultView(_DepositView);
            BonusView = CollectionViewSource.GetDefaultView(_BonusView);
            CustomerEventsView = CollectionViewSource.GetDefaultView(_CustomerEventsView);
            VisitsView = CollectionViewSource.GetDefaultView(_VisitsView);
            TicketsView = CollectionViewSource.GetDefaultView(Tickets);
            GoodsReserveView = CollectionViewSource.GetDefaultView(_GoodsReserveView);
#if BEAUTINIKA
            RecommendationsView = CollectionViewSource.GetDefaultView(_RecommendationsView);
            RecommendationsView.CurrentChanged += RecommendationsView_CurrentChanged;
#endif
            System.Diagnostics.Debug.WriteLine("ClientLargeViewModel creating takes " + w.ElapsedMilliseconds + " ms.");
        }

        void RecommendationsView_CurrentChanged(object sender, EventArgs e)
        {
            OnPropertyChanged("IsRecommendationPlanAllowed");
        }

        public void CustomerTargetRequest(GuidEventArgs e)
        {
            SelectClient(ClientContext.GetCustomerIdByTargetId(e.Guid));
            SelectedTarget = _targets.FirstOrDefault(i => i.Id == e.Guid);
            TargetsView.Refresh();
            TargetsView.MoveCurrentTo(SelectedTarget);
        }

        void navMan_ClientRequest(object sender, ClientEventArgs e)
        {
            SelectClient(e.ClientId);
        }

        protected override void RefreshDataInternal()
        {
            AdvertTypes.Clear();
            AdvertTypes.AddRange(ClientContext.GetAdvertTypes());
            Managers.Clear();
            Managers.AddRange(ClientContext.GetEmployees(true, true));
            var tmp = ClientContext.GetAddressLists();
            cities.Clear();
            cities.AddRange(tmp[0]);
            metros.Clear();
            metros.AddRange(tmp[1]);
            streets.Clear();
            streets.AddRange(tmp[2]);
            AllContras.Clear();
            AllContras.AddRange(ClientContext.GetAllContras());
            tmp = ClientContext.GetWorkData();
            workplaces.Clear();
            workplaces.AddRange(tmp[0]);
            positions.Clear();
            positions.AddRange(tmp[1]);
            AllStatuses.Clear();
            foreach(var i in ClientContext.GetAllStatuses().OrderBy(i => i.Value))
            {
                AllStatuses.Add(new CustomerStatus { Id = i.Key, Name = i.Value });
            }
        }

        protected override void RefreshFinished()
        {
            base.RefreshFinished();
            AdvertTypesView.Refresh();
            ManagersView.Refresh();
            RefreshContras();
            RefreshStatuses();

            WorkPlaces.Refresh();
            Positions.Refresh();

            OnPropertyChanged("AllStatuses");
        }

        public void SelectClient(Guid clientId)
        {
            ClientContext.ExecuteMethodAsync(i => i.GetCustomer(clientId, true)).ContinueWith(i =>
              {
                  Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                  {
                      CurrentCustomer = i.Result;

                      RefreshContras();
                      RefreshStatuses();
                      RefreshTargets();
                      RefreshStatistics();
                      RefreshAnthros();
                      RefreshDoctors();
                      RefreshNutritions();
                      RefreshMeasures();
                      RefreshEvents();
                      RefreshChildren();
                      RefreshShelves();
                      RefreshSafes();
                      RefreshSolarium();
                      RefreshSales();
                      RefreshRent();
                      OnClientSelected();
                      RefreshDeposit();
                      RefreshBonus();
                      RefreshCustomerEvents();
                      RefreshVisits();
                      RefreshTickets();
                      RefreshGoodsReserve();
                      if(CurrentCustomer != null)
                      {
                          NurseryEnabled = CurrentCustomer.IsInClub;
                      }
                      else
                      {
                          NurseryEnabled = false;
                      }
                      OnPropertyChanged("NurseryEnabled");
                  }));
              });
        }

#if BEAUTINIKA
        public void RefreshRecommendations()
        {
            if (CurrentCustomer == null)
            {
                _RecommendationsView.Clear();
                RecommendationsView.Refresh();
                return;
            }
            RefreshAsync(_RecommendationsView, RecommendationsView, () => ClientContext.GetRecommendationsForCustomer(CurrentCustomer.Id));
        }
#endif

        private void RefreshTickets()
        {
            if(CurrentCustomer == null)
            {
                Tickets.Clear();
                TicketsView.Refresh();
                return;
            }
            RefreshAsync(Tickets, TicketsView, () => ClientContext.GetTicketsForCustomer(CurrentCustomer.Id));
        }

        private void RefreshVisits()
        {
            if(CurrentCustomer == null)
            {
                _VisitsView.Clear();
                VisitsView.Refresh();
                return;
            }
            RefreshAsync(_VisitsView, VisitsView, () => ClientContext.GetCustomerVisits(CurrentCustomer.Id));
        }

        public void RefreshBonus()
        {
            if(CurrentCustomer == null)
            {
                _BonusView.Clear();
                BonusView.Refresh();
                return;
            }
            RefreshAsync(_BonusView, BonusView, () => ClientContext.GetCustomerBonus(CurrentCustomer.Id, bonusStart, bonusEnd));
        }

        internal void RefreshDeposit()
        {
            if(CurrentCustomer == null)
            {
                _DepositView.Clear();
                DepositView.Refresh();
                return;
            }
            RefreshAsync(_DepositView, DepositView, () => ClientContext.GetCustomerDeposit(CurrentCustomer.Id, depositStart, depositEnd));
        }

        internal void RefreshRent()
        {
            if(CurrentCustomer == null)
            {
                _RentView.Clear();
                RentView.Refresh();
                return;
            }
            RefreshAsync(_RentView, RentView, () => ClientContext.GetCustomerRent(CurrentCustomer.Id));
        }

        internal void RefreshSales()
        {
            if(CurrentCustomer == null)
            {
                _SalesView.Clear();
                SalesView.Refresh();
                return;
            }
            RefreshAsync(_SalesView, SalesView, () => ClientContext.GetCustomerSales(CurrentCustomer.Id, barSalesStart, barSalesEnd));
        }

        public void RefreshSolarium()
        {
            if(CurrentCustomer == null)
            {
                _SolariumView.Clear();
                SolariumView.Refresh();
                return;
            }
            RefreshAsync(_SolariumView, SolariumView, () => ClientContext.GetCustomerSolarium(CurrentCustomer.Id));
        }

        public void RefreshChildren()
        {
            if(CurrentCustomer == null)
            {
                _children.Clear();
                ChildrenView.Refresh();
                return;
            }
            RefreshAsync(_children, ChildrenView, () => ClientContext.GetCustomerChildren(CurrentCustomer.Id));
        }

        internal void RefreshMeasures()
        {
            if(CurrentCustomer == null)
            {
                _measures.Clear();
                MeasuresView.Refresh();
                return;
            }

            if(CurrentCustomer != null)
            {
                RefreshAsync(_measures, MeasuresView, () => ClientContext.GetMeasuresForCustomer(CurrentCustomer.Id));
            }
        }

        internal void RefreshShelves()
        {
            if(CurrentCustomer == null)
            {
                _CustomerShelves.Clear();
                CustomerShelvesView.Refresh();
                return;
            }
            if(CurrentCustomer != null)
            {
                RefreshAsync(_CustomerShelves, CustomerShelvesView, () => ClientContext.GetCustomerShelves(CurrentCustomer.Id, false));
            }
        }

        internal void RefreshSafes()
        {
            if(CurrentCustomer == null)
            {
                _CustomerSafes.Clear();
                CustomerSafesView.Refresh();
                return;
            }

            if(CurrentCustomer != null)
            {
                RefreshAsync(_CustomerSafes, CustomerSafesView, () => ClientContext.GetCustomerShelves(CurrentCustomer.Id, true));
            }
        }

        internal void RefreshNutritions()
        {
            if(CurrentCustomer == null)
            {
                _nutritions.Clear();
                NutritionsView.Refresh();
                return;
            }
            if(CurrentCustomer != null)
            {
                RefreshAsync(_nutritions, NutritionsView, () => ClientContext.GetNutritionsForCustomer(CurrentCustomer.Id));
            }
        }

        internal void RefreshGoodsReserve()
        {
            if(CurrentCustomer == null)
            {
                _GoodsReserveView.Clear();
                GoodsReserveView.Refresh();
                return;
            }
            if(CurrentCustomer != null)
            {
                RefreshAsync(_GoodsReserveView, GoodsReserveView, () => ClientContext.GetGoodsReserve(CurrentCustomer.Id));
            }
        }

        internal void RefreshAnthros()
        {
            if(CurrentCustomer == null)
            {
                _Anthros.Clear();
                AnthrosView.Refresh();
                return;
            }
            if(CurrentCustomer != null)
            {
                RefreshAsync(_Anthros, AnthrosView, () => ClientContext.GetAnthropometricsForCustomer(CurrentCustomer.Id));
            }
        }

        public event EventHandler ClientSelected;

        private void OnClientSelected()
        {
            if(ClientSelected != null) ClientSelected.Invoke(this, new EventArgs());
        }

        private void RefreshStatistics()
        {
            RedLetterDayConverter.Days.Clear();
            foreach(var ev in _events.OrderBy(i => i.VisitDate))
            {
                if(!RedLetterDayConverter.Days.ContainsKey(ev.VisitDate.Date))
                {
                    RedLetterDayConverter.Days.Add(ev.VisitDate.Date, UIControls.Localization.Resources.VisitStart + ev.VisitDate.ToString("HH:mm"));
                }
            }
        }

        public void RefreshTargets()
        {
            if(CurrentCustomer == null)
            {
                _targets.Clear();
                TargetsView.Refresh();
                return;
            }

            if(CurrentCustomer != null)
            {
                _targets.Clear();
                _targets.AddRange(ClientContext.GetCustomerTargets(CurrentCustomer.Id));
                TargetsView.Refresh();
            }
        }

        private void RefreshContras()
        {
            foreach(var i in _currentContras)
            {
                i.PropertyChanged -= CurrentContra_PropertyChanged;
            }
            _currentContras.Clear();

            if(CurrentCustomer == null)
            {
                CurrentContras.Refresh();
                return;
            }

            if(CurrentCustomer != null)
            {
                var customerContras = ClientContext.GetCustomerContrasIds(CurrentCustomer.Id);
                foreach(var i in AllContras)
                {
                    var cv = new ContraView(i, customerContras.Contains(i.Id));
                    cv.PropertyChanged += CurrentContra_PropertyChanged;
                    _currentContras.Add(cv);
                }
                CurrentContras.Refresh();
                OnPropertyChanged("ContrasChanged");
                OnPropertyChanged("CurrentContrasBlocking");
                OnPropertyChanged("NullContraIndications");
                OnPropertyChanged("NoContraIndications");
            }
        }

        private void RefreshStatuses()
        {
            foreach(var i in _currentStatuses)
            {
                i.PropertyChanged -= CurrentStatus_PropertyChanged;
            }
            _currentStatuses.Clear();

            if(CurrentCustomer != null)
            {
                var customerStatuses = ClientContext.GetCustomerStatusesIds(CurrentCustomer.Id);
                foreach(var i in AllStatuses.OrderBy(j => j.Name))
                {
                    var cv = new StatusView(i, customerStatuses.Contains(i.Id));
                    cv.PropertyChanged += CurrentStatus_PropertyChanged;
                    _currentStatuses.Add(cv);
                }
            }
            CurrentStatuses.Refresh();
            OnPropertyChanged("StatusesChanged");
        }

        void CurrentContra_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("ContrasChanged");
            OnPropertyChanged("CurrentContrasBlocking");
        }
        void CurrentStatus_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("StatusesChanged");
        }

        private Ticket _currentTicket;
        public Ticket CurrentTicket
        {
            get
            {
                return _currentTicket;
            }
            set
            {
                _currentTicket = value;
                OnPropertyChanged("CurrentTicket");
            }
        }

        internal bool IsTicketActivationNeeded()
        {
            return !Tickets.Any(t => t.Status == TicketStatus.Active && t.DivisionId == ClientContext.CurrentDivision.Id);
        }

        internal bool HasTicketToActivate()
        {
            return Tickets.Any(t => t.Status == TicketStatus.Available && t.DivisionId == ClientContext.CurrentDivision.Id);
        }

        internal void ActivateCurrentTicket()
        {
            ClientContext.ActivateTicket(_currentTicket.Id);
            SelectClient(CurrentCustomer.Id);
        }

        internal void SaveCustomer()
        {
            ClientContext.UpdateCustomerForm(CurrentCustomer);
            SelectClient(CurrentCustomer.Id);
        }

        internal void CancelTreatmentEvents(IEnumerable<object> events)
        {
            var cancelable = new List<Guid>();
            foreach(TreatmentEvent ev in events)
            {
                if(ev.CanBeCancelled && Math.Abs((ev.VisitDate - DateTime.Now).TotalHours) >= ClientContext.CurrentCompany.MaxCancellationPeriod) cancelable.Add(ev.Id);
            }
            ClientContext.CancelTreatmentEvents(cancelable);
            RefreshEvents();
        }

        internal void SetNoContras()
        {
            if(CurrentCustomer == null) return;
            ClientContext.ClearCustomerContras(CurrentCustomer.Id);
            CurrentCustomer.NoContraIndications = true;
            RefreshContras();
        }


        internal void SaveContras()
        {
            if(CurrentCustomer != null)
            {
                var list = new List<Guid>();
                _currentContras.Where(i => i.HasContra).ToList().ForEach(i => list.Add(i.Indication.Id));
                if(list.Count == 0)
                {
                    SetNoContras();
                    return;
                }
                else
                {
                    CurrentCustomer.NoContraIndications = false;
                }
                ClientContext.PostContraIndications(CurrentCustomer.Id, list);
                RefreshContras();
            }
        }

        public System.Windows.Controls.CalendarBlackoutDatesCollection Blackouts { get; set; }

        public Anthropometric SelectedAnthro { get; set; }

        internal void RefreshDoctors()
        {
            if(CurrentCustomer == null)
            {
                _doctors.Clear();
                DoctorVisitsView.Refresh();
                return;
            }


            if(CurrentCustomer != null)
            {
                RefreshAsync(_doctors, DoctorVisitsView, () => ClientContext.GetDoctorVisitsForCustomer(CurrentCustomer.Id));
            }
        }

        internal void RefreshEvents()
        {
            if(CurrentCustomer == null)
            {
                _events.Clear();
                EventsView.Refresh();
                return;
            }

            if(CurrentCustomer != null)
            {
                RefreshAsync(_events, EventsView, () => ClientContext.GetCustomerEvents(CurrentCustomer.Id, EventsViewStart, EventsViewEnd, true));
            }
        }

        internal void SaveStatuses()
        {
            if(CurrentCustomer != null)
            {
                var list = new List<Guid>();
                _currentStatuses.Where(i => i.HasStatus).ToList().ForEach(i => list.Add(i.Status.Id));
                ClientContext.PostCustomerStatuses(CurrentCustomer.Id, list);
                RefreshStatuses();
            }
        }

        internal void RefreshCustomerEvents()
        {
            if(CurrentCustomer == null)
            {
                _CustomerEventsView.Clear();
                CustomerEventsView.Refresh();
                return;
            }
            RefreshAsync(_CustomerEventsView, CustomerEventsView, () => ClientContext.GetCrmEvents(CurrentCustomer.Id));
        }

        internal void EditEventComment(TreatmentEvent item, string comment)
        {
            ClientContext.AddCommentToTreatmentEvent(item.Id, comment);
            item.Comment = comment;
        }

        internal void UpdateCustomerImage()
        {
            CurrentCustomer.Image = ClientContext.GetCustomerImage(CurrentCustomer.Id);
            OnPropertyChanged("CustomerImage");
        }

        internal void UnmissTreatmentEvents(IEnumerable<object> events)
        {
            foreach(TreatmentEvent ev in events)
            {
                if(ev.VisitStatus == 3) ClientContext.SetTreatmentAsVisited(ev.Id);

            }
            RefreshEvents();
        }
#if BEAUTINIKA
        internal void RefreshCanChargeEvent()
        {
            OnPropertyChanged("CanChargeEvent");
        }
        internal void RefreshCanChangeEvent()
        {
            OnPropertyChanged("CanChangeEvent");
        }
#endif
    }
}
