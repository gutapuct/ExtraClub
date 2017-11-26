using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Practices.Unity;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.ScheduleView;
using ExtraClub.Infrastructure;
using ExtraClub.ScheduleModule.Controls;
using ExtraClub.ScheduleModule.Views.Solarium;
using ExtraClub.ScheduleModule.Views.Treatments.Windows;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls;
using ExtraClub.UIControls.Localization;
using ExtraClub.UIControls.Windows;
using ViewModelBase = ExtraClub.UIControls.BaseClasses.ViewModelBase;

namespace ExtraClub.ScheduleModule.ViewModels
{
    public class ScheduleLargeViewModel : ViewModelBase
    {
        #region solarium
        public ICollectionView SolariumResourcesView { get; set; }
        private List<ResourceType> _solResources = new List<ResourceType>();
        public ObservableCollection<Appointment> SolAppointmentsView { get; set; }
        Timer _solariumTimer = new Timer(180000);

        private bool _autorefreshSolarium;
        public bool AutorefreshSolarium
        {
            get
            {
                return _autorefreshSolarium;
            }
            set
            {
                if (_autorefreshSolarium != value)
                {
                    if (value)
                    {
                        _solariumTimer.Start();
                    }
                    else
                    {
                        _solariumTimer.Stop();
                    }
                    _autorefreshSolarium = value;
                    OnPropertyChanged("AutorefreshSolarium");
                }
            }
        }

        public ObservableCollection<Slot> SolariumSlots { get; set; }

        void solariumTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            LoadSolariumAppointments(null);
        }

        private bool _changeSolDefinitionFlag;
        public bool ChangeSolDefinitionFlag
        {
            get
            {
                if (_changeSolDefinitionFlag)
                {
                    _changeSolDefinitionFlag = false;
                    return true;
                }
                return false;
            }
            set
            {
                _changeSolDefinitionFlag = value;
            }
        }

        private DateSpan _solLastSpan;
        private void LoadSolariumAppointments(DateSpan dateSpan)
        {
            if (dateSpan == null)
            {
                if (_solLastSpan != null)
                {
                    dateSpan = _solLastSpan;
                }
                else
                {
                    dateSpan = new DateSpan(DateTime.Today, DateTime.Now);
                }
            }
            _solLastSpan = dateSpan;

            ClientContext.ExecuteMethodAsync(c => c.GetDivisionSolariumVisits(ClientContext.CurrentDivision.Id, dateSpan.Start, dateSpan.End))
                .ContinueWith(res =>
                {
                    SolAppointmentsView.RemoveAll();
                    res.Result.ForEach(i =>
                    {
                        if (i.eStatus != SolariumVisitStatus.Canceled)
                        {
                            var app = new Appointment { End = i.VisitDate.AddMinutes(i.Amount), Start = i.VisitDate, Subject = i.SerializedCustomerName + "\n" + i.TimeText + "\n" + i.Amount + " мин.\n" + i.Comment, UniqueId = i.Id.ToString() };
                            app.Resources.Add(new Resource { ResourceName = i.SolariumId.ToString(), ResourceType = "Solarium" });
                            app.Resources.Add(new Resource { ResourceName = i.Status.ToString(), ResourceType = "Status" });
                            app.Resources.Add(new Resource { ResourceName = i.CustomerId.ToString(), ResourceType = "Customer" });
                            SolAppointmentsView.Add(app);
                        }
                    });

                }, TaskScheduler.FromCurrentSynchronizationContext());

        }



        #endregion

        public ICollectionView TreatmentsResourcesView { get; set; }
        private readonly List<ResourceType> _treResources = new List<ResourceType>();

        public ObservableCollection<CustomAppointment> TreatmentsAppointmentsView { get; set; }

        bool _treatmentInactivesVisibility;

        public bool HasNewRecords
        {
            get
            {
                return TreatmentsAppointmentsView.Any(i => i.Subject == Resources.New);
            }
        }

        private string _searchText = "";
        public string SearchText
        {
            get
            {
                return _searchText;
            }
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    ProcessSearch();
                    OnPropertyChanged("SearchText");
                }
            }
        }

        public string SearchCardNumber
        {
            get
            {
                return Resources.ClickCardToSearch;
            }
            set
            {
                SearchText = value;
            }
        }

        Timer _treatmentsTimer = new Timer(180000);

        private bool _autorefreshTreatments;
        public bool AutorefreshTreatments
        {
            get
            {
                return _autorefreshTreatments;
            }
            set
            {
                if (_autorefreshTreatments != value)
                {
                    if (value)
                    {
                        _treatmentsTimer.Start();
                    }
                    else
                    {
                        _treatmentsTimer.Stop();
                    }
                    _autorefreshTreatments = value;
                    OnPropertyChanged("AutorefreshTreatments");
                }
            }
        }


        Dictionary<Guid, TreatmentType> _ttypes;
        List<TreatmentConfig> _tconfs;


        public ObservableCollection<Slot> TreatmentSlots { get; set; }

        public Division Division => ClientContext.CurrentDivision;

        public ScheduleLargeViewModel()
        {
            TreatmentSlots = new ObservableCollection<Slot>();
            SolariumSlots = new ObservableCollection<Slot>();

            SolAppointmentsView = new ObservableCollection<Appointment>();
            TreatmentsAppointmentsView = new ObservableCollection<CustomAppointment>();
            TreatmentsAppointmentsView.CollectionChanged += TreatmentsAppointmentsView_CollectionChanged;
            _solResources.Add(new ResourceType { DisplayName = Resources.Solarium, Name = "Solarium" });
            _solResources.Add(new ResourceType { DisplayName = Resources.Status, Name = "Status" });
            _solResources.Add(new ResourceType { DisplayName = Resources.Customer, Name = "Customer" });
            _solResources[1].Resources.Add(new Resource { ResourceName = "0", DisplayName = Resources.PlannedM });
            _solResources[1].Resources.Add(new Resource { ResourceName = "1", DisplayName = Resources.CancelledM });
            _solResources[1].Resources.Add(new Resource { ResourceName = "2", DisplayName = Resources.VisitedM });
            _solResources[1].Resources.Add(new Resource { ResourceName = "3", DisplayName = Resources.MissedM });
            SolariumResourcesView = CollectionViewSource.GetDefaultView(_solResources);

            _treResources.Add(new ResourceType { DisplayName = Resources.Treatment, Name = "Treatment" });
            _treResources.Add(new ResourceType { DisplayName = Resources.Status, Name = "Status" });
            _treResources.Add(new ResourceType { DisplayName = Resources.Customer, Name = "Customer" });
            _treResources.Add(new ResourceType { DisplayName = "Id", Name = "Id" });

            _treResources[1].Resources.Add(new Resource { ResourceName = "0", DisplayName = Resources.PlannedM });
            _treResources[1].Resources.Add(new Resource { ResourceName = "1", DisplayName = Resources.CancelledM });
            _treResources[1].Resources.Add(new Resource { ResourceName = "2", DisplayName = Resources.VisitedM });
            _treResources[1].Resources.Add(new Resource { ResourceName = "3", DisplayName = Resources.MissedM });
            TreatmentsResourcesView = CollectionViewSource.GetDefaultView(_treResources);

            _treatmentsTimer.Elapsed += treatmentsTimer_Elapsed;
            _solariumTimer.Elapsed += solariumTimer_Elapsed;

            VisibleRangeChanged = new DelegateCommand(VisibleRangeExecuted, CanVisibleRangeCanExecuted);
            TreatmentsVisibleRangeChanged = new DelegateCommand(TreatmentsVisibleRangeExecuted, CanVisibleRangeCanExecuted);
        }

        void treatmentsTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            LoadTreatmentsAppointments(null);
        }

        void TreatmentsAppointmentsView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("HasNewRecords");
        }

        private List<Solarium> _sols;
        private List<Treatment> _tres;

        internal void RefreshTreGrid()
        {
            _ttypes = ClientContext.GetAllTreatmentTypes().Where(i => i.IsActive).ToDictionary(i => i.Id, i => i);
            _tconfs = ClientContext.GetAllTreatmentConfigs().Where(i => i.IsActive).ToList();
            _tres = ClientContext.GetAllTreatments().Where(i => i.IsActive || _treatmentInactivesVisibility).ToList();

            TreatmentSlots.Clear();
            _treResources[0].Resources.Clear();

            _tres.ForEach(CreateSlots);
        }

        protected override void RefreshDataInternal()
        {
            _ttypes = ClientContext.GetAllTreatmentTypes().Where(i => i.IsActive).ToDictionary(i => i.Id, i => i);
            _tconfs = ClientContext.GetAllTreatmentConfigs().Where(i => i.IsActive).ToList();
            _sols = ClientContext.GetDivisionSolariums(false);
            _tres = ClientContext.GetAllTreatments().Where(i => i.IsActive || _treatmentInactivesVisibility).ToList();
        }

        protected override void RefreshFinished()
        {
            var flag = _solResources?[0].Resources != null && _solResources[0].Resources.Count > 0;
            if (_sols.Count != _solResources[0].Resources.Count)
            {
                _changeSolDefinitionFlag = true;
            }
            _solResources[0].Resources.Clear();
            _sols.ForEach(i =>
                {
                    var res = new Resource { ResourceName = i.Id.ToString(), DisplayName = i.Name };
                    _solResources[0].Resources.Add(res);
                    if (!i.IsActive)
                    {
                        var slot = new Slot(DateTime.MinValue, DateTime.MaxValue);
                        slot.IsReadOnly = true;
                        slot.Resources.Add(res);
                        SolariumSlots.Add(slot);
                    }
                });
            if (!flag) SolAppointmentsView.Clear();
            LoadSolariumAppointments(_solLastSpan);


            flag = false;
            if (_treResources[0].Resources.Count > 0) flag = true;

            TreatmentSlots.Clear();

#if BEAUTINIKA
            treResources[4].Resources.Clear();
            emps.ForEach(i =>
            {
                CreateSlots(i);
            });
#else
            _treResources[0].Resources.Clear();
            _tres.ForEach(i =>
                {
                    CreateSlots(i);
                });
#endif

            if (!flag) TreatmentsAppointmentsView.Clear();
            LoadTreatmentsAppointments(TreLastSpan);

        }

#if BEAUTINIKA
        private void CreateSlots(Employee i)
#else
        private void CreateSlots(Treatment i)
#endif
        {
#if BEAUTINIKA
            var res = new Resource { ResourceName = i.Id.ToString(), DisplayName = i.SerializedCustomer.LastName };
            treResources[4].Resources.Add(res);
#else
            var res = new Resource { ResourceName = i.Id.ToString(), DisplayName = i.Tag };
            _treResources[0].Resources.Add(res);
            if (!i.IsActive)
            {
                var slot = new Slot(DateTime.MinValue, DateTime.MaxValue);
                slot.IsReadOnly = true;
                slot.Resources.Add(res);
                TreatmentSlots.Add(slot);
            }
#endif
            if (ClientContext.CurrentDivision.OpenTime.HasValue && ClientContext.CurrentDivision.CloseTime.HasValue)
            {
                var n = ClientContext.CurrentDivision.OpenTime.Value.Hours;
                while (n < ClientContext.CurrentDivision.CloseTime.Value.Hours + 1)
                {
                    var hs = new MySlot
                    {
                        Start = _solLastSpan.Start.Date.AddMonths(-1).AddHours(n + 0.5),
                        End = _solLastSpan.Start.Date.AddMonths(-1).AddHours(n + 0.5).AddMinutes(30),
                        RecurrencePattern = new RecurrencePattern { Frequency = RecurrenceFrequency.Daily },
                        MainAlternation = false,
                        SubAlternation = true
                    };
                    hs.Resources.Add(res);
                    TreatmentSlots.Add(hs);
                    hs = new MySlot
                    {
                        Start = _solLastSpan.Start.Date.AddMonths(-1).AddHours(n + 1),
                        End = _solLastSpan.Start.Date.AddMonths(-1).AddHours(n + 1).AddMinutes(30),
                        RecurrencePattern = new RecurrencePattern { Frequency = RecurrenceFrequency.Daily },
                        MainAlternation = true,
                        SubAlternation = false
                    };
                    hs.Resources.Add(res);
                    TreatmentSlots.Add(hs);
                    hs = new MySlot
                    {
                        Start = _solLastSpan.Start.Date.AddMonths(-1).AddHours(n + 1.5),
                        End = _solLastSpan.Start.Date.AddMonths(-1).AddHours(n + 1.5).AddMinutes(30),
                        RecurrencePattern = new RecurrencePattern { Frequency = RecurrenceFrequency.Daily },
                        MainAlternation = true,
                        SubAlternation = true
                    };
                    hs.Resources.Add(res);
                    TreatmentSlots.Add(hs);
                    n += 2;
                }
            }
        }

        private ICommand _visibleRangeChanged;

        public ICommand VisibleRangeChanged
        {
            get
            {
                return _visibleRangeChanged;
            }
            set
            {
                _visibleRangeChanged = value;
            }
        }

        private ICommand _treatmentsVisibleRangeChanged;

        public ICommand TreatmentsVisibleRangeChanged
        {
            get
            {
                return _treatmentsVisibleRangeChanged;
            }
            set
            {
                _treatmentsVisibleRangeChanged = value;
            }
        }

        private void VisibleRangeExecuted(object param)
        {
            LoadSolariumAppointments(param as DateSpan);
        }

        private void TreatmentsVisibleRangeExecuted(object param)
        {
            _treatmetnsVisibleRange = param as DateSpan;
            LoadTreatmentsAppointments(param as DateSpan);
        }

        private bool CanVisibleRangeCanExecuted(object param)
        {
            return param != null;
        }

        public DateSpan TreLastSpan;

        public void LoadTreatmentsAppointments(DateSpan dateSpan)
        {
            if (dateSpan == null)
            {
                dateSpan = TreLastSpan ?? new DateSpan(DateTime.Today, DateTime.Now);
            }

            TreLastSpan = dateSpan;
            ClientContext.ExecuteMethodAsync(
                c => c.GetDivisionTreatmetnsVisits(ClientContext.CurrentDivision.Id, dateSpan.Start, dateSpan.End))
                .ContinueWith(res =>
                {
                    TreatmentsAppointmentsView.RemoveAll();

                    res.Result.ForEach(i =>
                    {
                        if (i.VisitStatus != 1)
                        {
                            var app = new CustomAppointment
                            {
                                CustomerName = i.SerializedCustomerInfo,
                                CustomColor = GetColorById(i.CustomColorId),
                                End = i.VisitDate.AddMinutes(i.SerializedDuration),
                                Start = i.VisitDate,
                                Subject = GetAppointmentText(i),
                                UniqueId = i.Id.ToString()
                            };
                            app.Resources.Add(new Resource { ResourceName = i.TreatmentId.ToString(), ResourceType = "Treatment" });
                            app.Resources.Add(new Resource { ResourceName = i.VisitStatus.ToString(), ResourceType = "Status" });
                            app.Resources.Add(new Resource { ResourceName = i.CustomerId.ToString(), ResourceType = "Customer" });
                            app.Resources.Add(new Resource { ResourceName = i.Id.ToString(), ResourceType = "Id" });
                            TreatmentsAppointmentsView.Add(app);
                        }
                    });

                    ProcessSearch();

                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private string GetAppointmentText(TreatmentEvent i)
        {
            var sb = new StringBuilder();

            if (AppSettingsManager.GetSettingCached("ShowNames"))
            {
                var s = i.SerializedCustomerInfo ?? "";
                if (s.Contains(']')) s = s.Substring(s.IndexOf(']') + 1).Trim();
                /*if (i.SerializedCardNumber != null)*/
                sb.AppendLine(s);
            }
            if (AppSettingsManager.GetSettingCached("ShowCardNums"))
            {
                if (i.SerializedCardNumber != null) sb.AppendLine(i.SerializedCardNumber);
            }
            if (AppSettingsManager.GetSettingCached("ShowPhones"))
            {
                sb.AppendLine("Телефон: " + i.SerializedCustomerPhone);
            }
            sb.AppendLine(i.TimeText);
            sb.AppendLine(i.SerializedDuration + Resources.min);
            if (!String.IsNullOrWhiteSpace(i.Comment)) sb.AppendLine(i.Comment);
            return sb.ToString();
        }

        private Brush GetColorById(int? colorId)
        {
            if (colorId == 0) return Brushes.Red;
            if (colorId == 1) return Brushes.DarkGreen;
            if (colorId == 2) return Brushes.MediumBlue;
            if (colorId == 3) return Brushes.Gold;
            if (colorId == 4) return Brushes.DarkOrange;
            if (colorId == 5) return Brushes.DarkOrchid;
            if (colorId == 6) return Brushes.HotPink;
            if (colorId == 7) return Brushes.PaleGreen;
            if (colorId == 8) return Brushes.Aqua;
            return Brushes.Transparent;
        }

        internal void AddNewAppointment(IAppointment e)
        {
            if (e.Start < DateTime.Today) return;
            var app = new CustomAppointment();
            app.Subject = Resources.New;
            app.Resources.Add(new Resource { ResourceName = "0", ResourceType = "Status" });
            app.Resources.Add(new Resource { ResourceName = Guid.Empty.ToString(), ResourceType = "Customer" });
            app.Resources.AddRange(e.Resources);
            var treatmentId = Guid.Parse(((Resource)e.Resources[0]).ResourceName);

            var treatment = _tres.Single(i => i.Id == treatmentId);
            var tcs = _tconfs.Where(i => i.TreatmentTypeId == treatment.TreatmentTypeId && i.IsActive).OrderBy(o => o.SerializedFullDuration).ToArray();
            if (tcs.Length == 0) return;

            var start = e.Start;

            var okAct = new Action<TreatmentConfig>(x =>
            {
                if (x != null)
                {
                    app.Start = BindEvent(start, x);
                    app.End = app.Start.AddMinutes(x.SerializedFullDuration);
                    app.Resources.Add(new Resource { ResourceName = x.Id.ToString(), ResourceType = "TreatmentConfig" });
                    TreatmentsAppointmentsView.Add(app);
                }
            });

            if (tcs.Length == 1)
            {
                okAct(tcs[0]);
            }
            else
            {
                SelectTreatmentConfig(tcs, okAct);
            }
        }

        private void SelectTreatmentConfig(TreatmentConfig[] tcs, Action<TreatmentConfig> okAct)
        {
            ModuleViewBase.ProcessUserDialog<SelectConfigWindow>(ApplicationDispatcher.UnityContainer, w =>
            {
                Dispatcher.CurrentDispatcher.Invoke(new Action(() =>
                {
                    if (w.TcResult != null)
                    {
                        okAct(w.TcResult);
                    }
                }));
            }, new ParameterOverride("tcs", tcs));
        }

        private DateTime BindEvent(DateTime dateTime, TreatmentConfig tc)
        {
            var tt = _ttypes[tc.TreatmentTypeId];
            var ts = ClientContext.CurrentDivision.OpenTime ?? new TimeSpan(0, 0, 0);
            var ot = dateTime.Date.Add(ts);
            DateTime older;
            while (true)
            {
                older = ot;
                ot = ot.AddMinutes(tt.Duration);

                if (!(dateTime > ot && dateTime < ot.AddMinutes(tc.SerializedFullDuration)) && (dateTime > older && dateTime < older.AddMinutes(tc.SerializedFullDuration)))
                {
                    return ot;
                }
                if (ot > dateTime.AddDays(1)) return dateTime;
            }
        }

        internal void RejectNewPlanning()
        {
            TreatmentsAppointmentsView.Where(i => i.Subject == Resources.New).ToList().ForEach(i => TreatmentsAppointmentsView.Remove(i));
        }

        internal void CommitNewPlanning()
        {
            var items = TreatmentsAppointmentsView.Where(i => i.Subject == Resources.New).ToList();
            if (items.Count == 0) return;

            var list = new List<PlanningElement>();
            items.ForEach(i =>
            {
                list.Add(new PlanningElement
                {
                    StartTime = i.Start,
                    EndTime = i.End,
                    Treatment = _tres.Single(k => k.Id == Guid.Parse(i.Resources.Single(j => j.ResourceType == "Treatment").ResourceName)),
                    Config = _tconfs.Single(k => k.Id == Guid.Parse(i.Resources.Single(j => j.ResourceType == "TreatmentConfig").ResourceName))
                });
            });

            ModuleViewBase.ProcessUserDialog<CommitPlanningWindow>(ApplicationDispatcher.UnityContainer,
                () => LoadTreatmentsAppointments(_treatmetnsVisibleRange), new ParameterOverride("treatments", list), new ParameterOverride("confs", _tconfs), new ParameterOverride("trms", _tres));
        }

        private DateSpan _treatmetnsVisibleRange;

        internal void CancelTreatment(Appointment appointment)
        {
            var id = SolariumGrid.LocateRes(appointment.Resources, "Id");
            if (id == null) return;
            ClientContext.CancelTreatmentEvents(new[] { Guid.Parse(id) }.ToList());
            LoadTreatmentsAppointments(_treatmetnsVisibleRange);
        }

        internal void SetTreatmentInctivesVisibility(bool visibility)
        {
            _treatmentInactivesVisibility = visibility;
        }

        internal void SetAppointmentColor(CustomAppointment app, object p)
        {
            int colorId = Int32.Parse(p.ToString());
            Guid id;
            if (Guid.TryParse(app.UniqueId, out id))
            {
                ClientContext.SetTreatmentEventColor(id, colorId);
                app.CustomColor = GetColorById(colorId);
            }
        }

        internal void ParallelSign(Appointment appointment)
        {
            var id = SolariumGrid.LocateRes(appointment.Resources, "Id");
            if (id == null) return;
            try
            {
                NavigationManager.MakeParallelSigningRequest(Guid.Parse(id), () =>
                {
                    try
                    {
                        LoadTreatmentsAppointments(_treatmetnsVisibleRange);
                    }
                    catch
                    {
                        ExtraWindow.Alert(Resources.Error,
                            Resources.ParallelImossible);
                    }
                });
            }
            catch
            {
            }
        }

        internal void EditComment(CustomAppointment app)
        {
            Guid eventId;
            if (Guid.TryParse(app.UniqueId, out eventId))
            {
                var item = ClientContext.GetTreatmentEventById(eventId);

                ExtraWindow.Prompt(Resources.Comment,
                     Resources.ProvideComment,
                    item.Comment ?? "",
                    ev =>
                    {
                        if (ev.DialogResult ?? false)
                        {
                            ClientContext.AddCommentToTreatmentEvent(item.Id, ev.TextResult.Trim());
                            LoadTreatmentsAppointments(null);
                        }
                    });
            }
        }

        private void ProcessSearch()
        {
            if (String.IsNullOrEmpty(SearchText))
            {
                foreach (var app in TreatmentsAppointmentsView)
                {
                    app.SearchHighlight = false;
                }
                return;
            }
            var value = SearchText.ToLower();
            foreach (var app in TreatmentsAppointmentsView)
            {
                app.SearchHighlight =
                    (app.Subject != null && app.Subject.ToLower().Contains(value)) ||
                    (app.CustomerName != null && app.CustomerName.ToLower().Contains(value));
            }
        }

        internal DataTable GetCurrentAsDataTable()
        {
            var dt = new DataTable();
            if (!ClientContext.CurrentDivision.OpenTime.HasValue) return dt;
            if (!ClientContext.CurrentDivision.CloseTime.HasValue) return dt;
            dt.Columns.Add("Время", typeof(string));
            var cols = new Dictionary<Guid, DataColumn>();
            var rows = new Dictionary<int, DataRow>();
            foreach (var tre in _tres)
            {
                var dn = tre.DisplayName;
                if (cols.Values.Any(i => i.ColumnName == tre.DisplayName))
                {
                    dn = dn + " " + tre.Order;
                }
                cols.Add(tre.Id, dt.Columns.Add(dn, typeof(string)));
            }
            var ot = new DateTime(1, 1, 1, ClientContext.CurrentDivision.OpenTime.Value.Hours, ClientContext.CurrentDivision.OpenTime.Value.Minutes, 0);
            var ct = new DateTime(1, 1, 1, ClientContext.CurrentDivision.CloseTime.Value.Hours, ClientContext.CurrentDivision.CloseTime.Value.Minutes, 0);
            var cur = ot;
            while (cur < ct)
            {
                rows.Add((int)cur.TimeOfDay.TotalMinutes, dt.Rows.Add(cur.ToString("H:mm")));
                cur = cur.AddMinutes(30);
            }

            foreach (var app in TreatmentsAppointmentsView)
            {
                var res = app.Resources.Cast<Resource>().FirstOrDefault(i => i.ResourceType == "Treatment");
                if (res != null)
                {
                    var col = cols[Guid.Parse(res.ResourceName)];
                    var rowId = ((int)Math.Floor(app.Start.TimeOfDay.TotalMinutes / 30)) * 30;
                    if (rows.ContainsKey(rowId))
                    {
                        if (rows[rowId][col] == null) rows[rowId][col] = "";
                        if (!String.Empty.Equals(rows[rowId][col]))
                        {
                            rows[rowId][col] = rows[rowId][col] + "\n";
                        }
                        rows[rowId][col] = rows[rowId][col] + app.Subject;
                    }
                }

            }
            return dt;
        }

        internal void UnmissTreatment(Appointment appointment)
        {
            var id = SolariumGrid.LocateRes(appointment.Resources, "Id");
            if (id == null) return;
            ClientContext.UnmissTreatment(new[] { Guid.Parse(id) }.ToList());
            LoadTreatmentsAppointments(_treatmetnsVisibleRange);
        }
    }

    public class PlanningElement
    {
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public Treatment Treatment { get; set; }

        public TreatmentConfig Config { get; set; }
    }
}
