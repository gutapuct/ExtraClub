using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ExtraClub.ServiceModel;
using ExtraClub.Infrastructure.Interfaces;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections;
using System.Data;
using System.Configuration;
using ExtraClub.Infrastructure.BaseClasses;
using Microsoft.Practices.Unity;
using ExtraClub.ServiceModel.Turnover;
using ExtraClub.UIControls.BaseClasses;

namespace ExtraClub.TurnoverModule.ViewModels
{
    public class TurnoverLargeViewModel : ViewModelBase
    {
        public GoodAction SelectedGoodAction { get; set; }
        public GoodPrice SelectedGoodPrice { get; set; }

        public List<BarPointGood> GoodPresence = new List<BarPointGood>();

        private List<Good> Goods { get; set; }
        private List<Provider> Providers { get; set; }
        private IList<ProviderPayment> ProviderPayments { get; set; }
        private IList<Consignment> Consignments { get; set; }
        private IList<GoodPrice> GoodPrices { get; set; }
        private IList<GoodAction> GoodActions { get; set; }
        private List<ProviderFolder> ProviderFolders = new List<ProviderFolder>();
        private List<GoodSale> _goodSales = new List<GoodSale>();
        private List<Consignment> _providerOrders = new List<Consignment>();
        private List<Certificate> _certificates = new List<Certificate>();
        public List<BarOrder> _cashPayments = new List<BarOrder>();
        public List<Spending> _spendings = new List<Spending>();
        private List<Income> _incomes = new List<Income>();
        private List<CompanyFinance> _finances = new List<CompanyFinance>();
        private List<DivisionFinance> _divfinances = new List<DivisionFinance>();
        private List<SalesPlan> _SalesPlanView = new List<SalesPlan>();
#if BEAUTINIKA
        public List<MaterialsReportLine> _MaterialsLeft = new List<MaterialsReportLine>();
#endif
        public ICollectionView GoodsView { get; set; }
        public ICollectionView ProvidersView { get; set; }
        public ICollectionView ProviderPaymentsView { get; set; }
        public ICollectionView ConsignmentsView { get; set; }
        public ListCollectionView GoodPricesView { get; set; }
        public ICollectionView GoodPresenceView { get; set; }
        public ICollectionView GoodActionsView { get; set; }
        public ICollectionView GoodSalesView { get; set; }
        public ICollectionView ProviderFoldersView { get; set; }
        public ICollectionView ProviderOrdersView { get; set; }
        public ICollectionView CertificatesView { get; set; }
        public ICollectionView CashPaymentsView { get; set; }
        public ICollectionView SpendingsView { get; set; }
        public ICollectionView IncomesView { get; set; }
        public ICollectionView CompanyFinancesView { get; set; }
        public ICollectionView DivisionFinancesView { get; set; }
        public ICollectionView SalesPlanView { get; set; }
#if BEAUTINIKA
        public ICollectionView MaterialsLeftView { get; set; }
#endif

        private ProviderFolder _currentProviderFolder;
        public ProviderFolder CurrentProviderTreeItem
        {
            get
            {
                return _currentProviderFolder;
            }
            set
            {
                _currentProviderFolder = value;
                ProvidersView.Filter = delegate(object item)
                {
                    if (item == null) return false;
                    var id = ((Provider)item).ProviderFolderId;
                    if (value == null) return !id.HasValue;
                    return (id ?? Guid.Empty) == value.Id;
                };
            }
        }

        private DateTime salesStart = DateTime.Today.AddDays(-14);
        public DateTime SalesStart
        {
            get
            {
                return salesStart;
            }
            set
            {
                salesStart = value;
                if (salesStart > SalesEnd)
                {
                    salesStart = salesEnd;
                }
                OnPropertyChanged("SalesStart");
                RefreshSales();
            }
        }

        private DateTime salesEnd = DateTime.Today;
        public DateTime SalesEnd
        {
            get
            {
                return salesEnd;
            }
            set
            {
                salesEnd = value;
                if (SalesStart > salesEnd)
                {
                    salesEnd = salesStart;
                }
                OnPropertyChanged("SalesEnd");
                RefreshSales();
            }
        }

        private DateTime _cashPaymentsStart = DateTime.Today.AddDays(-1);
        public DateTime CashPaymentsStart
        {
            get
            {
                return _cashPaymentsStart;
            }
            set
            {
                _cashPaymentsStart = value;
                if (_cashPaymentsStart > _cashPaymentsEnd)
                {
                    _cashPaymentsStart = _cashPaymentsEnd;
                }
                OnPropertyChanged("CashPaymentsStart");
                RefreshCashPayments();
            }
        }

        private DateTime _cashPaymentsEnd = DateTime.Today;
        public DateTime CashPaymentsEnd
        {
            get
            {
                return _cashPaymentsEnd;
            }
            set
            {
                _cashPaymentsEnd = value;
                if (_cashPaymentsStart > _cashPaymentsEnd)
                {
                    _cashPaymentsEnd = _cashPaymentsStart;
                }
                OnPropertyChanged("CashPaymentsEnd");
                RefreshCashPayments();
            }
        }

        private DateTime _spendingsStart = DateTime.Today.AddDays(-14);
        public DateTime SpendingsStart
        {
            get
            {
                return _spendingsStart;
            }
            set
            {
                _spendingsStart = value;
                if (_spendingsStart > _spendingsEnd)
                {
                    _spendingsStart = _spendingsEnd;
                }
                OnPropertyChanged("SpendingsStart");
                RefreshSpendings();
            }
        }

        private DateTime _spendingsEnd = DateTime.Today;
        public DateTime SpendingsEnd
        {
            get
            {
                return _spendingsEnd;
            }
            set
            {
                _spendingsEnd = value;
                if (_spendingsStart > _spendingsEnd)
                {
                    _spendingsEnd = _spendingsStart;
                }
                OnPropertyChanged("SpendingsEnd");
                RefreshSpendings();
            }
        }

        private DateTime _incomesStart = DateTime.Today.AddDays(-14);
        public DateTime IncomesStart
        {
            get
            {
                return _incomesStart;
            }
            set
            {
                _incomesStart = value;
                if (_incomesStart > _incomesEnd)
                {
                    _incomesStart = _incomesEnd;
                }
                OnPropertyChanged("IncomesStart");
                RefreshIncomes();
            }
        }

        private DateTime _incomesEnd = DateTime.Today;
        public DateTime IncomesEnd
        {
            get
            {
                return _incomesEnd;
            }
            set
            {
                _incomesEnd = value;
                if (_incomesStart > _incomesEnd)
                {
                    _incomesEnd = _incomesStart;
                }
                OnPropertyChanged("IncomesEnd");
                RefreshIncomes();
            }
        }


        private DateTime conssStart = DateTime.Today.AddDays(-14);
        public DateTime ConsignmentsStart
        {
            get
            {
                return conssStart;
            }
            set
            {
                conssStart = value;
                if (conssStart > ConsignmentsEnd)
                {
                    conssStart = ConsignmentsEnd;
                }
                OnPropertyChanged("ConsignmentsStart");
                RefreshConsignments();
            }
        }

        private DateTime conssEnd = DateTime.Today;
        public DateTime ConsignmentsEnd
        {
            get
            {
                return conssEnd;
            }
            set
            {
                conssEnd = value;
                if (ConsignmentsStart > conssEnd)
                {
                    conssEnd = ConsignmentsStart;
                }
                OnPropertyChanged("ConsignmentsEnd");
                RefreshConsignments();
            }
        }


        private DateTime poStart = DateTime.Today.AddDays(-1);
        public DateTime OrdersStart
        {
            get
            {
                return poStart;
            }
            set
            {
                poStart = value;
                if (poStart > OrdersEnd)
                {
                    poStart = OrdersEnd;
                }
                OnPropertyChanged("OrdersStart");
                RefreshProviderOrders();
                RefreshPayments();
            }
        }

        private DateTime poEnd = DateTime.Today;
        public DateTime OrdersEnd
        {
            get
            {
                return poEnd;
            }
            set
            {
                poEnd = value;
                if (OrdersStart > poEnd)
                {
                    poEnd = OrdersStart;
                }
                OnPropertyChanged("OrdersEnd");
                RefreshProviderOrders();
                RefreshPayments();
            }
        }

        internal void RefreshProviderOrders()
        {
            _providerOrders.Clear();
            _providerOrders.AddRange(ClientContext.GetAllConsignments(OrdersStart, OrdersEnd, false));
            ProviderOrdersView.Refresh();
        }

        internal void RefreshSpendings()
        {
            _spendings.Clear();
            _spendings.AddRange(ClientContext.GetDivisionSpendings(SpendingsStart, SpendingsEnd));
            SpendingsView.Refresh();
        }

        internal void RefreshCashPayments()
        {
            _cashPayments.Clear();
            _cashPayments.AddRange(ClientContext.GetDivisionBarOrders(CashPaymentsStart, CashPaymentsEnd));
            CashPaymentsView.Refresh();
        }

        public TurnoverLargeViewModel(IUnityContainer container)
            : base()
        {
            Goods = new List<Good>();
            Providers = new List<Provider>();
            //            Providers.ItemPropertyChanged += new EventHandler(Providers_ItemPropertyChanged);
            ProviderPayments = new List<ProviderPayment>();
            Consignments = new List<Consignment>();
            GoodPrices = new List<GoodPrice>();
            GoodActions = new List<GoodAction>();

            GoodsView = CollectionViewSource.GetDefaultView(Goods);
            ProvidersView = CollectionViewSource.GetDefaultView(Providers);
            ProviderPaymentsView = CollectionViewSource.GetDefaultView(ProviderPayments);
            ConsignmentsView = CollectionViewSource.GetDefaultView(Consignments);
            GoodPricesView = (ListCollectionView)CollectionViewSource.GetDefaultView(GoodPrices);
            GoodActionsView = CollectionViewSource.GetDefaultView(GoodActions);

            GoodPresenceView = CollectionViewSource.GetDefaultView(GoodPresence);

            ProviderFoldersView = CollectionViewSource.GetDefaultView(ProviderFolders);
            GoodSalesView = CollectionViewSource.GetDefaultView(_goodSales);
            ProviderOrdersView = CollectionViewSource.GetDefaultView(_providerOrders);
            CertificatesView = CollectionViewSource.GetDefaultView(_certificates);
            CashPaymentsView = CollectionViewSource.GetDefaultView(_cashPayments);
            SpendingsView = CollectionViewSource.GetDefaultView(_spendings);
            IncomesView = CollectionViewSource.GetDefaultView(_incomes);
            CompanyFinancesView = CollectionViewSource.GetDefaultView(_finances);
            DivisionFinancesView = CollectionViewSource.GetDefaultView(_divfinances);
            SalesPlanView = CollectionViewSource.GetDefaultView(_SalesPlanView);
#if BEAUTINIKA
            MaterialsLeftView = CollectionViewSource.GetDefaultView(_MaterialsLeft);
#endif
        }

        protected override void RefreshDataInternal()
        {
            CancelAddEdit(GoodsView);
            CancelAddEdit(ProvidersView);

            Goods.Clear();
            ClientContext.GetAllGoods().ForEach(g => Goods.Add(g));

            Providers.Clear();
            ClientContext.GetAllProviders().ForEach(p => Providers.Add(p));

            ProviderPayments.Clear();
            ClientContext.GetAllProviderPayments(OrdersStart, OrdersEnd).ForEach(p => ProviderPayments.Add(p));

            Consignments.Clear();
            ClientContext.GetAllConsignments(ConsignmentsStart, ConsignmentsEnd, true).ForEach(p => { Consignments.Add(p); });

            GoodPrices.Clear();
            ClientContext.GetGoodPrices().ForEach(p => { GoodPrices.Add(p); });

            GoodActions.Clear();
            ClientContext.GetGoodActions(false).ForEach(p => { GoodActions.Add(p); });


            GoodPresence.Clear();
            ClientContext.GetGoodsPresence().ForEach(p => GoodPresence.Add(p));

            _goodSales.Clear();
            _goodSales.AddRange(ClientContext.GetDivisionSales(SalesStart, SalesEnd));

            _providerOrders.Clear();
            _providerOrders.AddRange(ClientContext.GetAllConsignments(OrdersStart, OrdersEnd, false));

            _certificates.Clear();
            _certificates.AddRange(ClientContext.GetDivisionCertificates());

            _cashPayments.Clear();
            _cashPayments.AddRange(ClientContext.GetDivisionBarOrders(CashPaymentsStart, CashPaymentsEnd));

            _spendings.Clear();
            _spendings.AddRange(ClientContext.GetDivisionSpendings(SpendingsStart, SpendingsEnd));

            RefreshIncomes();

            RefreshCompanyFinances();
            RefreshDivisionFinances();

            ConstructProviderFolders();

            _SalesPlanView.Clear();
            _SalesPlanView.AddRange(ClientContext.GetSalesPlan());
            #if BEAUTINIKA
            _MaterialsLeft.Clear();
            _MaterialsLeft.AddRange(ClientContext.GetCurrentMaterialsAmount());
#endif
        }

        public void RefreshIncomes()
        {
            RefreshAsync(_incomes, IncomesView, () => ClientContext.GetDivisionIncomes(IncomesStart, IncomesEnd));
        }

        public void RefreshCompanyFinances()
        {
            RefreshAsync(_finances, CompanyFinancesView, () => ClientContext.GetCompanyFinances());
        }

        public void RefreshDivisionFinances()
        {
            RefreshAsync(_divfinances, DivisionFinancesView, () => ClientContext.GetDivisionFinances());
        }

        private void ConstructProviderFolders()
        {
            ProviderFolders.Clear();
            ProviderFolders.Add(new ProviderFolder { Id = Guid.Empty, Name = UIControls.Localization.Resources.Contragents });
            var src = ClientContext.GetProviderFolders();
            src.Where(i => !i.ParentFolderId.HasValue).ToList().ForEach(i =>
            {
                ProviderFolders[0].Children.Add(i);
                src.Remove(i);
            });
            int cnt = 0;
            while (src.Count != cnt)
            {
                cnt = src.Count;
                foreach (var i in src.ToList())
                {
                    var host = SearchList(ProviderFolders, i.ParentFolderId.Value);
                    if(host != null)
                    {
                        host.Children.Add(i);
                        src.Remove(i);
                    }
                }
            }
        }

        public void RefreshCertificates()
        {
            _certificates.Clear();
            _certificates.AddRange(ClientContext.GetDivisionCertificates());
            CertificatesView.Refresh();
        }

        public void RefreshSales()
        {
            _goodSales.Clear();
            _goodSales.AddRange(ClientContext.GetDivisionSales(SalesStart, SalesEnd));
            GoodSalesView.Refresh();
        }

        public ProviderFolder SearchList(List<ProviderFolder> src, Guid targetId)
        {
            foreach (var i in src)
            {
                if (i.Id == targetId)
                {
                    return i;
                }
                var res = SearchList(i.Children, targetId);
                if (res != null) return res;
            }
            return null;
        }

        protected override void RefreshFinished()
        {
            base.RefreshFinished();

            ProvidersView.Filter = delegate(object item)
            {
                if (item == null) return false;
                var id = ((Provider)item).ProviderFolderId;
                return !id.HasValue;
            };

            GoodPresenceView.Refresh();
            GoodPricesView.Refresh();
            ConsignmentsView.Refresh();
            ProviderPaymentsView.Refresh();
            GoodsView.Refresh();
            ProvidersView.Refresh();
            GoodActionsView.Refresh();

            ProviderFoldersView.Refresh();
            GoodSalesView.Refresh();
            ProviderOrdersView.Refresh();
            CertificatesView.Refresh();
            CashPaymentsView.Refresh();
            SpendingsView.Refresh();
            SalesPlanView.Refresh();
#if BEAUTINIKA
            MaterialsLeftView.Refresh();
#endif
        }

        internal void RemoveCurrentGoodAction()
        {
            if (SelectedGoodAction != null)
            {
                ClientContext.DeleteGoodAction(SelectedGoodAction.Id);
                RefreshActions();
            }
        }

        internal void RefreshActions()
        {
            GoodActions.Clear();
            ClientContext.GetGoodActions(false).ForEach(p => { GoodActions.Add(p); });
            GoodActionsView.Refresh();
        }

        internal void RefreshPrices()
        {
            GoodPrices.Clear();
            ClientContext.GetGoodPrices().ForEach(p => { GoodPrices.Add(p); });
            GoodPricesView.Refresh();
        }

        internal void SetGoodActionEnabled(bool isEnabled)
        {
            if (SelectedGoodAction != null)
            {
                ClientContext.SetObjectActive("GoodActions", SelectedGoodAction.Id, isEnabled);
                SelectedGoodAction.IsActive = isEnabled;
            }
        }

        internal void SetPricePresence(bool isInPrice)
        {
            if (SelectedGoodPrice != null && SelectedGoodPrice.InPricelist != isInPrice)
            {
                SelectedGoodPrice.InPricelist = isInPrice;
                SelectedGoodPrice.Date = DateTime.Now;
                ClientContext.PostGoodPrice(SelectedGoodPrice);
                RefreshPrices();
            }
        }

        internal void RefreshPayments()
        {
            ProviderPayments.Clear();
            ClientContext.GetAllProviderPayments(OrdersStart, OrdersEnd).ForEach(p => { ProviderPayments.Add(p); });
            ProviderPaymentsView.Refresh();
        }

        public void RefreshProviders()
        {
            Providers.Clear();
            ClientContext.GetAllProviders().ForEach(p => Providers.Add(p));
            ProvidersView.Refresh();
        }

        internal void RefreshGoods()
        {
            Goods.Clear();
            ClientContext.GetAllGoods().ForEach(p => Goods.Add(p));
            GoodsView.Refresh();
        }

        internal void RefreshProviderFolders()
        {
            ConstructProviderFolders();
            ProviderFoldersView.Refresh();
            RefreshProviders();
            OnUpdateFinished();
        }

        internal void RefreshSalesPlan()
        {
            _SalesPlanView.Clear();
            _SalesPlanView.AddRange(ClientContext.GetSalesPlan());
            SalesPlanView.Refresh();
        }

        internal void RefreshConsignments()
        {
            Consignments.Clear();
            ClientContext.GetAllConsignments(ConsignmentsStart, ConsignmentsEnd, true).ForEach(p => { Consignments.Add(p); });
            ConsignmentsView.Refresh();
        }
    }
}
