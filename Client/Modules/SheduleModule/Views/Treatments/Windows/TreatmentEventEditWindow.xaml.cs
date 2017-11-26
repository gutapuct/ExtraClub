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
using Telerik.Windows.Controls.ScheduleView;
using ExtraClub.ServiceModel;
using ExtraClub.ScheduleModule.Views.Solarium;
using ExtraClub.UIControls.Windows;
using ExtraClub.UIControls;

namespace ExtraClub.ScheduleModule.Views.Treatments.Windows
{
    /// <summary>
    /// Interaction logic for TreatmentEventEditWindow.xaml
    /// </summary>
    public partial class TreatmentEventEditWindow
    {
        public Customer Customer { get; set; }
        public TreatmentEvent TreatmentEvent { get; set; }
        public DateTime OldVisitDate { get; set; }

        public TreatmentEventEditWindow(ClientContext context, Appointment appointment, TreatmentsGrid.AppointmentInfo original)
            : base(context)
        {
            InitializeComponent();
            Customer = context.GetCustomer(Guid.Parse(SolariumGrid.LocateRes(appointment.Resources, "Customer")));
            TreatmentEvent = context.GetTreatmentEventById(Guid.Parse(SolariumGrid.LocateRes(appointment.Resources, "Id")));
            TreatmentEvent.VisitDate = appointment.Start;
            OldVisitDate = original.Start;
            DataContext = this;
            if (!TreatmentEvent.TicketId.HasValue)
            {
                ExtraWindow.Alert("Ошибка", "Редактирование записи без абонемента невозможно!");
            }
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            _context.PostTreatmentEventChange(TreatmentEvent.Id, TreatmentEvent.VisitDate);
            DialogResult = true;
            Close();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (!TreatmentEvent.TicketId.HasValue)
            {
                ExtraWindow.Alert("Ошибка", "Невозможно провести подобное перемещение!");
                return;
            }
            var sp = _context.FixSchedule(Customer.Id,
                TreatmentEvent.TicketId.Value,
                new List<ScheduleProposalElement> {
                    new ScheduleProposalElement{
                        ConfigId = TreatmentEvent.TreatmentConfigId, 
                        StartTime = TreatmentEvent.VisitDate,
                        EndTime = TreatmentEvent.VisitDate.AddMinutes( TreatmentEvent.SerializedDuration), 
                        Treatment = new TreatmentProposal
                        {
                            ConfigId = TreatmentEvent.TreatmentConfigId,
                            Id = TreatmentEvent.TreatmentId
                        }
                    }
                });
            if (sp.Count == 1)
            {
                TreatmentEvent.VisitDate = sp.First().StartTime;
                TestButton.Visibility = System.Windows.Visibility.Collapsed;
                AuthorizationManager.SetElementVisible(CommitButton);
                newDateText.Foreground = Brushes.Red;
            }
            else
            {
                ExtraWindow.Alert("Ошибка", "Невозможно провести подобное перемещение!");
            }

        }
    }
}
