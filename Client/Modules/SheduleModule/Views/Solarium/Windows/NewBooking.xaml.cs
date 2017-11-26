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
using ExtraClub.Infrastructure.Interfaces;
using ExtraClub.ServiceModel;
using System.ComponentModel;
using Telerik.Windows.Controls;
using ExtraClub.Infrastructure.ParameterClasses;
using ExtraClub.UIControls.Windows;
using System.ServiceModel;
using ExtraClub.ScheduleModule.Views.Windows;
using Microsoft.Practices.Unity;
using ExtraClub.UIControls;

namespace ExtraClub.ScheduleModule.Views.Solarium
{
    /// <summary>
    /// Interaction logic for NewBooking.xaml
    /// </summary>
    public partial class NewBooking
    {

        private Customer _Customer;
        public Customer Customer
        {
            get
            {
                return _Customer;
            }
            set
            {
                _Customer = value;
                OnPropertyChanged("Customer");
                OnPropertyChanged("IsSearchAllowed");
            }
        }

        private List<Ticket> _tickets;
        public ICollectionView TicketsView { get; set; }

        public string Comments { get; set; }

        public bool IsSearchAllowed
        {
            get
            {
                return Customer != null
                    && VisitDate.Add(VisitTime) > DateTime.Now
                    && VisitDate.Add(VisitTime) >= VisitDate.Date.Add(_context.CurrentDivision.OpenTime ?? new TimeSpan(0))
                    && VisitDate.Add(VisitTime).AddMinutes(MinutesAmount) <= VisitDate.Date.Add(_context.CurrentDivision.CloseTime ?? new TimeSpan(24, 0, 0));
            }
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
                OnPropertyChanged("IsSearchAllowed");
            }
        }
        public string TextActions { get; private set; }


        private TimeSpan _VisitTime;
        public TimeSpan VisitTime
        {
            get
            {
                return _VisitTime;
            }
            set
            {
                _VisitTime = value;
                OnPropertyChanged("VisitTime");
                OnPropertyChanged("IsSearchAllowed");
            }
        }

        public Dictionary<Guid, string> AvailSolariums { get; set; }

        private Guid _SelectedSolariumId;
        public Guid SelectedSolariumId
        {
            get
            {
                return _SelectedSolariumId;
            }
            set
            {
                _SelectedSolariumId = value;
                OnPropertyChanged("SelectedSolariumId");
            }
        }

        private int _MinutesAmount;
        public int MinutesAmount
        {
            get
            {
                return _MinutesAmount;
            }
            set
            {
                _MinutesAmount = value;
                WarningBlock.Visibility = System.Windows.Visibility.Hidden;
                WarningBlock.Text = "";

                foreach (var i in warns)
                {
                    if (i.Key <= value)
                    {
                        WarningBlock.Text = i.Value;
                    }
                }

                if (!String.IsNullOrWhiteSpace(WarningBlock.Text)) WarningBlock.Visibility = System.Windows.Visibility.Visible;
                OnPropertyChanged("MinutesAmount");
                OnPropertyChanged("IsSearchAllowed");
            }
        }
        public int MinMinutes { get; set; }
        public int MaxMinutes { get; set; }

        private Dictionary<int, string> warns;

        public Division Division { get; set; }

        IUnityContainer _container;

        public NewBooking(NewSolariumVisitParams parameters)
        {
            InitializeComponent();
            Division = _context.CurrentDivision;
            this.DataContext = this;
            warns = _context.GetSolariumWarnings();
            MinutesAmount = MinMinutes = _context.CurrentDivision.MinSolarium;
            MaxMinutes = _context.CurrentDivision.MaxSolarium;
            if (parameters.CustomerId != Guid.Empty)
            {
                CustomerSearch.SelectById(parameters.CustomerId);
                CustomerSearch.IsEnabled = false;
            }
            if (parameters.StartDate >= DateTime.Now)
            {
                VisitDate = parameters.StartDate.Date;
                VisitTime = parameters.StartDate - parameters.StartDate.Date;
            }
            else
            {
                VisitDate = DateTime.Today;
                VisitTime = new TimeSpan(DateTime.Now.Hour + 1, 0, 0);
                if (VisitTime.Hours == 0) VisitDate = VisitDate.AddDays(1);
            }

            var tmp = _context.GetDivisionSolariums(true).OrderBy(i => i.Name);
            AvailSolariums = tmp.ToDictionary(i => i.Id, i => i.Name);
            AvailSolariums.Add(Guid.Empty, UIControls.Localization.Resources.Any);
            SelectedSolariumId = parameters.SolariumId;

            if (AvailSolariums.Count <= 2)
            {
                SolGroup.Visibility = System.Windows.Visibility.Collapsed;
            }

            _tickets = new List<Ticket>();
            TicketsView = CollectionViewSource.GetDefaultView(_tickets);
            var ta = _context.GetCurrentActions();
            if (ta.Count > 0)
            {
                TextActions = "";
                foreach (var a in ta)
                {
                    if (!String.IsNullOrEmpty(TextActions)) TextActions += "\n";
                    TextActions += String.Format("{3} {0:d} {4} {1:d}: {2}", a.StartDate, a.FinishDate, a.ActionText, UIControls.Localization.Resources.From, UIControls.Localization.Resources.To);
                }
            }
            else
            {
                ActionsGroup.Visibility = System.Windows.Visibility.Collapsed;
            }

        }

        private void CustomerSearch_SelectedClientChanged(object sender, GuidEventArgs e)
        {
            Customer = _context.GetCustomer(e.Guid, true);
            _tickets.Clear();
            _tickets.AddRange(_context.GetTicketsForPlanning(e.Guid));
            if (_tickets.Count == 0)
            {
                _tickets.Add(new Ticket { Id = Guid.Empty, Number = UIControls.Localization.Resources.WithoutTicket, SerializedTicketType = new TicketType { Name = UIControls.Localization.Resources.Cash } });
            }
            TicketsView.Refresh();
            OnPropertyChanged("IsSearchAllowed");
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((VisitDate.Date - DateTime.Today).TotalDays > 30)
                {
                    ExtraWindow.Alert(UIControls.Localization.Resources.Error,
                        UIControls.Localization.Resources.Solarium30DaysWarn);
                    return;
                }
                if (VisitDate.Date.Add(VisitTime) < DateTime.Now)
                {
                    ExtraWindow.Alert(UIControls.Localization.Resources.Error,
                        UIControls.Localization.Resources.SolariumBackDateWarn);
                    return;
                }

                var res = _context.GetSolariumProposal(Customer.Id, VisitDate.Add(VisitTime), MinutesAmount, SelectedSolariumId, Guid.Empty);
                ExtraWindow.Confirm(UIControls.Localization.Resources.SolariumBook,
                    String.Format(UIControls.Localization.Resources.SolBookMessage, AvailSolariums[res.Key], res.Value),
                    e1 =>
                    {
                        if (e1.DialogResult ?? false)
                        {
                            Guid? t = null;
                            var ct = TicketsView.CurrentItem as Ticket;
                            if (ct != null)
                            {
                                t = ct.Id;
                            }


                            var bId = _context.PostSolariumBookingEx(Customer.Id, res.Key, res.Value, MinutesAmount, Comments, t);

                            DialogResult = true;
                            this.Close();

                            if ((res.Value - DateTime.Now).TotalMinutes < 10)
                            {
                                ExtraWindow.Confirm(UIControls.Localization.Resources.Process,
                                    UIControls.Localization.Resources.PaySolMessage,
                                    w1 =>
                                    {
                                        if (w1.DialogResult ?? false)
                                        {
                                            ModuleViewBase.ProcessUserDialog<SolStartWindow>(_container, () => { }, new ResolverOverride[] { 
                                            new ParameterOverride("visitId", bId),
                                            new ParameterOverride("customerId", Customer.Id) });
                                        }
                                    });
                            }

                        }
                    });

            }
            catch (FaultException ex)
            {
                ExtraWindow.Alert(new DialogParameters { Header = UIControls.Localization.Resources.Error, Content = ex.Message });
            }
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
