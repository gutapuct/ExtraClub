using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using TonusClub.Infrastructure;
using TonusClub.ServiceModel;
using TonusClub.ServiceModel.Schedule;
using TonusClub.UIControls;
using TonusClub.UIControls.Windows;

namespace TonusClub.ScheduleModule.Views
{
    public partial class SmartScheduleWindow : INotifyPropertyChanged
    {

        #region DataContext

        public Division Division { get; set; }

        bool _allowParallel = true;
        public bool AllowParallel
        {
            get
            {
                return _allowParallel;
            }
            set
            {
                _allowParallel = value;
                OnPropertyChanged("AllowParallel");
                TriggerSearch();
            }
        }

        public bool IsSearchAllowed
        {
            get
            {
                var res = Customer != null && /*Customer.NoContraIndications.HasValue
                    &&*/ _tickets.Any(i => i.Helper);
                if(!res)
                {
                    SearchResult = new List<ScheduleProposal>();
                }
                return res;
            }
        }

        private Customer _customer;
        public Customer Customer
        {
            get { return _customer; }
            set
            {
                _customer = value;
                if(_customer != null)
                {
                    LastVisitText = _context.GetLastVisitText(_customer.Id);
                    CustomerTargets = _context.GetCustomerTargets(_customer.Id);
                }
                OnPropertyChanged("Customer");
                OnPropertyChanged("IsSearchAllowed");
                TriggerSearch();
            }
        }

        Guid _selectedTargetId;
        public Guid SelectedTargetId
        {
            get
            {
                return _selectedTargetId;
            }
            set
            {
                _selectedTargetId = value;
                TriggerSearch();
            }
        }

        private List<CustomerTarget> _customerTargets;
        public List<CustomerTarget> CustomerTargets
        {
            get
            {
                return _customerTargets;
            }
            set
            {
                _customerTargets = value;
                OnPropertyChanged("CustomerTargets");
            }
        }

        bool _isSearchProgress;
        public bool IsSearchProgress
        {
            get
            {
                return _isSearchProgress;
            }
            set
            {
                _isSearchProgress = value;
                OnPropertyChanged("IsSearchProgress");
            }
        }

        string _lastVisitText = "";
        public string LastVisitText
        {
            get
            {
                return _lastVisitText;
            }
            set
            {
                _lastVisitText = value;
                OnPropertyChanged("LastVisitText");
            }
        }

        private readonly List<Ticket> _tickets;

        public ICollectionView TicketsView { get; set; }

        private DateTime _visitDate;
        public DateTime VisitDate
        {
            get
            {
                return _visitDate;
            }
            set
            {
                if(_visitDate != value)
                {
                    _visitDate = value;
                    OnPropertyChanged("VisitDate");
                    TriggerSearch();
                }
            }
        }

        private TimeSpan _visitTime;
        public TimeSpan VisitTime
        {
            get
            {
                return _visitTime;
            }
            set
            {
                if(_visitTime != value)
                {
                    _visitTime = value;
                    OnPropertyChanged("VisitTime");
                    TriggerSearch();
                }
            }
        }

        private string _searchError;
        public string SearchError
        {
            get
            {
                return _searchError;
            }
            set
            {
                _searchError = value;
                OnPropertyChanged("SearchError");
            }
        }


        private List<ScheduleProposal> _searchResult;
        public List<ScheduleProposal> SearchResult
        {
            get
            {
                return _searchResult;
            }
            set
            {
                _searchResult = value;
                OnPropertyChanged("SearchResult");
            }
        }

        #endregion

        readonly BackgroundWorker _bw = new BackgroundWorker();
        private ClientContext _context => ApplicationDispatcher.UnityContainer.Resolve<ClientContext>();

        public readonly ScheduleRequestParams ScheduleRequestParams;

        public SmartScheduleWindow(ScheduleRequestParams pars)
        {
            _bw.DoWork += bw_DoWork;
            _bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            ScheduleRequestParams = pars;
            Owner = Application.Current.MainWindow;
            InitializeComponent();
            Division = _context.CurrentDivision;
            DataContext = this;
            _tickets = new List<Ticket>();
            TicketsView = CollectionViewSource.GetDefaultView(_tickets);

            VisitDate = DateTime.Today;
            VisitTime = new TimeSpan(DateTime.Now.Hour + 1, 0, 0);


            if(pars != null)
            {
                if(pars.Customer != null)
                {
                    CustomerSearch.SelectById(pars.Customer.Id);
                }
                if(pars.Date.HasValue)
                {
                    VisitDate = pars.Date.Value.Date;
                    VisitTime = pars.Date.Value.TimeOfDay;
                }
            }
            DatePicker.SelectableDateStart = DateTime.Today;
        }

        private void TriggerSearch()
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                lock(_bw)
                {
                    while(_bw.IsBusy)
                    {
                        Thread.Sleep(200);
                    }
                    if(IsSearchAllowed)
                    {
                        _bw.RunWorkerAsync();
                    }
                }
            });
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if(e.Result is FaultException)
                {
                    SearchError = (e.Result as FaultException).Reason.ToString();
                }
                if(e.Result is string)
                {
                    SearchError = e.Result as string;
                }
                if(e.Result is ScheduleProposalResult)
                {
                    var res = e.Result as ScheduleProposalResult;
                    SearchError = res.Result;
                    if(res.List != null)
                    {
                        var sres = res.List;//.Take(3).ToList();
                        NewScheduleWindow.CalculateTimesEx(sres);
                        var rand = new Random();
                        SearchResult = sres.OrderBy(i => i.List.Any(j => j.BackColor != null)).ThenBy(j => rand.Next(1000)).ToList();
                    }
                    else
                    {
                        SearchResult = new List<ScheduleProposal>();
                    }
                }
            }
            finally
            {
                IsSearchProgress = false;
            }
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                IsSearchProgress = true;
                if(!_tickets.Any(i => i.Helper))
                {
                    e.Result = "Укажите абонемент!";
                    return;
                }
                var firstOrDefault = _tickets.FirstOrDefault(t => t.Helper);
                if(firstOrDefault != null)
                {
                    var res = _context.GetSmartProposals(_customer.Id,
                        firstOrDefault.Id,
                        VisitDate.Add(VisitTime),
                        SelectedTargetId,
                        AllowParallel);
                    e.Result = res;
                }
            }
            catch(FaultException error)
            {
                e.Result = error;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string prop)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        private void CustomerSearch_SelectedClientChanged(object sender, GuidEventArgs e)
        {
            Customer = _context.GetCustomer(e.Guid);

            _tickets.ForEach(i => i.PropertyChanged -= i_PropertyChanged);
            _tickets.Clear();
            _tickets.AddRange(_context.GetTicketsForPlanning(Customer.Id).Where(i => i.SerializedTicketType.IsSmart));

            SearchError = !_tickets.Any() ? "У клиента нет ни одного смарт-абонемента!" : "";
            TicketsView.Refresh();
            if(_tickets.Count == 1) _tickets[0].Helper = true;
            _tickets.ForEach(i => i.PropertyChanged += i_PropertyChanged);
            OnPropertyChanged("IsSearchAllowed");
            TriggerSearch();
        }

        void i_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            TriggerSearch();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void PostScheduleClick(object sender, RoutedEventArgs e)
        {
            var dc = ((FrameworkElement)sender).DataContext as ScheduleProposal;
            var firstOrDefault = _tickets.FirstOrDefault(t => t.Helper);
            if(firstOrDefault != null)
            {
                try
                {
                    _context.PostScheduleProposal(Customer.Id, firstOrDefault.Id, dc);
                }
                catch(FaultException ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                if(ScheduleRequestParams != null) ScheduleRequestParams.ExecuteCloseMethod();
                Close();
            }
        }

        private void StandartMaster_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.MakeScheduleRequest(new ScheduleRequestParams
            {
                Customer = Customer,
                Date = _visitDate.Add(_visitTime),
                TreatmentConfigIds = ((_searchResult ?? new List<ScheduleProposal>()).FirstOrDefault() ?? new ScheduleProposal()).List.Select(j => j.ConfigId).ToArray()
            });
            Close();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var sp = (sender as FrameworkElement).DataContext as ScheduleProposal;
            NavigationManager.MakeScheduleRequest(new ScheduleRequestParams
            {
                Customer = Customer,
                Date = _visitDate.Add(_visitTime),
                TreatmentConfigIds = sp.List.Select(j => j.ConfigId).ToArray()
            });
            Close();

        }
    }
}
