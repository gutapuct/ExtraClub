using System;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Practices.Unity;
using Telerik.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.ScheduleView;
using ExtraClub.Infrastructure;
using ExtraClub.ScheduleModule.Controls;
using ExtraClub.ScheduleModule.ViewModels;
using ExtraClub.ScheduleModule.Views.Solarium;
using ExtraClub.ScheduleModule.Views.Treatments.Windows;
using ExtraClub.ScheduleModule.Views.Windows;
using ExtraClub.UIControls;
using ExtraClub.UIControls.Localization;
using ExtraClub.UIControls.Windows;

namespace ExtraClub.ScheduleModule.Views.Treatments
{
    public partial class TreatmentsGrid
    {
        private ScheduleLargeViewModel Model
        {
            get
            {
                return DataContext as ScheduleLargeViewModel;
            }
        }

        public TreatmentsGrid(ScheduleLargeViewModel model)
        {
            InitializeComponent();
            LocalizationManager.Manager = new CustomLocalizationManager();
            DataContext = model;
            NavigationManager.ParallelSigningRequest += NavigationManager_ParallelSigningRequest;
            Model.UpdateFinished += Model_UpdateFinished;
        }

        void Model_UpdateFinished(object sender, EventArgs e)
        {
            if (Model != null && Model.Division != null)
            {
                var dvd1 = (TreatmentsSchedule.ViewDefinitions[0] as DayViewDefinition);
                var dvd2 = (TreatmentsSchedule.ViewDefinitions[1] as DayViewDefinition);
                if (dvd1.DayStartTime != (Model.Division.OpenTime ?? new TimeSpan(0, 0, 0)))
                {
                    dvd1.DayStartTime = Model.Division.OpenTime ?? new TimeSpan(0, 0, 0);
                    dvd2.DayStartTime = Model.Division.OpenTime ?? new TimeSpan(0, 0, 0);
                }
                if (dvd1.DayEndTime != (Model.Division.CloseTime ?? new TimeSpan(0, 0, 0)))
                {
                    dvd1.DayEndTime = Model.Division.CloseTime ?? new TimeSpan(0, 0, 0);
                    dvd2.DayEndTime = Model.Division.CloseTime ?? new TimeSpan(23, 59, 59);
                }
            }
        }

        void NavigationManager_ParallelSigningRequest(object sender, GuidEventArgs e)
        {
            try
            {
                var availEvents = ClientContext.GetAvailableParallels(e.Guid);
                if (availEvents.Count == 0)
                {
                    ExtraWindow.Alert("Ошибка", "Дозапись к данной услуге невозможна!");
                }
                else
                {
                    ProcessUserDialog<ParallelSigningWindow>(e.OnClose, new ParameterOverride("eventId", e.Guid), new ParameterOverride("availEvents", availEvents));
                }
            }
            catch (FaultException ex)
            {
                ExtraWindow.Alert("Ошибка", ex.Message);
            }
        }

        AppointmentInfo editing;

        private void TreatmentsSchedule_AppointmentEditing(object sender, AppointmentEditingEventArgs e)
        {
            if (!ClientContext.CheckPermission("EditTreatmentEvent")) return;
            var statRes = SolariumGrid.LocateRes(e.Appointment.Resources, "Status");
            if (statRes != "0")
            {
                e.Cancel = true;
                return;
            }
            if (e.Appointment.Start < DateTime.Now.AddHours(ClientContext.CurrentCompany.MaxCancellationPeriod) && e.Appointment.Subject != "Новое")
            {
                e.Cancel = true;
                return;
            }
            editing = new AppointmentInfo
            {
                Start = e.Appointment.Start,
                Finish = e.Appointment.End,
                TreatmentId = SolariumGrid.LocateRes(e.Appointment.Resources, "Treatment")
            };
        }

        private void TreatmentsSchedule_AppointmentEdited(object sender, AppointmentEditedEventArgs e)
        {
            if (!ClientContext.CheckPermission("EditTreatmentEvent")) return;
            var statRes = SolariumGrid.LocateRes(e.Appointment.Resources, "Status");
            if (statRes == "0" && e.Appointment.Subject != UIControls.Localization.Resources.New)
            {
                var id = SolariumGrid.LocateRes(e.Appointment.Resources, "Id");
                ProcessUserDialog<TreatmentEventEditWindow>(w => Model.LoadTreatmentsAppointments(null), new ParameterOverride("appointment", e.Appointment), new ParameterOverride("original", editing));
            }
        }

        private void TreatmentsSchedule_AppointmentSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cancelAppointmentItem != null)
            {
                cancelAppointmentItem.Visibility = Visibility.Collapsed;
                parallelSignAppointmentItem.Visibility = Visibility.Collapsed;
                saveAppointmentItem.Visibility = Visibility.Collapsed;
                rejectAppointmentItem.Visibility = Visibility.Collapsed;
                navigateToCustomerAppointmentItem.Visibility = Visibility.Collapsed;
                ColorsMenu.Visibility = Visibility.Collapsed;
            }
            if (TreatmentsSchedule.SelectedAppointment == null)
            {
                //sm4.Visibility = System.Windows.Visibility.Visible;
                //CanTreMenu = true;
            }
            else
            {
                var app = (Appointment)TreatmentsSchedule.SelectedAppointment;
                AuthorizationManager.SetElementVisible(ColorsMenu);
                if (SolariumGrid.LocateRes(app.Resources, "Customer") != Guid.Empty.ToString())
                {
                    AuthorizationManager.SetElementVisible(navigateToCustomerAppointmentItem);
                }

                if (SolariumGrid.LocateRes(app.Resources, "Status") == "0" && app.Subject != UIControls.Localization.Resources.New)
                {
                    AuthorizationManager.SetElementVisible(parallelSignAppointmentItem);
                    if (ClientContext != null && app.Start >= DateTime.Now.AddHours(ClientContext.CurrentCompany.MaxCancellationPeriod))
                    {
                        AuthorizationManager.SetElementVisible(cancelAppointmentItem);
                    }
                }
                if (app.Subject == UIControls.Localization.Resources.New)
                {
                    AuthorizationManager.SetElementVisible(saveAppointmentItem);
                    AuthorizationManager.SetElementVisible(rejectAppointmentItem);
                }
            }
        }

        private void TreatmentsSchedule_AppointmentCreating(object sender, AppointmentCreatingEventArgs e)
        {
            if (!ClientContext.CheckPermission("CreateNewTreatmentScheduleAlter")) return;
            Model.AddNewAppointment(e.Appointment);
        }

        private void TreatmentsSchedule_ShowDialog(object sender, ShowDialogEventArgs e)
        {
            e.Cancel = true;
        }

        private void TreatmentsSchedule_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement originalSender = e.OriginalSource as FrameworkElement;
            if (originalSender != null && originalSender.DataContext is AppointmentItemProxy)
            {
                var app = (CustomAppointment)((AppointmentItemProxy)originalSender.DataContext).Appointment;
                if (app.Subject != UIControls.Localization.Resources.New)
                {
                    Model.EditComment(app);
                }
                else
                {
                    if (!ClientContext.CheckPermission("CreateNewTreatmentScheduleAlter")) return;
                    Model.TreatmentsAppointmentsView.Remove(app);
                }
            }
        }

        private void RejectClick(object sender, RoutedEventArgs e)
        {
            Model.RejectNewPlanning();
        }

        private void CommitClick(object sender, RoutedEventArgs e)
        {
            Model.CommitNewPlanning();
        }

        public class AppointmentInfo
        {
            public DateTime Start { get; set; }
            public DateTime Finish { get; set; }
            public string TreatmentId { get; set; }
        }

        private void treMenu_Opened(object sender, RoutedEventArgs e)
        {
            //treMenu.IsOpen = CanTreMenu;
        }

        private void CancelAppointment_Click(object sender, RadRoutedEventArgs e)
        {
            if (!ClientContext.CheckPermission("CancelTreatmentEvent")) return;

            ExtraWindow.Confirm(UIControls.Localization.Resources.CancelBooking, UIControls.Localization.Resources.CancelBookingMsg,
                w =>
                {
                    if (w.DialogResult ?? false)
                    {
                        Model.CancelTreatment((Appointment)TreatmentsSchedule.SelectedAppointment);
                    }
                });
        }

        private void ChangeToVisited_Click(object sender, RadRoutedEventArgs e)
        {
            if (!ClientContext.CheckPermission("CancelTreatmentEvent")) return;

            ExtraWindow.Confirm("Отметка о посещении", "Пометить процедуру посещенной?",
                w =>
                {
                    if (w.DialogResult ?? false)
                    {
                        Model.UnmissTreatment((Appointment)TreatmentsSchedule.SelectedAppointment);
                    }
                });
        }

        private void SaveAppointment_Click(object sender, RadRoutedEventArgs e)
        {
            if (!ClientContext.CheckPermission("CreateNewTreatmentScheduleAlter")) return;
            Model.CommitNewPlanning();
        }

        private void RejectAppointment_Click(object sender, RadRoutedEventArgs e)
        {
            Model.RejectNewPlanning();
        }

        private void NavigateToCustomer_Click(object sender, RadRoutedEventArgs e)
        {
            NavigationManager.MakeClientRequest(Guid.Parse(SolariumGrid.LocateRes(((Appointment)TreatmentsSchedule.SelectedAppointment).Resources, "Customer")));
        }

        private void CreateUndockedWindow(object sender, RoutedEventArgs e)
        {
            var window = new Window
            {
                Title = String.Format(UIControls.Localization.Resources.GridTitle, ClientContext.CurrentDivision.Name)
            };
            var dc = ApplicationDispatcher.UnityContainer.Resolve<ScheduleLargeViewModel>();
            var tg = new TreatmentsGrid(Model)
            {
                DataContext = dc,
                ParamsGroup = {Visibility = Visibility.Collapsed}
            };
            dc.AutorefreshTreatments = true;
            tg.ParametersItem.IsChecked = false;
            AuthorizationManager.SetElementVisible(tg.AutorefreshItem);
            window.Content = tg;
            window.Show();
            dc.RefreshDataAsync();
            tg.Model_UpdateFinished(null, null);
        }

        private void ShowParametersClick(object sender, RadRoutedEventArgs e)
        {
            ParamsGroup.Visibility = ParametersItem.IsChecked ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ShowActivesClick(object sender, RadRoutedEventArgs e)
        {
            Model.SetTreatmentInctivesVisibility(ActivesItem.IsChecked);
            RefreshView();
        }

        private void ColorMenuItem_Click(object sender, RadRoutedEventArgs e)
        {
            Model.SetAppointmentColor((CustomAppointment)TreatmentsSchedule.SelectedAppointment, ((RadMenuItem)sender).Tag);
        }

        private void ParallelSign_Click(object sender, RadRoutedEventArgs e)
        {
            if (!ClientContext.CheckPermission("CreateNewTreatmentScheduleAlter")) return;

            Model.ParallelSign((Appointment)TreatmentsSchedule.SelectedAppointment);
        }

        private void MoveLeft(object sender, RadRoutedEventArgs e)
        {
            if (TreatmentsSchedule.SelectedSlot != null)
            {
                var res = TreatmentsSchedule.SelectedSlot.Resources.Cast<Resource>().FirstOrDefault(i => i.ResourceType == "Treatment");
                if (res != null)
                {
                    Model.ClientContext.MoveTreatment(Guid.Parse(res.ResourceName), true);
                    RefreshView();
                }
            }
        }

        private void MoveRigth(object sender, RadRoutedEventArgs e)
        {
            if (TreatmentsSchedule.SelectedSlot != null)
            {
                var res = TreatmentsSchedule.SelectedSlot.Resources.Cast<Resource>().FirstOrDefault(i => i.ResourceType == "Treatment");
                if (res != null)
                {
                    Model.ClientContext.MoveTreatment(Guid.Parse(res.ResourceName), false);
                    RefreshView();
                }
            }
        }

        private void RefreshView()
        {
            Model.RefreshTreGrid();
            var tmp = TreatmentsSchedule.ActiveViewDefinition;
            TreatmentsSchedule.ActiveViewDefinition = null;
            TreatmentsSchedule.ActiveViewDefinition = tmp;
        }

        private void ExportExcel(object sender, RoutedEventArgs e)
        {
            ReportManager.ExportDataTableToExcel(String.Format("Часовая сетка по клубу {0} за {1:d.MM.yyyy}", Model.ClientContext.CurrentDivision.Name, Model.TreLastSpan.Start), Model.GetCurrentAsDataTable());
        }

    }
}
