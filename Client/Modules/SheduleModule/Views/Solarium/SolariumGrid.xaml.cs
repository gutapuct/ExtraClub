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
using Telerik.Windows.Controls;
using TonusClub.ScheduleModule.ViewModels;
using Telerik.Windows.Controls.ScheduleView;
using Microsoft.Practices.Unity;
using TonusClub.Infrastructure.ParameterClasses;
using TonusClub.ScheduleModule.Views.Windows;
using TonusClub.UIControls.Windows;
using TonusClub.UIControls;
using TonusClub.Infrastructure;

namespace TonusClub.ScheduleModule.Views.Solarium
{
    /// <summary>
    /// Interaction logic for SolariumGrid.xaml
    /// </summary>
    public partial class SolariumGrid
    {
        private ScheduleLargeViewModel Model
        {
            get
            {
                return DataContext as ScheduleLargeViewModel;
            }
        }

        public SolariumGrid(ScheduleLargeViewModel model)
        {
            InitializeComponent();
            DataContext = model;
            LocalizationManager.Manager = new UIControls.Localization.CustomLocalizationManager();
            Model.UpdateFinished += Model_UpdateFinished;

        }

        void Model_UpdateFinished(object sender, EventArgs e)
        {
            if (Model != null && Model.Division != null)
            {
                if ((SolariumSchedule.ViewDefinitions[0] as DayViewDefinition).DayStartTime != (Model.Division.OpenTime ?? new TimeSpan(0, 0, 0)))
                {
                    (SolariumSchedule.ViewDefinitions[0] as DayViewDefinition).DayStartTime = Model.Division.OpenTime ?? new TimeSpan(0, 0, 0);
                    (SolariumSchedule.ViewDefinitions[1] as DayViewDefinition).DayStartTime = Model.Division.OpenTime ?? new TimeSpan(0, 0, 0);
                    (SolariumSchedule.ViewDefinitions[0] as DayViewDefinition).DayEndTime = Model.Division.CloseTime ?? new TimeSpan(0, 0, 0);
                    (SolariumSchedule.ViewDefinitions[1] as DayViewDefinition).DayEndTime = Model.Division.CloseTime ?? new TimeSpan(23, 59, 59);
                    (SolariumSchedule.ViewDefinitions[2] as WeekViewDefinition).DayStartTime = Model.Division.OpenTime ?? new TimeSpan(0, 0, 0);
                    (SolariumSchedule.ViewDefinitions[2] as WeekViewDefinition).DayEndTime = Model.Division.CloseTime ?? new TimeSpan(23, 59, 59);
                }
            }
        }

        public static string LocateRes(System.Collections.IList resources, string resType)
        {
            for (int i = 0; i < resources.Count; i++)
            {
                if (((Resource)resources[i]).ResourceType == resType)
                {
                    return ((Resource)resources[i]).ResourceName;
                }
            }
            return null;
        }

        private void SolariumSchedule_AppointmentEditing(object sender, AppointmentEditingEventArgs e)
        {
            if (!ClientContext.CheckPermission("EditSolatiumEvent")) return;
            var statRes = LocateRes(e.Appointment.Resources, "Status");
            if (statRes != "0")
            {
                e.Cancel = true;
                return;
            }

            //startTime = e.Appointment.Start;
            //endTime = e.Appointment.End;
            //resource = ((Resource)e.Appointment.Resources[0]).ResourceName;
        }

        private void SolariumSchedule_ShowDialog(object sender, ShowDialogEventArgs e)
        {
            e.Cancel = true;
        }

        private void SolariumSchedule_AppointmentEdited(object sender, AppointmentEditedEventArgs e)
        {
            if (!ClientContext.CheckPermission("EditSolatiumEvent")) return;
            var statRes = LocateRes(e.Appointment.Resources, "Status");
            if (statRes != "0")
            {
                return;
            }

            e.Appointment.Start = e.Appointment.Start.AddSeconds(-e.Appointment.Start.Second);
            e.Appointment.End = e.Appointment.End.AddSeconds(-e.Appointment.End.Second);

            ProcessUserDialog<SolEditMoveWindow>(w => ExecuteRefresh(), new ResolverOverride[] { new ParameterOverride("parameters", new SolMoveParams { VisitId = Guid.Parse(((Appointment)e.Appointment).UniqueId), NewStart = e.Appointment.Start, NewEnd = e.Appointment.End, NewSolarium = Guid.Parse(LocateRes(e.Appointment.Resources, "Solarium")), CustomerId = Guid.Parse(LocateRes(e.Appointment.Resources, "Customer")) }) });
        }

        private void CancelAppointment_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (!ClientContext.CheckPermission("CancelSolatiumEvent")) return;

            if (SolariumSchedule.SelectedAppointment != null)
            {
                var app = (Appointment)SolariumSchedule.SelectedAppointment;
                if (LocateRes(app.Resources, "Status") != "0") return;

                TonusWindow.Confirm(UIControls.Localization.Resources.CancelBooking,
                    UIControls.Localization.Resources.CancelBookingMsg,
                    e1 =>
                    {
                        if (e1.DialogResult ?? false)
                        {
                            ClientContext.CancelSolariumEvent(Guid.Parse(((Appointment)SolariumSchedule.SelectedAppointment).UniqueId), false);
                            ExecuteRefresh();
                        }
                    });
            }
        }

        private void StartAppointment_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (!ClientContext.CheckPermission("StartSolatiumEvent")) return;

            if (SolariumSchedule.SelectedAppointment != null)
            {
                var app = (Appointment)SolariumSchedule.SelectedAppointment;
                if (LocateRes(app.Resources, "Status") != "0") return;
                ProcessUserDialog<SolStartWindow>(() => ExecuteRefresh(), new ResolverOverride[] { new ParameterOverride("visitId", Guid.Parse(app.UniqueId)), new ParameterOverride("customerId", Guid.Parse(LocateRes(app.Resources, "Customer"))) });
            }
        }

        private void EditAppointment_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (!ClientContext.CheckPermission("EditSolatiumEvent")) return;

            if (SolariumSchedule.SelectedAppointment != null)
            {
                var app = (Appointment)SolariumSchedule.SelectedAppointment;
                if (LocateRes(app.Resources, "Status") != "0") return;
                ProcessUserDialog<SolEditMoveWindow>(() => ExecuteRefresh(), new ResolverOverride[] { new ParameterOverride("parameters", new SolMoveParams { VisitId = Guid.Parse(app.UniqueId), NewStart = app.Start, NewEnd = app.End, NewSolarium = Guid.Parse(LocateRes(app.Resources, "Solarium")), CustomerId = Guid.Parse(LocateRes(app.Resources, "Customer")) }) });
            }
        }

        private void NewAppointment_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (!ClientContext.CheckPermission("NewSolariumEvent")) return;

            var StartTime = DateTime.Now.AddMinutes(5);
            var SolId = Guid.Empty;
            if (SolariumSchedule.SelectedSlot != null)
            {
                StartTime = SolariumSchedule.SelectedSlot.Start;
                Guid.TryParse(LocateRes(SolariumSchedule.SelectedSlot.Resources, "Solarium"), out SolId);
            }
            ProcessUserDialog<NewBooking>(() => ExecuteRefresh(), new ResolverOverride[] {
                new ParameterOverride("parameters",
                    new NewSolariumVisitParams{ SolariumId= SolId, StartDate = StartTime}) });
        }

        private void SolariumSchedule_AppointmentSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            sm1.Visibility = System.Windows.Visibility.Collapsed;
            ss1.Visibility = System.Windows.Visibility.Collapsed;
            sm2.Visibility = System.Windows.Visibility.Collapsed;
            sm3.Visibility = System.Windows.Visibility.Collapsed;
            sm4.Visibility = System.Windows.Visibility.Collapsed;

            if (SolariumSchedule.SelectedAppointment == null)
            {
                AuthorizationManager.SetElementVisible(sm4);
                return;
            }
            else
            {
                var app = (Appointment)SolariumSchedule.SelectedAppointment;
                if (LocateRes(app.Resources, "Status") == "0")
                {
                    if (app.Start.Date == DateTime.Today)
                    {
                        AuthorizationManager.SetElementVisible(sm1);
                        AuthorizationManager.SetElementVisible(ss1);
                    }
                    AuthorizationManager.SetElementVisible(sm2);
                    AuthorizationManager.SetElementVisible(sm3);
                    return;
                }
            }
        }

        private void SolariumSchedule_AppointmentCreating(object sender, AppointmentCreatingEventArgs e)
        {
            e.Cancel = true;
        }

        private void solMenu_Opened(object sender, RoutedEventArgs e)
        {
            //solMenu.IsOpen = CanSolMenu;
        }


        internal void UnWireBeforeRefresh()
        {
            SolariumSchedule.AppointmentEdited -= SolariumSchedule_AppointmentEdited;
            SolariumSchedule.AppointmentEdited -= SolariumSchedule_AppointmentEdited;
        }

        internal void WireAfterRefresh()
        {

            SolariumSchedule.AppointmentEdited += SolariumSchedule_AppointmentEdited;
            if (Model.ChangeSolDefinitionFlag)
            {
                var tmp = SolariumSchedule.ActiveViewDefinition;
                SolariumSchedule.ActiveViewDefinition = null;
                SolariumSchedule.ActiveViewDefinition = tmp;
            }
        }


        private void ExecuteRefresh()
        {
            UnWireBeforeRefresh();
            Model.RefreshDataSync();
            WireAfterRefresh();
        }

        private void CreateUndockedWindow(object sender, RoutedEventArgs e)
        {
            var window = new Window();
            window.Title = String.Format(UIControls.Localization.Resources.SolGridTitle, ClientContext.CurrentDivision.Name);
            var dc = ApplicationDispatcher.UnityContainer.Resolve<ScheduleLargeViewModel>();
            dc.RefreshDataAsync();
            window.DataContext = dc;
            var sg = new SolariumGrid(Model);
            sg.ParamsGroup.Visibility = System.Windows.Visibility.Collapsed;
            sg.ParametersItem.IsChecked = false;
            dc.AutorefreshSolarium = true;
            AuthorizationManager.SetElementVisible(sg.AutorefreshItem);
            window.Content = sg;
            window.Show();
        }

        private void ShowParametersClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            ParamsGroup.Visibility = ParametersItem.IsChecked ? Visibility.Visible : System.Windows.Visibility.Collapsed;
        }
    }
}
