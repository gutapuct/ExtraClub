
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using FlagmaxControls;
using Microsoft.Practices.Unity;
using System.Windows.Input;
using System.Windows.Threading;
using ExtraClub.Clients.Views.Windows;
using ExtraClub.ScheduleModule.ViewModels;
using ExtraClub.ScheduleModule.Views.Treatments;
using ExtraClub.ScheduleModule.Views.Solarium;
using ExtraClub.ScheduleModule.Views;
using ExtraClub.TreatmentsModule.ViewModels;
using ExtraClub.TreatmentsModule.Views;
using ExtraClub.TreatmentsModule.Views.Windows;
using ExtraClub.Clients.ViewModels;
using ExtraClub.Clients.Views;
using ExtraClub.Clients.Views.ContainedControls;
using ExtraClub.OrganizerModule.ViewModels;
using ExtraClub.OrganizerModule.Views;
using ExtraClub.OrganizerModule.Views.Ankets;
using ExtraClub.OrganizerModule.Views.Calls;
using ExtraClub.OrganizerModule.Views.Claims;
using ExtraClub.OrganizerModule.Views.Files;
using ExtraClub.OrganizerModule.Business;
using ExtraClub.TurnoverModule.Views.ContainedControls;
using ExtraClub.TurnoverModule.ViewModels;
using ExtraClub.TurnoverModule.Views;
using ExtraClub.TurnoverModule.Views.Windows;
using ExtraClub.EmployeesModule.ViewModels;
using ExtraClub.EmployeesModule.Views;
using ExtraClub.EmployeesModule.Views.ContainedControls.Employees.Windows;
using ExtraClub.Reports.ViewModels;
using ExtraClub.Reports.Views;
using ExtraClub.SettingsModule.Views.ContainedControls;
using ExtraClub.SettingsModule.ViewModels;
using ExtraClub.SettingsModule.Views;

using System.Windows;
using ExtraClub.UIControls;
using ExtraClub.Infrastructure.BaseClasses;
using ExtraClub.ServiceModel;
using System.Windows.Navigation;
using ExtraClub.CashRegisterModule;
using ExtraClub.Infrastructure;
using ExtraClub.UIControls.BaseClasses;

namespace ExtraClub.WinClient
{
    public partial class MainContent : INotifyPropertyChanged
    {
        ScheduleLargeViewModel scheduleModel;
        BarPointViewModel barModel;
        TurnoverLargeViewModel turnoverModel;

        public MainContent()
        {
            InitializeComponent();
            DataContext = this;

            NavigationManager.ClientRequest += NavigationManagerClientRequest;
            NavigationManager.ScheduleRequest += NavManScheduleRequest;
            NavigationManager.SmartScheduleRequest += NavManSmartScheduleRequest;

            NavigationManager.NewSolariumVisitRequest += NavManSolRequest;
            NavigationManager.ActivateSolariumScheduleRequest += NavigationManagerDoActivateSolariumSchedule;
            NavigationManager.ActivateTreatmentsScheduleRequest += NavigationManagerActivateTreatmentsScheduleRequest;
            NavigationManager.BarRequest += NavigationManagerBarRequest;
            NavigationManager.ActivateLoginsRequest += NavigationManagerActivateLoginsRequest;


            NavigationManager.CustomerTargetRequest += NavigationManagerTreatmentEventRequest;
            NavigationManager.TreatmentEventRequest += NavigationManagerTreatmentEventRequest;
            NavigationManager.TicketRequest += NavigationManagerTreatmentEventRequest;
            NavigationManager.CustomerCardRequest += NavigationManagerTreatmentEventRequest;
            NavigationManager.GoodSalesRequest += NavigationManagerTreatmentEventRequest;
            NavigationManager.SpendingRequest += NavigationManagerSpendingRequest;
            NavigationManager.TicketRemainReportRequest += NavigationManagerTicketRemainReportRequest;
            NavigationManager.UserActionsReportRequest += NavigationManagerTicketRemainReportRequest;
            NavigationManager.AllCustomersReportRequest += NavigationManager_AllCustomersReportRequest;
            NavigationManager.AllTicketsReportRequest += NavigationManager_AllCustomersReportRequest;

            NavigationManager.QueryOneEmployee += NavManQueryOneEmployee;
            NavigationManager.EmployeeRequest += NavigationManagerEmployeeRequest;
            NavigationManager.NavigateToCashFlow += NavigationNavigateToCashFlow;
            NavigationManager.NavigateToJob += NavigationNavigateToJob;

            NavigationManager.RefreshBarRequest += NavigationManager_RefreshBarRequest;
            NavigationManager.RefreshGoodsRequest += NavigationManager_RefreshGoodsRequest;


        }

        void NavigationManager_AllCustomersReportRequest(object sender, EventArgs e)
        {
            CurrentRegion = _loadedViews["Reports"];
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Guid g;
            if(Guid.TryParse(e.Uri.ToString(), out g))
            {
                NavigationManager.MakeDownloadFileRequest(g);
                e.Handled = true;
                return;
            }
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        void NavigationManager_RefreshBarRequest(object sender, EventArgs e)
        {
            if(barModel != null)
            {
                barModel.RefreshDataAsync();
            }
        }

        void NavigationManager_RefreshGoodsRequest(object sender, EventArgs e)
        {
            if(turnoverModel != null)
            {
                turnoverModel.RefreshDataAsync();
            }
        }

        private void NavigationNavigateToCashFlow(object sender, StringEventArgs e)
        {
            CurrentRegion = _loadedViews["Employees"];
        }
        private void NavigationNavigateToJob(object sender, GuidEventArgs e)
        {
            CurrentRegion = _loadedViews["StateSchedule"];
        }


        void NavManQueryOneEmployee(object sender, ObjectEventArgs e)
        {
            ProcessUserDialog<SelectEmployeeWindow>(w => e.OnEmployee(w.EmployeeResult));
        }


        void NavigationManagerTicketRemainReportRequest(object sender, ObjectEventArgs e)
        {
            CurrentRegion = _loadedViews["Reports"];
        }

        void NavigationManagerEmployeeRequest(object sender, GuidEventArgs e)
        {
            CurrentRegion = _loadedViews["Employees"];
        }

        void NavigationManagerSpendingRequest(object sender, GuidEventArgs e)
        {
            CurrentRegion = _loadedViews["Finances"];
        }

        void NavigationManagerTreatmentEventRequest(object sender, GuidEventArgs e)
        {
            CurrentRegion = _loadedViews["ClientsRegion"];
        }

        void NavigationManagerActivateLoginsRequest(object sender, ObjectEventArgs e)
        {
            CurrentRegion = _loadedViews["FranchRegion"];
        }

        void NavigationManagerActivateTreatmentsScheduleRequest(object sender, EventArgs e)
        {
            CurrentRegion = _loadedViews["MainSchedule"];
        }

        void NavigationManagerBarRequest(object sender, ClientEventArgs e)
        {
            CurrentRegion = _loadedViews["BarPoint"];
            if(e.ClientId != Guid.Empty)
            {
                ((BarPointView)CurrentRegion).Search.SelectById(e.ClientId);
            }

        }

        void NavigationManagerDoActivateSolariumSchedule(object sender, EventArgs e)
        {
            CurrentRegion = _loadedViews["SolariumSchedule"];
        }

        void NavManScheduleRequest(object sender, ScheduleRequestEventArgs e)
        {
            var wnd = ApplicationDispatcher.UnityContainer.Resolve<NewScheduleWindow>(new ParameterOverride("pars", e.ScheduleRequestParams));
            wnd.Closed += wnd_Closed;
            wnd.Show();
        }

        void NavManSmartScheduleRequest(object sender, ScheduleRequestEventArgs e)
        {
            var wnd = ApplicationDispatcher.UnityContainer.Resolve<SmartScheduleWindow>(new ParameterOverride("pars", e.ScheduleRequestParams));
            wnd.Closed += wnd_Closed1;
            wnd.Show();
        }

        void wnd_Closed(object sender, EventArgs e)
        {
            if((sender as NewScheduleWindow).ScheduleRequestParams.OnClose != null)
            {
                (sender as NewScheduleWindow).ScheduleRequestParams.OnClose();
            }
        }
        void wnd_Closed1(object sender, EventArgs e)
        {
            if((sender as SmartScheduleWindow).ScheduleRequestParams.OnClose != null)
            {
                (sender as SmartScheduleWindow).ScheduleRequestParams.ExecuteCloseMethod();
            }
        }

        void NavManSolRequest(object sender, NewSolariumVisitEventArgs e)
        {
            ProcessUserDialog<NewBooking>(e.NewSolariumVisitParams.CloseAction, new ParameterOverride("parameters", e.NewSolariumVisitParams));
        }

        void NavigationManagerClientRequest(object sender, ClientEventArgs e)
        {
            CurrentRegion = _loadedViews["ClientsRegion"];
        }

        readonly Dictionary<string, UserControl> _loadedViews = new Dictionary<string, UserControl>();
        NewsModel newsModel;


        object _currentRegion;
        public object CurrentRegion
        {
            get
            {
                return _currentRegion;
            }
            set
            {
                _currentRegion = value;
                tCC.Content = value;
                OnPropertyChanged("CurrentRegion");
            }
        }

        public void LoadUiAsync()
        {
            var s = Stopwatch.StartNew();
            var cliModel = ApplicationDispatcher.UnityContainer.Resolve<ClientLargeViewModel>();
            barModel = ApplicationDispatcher.UnityContainer.Resolve<BarPointViewModel>();
            turnoverModel = ApplicationDispatcher.UnityContainer.Resolve<TurnoverLargeViewModel>();
            scheduleModel = ApplicationDispatcher.UnityContainer.Resolve<ScheduleLargeViewModel>();
            var treatmentsModel = ApplicationDispatcher.UnityContainer.Resolve<TreatmentsLargeViewModel>();
            var organizerModel = ApplicationDispatcher.UnityContainer.Resolve<OrganizerLargeViewModel>();
            var employeesModel = ApplicationDispatcher.UnityContainer.Resolve<EmployeesLargeViewModel>();
            var reportsModel = ApplicationDispatcher.UnityContainer.Resolve<ReportLargeViewModel>();
            var settingsModel = ApplicationDispatcher.UnityContainer.Resolve<SettingsLargeViewModel>();
            var uniedControlModel = ApplicationDispatcher.UnityContainer.Resolve<UniedReportViewModel>();
            var workbenchReport = ApplicationDispatcher.UnityContainer.Resolve<WorkbenchReportViewModel>();
            var recurrentModel = ApplicationDispatcher.UnityContainer.Resolve<RecurrentReportsViewModel>();
            var sshFilesModel = ApplicationDispatcher.UnityContainer.Resolve<SshFilesViewModel>();
            var cashierDocsModel = ApplicationDispatcher.UnityContainer.Resolve<CashierDocumentsViewModel>();

            newsModel = ApplicationDispatcher.UnityContainer.Resolve<NewsModel>();

            Debug.WriteLine("CreateModel " + s.ElapsedMilliseconds);

            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                if(ClientContext.CheckPermission("UniedReport"))
                {
                    _loadedViews.Add("UniedReport", new UniedReportControl(uniedControlModel));
                    CurrentRegion = _loadedViews["UniedReport"];
                }
                else
                {
                    var item = SearchDBS(ReportsLeftButton.DropdownButtonsSource, "UniedReport");
                    if(item != null)
                    {
                        ReportsLeftButton.DropdownButtonsSource.Remove(item);
                    }
                }

                if(ClientContext.CheckPermission("WorkbenchReport"))
                {
                    _loadedViews.Add("WorkbenchReport", new WorkbenchReport(workbenchReport));
                    if(!ClientContext.CheckPermission("UniedReport"))
                    {
                        CurrentRegion = _loadedViews["WorkbenchReport"];
                    }
                }
                else
                {
                    var item = SearchDBS(ReportsLeftButton.DropdownButtonsSource, "WorkbenchReport");
                    if(item != null)
                    {
                        ReportsLeftButton.DropdownButtonsSource.Remove(item);
                    }
                }

                NewsPopup.DataContext = newsModel; 
                

                if (!ApplicationDispatcher.UnityContainer.Resolve<ClientContext>().CheckPermission("KKMOperationsInMedium"))
                {
                    BarLeftButton.DropdownButtonsSource.Clear();
                }


            }));


            Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                                                                            {
                                                                                _loadedViews.Add("ClientsRegion",
                                                                                                 ApplicationDispatcher.UnityContainer.Resolve
                                                                                                     <ClientLargeView>(
                                                                                                         new ParameterOverride
                                                                                                             ("model",
                                                                                                              cliModel)));
                                                                                _loadedViews.Add("BarPoint",
                                                                                                 ApplicationDispatcher.UnityContainer.Resolve
                                                                                                     <BarPointView>(
                                                                                                         new ParameterOverride
                                                                                                             ("model",
                                                                                                              barModel)));


                                                                                SetRegionAssociation("TurnoverBarPanel",
                                                                                                     "Turnover",
                                                                                                     () => ApplicationDispatcher.UnityContainer.Resolve
                                                                                                         <BarModule>(
                                                                                                             new ParameterOverride
                                                                                                                 ("model",
                                                                                                                  turnoverModel)),
                                                                                                     null);

                                                                                SetRegionAssociation(
                                                                                    "TurnoverFinansesPanel",
                                                                                    "Finances",
                                                                                    () => ApplicationDispatcher.UnityContainer.Resolve<FinancesControl>(
                                                                                        new ParameterOverride("model",
                                                                                                              turnoverModel)),
                                                                                    TurnoverLeftButton);

                                                                                SetRegionAssociation(
                                                                                    "TurnoverFlowPanel",
                                                                                    "Consignments",
                                                                                    () => ApplicationDispatcher.UnityContainer.Resolve
                                                                                        <TurnoverAndProvidersControl>(
                                                                                            new ParameterOverride(
                                                                                                "model", turnoverModel)),
                                                                                    TurnoverLeftButton);

                                                                                SetRegionAssociation(
                                                                                    "TurnoverCatalogPanel",
                                                                                    "Dictionaries",
                                                                                    () => ApplicationDispatcher.UnityContainer.Resolve<GoodsListControl>
                                                                                        (new ParameterOverride("model",
                                                                                                               turnoverModel)),
                                                                                    TurnoverLeftButton);

                                                                                SetRegionAssociation(
                                                                                    "TurnoverFinansesPanel",
                                                                                    "CashierDocuments",
                                                                                    () => ApplicationDispatcher.UnityContainer.Resolve<CashierDocumentsControl>
                                                                                        (new ParameterOverride("model",
                                                                                                               cashierDocsModel)),
                                                                                    TurnoverLeftButton);
                                                                                AuthorizationManager.ApplyPermissions(null);
                                                                            }));


            Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, new Action(() =>
            {
                _loadedViews.Add("Treatments", ApplicationDispatcher.UnityContainer.Resolve<TreatmentsLargeView>(new ParameterOverride("model", treatmentsModel)));
                SetRegionAssociation("TreatmentScheduleGrid", "MainSchedule", () => ApplicationDispatcher.UnityContainer.Resolve<TreatmentsGrid>(new ParameterOverride("model", scheduleModel)), null);
                SetRegionAssociation("SolariumScheduleGrid", "SolariumSchedule", () => ApplicationDispatcher.UnityContainer.Resolve<SolariumGrid>(new ParameterOverride("model", scheduleModel)), null);

                SetRegionAssociation("SmartScheduleMaster", "SmartScheduleMaster", null, ScheduleLeftButton);

                SetRegionAssociation("_DISABLE", "ConsScheduleMaster", null, ScheduleLeftButton);
                AuthorizationManager.ApplyPermissions(null);

                Mouse.OverrideCursor = null;
            }));
            Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                _loadedViews.Add("Organizer", new OrganizerLargeView(organizerModel));

                SetRegionAssociation("OrganizerCallsBar", "Calls", () => new CallListControl { DataContext = organizerModel }, OrganizerLeftButton);
                SetRegionAssociation("IncomingCallButton", "IncomingCallButton", null, OrganizerLeftButton);
                SetRegionAssociation("OutgoingCallButton", "OutgoingCallButton", null, OrganizerLeftButton);

                SetRegionAssociation("Claims", "Claims", () => new ClaimsControl { DataContext = organizerModel }, OrganizerLeftButton);
                SetRegionAssociation("Ankets", "Ankets", () => new AnketsControl { DataContext = organizerModel }, OrganizerLeftButton);

                SetRegionAssociation("SshControl", "SshFiles", () => new CloseControl { DataContext = sshFilesModel }, OrganizerLeftButton);
                //SetRegionAssociation("BudVKurse", "BudVKurse", null, OrganizerLeftButton);
                AuthorizationManager.ApplyPermissions(null);

            }));
            Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                if(ClientContext.CheckPermission("EmployeesDocumentsBar"))
                {
                    _loadedViews.Add("Employees", new EmployeesControl() { DataContext = employeesModel });
                }

                SetRegionAssociation("VacationsBar", "Vacations", () => new VacationsControl() { DataContext = employeesModel }, EmployeesLeftButton);
                SetRegionAssociation("WorkGraphBar", "WorkGraph", () => new WorkGraphControl() { DataContext = employeesModel }, EmployeesLeftButton);
                SetRegionAssociation("EmployeeScheduleBar", "StateSchedule", () => new VacanciesControl() { DataContext = employeesModel }, EmployeesLeftButton);
                AuthorizationManager.ApplyPermissions(null);


            }));
            Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                _loadedViews.Add("Reports", new ReportsLargeView(reportsModel));
                _loadedViews.Add("RecurrentReports", new RecurrentReportsView(recurrentModel));
                SetRegionAssociation("GetAllCustomersEx", "AllCustomers", null, ReportsLeftButton);
                SetRegionAssociation("GetAllTicketsEx", "AllTickets", null, ReportsLeftButton);

                _loadedViews.Add("Settings", new ApplicationSettingsControl(settingsModel, ApplicationDispatcher.UnityContainer.Resolve<CashRegisterManager>()));


                SetRegionAssociation("ClubConfigMgmt", "ClubRegion", () => new ClubControl(settingsModel), SettingsLeftButton);
                SetRegionAssociation("FranchConfigMgmt", "FranchRegion", () => new FranchControl(settingsModel), SettingsLeftButton);
                SetRegionAssociation("GeneralConfigMgmt", "CSRegion", () => new SettingsLargeView(settingsModel), SettingsLeftButton, false);
                AuthorizationManager.ApplyPermissions(null);

            }));
            Dispatcher.Invoke(DispatcherPriority.DataBind, new Action(() =>
            {
                if(ClientContext.CheckPermission("UniedReport"))
                {
                    uniedControlModel.RefreshDataAsync();
                }
                if(ClientContext.CheckPermission("WorkbenchReport"))
                {
                    workbenchReport.RefreshDataAsync();
                }
            }));
            Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                newsModel.RefreshDataAsync();
                scheduleModel.RefreshDataAsync();
                treatmentsModel.RefreshDataAsync();
                cliModel.RefreshDataAsync();
                turnoverModel.RefreshDataAsync(() => barModel.RefreshDataAsync(turnoverModel));
                organizerModel.RefreshDataAsync();
                employeesModel.RefreshDataAsync();
                reportsModel.RefreshDataAsync();
                settingsModel.RefreshDataAsync();
                sshFilesModel.RefreshDataAsync();
                cashierDocsModel.RefreshDataAsync();
                recurrentModel.RefreshDataAsync();
            }));


        }

        private void SetRegionAssociation(string permissionName, string regionName, Func<UserControl> region, FlagmaxControls.LeftButtonControl buttonContainer, bool disableForRs = false)
        {
            if(ClientContext.CheckPermission(permissionName) && (!disableForRs || !ClientContext.CheckPermission("DisableCentral")))
            {
                if(region != null)
                {
                    _loadedViews.Add(regionName, region());
                }
            }
            else
            {
                if(buttonContainer != null)
                {
                    var item = SearchDBS(buttonContainer.DropdownButtonsSource, regionName);
                    if(item != null)
                    {
                        buttonContainer.DropdownButtonsSource.Remove(item);
                    }
                }
            }
        }

        private SubButtonInfo SearchDBS(IEnumerable<SubButtonInfo> observableCollection, string p)
        {
            return observableCollection.FirstOrDefault(i => i.RegionKey == p);
        }

        private void LeftButtonControlRegionRequested(object sender, FlagmaxControls.Infrastructure.LeftClickEventArgs e)
        {
            if(e.RegionName == "NewCustomer")
            {
                ProcessUserDialog<NewCustomerWindow>(() => { });
                return;
            }
            if(e.RegionName == "CustomerIn")
            {
                ProcessUserDialog<RegisterComeIn>(() => { barModel.RefreshDataAsync(); });
                return;
            }
            if(e.RegionName == "CustomerOut")
            {
                ProcessUserDialog<RegisterComeOut>(() => { barModel.RefreshDataAsync(); });
                return;
            }
            if(e.RegionName == "BrokenTreatment")
            {
                ProcessUserDialog<BrokenTreatmentWindow>(() => { });
                return;
            }
            if(e.RegionName == "MainScheduleMaster")
            {
                var w = ApplicationDispatcher.UnityContainer.Resolve<NewScheduleWindow>();
                w.Closed += w_Closed;
                w.Show();
                return;
            }
            if(e.RegionName == "SmartScheduleMaster")
            {
                var w = ApplicationDispatcher.UnityContainer.Resolve<SmartScheduleWindow>();
                w.Closed += w_Closed;
                w.Show();
                return;
            }

            if(e.RegionName == "SolariumScheduleMaster")
            {
                ProcessUserDialog<NewBooking>(() => { });
                return;
            }
            if(e.RegionName == "FR")
            {
                ProcessUserDialog<KKMOperations>(() => { });
                return;
            }
            if(e.RegionName == "AllCustomers")
            {
                NavigationManager.MakeAllCustomersReportRequest();
                return;
            }
            if(e.RegionName == "AllTickets")
            {
                NavigationManager.MakeAllTicketsReportRequest();
                return;
            }
            if(e.RegionName == "IncomingCallButton")
            {
                new CallManager(null);
                return;
            }
            if(e.RegionName == "OutgoingCallButton")
            {
                NavigationManager.MakeNewOutgoingCallRequest(null, () => { });
                return;
            }
            if(e.RegionName == "AddPhotoToFR")
            {
                System.Diagnostics.Process.Start("http://crm.sendika.ru/Clubs/AddPhotoToFR?clubId=" + ClientContext.CurrentDivision.Id);
                return;
            }
            if(e.RegionName == "EmpVisit")
            {
                ProcessUserDialog<EmployeeInOutWindow>(() => { });
                return;
            }
            if(_loadedViews.ContainsKey(e.RegionName))
            {
                CurrentRegion = _loadedViews[e.RegionName];
            }
        }

        void w_Closed(object sender, EventArgs e)
        {
            (sender as Window).Closed -= w_Closed;
            scheduleModel.RefreshDataAsync();
        }

        private void RefreshClick(object sender, System.Windows.RoutedEventArgs e)
        {
            if(CurrentRegion != null)
            {
                var fe = CurrentRegion as FrameworkElement;
                if(fe != null)
                {
                    var dc = fe.DataContext as ViewModelBase;
                    if(dc != null)
                    {
                        dc.RefreshDataSync();
                        if(dc is ClientLargeViewModel)
                        {
                            var cm = dc as ClientLargeViewModel;
                            if(cm.CurrentCustomer != null)
                            {
                                cm.SelectClient(cm.CurrentCustomer.Id);
                            }
                        }
                    }
                }
            }
        }
    }
}
