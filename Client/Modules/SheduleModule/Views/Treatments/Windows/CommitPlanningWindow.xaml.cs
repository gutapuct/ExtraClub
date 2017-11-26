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
using Microsoft.Practices.Unity;
using ExtraClub.Infrastructure.Interfaces;
using ExtraClub.ServiceModel;
using System.ComponentModel;
using Telerik.Windows.Controls.ScheduleView;
using ExtraClub.ScheduleModule.ViewModels;
using ExtraClub.UIControls.Windows;
using System.ServiceModel;
using ExtraClub.UIControls;

namespace ExtraClub.ScheduleModule.Views.Treatments.Windows
{
    public partial class CommitPlanningWindow
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

        private List<PlanningElement> _treatments;
        public ICollectionView Treatments { get; set; }


        private Dictionary<Guid, TreatmentConfig> _confs;
        private Dictionary<Guid, Treatment> _trms;

        public CommitPlanningWindow(ClientContext context, List<PlanningElement> treatments, List<TreatmentConfig> confs, List<Treatment> trms)
            : base(context)
        {
            InitializeComponent();
            _tickets = new List<Ticket>();
            _confs = confs.ToDictionary(i => i.Id, i => i);
            _trms = trms.ToDictionary(i => i.Id, i => i);
            TicketsView = CollectionViewSource.GetDefaultView(_tickets);
            _treatments = treatments.OrderBy(i => i.StartTime).ToList();
            Treatments = CollectionViewSource.GetDefaultView(_treatments);
            this.DataContext = this;
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (Customer == null)
            {
                ExtraWindow.Alert(UIControls.Localization.Resources.Error, UIControls.Localization.Resources.ProvideCustomer);
                return;
            }
            if (TicketsView.CurrentItem == null)
            {
                ExtraWindow.Alert(UIControls.Localization.Resources.Error, UIControls.Localization.Resources.ProvideTicket);
                return;
            }
            try
            {
                var res = _context.FixSchedule(Customer.Id, ((Ticket)TicketsView.CurrentItem).Id, GetScheduleProposalElements());
                SetScheduleProposalElements(res);
                if (res.Any(i => i.MovedByRules))
                {
                    ExtraWindow.Alert("Корректировка","Некоторые процедуры были передвинуты из-за правил планирования!");
                }
                TestButton.Visibility = System.Windows.Visibility.Collapsed;
                AuthorizationManager.SetElementVisible(CommitButton);
            }
            catch (FaultException ex)
            {
                ExtraWindow.Alert(UIControls.Localization.Resources.Error, ex.Message);
            }
        }

        private void SetScheduleProposalElements(List<ScheduleProposalElement> res)
        {
            _treatments.Clear();
            res.ForEach(i =>
            {
                _treatments.Add(new PlanningElement
                {
                    Config = _confs[i.ConfigId],
                    StartTime = i.StartTime,
                    EndTime = i.EndTime,
                    Treatment = _trms[i.Treatment.Id]
                });
            });
            Treatments.Refresh();
        }

        private List<ScheduleProposalElement> GetScheduleProposalElements()
        {
            var res = new List<ScheduleProposalElement>();
            foreach (var i in _treatments)
            {
                res.Add(new ScheduleProposalElement
                {
                    StartTime = i.StartTime,
                    EndTime = i.EndTime,
                    ConfigId = i.Config.Id,
                    Treatment = new TreatmentProposal
                    {
                        ConfigId = i.Config.Id,
                        Id = i.Treatment.Id
                    }
                });
            }
            return res;
        }

        private void CustomerSearch_SelectedClientChanged(object sender, GuidEventArgs e)
        {
            Customer = _context.GetCustomer(e.Guid);
            _tickets.Clear();
            _tickets.AddRange(_context.GetTicketsForPlanning(Customer.Id));
            if (_tickets.Count == 0)
            {
                _tickets.Add(new Ticket { Id = Guid.Empty, Number = UIControls.Localization.Resources.WithoutTicket});
            }
            TicketsView.Refresh();
            TicketsView.MoveCurrentToFirst();
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _context.PostScheduleProposal(Customer.Id, ((Ticket)TicketsView.CurrentItem).Id, new ScheduleProposal { List = GetScheduleProposalElements(), ProgramId = Guid.Empty });
            }
            catch (FaultException ex)
            {
                ExtraWindow.Alert(UIControls.Localization.Resources.Error, ex.Message);
                return;
            }
            DialogResult = true;
            Close();
        }
    }
}
