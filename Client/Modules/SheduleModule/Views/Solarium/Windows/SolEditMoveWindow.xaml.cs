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
using System.Windows.Shapes;
using TonusClub.ServiceModel;
using TonusClub.Infrastructure.Interfaces;
using Telerik.Windows.Controls;
using TonusClub.UIControls;
using TonusClub.UIControls.Windows;

namespace TonusClub.ScheduleModule.Views.Windows
{
    /// <summary>
    /// Interaction logic for SolEditMoveWindow.xaml
    /// </summary>
    public partial class SolEditMoveWindow
    {
        public List<ServiceModel.Solarium> Solariums { get; set; }

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
                OnPropertyChanged("IsPostEnabled");
            }
        }

        public bool IsPostEnabled
        {
            get
            {
                return Customer != null &&
                    VisitDate > DateTime.Now
                    && VisitDate >= VisitDate.Date.Add(_context.CurrentDivision.OpenTime ?? new TimeSpan(0))
                    && VisitDate.AddMinutes(Length) <= VisitDate.Date.Add(_context.CurrentDivision.CloseTime ?? new TimeSpan(24, 0, 0));
            }
        }

        private Guid _SolariumId;
        public Guid SolariumId
        {
            get
            {
                return _SolariumId;
            }
            set
            {
                _SolariumId = value;
                OnPropertyChanged("SolariumId");
            }
        }


        private SolariumVisit _SolariumVisit;
        public SolariumVisit SolariumVisit
        {
            get
            {
                return _SolariumVisit;
            }
            set
            {
                _SolariumVisit = value;
                OnPropertyChanged("SolariumVisit");
            }
        }

        private DateTime _VisitDate;
        public DateTime VisitDate
        {
            get
            {
                return _VisitDate;
            }
            set
            {
                _VisitDate = value;
                OnPropertyChanged("VisitDate");
                OnPropertyChanged("IsPostEnabled");
            }
        }


        private int _Length;
        public int Length
        {
            get
            {
                return _Length;
            }
            set
            {
                _Length = value;
                OnPropertyChanged("Length");
                OnPropertyChanged("IsPostEnabled");
            }
        }

        public int MinMinutes { get; set; }
        public int MaxMinutes { get; set; }

        public SolEditMoveWindow(ClientContext context, SolMoveParams parameters)
            : base(context)
        {
            this.DataContext = this;

            MinMinutes = _context.CurrentDivision.MinSolarium;
            MaxMinutes = _context.CurrentDivision.MaxSolarium;

            Customer = _context.GetCustomer(parameters.CustomerId);
            Solariums = _context.GetDivisionSolariums(true);
            try
            {
                SolariumVisit = _context.GetSolariumVisitById(parameters.VisitId);
            }
            catch { }
            SolariumId = parameters.NewSolarium;

            VisitDate = parameters.NewStart;
            Length = (int)(parameters.NewEnd - parameters.NewStart).TotalMinutes;

            Length = Math.Max(_context.CurrentDivision.MinSolarium, Length);
            Length = Math.Min(_context.CurrentDivision.MaxSolarium, Length);

            Mouse.OverrideCursor = null;
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (SolariumVisit == null)
            {
                Close();
                return;
            }
            var res = _context.GetSolariumProposal(Customer.Id, VisitDate, Length, SolariumId, SolariumVisit.Id);
            if (res.Value != VisitDate)
            {
                TonusWindow.Confirm(UIControls.Localization.Resources.SolariumBook,
                     String.Format(UIControls.Localization.Resources.SolBookMessage2, res.Value),
                    e1 =>
                    {
                        if (e1.DialogResult ?? false)
                        {
                            if (_context.CancelSolariumEvent(SolariumVisit.Id, true))
                            {
                                _context.PostSolariumBooking(Customer.Id, res.Key, res.Value, Length, SolariumVisit.Comment);
                            }

                            DialogResult = true;
                            this.Close();
                        }
                    });
            }
            if (_context.CancelSolariumEvent(SolariumVisit.Id, true))
            {
                _context.PostSolariumBooking(Customer.Id, res.Key, res.Value, Length, SolariumVisit.Comment);
            }

            DialogResult = true;
            this.Close();
        }
    }

    public class SolMoveParams
    {
        public Guid CustomerId { get; set; }
        public Guid VisitId { get; set; }
        public DateTime NewStart { get; set; }
        public DateTime NewEnd { get; set; }
        public Guid NewSolarium { get; set; }
    }
}
