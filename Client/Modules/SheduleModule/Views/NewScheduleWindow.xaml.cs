using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Practices.Unity;
using System.ComponentModel;
using ExtraClub.ServiceModel;
using System.Collections.ObjectModel;
using ExtraClub.UIControls;
using System.Diagnostics;
using System.ServiceModel;
using ExtraClub.ScheduleModule.Controls;
using ExtraClub.Infrastructure;
using ExtraClub.UIControls.Windows;
using System.Windows.Media;

namespace ExtraClub.ScheduleModule.Views
{
    public partial class NewScheduleWindow : INotifyPropertyChanged
    {
        public static int MaxTreatmentForOpt = 9;

        private int _maxTreatmentReserve;

        #region DataContext

        public Division Division { get; set; }

        public string TextActions { get; private set; }

        public bool IsSearchAllowed
        {
            get
            {
                return Customer != null && Customer.NoContraIndications.HasValue
                    && (_maxTreatmentReserve == 0 || SelectedTreatments.Count < _maxTreatmentReserve + 2)
                    && _tickets.Any(i => i.Helper);
            }
        }

        private Customer _customer;
        public Customer Customer
        {
            get { return _customer; }
            set
            {
                _customer = value;
                OnPropertyChanged("Customer");
                OnPropertyChanged("IsSearchAllowed");
            }
        }

        public ObservableCollection<TreatmentPlan> SelectedTreatments { get; set; }

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

        bool _optimalChecked = true;
        public bool IsOptimalChecked
        {
            get
            {
                return _optimalChecked;
            }
            set
            {
                if(_optimalChecked != value)
                {
                    _optimalChecked = value;
                    OnPropertyChanged("IsOptimalChecked");
                }
            }
        }

        bool _optimalAllowed = true;
        public bool IsOptimalAllowed
        {
            get
            {
                return _optimalAllowed;
            }
            set
            {
                if(_optimalAllowed != value)
                {
                    _optimalAllowed = value;
                    IsOptimalChecked = value;
                    OnPropertyChanged("IsOptimalAllowed");
                }
            }
        }


        private readonly List<Ticket> _tickets;
        private readonly List<ScheduleProposal> _proposals;

        public ICollectionView TicketsView { get; set; }
        public ICollectionView ProposalsView { get; set; }

        public bool IsParallelAllowed { get; set; }

        public IEnumerable<TreatmentConfig> TreatmentConfigs { get; set; }

        public List<TreatmentProgram> TreatmentPrograms { get; set; }

        private TreatmentProgram _selectedTreatmentProgram;

        public TreatmentProgram SelectedTreatmentProgram
        {
            get
            {
                return _selectedTreatmentProgram;
            }
            set
            {
                _selectedTreatmentProgram = value;
                if(_selectedTreatmentProgram.Id == Guid.Empty)
                {
                    SelectedTreatments.Clear();
                    IsOptimalAllowed = true;
                    listView.IsEnabled = true;
                    var tp = new TreatmentPlan { TreatmentConfigs = TreatmentConfigs };
                    //tp.PropertyChanged += new PropertyChangedEventHandler(tp_PropertyChanged);
                    SelectedTreatments.Add(tp);
                }
                else
                {
                    IsOptimalAllowed = false;
                    listView.IsEnabled = false;
                    SelectedTreatments.Clear();
                    foreach(var pl in _selectedTreatmentProgram.SerializedTreatmentProgramLines)
                    {
                        var tp = new TreatmentPlan { TreatmentConfigs = TreatmentConfigs };
                        var tc = tp.TreatmentConfigs.FirstOrDefault(tt => tt.Id == pl.TreatmentConfigId);
                        if(tc == null) continue;
                        //tp.PropertyChanged += new PropertyChangedEventHandler(tp_PropertyChanged);
                        SelectedTreatments.Add(tp);
                        tp.TreatmentConfigId = tp.TreatmentConfigs.First(tt => tt.Id == pl.TreatmentConfigId).Id;
                    }
                }

                OnPropertyChanged("SelectedTreatmentProgram");
                OnPropertyChanged("IsSearchAllowed");
            }
        }

        #endregion

        public NewScheduleWindow(ClientContext context, ScheduleRequestParams pars)
        {
            IsSmartTicket = Visibility.Collapsed;
            _context = context;
            IsParallelAllowed = true;
            _bw.DoWork += bw_DoWork;
            _bw.RunWorkerCompleted += bw_RunWorkerCompleted;

            _proposals = new List<ScheduleProposal>();
            ProposalsView = CollectionViewSource.GetDefaultView(_proposals);

            Owner = Application.Current.MainWindow;
            InitializeComponent();
            SelectedTreatments = new ObservableCollection<TreatmentPlan>();
            var tres = context.GetAllTreatments().Where(i => i.IsActive).Select(i => i.TreatmentTypeId).Distinct().ToList();
            TreatmentConfigs = context.GetAllTreatmentConfigs().Where(i => i.IsActive && tres.Contains(i.TreatmentTypeId)).ToArray();

            TreatmentPrograms = _context.GetTreatmentPrograms();
            TreatmentPrograms.Insert(0, new TreatmentProgram { Id = Guid.Empty, ProgramName = UIControls.Localization.Resources.Manual });
            SelectedTreatmentProgram = TreatmentPrograms[0];

            if(pars != null && pars.TreatmentConfigIds != null)
            {
                foreach(var i in pars.TreatmentConfigIds)
                {
                    var tp = new TreatmentPlan { TreatmentConfigs = TreatmentConfigs };
                    var tc = tp.TreatmentConfigs.FirstOrDefault(tt => tt.Id == i);
                    if(tc == null) continue;
                    SelectedTreatments.Add(tp);
                    tp.TreatmentConfigId = tp.TreatmentConfigs.First(tt => tt.Id == i).Id;
                }
            }
            else
            {
                var tp1 = new TreatmentPlan { TreatmentConfigs = TreatmentConfigs };
                SelectedTreatments.Add(tp1);
            }
            _context = context;
            Division = _context.CurrentDivision;
            DataContext = this;
            listView.ItemsSource = SelectedTreatments;
            var dm = new ListViewDragDropManager<TreatmentPlan>(listView)
            {
                ShowDragAdorner = true,
                DragAdornerOpacity = 0.75
            };
            _tickets = new List<Ticket>();
            TicketsView = CollectionViewSource.GetDefaultView(_tickets);
            listView.DragEnter += OnListViewDragEnter;
            if(pars != null && pars.Date.HasValue && pars.Date > DateTime.MinValue)
            {
                VisitDate = pars.Date.Value.Date;
                VisitTime = pars.Date.Value.TimeOfDay;
            }
            else
            {
                VisitDate = DateTime.Today;
                VisitTime = new TimeSpan(DateTime.Now.Hour + 1, 0, 0);
            }
            proposalsControl.SizeChanged += proposalsControl_SizeChanged;

            var ta = _context.GetCurrentActions();
            if(ta.Count > 0)
            {
                TextActions = "";
                foreach(var a in ta)
                {
                    if(!String.IsNullOrEmpty(TextActions)) TextActions += "\n";
                    TextActions += String.Format("{3} {0:d} {4} {1:d}: {2}", a.StartDate, a.FinishDate, a.ActionText, UIControls.Localization.Resources.From, UIControls.Localization.Resources.To);
                }
            }
            else
            {
                ActionsGroup.Visibility = Visibility.Collapsed;
            }

            if(pars != null && pars.Customer != null)
            {
                CustomerSearch.SelectById(pars.Customer.Id);
            }
            DatePicker.SelectableDateStart = DateTime.Today;
            ScheduleRequestParams = pars;
        }

        public ScheduleRequestParams ScheduleRequestParams { get; set; }

        void proposalsControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            customHeader.ClientWidth = e.NewSize.Width;
            customHeader.InvalidateVisual();
        }

        private void CustomerSearch_SelectedClientChanged(object sender, GuidEventArgs e)
        {
            Customer = _context.GetCustomer(e.Guid);
            if(!Customer.NoContraIndications.HasValue)
            {
                MessageBox.Show(UIControls.Localization.Resources.NoContrasMessage);
            }
            _tickets.ForEach(i => i.PropertyChanged -= i_PropertyChanged);
            _tickets.Clear();
            _tickets.AddRange(
                _context.GetTicketsForPlanning(Customer.Id));
            if(_tickets.Count == 0)
            {
                _tickets.Add(new Ticket
                {
                    Id = Guid.Empty,
                    Number = UIControls.Localization.Resources.WithoutTicket,
                    SerializedTicketType = new TicketType { Name = UIControls.Localization.Resources.Reserve }
                });
                _maxTreatmentReserve = _context.CurrentCompany.MaxTreatmentReserve;
            }
            else
            {
                _maxTreatmentReserve = 0;
            }
            _tickets.ForEach(i => i.PropertyChanged += i_PropertyChanged);
            TicketsView.Refresh();
            var prId = _context.GetTreatmentProgramIdForCustomer(Customer.Id);
            if(prId.HasValue)
            {
                SelectedTreatmentProgram = TreatmentPrograms.FirstOrDefault(i => i.Id == prId);
            }
            if(_tickets.Count == 1) _tickets[0].Helper = true;
            OnPropertyChanged("IsSearchAllowed");
        }

        private void i_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var ticket = sender as Ticket;
            if(ticket != null)
            {
                IsSmartTicket = ticket.SerializedTicketType.IsSmart ? Visibility.Visible : Visibility.Collapsed;
                OnPropertyChanged("IsSmartTicket");
            }

            OnPropertyChanged("IsSearchAllowed");
        }

        public Visibility IsSmartTicket { get; set; }

        void OnListViewDragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
        }

        private void RemoveTreatmentPlanButton_Click(object sender, RoutedEventArgs e)
        {
            var tp = ((Button)sender).DataContext as TreatmentPlan;
            if(tp != null && tp.TreatmentConfigId == Guid.Empty) return;
            SelectedTreatments.Remove(tp);

            IsOptimalAllowed = SelectedTreatments.Count <= MaxTreatmentForOpt;
            OnPropertyChanged("IsSearchAllowed");
        }

        private void TreatmentType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.RemovedItems.Count == 0)
            {
                var tp = new TreatmentPlan { TreatmentConfigs = TreatmentConfigs };
                //tp.PropertyChanged += new PropertyChangedEventHandler(tp_PropertyChanged);
                SelectedTreatments.Add(tp);
            }
            IsOptimalAllowed = (SelectedTreatments.Count <= MaxTreatmentForOpt && SelectedTreatmentProgram.Id == Guid.Empty);
            OnPropertyChanged("IsSearchAllowed");
        }

        readonly BackgroundWorker _bw = new BackgroundWorker();

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {

            proposalsControl.Children.Clear();

            var sel = _tickets.FirstOrDefault(t => t.Helper);
            if(sel == null) return;
            IsSearchProgress = true;
            var selected = (from t in SelectedTreatments where t.TreatmentConfigId != Guid.Empty select t.TreatmentConfigId).ToList();
            _proposals.ForEach(i => i.PropertyChanged -= proposal_PropertyChanged);
            _proposals.Clear();
            _bw.RunWorkerAsync(selected);
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if(e.Result is FaultException)
                {
                    MessageBox.Show((e.Result as FaultException).Reason.ToString());
                }
                if(e.Result is string)
                {
                    MessageBox.Show(e.Result as string);
                }
                _proposals.ForEach(i => i.PropertyChanged += proposal_PropertyChanged);

                CalculateTimes(_proposals);
                foreach(var p in _proposals)
                {
                    var tc = new TimelineControl { DataContext = p, Height = 40 };
                    tc.GotFocus += tc_GotFocus;
                    tc.MouseDown += tc_MouseDown;
                    proposalsControl.Children.Add(tc);
                }
                proposalsControl.InvalidateVisual();
                customHeader.InvalidateVisual();
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
                var firstOrDefault = _tickets.FirstOrDefault(t => t.Helper);
                if(firstOrDefault != null)
                {
                    var res = _context.GetScheduleProposals(_customer.Id,
                        firstOrDefault.Id,
                        VisitDate.Add(VisitTime),
                        IsParallelAllowed,
                        e.Argument as List<Guid>,
                        IsOptimalChecked, SelectedTreatmentProgram.Id);
                    _proposals.AddRange(res.List);
                    if(!String.IsNullOrEmpty(res.Result))
                    {
                        e.Result = res.Result;
                    }
                }
            }
            catch(FaultException error)
            {
                e.Result = error;
            }
        }

        void proposal_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "Prefer")
            {
                if(((ScheduleProposal)sender).Prefer)
                {
                    _proposals.ForEach(i =>
                    {
                        if(!i.Equals(sender)) i.Prefer = false;
                    });
                }
            }
        }

        void tc_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if(control != null) control.Focus();
        }

        void tc_GotFocus(object sender, RoutedEventArgs e)
        {
            _proposals.ForEach(i => i.Prefer = false);
            var control = sender as Control;
            if(control != null)
            {
                var scheduleProposal = control.DataContext as ScheduleProposal;
                if(scheduleProposal != null)
                    scheduleProposal.Prefer = true;
            }
        }

        public static Tuple<DateTime, DateTime> CalculateTimesEx(List<ScheduleProposal> proposals)
        {
            var w = Stopwatch.StartNew();
            var min = DateTime.MaxValue;
            var max = DateTime.MinValue;
            foreach(var p in proposals)
            {
                if(p.List.Count == 0) continue;
                var min1 = p.List.Min(i => i.StartTime);
                var max1 = p.List.Max(i => i.EndTime);
                if(min > min1) min = min1;
                if(max < max1) max = max1;

                for(int i = 1; i < p.List.Count; i++)
                {
                    if(!p.List.Select(x => (p.List[i].StartTime - x.EndTime).TotalMinutes).Any(x => x <= 5 && x >= 0))
                    {
                        p.List[i].BackColor = Brushes.Yellow;
                    }
                }
            }
            min = min.AddMinutes(-min.Minute);
            foreach(var p in proposals)
            {
                p.MinTime = min;
                p.MaxTime = max;
            }
            w.Stop();
            Debug.WriteLine("CalculateTimes for {0} items took {1} ms", proposals.Count, w.ElapsedMilliseconds);
            return new Tuple<DateTime, DateTime>(min, max);
        }

        private void CalculateTimes(List<ScheduleProposal> proposals)
        {
            var res = CalculateTimesEx(proposals);
            proposalsControl.MinTime = res.Item1;
            proposalsControl.MaxTime = res.Item2;
            customHeader.MinTime = res.Item1;
            customHeader.MaxTime = res.Item2;
        }

        private DateTime _visitDate;
        public DateTime VisitDate
        {
            get
            {
                return _visitDate;
            }
            set
            {
                _visitDate = value;
                OnPropertyChanged("VisitDate");
            }
        }

        public TimeSpan VisitTime { get; set; }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void proposalsControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var sel = _tickets.FirstOrDefault(t => t.Helper);
            if(sel == null) return;

            if(Customer == null) return;

            var prop = _proposals.FirstOrDefault(p => p.Prefer);
            if(prop == null) return;

            var infoMessage = "";

            if(sel.SerializedTicketFreezes != null && sel.SerializedTicketFreezes.Any(i => i.StartDate < prop.MaxTime && i.FinishDate > prop.MinTime))
            {
                infoMessage = "\nАбонемент заморожен в выделенный промежуток времени и будет автоматически разморожен, начиная с " + prop.MinTime.Date.ToString("d MMMM yyyy");
            }

            ExtraWindow.ConfirmOuter(UIControls.Localization.Resources.CustomerBooking,
                UIControls.Localization.Resources.MakeBookingMessage + infoMessage,
                e1 =>
                {
                    if((e1.DialogResult ?? false))
                    {
                        try
                        {
                            _context.PostScheduleProposal(Customer.Id, sel.Id, prop);
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            return;
                        }
                        Close();
                    }
                });

        }

        public event PropertyChangedEventHandler PropertyChanged;
        private readonly ClientContext _context;

        void OnPropertyChanged(string prop)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        private void MoveToSmartMaster(object sender, RoutedEventArgs e)
        {
            new SmartScheduleWindow(new ScheduleRequestParams { Customer = Customer, Date = VisitDate.Add(VisitTime), OnClose = ScheduleRequestParams.OnClose }).Show();

            Close();
        }
    }
}
