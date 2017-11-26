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
using ExtraClub.ServiceModel;
using ExtraClub.ScheduleModule.ViewModels;
using ExtraClub.UIControls.Windows;
using System.ServiceModel;
using ExtraClub.UIControls;

namespace ExtraClub.ScheduleModule.Views.Windows
{
    public partial class ParallelSigningWindow
    {
        public TreatmentEvent BaseEvent { get; set; }
        public List<PlanningElement> AvailEvents { get; set; }

        public ParallelSigningWindow(ClientContext context, Guid eventId, List<ScheduleProposalElement> availEvents)
            :base(context)
        {
            InitializeComponent();
            BaseEvent = context.GetTreatmentEventById(eventId);
            AvailEvents = new List<PlanningElement>();
            var tcs = context.GetAllTreatmentConfigs().Where(i => i.IsActive);
            availEvents.ForEach(i =>
            {
                if (tcs.Any(j => j.Id == i.ConfigId))
                {
                    AvailEvents.Add(new PlanningElement
                    {
                        StartTime = i.StartTime,
                        EndTime = i.EndTime,
                        Treatment = new Treatment { Tag = i.Treatment.Tag, Id = i.Treatment.Id },
                        Config = tcs.Single(j => j.Id == i.ConfigId)
                    });
                }
            });
            DataContext = this;
            if (AvailEvents.Count == 1)
            {
                AdditionalSignList.SelectedItem = AvailEvents[0];
            }
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            if (AdditionalSignList.SelectedItem == null)
            {
                ExtraWindow.Alert(UIControls.Localization.Resources.Error, UIControls.Localization.Resources.AppendWarn);
                return;
            }
            var pe = AdditionalSignList.SelectedItem as PlanningElement;
            try
            {
                _context.PostParallelSigning(BaseEvent.Id, pe.Config.Id, pe.Treatment.Id, pe.StartTime);
            }
            catch (FaultException ex)
            {
                ExtraWindow.Alert(UIControls.Localization.Resources.Error, ex.Message);
                return;
            }
            catch
            {
                throw;
            }
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
