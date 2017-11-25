using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TonusClub.Infrastructure.BaseClasses;
using Microsoft.Practices.Unity;
using TonusClub.Infrastructure.Interfaces;
using TonusClub.ServiceModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections;
using System.Timers;
using System.Windows.Threading;
using System.Windows;
using TonusClub.ServiceModel.Organizer;
using TonusClub.UIControls.BaseClasses;

namespace TonusClub.OrganizerModule.ViewModels
{
    public class OrganizerLargeViewModel : ViewModelBase
    {
        Timer t = new Timer(300000);

        private ArrayList _orgItems = new ArrayList();
        public ICollectionView OrganizerItems { get; private set; }

        OrganizerItem _SelectedOrganizerItem;
        public OrganizerItem SelectedOrganizerItem
        {
            get
            {
                return _SelectedOrganizerItem;
            }
            set
            {
                _SelectedOrganizerItem = value;
                OnPropertyChanged("SelectedOrganizerItem");
                OnPropertyChanged("IsCardButtonEnabled");
            }
        }

        public bool IsCardButtonEnabled
        {
            get
            {
                if (SelectedOrganizerItem == null) return false;
                var prop = SelectedOrganizerItem.Data.GetType().GetProperty("CustomerId");
                return prop != null;

            }
        }

        public ICollectionView OutboxOrganizerView { get; private set; }
        private List<OrganizerItem> _OutboxOrganizerView = new List<OrganizerItem>();

        public ICollectionView ArchivedOrganizerView { get; private set; }
        private List<OrganizerItem> _ArchivedOrganizerView = new List<OrganizerItem>();

        private List<Call> Calls = new List<Call>();
        public ICollectionView CallsView { get; private set; }

        private DateTime callsStart = DateTime.Today.AddDays(-14);
        public DateTime CallsStart
        {
            get
            {
                return callsStart;
            }
            set
            {
                callsStart = value;
                if (callsStart > CallsEnd)
                {
                    callsStart = callsEnd;
                }
                OnPropertyChanged("CallsStart");
                RefreshCalls();
            }
        }

        private DateTime callsEnd = DateTime.Today;
        public DateTime CallsEnd
        {
            get
            {
                return callsEnd;
            }
            set
            {
                callsEnd = value;
                if (CallsStart > callsEnd)
                {
                    callsEnd = callsStart;
                }
                OnPropertyChanged("CallsEnd");
                RefreshCalls();
            }
        }

        public ICollectionView ClaimsView { get; private set; }
        private List<Claim> Claims = new List<Claim>();

        private DateTime claimsStart = DateTime.Today.AddMonths(-1);
        public DateTime ClaimsStart
        {
            get
            {
                return claimsStart;
            }
            set
            {
                claimsStart = value;
                if (claimsStart > ClaimsEnd)
                {
                    claimsStart = ClaimsEnd;
                }
                OnPropertyChanged("ClaimsStart");
                RefreshClaims();
            }
        }

        private DateTime claimsEnd = DateTime.Today;
        public DateTime ClaimsEnd
        {
            get
            {
                return claimsEnd;
            }
            set
            {
                claimsEnd = value;
                if (claimsStart > claimsEnd)
                {
                    claimsEnd = claimsStart;
                }
                OnPropertyChanged("ClaimsEnd");
                RefreshClaims();
            }
        }

        public ICollectionView AnketsView { get; private set; }
        private List<AnketInfo> Ankets = new List<AnketInfo>();

        private DateTime anketsStart = DateTime.Today.AddMonths(-6);
        public DateTime AnketsStart
        {
            get
            {
                return anketsStart;
            }
            set
            {
                anketsStart = value;
                if (anketsStart > AnketsEnd)
                {
                    anketsStart = AnketsEnd;
                }
                OnPropertyChanged("AnketsStart");
                RefreshAnkets();
            }
        }

        private DateTime anketsEnd = DateTime.Today;
        public DateTime AnketsEnd
        {
            get
            {
                return anketsEnd;
            }
            set
            {
                anketsEnd = value;
                if (anketsStart > anketsEnd)
                {
                    anketsEnd = anketsStart;
                }
                OnPropertyChanged("AnketsEnd");
                RefreshAnkets();
            }
        }

        bool _ShowClosedClaims = false;
        public bool ShowClosedClaims
        {
            get
            {
                return _ShowClosedClaims;
            }
            set
            {
                _ShowClosedClaims = value;
                OnPropertyChanged("ShowClosedClaims");
                RefreshClaims();
            }
        }

        Claim _selectedClaim;
        public Claim SelectedClaim
        {
            get
            {
                return _selectedClaim;
            }
            set
            {
                _selectedClaim = value;
                OnPropertyChanged("SelectedClaim");
                OnPropertyChanged("HasSelectedClaim");
            }
        }

        public bool HasSelectedClaim
        {
            get
            {
                return SelectedClaim != null;
            }
        }

        private DateTime tasksStart = DateTime.Today.AddDays(-14);
        public DateTime TasksStart
        {
            get
            {
                return tasksStart;
            }
            set
            {
                tasksStart = value;
                if (tasksStart > TasksEnd)
                {
                    tasksStart = tasksEnd;
                }
                OnPropertyChanged("TasksStart");
                RefreshTasks();
            }
        }

        private DateTime tasksEnd = DateTime.Today;
        public DateTime TasksEnd
        {
            get
            {
                return tasksEnd;
            }
            set
            {
                tasksEnd = value;
                if (TasksStart > tasksEnd)
                {
                    tasksEnd = tasksStart;
                }
                OnPropertyChanged("TasksEnd");
                RefreshTasks();
            }
        }

        internal void RefreshTasks()
        {
            _orgItems.Clear();
            _orgItems.AddRange(ClientContext.GetOrganizerItems(TasksStart, TasksEnd));
            OrganizerItems.Refresh();
        }

        public OrganizerLargeViewModel(IUnityContainer container)
            : base()
        {
            OrganizerItems = CollectionViewSource.GetDefaultView(_orgItems);
            OutboxOrganizerView = CollectionViewSource.GetDefaultView(_OutboxOrganizerView);
            ArchivedOrganizerView = CollectionViewSource.GetDefaultView(_ArchivedOrganizerView);
            CallsView = CollectionViewSource.GetDefaultView(Calls);
            ClaimsView = CollectionViewSource.GetDefaultView(Claims);
            AnketsView = CollectionViewSource.GetDefaultView(Ankets);
            t.Elapsed += new ElapsedEventHandler(t_Elapsed);
            t.Start();
        }

        void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            RefreshClaims();
        }

        protected override void RefreshDataInternal()
        {
            _orgItems.Clear();
            _orgItems.AddRange(ClientContext.GetOrganizerItems(TasksStart, TasksEnd));

            Calls.Clear();
            Calls.AddRange(ClientContext.GetDivisionCalls(callsStart, callsEnd));

            _OutboxOrganizerView.Clear();
            _OutboxOrganizerView.AddRange(ClientContext.GetOutboxOrganizerItems());

            _ArchivedOrganizerView.Clear();
            _ArchivedOrganizerView.AddRange(ClientContext.GetArchivedOrganizerItems());

            Claims.Clear();
            Claims.AddRange(ClientContext.GetClaims(claimsStart, claimsEnd, _ShowClosedClaims));

            Ankets.Clear();
            Ankets.AddRange(ClientContext.GetAnkets(anketsStart, anketsEnd));
        }

        protected override void RefreshFinished()
        {
            base.RefreshFinished();
            OrganizerItems.Refresh();
            CallsView.Refresh();
            OutboxOrganizerView.Refresh();
            ArchivedOrganizerView.Refresh();
            ClaimsView.Refresh();
            AnketsView.Refresh();
        }

        public void RefreshCalls()
        {
            Calls.Clear();
            Calls.AddRange(ClientContext.GetDivisionCalls(callsStart, callsEnd));
            CallsView.Refresh();
        }

        public void RefreshClaims()
        {
            Claims.Clear();
            Claims.AddRange(ClientContext.GetClaims(claimsStart, claimsEnd, _ShowClosedClaims));
            Application.Current.Dispatcher.Invoke(new Action(() => ClaimsView.Refresh()));
        }

        internal void RefreshMyTasks()
        {
            _OutboxOrganizerView.Clear();
            _OutboxOrganizerView.AddRange(ClientContext.GetOutboxOrganizerItems());
            OutboxOrganizerView.Refresh();
        }

        internal void RefreshArchivedTasks()
        {
            _ArchivedOrganizerView.Clear();
            _ArchivedOrganizerView.AddRange(ClientContext.GetArchivedOrganizerItems());
            ArchivedOrganizerView.Refresh();
        }

        internal void RefreshAnkets()
        {
            Ankets.Clear();
            Ankets.AddRange(ClientContext.GetAnkets(anketsStart, anketsEnd));
            AnketsView.Refresh();
        }
    }
}
