using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Telerik.Windows.Controls;
using ExtraClub.ClientDal;
using ExtraClub.ClientDal.Wizards.NewClub;
using ExtraClub.ClientDal.Wizards.NewCompany;
using ExtraClub.Infrastructure;
using ExtraClub.ServiceModel;
using ExtraClub.ServiceModel.Employees;
using ExtraClub.ServiceModel.Organizer;
using ExtraClub.ServiceModel.Reports;
using ExtraClub.ServiceModel.Schedule;
using ExtraClub.ServiceModel.Ssh;
using ExtraClub.UIControls.Interfaces;
using ExtraClub.UIControls.Windows;

namespace ExtraClub.UIControls
{
    public class ClientContext : IClientContext
    {
        Lazy<string[]> Perms { get; }
        Lazy<User> User { get; }
        Lazy<Division> Division { get; set; }
        Lazy<Company> Company { get; set; }

        public string[] СlientPermissions => Perms.Value;
        public User CurrentUser => User.Value;
        public Division CurrentDivision => Division.Value;
        public Company CurrentCompany => Company.Value;

        public bool NeedAct { get; private set; }

        private ChannelFactory<IExtraService> _channelFactory;

        private IExtraService CreateChannel()
        {
            return _channelFactory.CreateChannel();
        }

        public ClientContext()
        {
            var w = Stopwatch.StartNew();

            //TODO: Remove this shit
            AuthorizationManager.Init(this);

            Perms =
                new Lazy<string[]>(() => ExecuteMethod(i => i.GetUserPermissions(CurrentDivision?.Id ?? Guid.Empty)));
            User = new Lazy<User>(() => ExecuteMethod(i => i.GetCurrentUser()));
            Division = new Lazy<Division>(() =>
            {
                var division = ExecuteMethod(i => i.GetDivision(new Guid(AppSettingsManager.GetSetting("DivisionId"))));
                if (division != null && String.IsNullOrEmpty(division.Act))
                    NeedAct = true;
                return division;
            });
            Company = new Lazy<Company>(() => ExecuteMethod(i => i.GetCompnay()));

            Debug.WriteLine("Start creating ClientContext");
            ServicePointManager.ServerCertificateValidationCallback = IgnoreCertificateErrorHandler;
            if (!Authenticate())
            {
                Environment.Exit(0);
            }
            Debug.WriteLine("ClientContext initialization takes " + w.ElapsedMilliseconds + " ms.");

            if (CurrentDivision == null)
            {
                NavigationManager.CloseSplash();

                if (String.IsNullOrWhiteSpace(CurrentCompany.GeneralManagerName))
                {
                    var wnd1 = new NewCompanyWizard(this);
                    wnd1.ShowDialog();
                    if (!(wnd1.DialogResult ?? false))
                    {
                        Environment.Exit(0);
                    }
                }
            }
        }

        private static bool IgnoreCertificateErrorHandler(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private bool Authenticate(string address = "", string message = null)
        {

            var login = new LoginWindow(message ?? Localization.Resources.ProvideCredentials);

            if (!login.ShowDialog() ?? false)
            {
                return false;
            }

            var w = Stopwatch.StartNew();
            Debug.WriteLine("Start Connecting");
            _channelFactory = new ChannelFactory<IExtraService>("TCEndpoint");

            if (!String.IsNullOrWhiteSpace(address))
            {
                _channelFactory.Endpoint.Address = new EndpointAddress(address);
            }
            else

                if (!String.IsNullOrWhiteSpace(AppSettingsManager.GetSetting("ServerAddress")))
            {
                _channelFactory.Endpoint.Address = new EndpointAddress(AppSettingsManager.GetSetting("ServerAddress"));
            }

            _channelFactory.Endpoint.Behaviors.Add(new CultureBehaviour());

            if (_channelFactory.Credentials != null)
            {
                _channelFactory.Credentials.UserName.UserName = login.UserName;
                _channelFactory.Credentials.UserName.Password = login.Password;
            }

            try
            {
                if (!CurrentUser.LastPasswordChanged.HasValue
                    || CurrentUser.LastPasswordChanged.Value < DateTime.Now.AddDays(-45))
                {
                    NeedChangePassword = true;
                }

                if (login.IsSave) login.ConfirmCredentials();
                Debug.WriteLine("connection takes " + w.ElapsedMilliseconds + " ms.");
                return true;
            }
            catch (EndpointNotFoundException)
            {
                MessageBox.Show(Localization.Resources.NoResponse);
                Application.Current.Shutdown();
                //var addr = "";
                //ExtraWindow.Prompt(new DialogParameters
                //{
                //    Header = UIControls.Localization.Resources.NoResponse,
                //    Content = UIControls.Localization.Resources.ProvideServerAddress,
                //    DefaultPromptResultValue = cf.Endpoint.Address.Uri.AbsoluteUri,
                //    Closed = delegate(object sender1, WindowClosedEventArgs e1)
                //        {
                //            if ((e1.DialogResult ?? false))
                //            {
                //                AppSettingsManager.SetSetting("ServerAddress", e1.PromptResult.Trim());
                //                addr = e1.PromptResult.Trim();
                //            }
                //        },
                //    CancelButtonContent = UIControls.Localization.Resources.Cancel
                //});

                //return Authenticate(addr);
            }
            catch (Exception ex)
            {
                if (ex is MessageSecurityException)
                {
                    if (ex.InnerException is FaultException)
                    {
                        return Authenticate(message: ex.InnerException.Message);
                    }
                    MessageBox.Show(ex.InnerException.Message);
                }
                MessageBox.Show(ex.Message);
            }
            return false;

        }

        public void StartChangePassword(string cause)
        {
            var wnd = new PasswordChangeWindow(cause) { Owner = Application.Current.MainWindow };
            wnd.ShowDialog();
            if (wnd.DialogResult ?? false)
            {
                NeedChangePassword = false;
            }
        }

        public void ExecuteMethod(Action<IExtraService> method)
        {
            ExecuteMethod(i =>
            {
                method(i);
                return default(object);
            });
        }

        public TResult ExecuteMethod<TResult>(Func<IExtraService, TResult> method)
        {
            var channel = CreateChannel();
            var sw = Stopwatch.StartNew();
            try
            {
                return method(channel);
            }
            finally
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                var commObject = channel as ICommunicationObject;
                try
                {
                    commObject?.Close();
                }
                catch (CommunicationException)
                {
                    commObject?.Abort();
                }
                catch (TimeoutException)
                {
                    commObject?.Abort();
                }

                if (Application.Current.Dispatcher.Thread == Thread.CurrentThread && sw.ElapsedMilliseconds > 1000)
                {
                    Debug.WriteLine(new String('-', 40));
                    Debug.WriteLine($"Sync invoke take {sw.ElapsedMilliseconds} ms for {method.Method.Name}");

#if DEBUG
                    foreach (var frame in new StackTrace(true).GetFrames())
                    {
                        if (!String.IsNullOrWhiteSpace(frame.GetFileName()))
                        {
                            Debug.Write(frame.ToString());
                        }
                    }
#endif
                }
            }
        }

        public Task<TResult> ExecuteMethodAsync<TResult>(Func<IExtraService, TResult> method)
        {
            return Task<TResult>.Factory.StartNew(() => ExecuteMethod(method));
        }

        public List<FoundCustomer> SearchCustomers(string searchKey)
        {
            return ExecuteMethod(client => client.SearchCustomers(searchKey));
        }

        public bool CheckPermission(string permissionName)
        {
            return СlientPermissions.Contains(permissionName);
        }


        public List<Good> GetAllGoods()
        {
            return ExecuteMethod(client => client.GetAllGoods(CurrentCompany.CompanyId));
        }

        public void PostGood(Good good)
        {
            ExecuteMethod(client => client.PostGood(good));
        }

        #region IClientContext Members


        public Dictionary<string, DictionaryInfo> GetAllDictionaryInfos()
        {
            return ExecuteMethod(i => i.GetAllDictionaryInfos());
        }

        public Dictionary<Guid, string> GetDictionaryList(string tableName)
        {
            return ExecuteMethod(i => i.GetDictionaryList(tableName));
        }

        #endregion

        public List<Provider> GetAllProviders()
        {
            return ExecuteMethod(i => i.GetAllProviders());
        }


        public void PostProvider(Provider provider)
        {
            ExecuteMethod(i => i.PostProvider(provider));
        }

        public List<ProviderPayment> GetAllProviderPayments(DateTime start, DateTime end)
        {
            if (CurrentDivision == null) return new List<ProviderPayment>();
            return ExecuteMethod(i => i.GetAllProviderPayments(CurrentDivision.Id, start, end));
        }

        public List<Consignment> GetAllConsignments(DateTime start, DateTime end, bool cons)
        {
            if (CurrentDivision == null) return new List<Consignment>();
            return ExecuteMethod(i => i.GetAllConsignments(CurrentDivision.Id, start, end, cons));
        }

        public void PostConsignment(Consignment consignment)
        {
            ExecuteMethod(i => i.PostConsignment(consignment));
        }


        public List<ConsignmentLine> GetAllConsignmentLines()
        {
            return ExecuteMethod(i => i.GetAllConsignmentLines());
        }

        public void PostConsignmentLines(IEnumerable<ConsignmentLine> changes)
        {
            ExecuteMethod(i => i.PostConsignmentLines(changes.ToList()));
        }

        public List<GoodPrice> GetGoodPrices()
        {
            if (CurrentDivision == null) return new List<GoodPrice>();
            return ExecuteMethod(i => i.GetAllPrices(CurrentDivision.Id));
        }

        public List<BarPointGood> GetGoodsPresence()
        {
            if (CurrentDivision == null) return new List<BarPointGood>();
            return ExecuteMethod(i => i.GetGoodsPresence(CurrentDivision.Id));
        }


        public List<Customer> GetPresentCustomers()
        {
            if (CurrentDivision == null) return new List<Customer>();
            return ExecuteMethod(i => i.GetPresentCustomers(CurrentDivision.Id));
        }

        public PaymentDetails ProcessPayment(PaymentDetails details, IEnumerable<PayableItem> basket, Guid goodActionId)
        {
            return ExecuteMethod(i => i.ProcessPayment(details, basket.ToList(), goodActionId));
        }

        public Customer GetCustomer(Guid customerId, bool loadDetails = false)
        {
            return ExecuteMethod(i => i.GetCustomer(customerId, loadDetails));
        }

        public int GetMaxPaymentNumber()
        {
            return ExecuteMethod(i => i.GetMaxPaymentNumber());
        }

        public IEnumerable<TicketType> GetTicketTypes(bool activeOnly)
        {
            return ExecuteMethod(i => i.GetTicketTypes(activeOnly));
        }

        public PaymentDetails ProcessReturn(PaymentDetails pmt, IEnumerable<PayableItem> items)
        {
            return ExecuteMethod(i => i.ProcessReturn(pmt, items.ToList()));
        }

        public IEnumerable<CustomerCardType> GetCustomerCardTypes(bool activeOnly)
        {
            return ExecuteMethod(i => i.GetCustomerCardTypes(activeOnly));
        }

        public bool RegisterCustomerVisit(Guid customerId, int shelfNumber, int safeNumber)
        {
            return ExecuteMethod(i => i.RegisterCustomerVisit(customerId, CurrentDivision.Id, shelfNumber, safeNumber));
        }

        public List<GoodAction> GetGoodActions(bool onlyActive)
        {
            return ExecuteMethod(i => i.GetGoodActions(onlyActive));
        }

        public void PostGoodAction(Guid actionId, string actionName, double discount, IEnumerable<KeyValuePair<Guid, int>> goods, bool isActive)
        {
            ExecuteMethod(i => i.PostGoodAction(actionId, actionName, discount, goods, isActive));
        }

        public void DeleteGoodAction(Guid goodActionId)
        {
            ExecuteMethod(i => i.DeleteGoodAction(goodActionId));
        }

        public void PostGoodPrice(GoodPrice goodPrice)
        {
            ExecuteMethod(i => i.PostGoodPrice(goodPrice));
        }


        public void PostProviderPayment(Guid orderId, DateTime date, string paymentType, decimal amount, string comment)
        {
            ExecuteMethod(i => i.PostProviderPayment(orderId, date, paymentType, amount, comment));
        }

        public void ActivateTicket(Guid ticketId)
        {
            ExecuteMethod(i => i.ActivateTicket(ticketId));
        }

        public Guid RegisterCustomerVisitEnd(Guid customerId, bool shelfReturned, bool safeReturned)
        {
            return ExecuteMethod(i => i.RegisterCustomerVisitEnd(customerId, CurrentDivision.Id, shelfReturned, safeReturned));
        }

        public Guid PostNewDictionaryElement(Guid dictionaryId, string newElementName)
        {
            return ExecuteMethod(i => i.PostNewDictionaryElement(dictionaryId, newElementName));
        }

        public void PostRenameDictionaryElement(Guid dictionaryId, Guid elementGuid, string elementName)
        {
            ExecuteMethod(i => i.PostRenameDictionaryElement(dictionaryId, elementGuid, elementName));
        }

        public string PostRemoveDictionaryElement(Guid dictionaryId, Guid elementGuid)
        {
            return ExecuteMethod(i => i.PostRemoveDictionaryElement(dictionaryId, elementGuid));
        }

        public Guid PostCustomer(Customer customer)
        {
            return ExecuteMethod(i => i.PostCustomer(customer));
        }

        public List<Treatment> GetAllTreatments()
        {
            if (CurrentDivision == null) return new List<Treatment>();
            return ExecuteMethod(i => i.GetAllTreatments(CurrentDivision.Id));
        }

        public Ticket GetTicketById(Guid ticketId)
        {
            return ExecuteMethod(i => i.GetTicketById(ticketId));
        }

        public IEnumerable<TreatmentConfig> GetAllTreatmentConfigs()
        {
            return ExecuteMethod(i => i.GetAllTreatmentConfigs());
        }

        public void PostTreatment(Treatment treatment)
        {
            ExecuteMethod(i => i.PostTreatment(treatment));
        }

        public IEnumerable<TreatmentSeqRest> GetAllTreatmentSeqRests()
        {
            return ExecuteMethod(i => i.GetAllTreatmentSeqRests());
        }


        public void PostTreatmentSeqRest(TreatmentSeqRest treatmentSr)
        {
            ExecuteMethod(i => i.PostTreatmentSeqRest(treatmentSr));
        }

        public ScheduleProposalResult GetScheduleProposals(Guid customerId, Guid ticketId, DateTime visitDate, bool isParallelAllowed, List<Guid> treatments, bool isOptimalAllowed, Guid programId)
        {
            if (CurrentDivision == null) return new ScheduleProposalResult { List = new List<ScheduleProposal>() };
            return ExecuteMethod(i => i.GetScheduleProposals(customerId, CurrentDivision.Id, ticketId, visitDate, isParallelAllowed, treatments, isOptimalAllowed, programId));
        }


        public void PostScheduleProposal(Guid customerId, Guid ticketId, ScheduleProposal scheduleProposal)
        {
            ExecuteMethod(i => i.PostScheduleProposal(customerId, CurrentDivision.Id, ticketId, scheduleProposal));
        }


        public Customer GetCustomerByCard(int cardNumber, bool loadDetails)
        {
            try
            {
                return ExecuteMethod(i => i.GetCustomerByCard(cardNumber, loadDetails));
            }
            catch (ArgumentOutOfRangeException)
            {
                Debug.WriteLine("Card search exception!");
                return GetCustomerByCard(cardNumber, loadDetails);
            }
        }

        public List<TreatmentProgram> GetTreatmentPrograms()
        {
            return ExecuteMethod(i => i.GetTreatmentPrograms());
        }


        public List<AdvertType> GetAdvertTypes()
        {
            return ExecuteMethod(i => i.GetAdvertTypes());
        }

        public void UpdateCustomerForm(Customer customer)
        {
            ExecuteMethod(i => i.UpdateCustomerForm(customer));
        }


        public List<string>[] GetAddressLists()
        {
            return ExecuteMethod(i => i.GetAddressLists().ToArray());
        }


        public Dictionary<decimal, string> GetDiscountsForCurrentUser(DiscountTypes discountType)
        {
            return ExecuteMethod(ji => ji.GetDiscountsForCurrentUser((short)discountType).ToDictionary(i => i, i => $"{i * 100:n2}%"));
        }


        public string UpdateInvitor(Guid invitedId, Guid invitorId)
        {
            return ExecuteMethod(i => i.UpdateInvitor(invitedId, invitorId));
        }

        public IEnumerable<TicketType> GetActiveTicketTypesForCustomer(Guid customerId)
        {
            return ExecuteMethod(i => i.GetActiveTicketTypesForCustomer(customerId));
        }


        public int GetMaxGuestUnits(Guid divisionId, Guid customerId)
        {
            return ExecuteMethod(i => i.GetMaxGuestUnits(divisionId, customerId));
        }

        public string GenerateCardContractReport(string cardNumber)
        {
            return ExecuteMethod(i => i.GenerateCardContractReport(cardNumber, "GenerateCardContractReport"));
        }

        public string GeneratePkoReport(Guid id)
        {
            return ExecuteMethod(i => i.GeneratePKOReport(id));
        }

        public string GenerateCashierPageReport(Guid id, DateTime date)
        {
            return ExecuteMethod(i => i.GenerateCashierPageReport(id, date));
        }

        public string GenerateRkoReport(Guid id)
        {
            return ExecuteMethod(i => i.GenerateRKOReport(id));
        }

        public string GenerateCardUpgradeReport(string str)
        {
            return ExecuteMethod(i => i.GenerateCardContractReport(str, "GenerateCardUpgradeReport"));
        }

        public string GenerateCardLostReport(string str)
        {
            return ExecuteMethod(i => i.GenerateCardContractReport(str, "GenerateCardLostReport"));
        }

        public void PostCustomerAddress(Customer customer)
        {
            ExecuteMethod(i => i.PostCustomerAddress(customer.Id, customer.AddrMetro, customer.AddrIndex, customer.AddrCity, customer.AddrStreet, customer.AddrOther));
        }

        public string GenerateLastTicketSaleReport(Guid ticketId)
        {
            return ExecuteMethod(i => i.GenerateTicketContractReport(ticketId, Guid.Empty, "GenerateLastTicketSaleReport"));
        }

        public string GenerateTicketVatReport(Guid id)
        {
            return ExecuteMethod(i => i.GenerateTicketContractReport(id, Guid.Empty, "GenerateTicketVatReport"));
        }

        public string GenerateTicketReceiptReport(Guid id)
        {
            return ExecuteMethod(i => i.GenerateTicketReceiptReport(id, "GenerateTicketReceiptReport"));
        }

        public string GenerateLastTicketChangeReport(Guid id)
        {
            return ExecuteMethod(i => i.GenerateTicketContractReport(id, Guid.Empty, "GenerateLastTicketChangeReport"));
        }

        public string GenerateTicketRebillStatementReport(Guid a, Guid b)
        {
            return ExecuteMethod(i => i.GenerateTicketContractReport(a, b, "GenerateTicketRebillStatementReport"));
        }

        public List<TicketFreezeReason> GetTicketFreezeReasons()
        {
            return ExecuteMethod(i => i.GetTicketFreezeReasons());
        }

        public string GenerateTicketFreezeStatementReport(Guid a, object b)
        {
            return ExecuteMethod(i => i.GenerateTicketContractReport(a, b, "GenerateTicketFreezeStatementReport"));
        }

        public void CancelTreatmentEvents(List<Guid> events)
        {
            ExecuteMethod(i => i.CancelTreatmentEvents(events));
        }


        public void PostCustomerCardType(CustomerCardType customerCardType)
        {
            ExecuteMethod(i => i.PostCustomerCardType(customerCardType));
        }

        public void DeleteObject(string collectionName, Guid id)
        {
            try
            {
                ExecuteMethod(i => i.DeleteObject(collectionName, id));
            }
            catch (FaultException ex)
            {
                ExtraWindow.Alert(new DialogParameters
                {
                    Header = Localization.Resources.Error,
                    Content = Localization.Resources.UnableToDelete + "\n" + ex.Message,
                    OkButtonContent = Localization.Resources.Ok,
                    Owner = Application.Current.MainWindow
                });
            }
        }

        public void SetObjectActive(string collectionName, Guid id, bool isActive)
        {
            ExecuteMethod(i => i.SetObjectActive(collectionName, id, isActive));
        }

        public Guid PostTicketType(TicketType ticketType)
        {
            return ExecuteMethod(i => i.PostTicketType(ticketType));
        }


        public void PostTicketTypeTreatmentTypes(Guid ticketTypeId, List<Guid> treatmentTypes)
        {
            ExecuteMethod(i => i.PostTicketTypeTreatmentTypes(ticketTypeId, treatmentTypes));
        }

        public void PostTicketTypeCustomerCardTypes(Guid ticketTypeId, Guid[] ccTypes)
        {
            ExecuteMethod(i => i.PostTicketTypeCustomerCardTypes(ticketTypeId, ccTypes));
        }

        public void ClearCustomerContras(Guid customerId)
        {
            ExecuteMethod(i => i.ClearCustomerContras(customerId));
        }


        public List<ContraIndication> GetAllContras()
        {
            return ExecuteMethod(i => i.GetAllContras());
        }

        public List<Guid> GetCustomerContrasIds(Guid customerId)
        {
            return ExecuteMethod(i => i.GetCustomerContrasIds(customerId));
        }


        public void PostContraIndications(Guid customerId, List<Guid> contraIds)
        {
            ExecuteMethod(i => i.PostContraIndications(customerId, contraIds));
        }


        public Guid PostContraIndication(ContraIndication contraIndication)
        {
            return ExecuteMethod(i => i.PostContraIndication(contraIndication));
        }

        public void PostContraIndicationTreatmentTypes(Guid contraId, List<Guid> treatmentIds)
        {
            ExecuteMethod(i => i.PostContraIndicationTreatmentTypes(contraId, treatmentIds));
        }


        public List<CustomerTarget> GetCustomerTargets(Guid customerId)
        {
            return ExecuteMethod(i => i.GetCustomerTargets(customerId));
        }

        public string GetLastVisitText(Guid customerId)
        {
            return ExecuteMethod(i => i.GetLastVisitText(customerId));
        }


        public Guid PostCustomerTarget(CustomerTarget customerTarget)
        {
            return ExecuteMethod(i => i.PostCustomerTarget(customerTarget));
        }


        public List<OrganizerItem> GetOrganizerItems(DateTime start, DateTime end)
        {
            return ExecuteMethod(i => i.GetOrganizerItemsEx(start, end));
        }


        public string GenerateTicketReturnReport(Guid id)
        {
            return ExecuteMethod(i => i.GenerateTicketContractReport(id, Guid.Empty, "GenerateTicketReturnReport"));
        }

        public void StartTicketReturn(Guid ticketId, string comment)
        {
            ExecuteMethod(i => i.StartTicketReturn(ticketId, comment));
        }


        public List<Anthropometric> GetAnthropometricsForCustomer(Guid customerId)
        {
            return ExecuteMethod(i => i.GetAnthropometricsForCustomer(customerId));
        }


        public Guid PostCustomerAnthropomentric(Anthropometric anthropometric)
        {
            return ExecuteMethod(i => i.PostCustomerAnthropomentric(anthropometric));
        }


        public List<DoctorVisit> GetDoctorVisitsForCustomer(Guid customerId)
        {
            return ExecuteMethod(i => i.GetDoctorVisitsForCustomer(customerId));
        }

        public Guid PostCustomerDoctorVisit(DoctorVisit doctorVisit)
        {
            return ExecuteMethod(i => i.PostCustomerDoctorVisit(doctorVisit));
        }


        public List<string> GetGetDoctorTemplates()
        {
            return ExecuteMethod(i => i.GetDoctorTemplates());
        }


        public List<Nutrition> GetNutritionsForCustomer(Guid customerId)
        {
            return ExecuteMethod(i => i.GetNutritionsForCustomer(customerId));
        }

        public Guid PostNutrition(Nutrition nutrition)
        {
            return ExecuteMethod(i => i.PostNutrition(nutrition));
        }

        public List<string>[] GetNutritionTemplates()
        {
            return ExecuteMethod(i => i.GetNutritionTemplates());
        }


        public List<CustomerMeasure> GetMeasuresForCustomer(Guid customerId)
        {
            return ExecuteMethod(i => i.GetMeasuresForCustomer(customerId));
        }

        public Guid PostCustomerMeasure(CustomerMeasure measure)
        {
            return ExecuteMethod(i => i.PostCustomerMeasure(measure));
        }


        public List<TreatmentType> GetAllTreatmentTypes()
        {
            return ExecuteMethod(i => i.GetAllTreatmentTypes());
        }


        public Guid? GetTreatmentProgramIdForCustomer(Guid customerId)
        {
            return ExecuteMethod(i => i.GetTreatmentProgramIdForCustomer(customerId));
        }

        public List<TreatmentEvent> GetCustomerEvents(Guid customerId, DateTime start, DateTime end, bool canceled)
        {
            return ExecuteMethod(i => i.GetCustomerEvents(customerId, start, end, canceled));
        }


        public List<TextAction> GetCurrentActions()
        {
            if (CurrentDivision == null) return new List<TextAction>();
            return ExecuteMethod(i => i.GetCurrentActions(CurrentDivision.Id));
        }

        public List<TreatmentTypeGroup> GetAllTreatmentTypeGroups()
        {
            return ExecuteMethod(i => i.GetAllTreatmentTypeGroups());
        }

        public Guid PostTreatmentType(TreatmentType treatmentType)
        {
            return ExecuteMethod(i => i.PostTreatmentType(treatmentType));
        }


        public Guid PostTreatmentConfig(TreatmentConfig treatmentConfig)
        {
            return ExecuteMethod(i => i.PostTreatmentConfig(treatmentConfig));
        }


        public List<GoodSale> GetBarOrdersForCustomer(Guid customerId, DateTime startDate, DateTime endDate)
        {
            return ExecuteMethod(i => i.GetBarOrdersForCustomer(customerId, startDate, endDate));
        }

        public void PostCompany(Company company)
        {
            ExecuteMethod(i => i.PostCompany(company));
        }


        public Guid PostTreatmentProgram(TreatmentProgram treatmentProgram, List<Guid> lines)
        {
            return ExecuteMethod(i => i.PostTreatmentProgram(treatmentProgram, lines));
        }

        public List<TextAction> GetAllActions()
        {
            return ExecuteMethod(i => i.GetAllActions());
        }

        public Guid PostTextAction(TextAction textAction, Guid[] divisionIds)
        {
            return ExecuteMethod(i => i.PostTextAction(textAction, divisionIds));
        }


        public void DeleteParallelRule(TreatmentsParalleling rule)
        {
            ExecuteMethod(i => i.DeleteParallelRule(rule));
        }


        public List<TreatmentsParalleling> GetParallelingRules()
        {
            return ExecuteMethod(i => i.GetParallelingRules());
        }


        public void PostTreatmentsParalleling(TreatmentsParalleling old, TreatmentsParalleling rule)
        {
            ExecuteMethod(i => i.PostTreatmentsParalleling(old, rule));
        }


        public List<Guid> GetCustomerStatusesIds(Guid customerId)
        {
            return ExecuteMethod(i => i.GetCustomerStatusesIds(customerId));
        }


        public void PostCustomerStatuses(Guid customerId, List<Guid> list)
        {
            ExecuteMethod(i => i.PostCustomerStatuses(customerId, list));
        }


        public List<ReportTemplate> GetAllTemplates()
        {
            return ExecuteMethod(i => i.GetAllTemplates());
        }


        public void PostReportTemplate(ReportTemplate template)
        {
            ExecuteMethod(i => i.PostReportTemplate(template));
        }


        public void PostTreatmentBreakdown(Guid treatmentId)
        {
            ExecuteMethod(i => i.PostTreatmentBreakdown(treatmentId));
        }


        public List<ChildrenRoom> GetCustomerChildren(Guid customerId)
        {
            return ExecuteMethod(i => i.GetCustomerChildren(customerId));
        }


        public string GenerateChildRequestReport(Guid id)
        {
            return ExecuteMethod(i => i.GenerateChildRequestReport(id, "GenerateChildRequestReport"));
        }


        public void PostDivision(Division division)
        {
            ExecuteMethod(i => i.PostDivision(division));
        }

        public void UpdateDivision()
        {
            Division = new Lazy<Division>(() => ExecuteMethod(i => i.GetDivision(new Guid(AppSettingsManager.GetSetting("DivisionId")))));
        }

        public List<int> GetAvailableShelfNumbers(bool isSafe)
        {
            return ExecuteMethod(i => i.GetAvailableShelfNumbers(CurrentDivision.Id, isSafe));
        }


        public List<CustomerShelf> GetCustomerShelves(Guid customerId, bool isSafe)
        {
            return ExecuteMethod(i => i.GetCustomerShelves(customerId, isSafe));
        }

        public List<CustomerShelf> GetDivisionShelves(DateTime startPeriod, DateTime endPeriod, bool isSafe)
        {
            if (CurrentDivision == null) return new List<CustomerShelf>();
            return ExecuteMethod(i => i.GetDivisionShelves(CurrentDivision.Id, startPeriod, endPeriod, isSafe));
        }

        public List<Solarium> GetDivisionSolariums(bool activeOnly)
        {
            if (CurrentDivision == null) return new List<Solarium>();
            return ExecuteMethod(i => i.GetDivisionSolariums(CurrentDivision.Id, activeOnly));
        }

        public void PostSolarium(Solarium solarium)
        {
            //solarium.DivisionId = CurrentDivision.Id;
            ExecuteMethod(i => i.PostSolarium(solarium));
        }

        public Dictionary<int, string> GetSolariumWarnings()
        {
            return ExecuteMethod(i => i.GetSolariumWarnings());
        }


        public KeyValuePair<Guid, DateTime> GetSolariumProposal(Guid customerId, DateTime dateTime, int amount, Guid selectedSolariumId, Guid toSkip)
        {
            return ExecuteMethod(i => i.GetSolariumProposal(CurrentDivision.Id, customerId, dateTime, amount, selectedSolariumId, toSkip));
        }

        public Guid PostSolariumBooking(Guid customerId, Guid solariumId, DateTime dateTime, int amount, string comment)
        {
            dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
            return ExecuteMethod(i => i.PostSolariumBooking(CurrentDivision.Id, customerId, solariumId, dateTime, amount, comment));
        }

        public Guid PostSolariumBookingEx(Guid customerId, Guid solariumId, DateTime dateTime, int amount, string comment, Guid? ticketId)
        {
            dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
            return ExecuteMethod(i => i.PostSolariumBookingEx(CurrentDivision.Id, customerId, solariumId, dateTime, amount, comment, ticketId));
        }


        public List<SolariumVisit> GetCustomerSolarium(Guid customerId)
        {
            return ExecuteMethod(i => i.GetCustomerSolarium(customerId));
        }


        public List<SolariumVisit> GetDivisionSolariumVisits(DateTime startDate, DateTime finishDate)
        {
            if (CurrentDivision == null) return new List<SolariumVisit>();
            return ExecuteMethod(i => i.GetDivisionSolariumVisits(CurrentDivision.Id, startDate, finishDate));
        }


        public SolariumVisit GetSolariumVisitById(Guid visitId)
        {
            return ExecuteMethod(i => i.GetSolariumVisitById(visitId));
        }


        public bool CancelSolariumEvent(Guid solVisitId, bool delete)
        {
            return ExecuteMethod(i => i.CancelSolariumEvent(solVisitId, delete));
        }


        public List<Ticket> GetCustomerTickets(Guid customerId)
        {
            return ExecuteMethod(i => i.GetCustomerTickets(customerId));
        }

        public void PostSolariumVisitStart(Guid solariumVisitId)
        {
            ExecuteMethod(i => i.PostSolariumVisitStart(solariumVisitId));
        }


        public void PostSolariumWarnings(List<KeyValuePair<int, string>> solariumWarnings)
        {
            ExecuteMethod(i => i.PostSolariumWarnings(solariumWarnings));
        }

        public List<CustomerCardType> GetAllCustomerCardTypes()
        {
            return ExecuteMethod(i => i.GetAllCustomerCardTypes());
        }

        public List<TicketType> GetAllTicketTypes()
        {
            return ExecuteMethod(i => i.GetAllTicketTypes());
        }


        public void PostCompanyCardTypeEnable(Guid ccardId, bool enable)
        {
            ExecuteMethod(i => i.PostCompanyCardTypeEnable(ccardId, enable));
        }


        public void PostCompanyTicketTypeEnable(Guid tTypeId, bool enable)
        {
            ExecuteMethod(i => i.PostCompanyTicketTypeEnable(tTypeId, enable));
        }


        public string GenerateRkoForTicketReturn(Guid id)
        {
            return ExecuteMethod(i => i.GenerateTicketContractReport(id, Guid.Empty, "GenerateRKOForTicketReturn"));
        }

        public string GenerateRkoForGoodReturn(Guid id)
        {
            return ExecuteMethod(i => i.GenerateGoodReport(id, "GenerateRKOForGoodReturn"));
        }

        public List<ProviderFolder> GetProviderFolders()
        {
            return ExecuteMethod(i => i.GetProviderFolders());
        }


        public void PostProviderFolder(ProviderFolder providerFolder)
        {
            providerFolder.CompanyId = CurrentCompany.CompanyId;
            ExecuteMethod(i => i.PostProviderFolder(providerFolder));
        }


        public void DeleteProviderFolder(Guid folderId)
        {
            ExecuteMethod(i => i.DeleteProviderFolder(folderId));
        }


        public void HideProvider(Guid providerId)
        {
            ExecuteMethod(i => i.HideProvider(providerId));
        }


        public void HideGood(Guid goodId)
        {
            ExecuteMethod(i => i.HideGood(goodId));
        }


        public List<Storehouse> GetStorehouses()
        {
            if (CurrentDivision == null) return new List<Storehouse>();
            return ExecuteMethod(i => i.GetStorehouses(CurrentDivision.Id));
        }


        public void PostStorehouse(Storehouse storehouse)
        {
            ExecuteMethod(i => i.PostStorehouse(storehouse));
        }

        public string GenerateConsignmentReport(Guid id)
        {
            return ExecuteMethod(i => i.GenerateConsignmentReport(id));
        }

        public List<ChildrenRoom> GetDivisionChildren(DateTime start, DateTime end)
        {
            if (CurrentDivision == null) return new List<ChildrenRoom>();
            return ExecuteMethod(i => i.GetDivisionChildren(CurrentDivision.Id, start, end));
        }


        public List<GoodSale> GetDivisionSales(DateTime start, DateTime end)
        {
            if (CurrentDivision == null) return new List<GoodSale>();
            return ExecuteMethod(i => i.GetDivisionSales(CurrentDivision.Id, start, end));
        }

        public List<GoodSale> GetCustomerSales(Guid customerId, DateTime start, DateTime end)
        {
            return ExecuteMethod(i => i.GetCustomerSales(customerId, start, end));
        }


        public string GenerateGoodReturnStatementReport(Guid id)
        {
            return ExecuteMethod(i => i.GenerateGoodReport(id, "GenerateGoodReturnStatementReport"));
        }


        public List<Certificate> GetDivisionCertificates()
        {
            if (CurrentDivision == null) return new List<Certificate>();
            return ExecuteMethod(i => i.GetDivisionCertificates(CurrentDivision.Id));
        }

        public void PostCertificate(Certificate certificate)
        {
            ExecuteMethod(i => i.PostCertificate(certificate));
        }

        public bool CancelCertificate(Guid certificateId)
        {
            return ExecuteMethod(i => i.CancelCertificate(certificateId));
        }

        public List<Rent> GetCustomerRent(Guid customerId)
        {
            return ExecuteMethod(i => i.GetCustomerRent(customerId));
        }

        public void PostRent(Rent rent)
        {
            ExecuteMethod(i => i.PostRent(rent));
        }


        public string GenerateCashMemoReport(int orderNumber)
        {
            return ExecuteMethod(i => i.GenerateBarOrderReport(orderNumber, "GenerateCashMemoReport"));
        }

        public Certificate GetCertificateByNumber(int id)
        {
            return ExecuteMethod(i => i.GetCertificateByNumber(id));
        }


        public List<BarOrder> GetDivisionBarOrders(DateTime start, DateTime end)
        {
            if (CurrentDivision == null) return new List<BarOrder>();
            return ExecuteMethod(i => i.GetDivisionBarOrders(CurrentDivision.Id, start, end));
        }


        public List<DepositAccount> GetCustomerDeposit(Guid customerId, DateTime start, DateTime end)
        {
            return ExecuteMethod(i => i.GetCustomerDeposit(customerId, start, end));
        }


        public void PostDepositAdd(Guid customerId, decimal amount, string description)
        {
            ExecuteMethod(i => i.PostDepositAdd(customerId, amount, description));
        }


        public string GenerateDepositOutStatementReport(Guid id)
        {
            return ExecuteMethod(i => i.GenerateDepositOutStatementReport(id));
        }

        public Guid RequestDepositOut(Guid customerId, decimal amount)
        {
            return ExecuteMethod(i => i.RequestDepositOut(customerId, amount));
        }

        public void PostDepositOutDone(Guid depositOutId, string comment, bool isDone)
        {
            ExecuteMethod(i => i.PostDepositOutDone(depositOutId, comment, isDone));
        }


        public void FinalizeCashlessPayment(Guid orderId, string comments, bool isSuccessful)
        {
            ExecuteMethod(i => i.FinalizeCashlessPayment(orderId, comments, isSuccessful));
        }


        public string GenerateOrderBillReport(int n)
        {
            return ExecuteMethod(i => i.GenerateBarOrderReport(n, "GenerateOrderBillReport"));
        }

        public string GenerateOrderContractReport(int n)
        {
            return ExecuteMethod(i => i.GenerateBarOrderReport(n, "GenerateOrderContractReport"));
        }

        public void ProcessBankReturn(Guid orderId)
        {
            ExecuteMethod(i => i.ProcessBankReturn(orderId));
        }

        public List<Spending> GetDivisionSpendings(DateTime start, DateTime end)
        {
            if (CurrentDivision == null) return new List<Spending>();
            return ExecuteMethod(i => i.GetDivisionSpendings(CurrentDivision.Id, start, end));
        }

        public List<SpendingType> GetDivisionSpendingTypes()
        {
            if (CurrentDivision == null) return new List<SpendingType>();
            return ExecuteMethod(i => i.GetDivisionSpendingTypes(CurrentDivision.Id));
        }

        public void PostSpending(Spending spending)
        {
            ExecuteMethod(i => i.PostSpending(spending));
        }

        public void PostSpendingType(Guid typeId, string name)
        {
            ExecuteMethod(i => i.PostSpendingType(CurrentDivision.Id, typeId, name));
        }

        public string GeneratePriceListReport()
        {
            return ExecuteMethod(i => i.GeneratePriceListReport(CurrentDivision.Id));
        }

        public List<BonusAccount> GetCustomerBonus(Guid customerId, DateTime start, DateTime end)
        {
            return ExecuteMethod(i => i.GetCustomerBonus(customerId, start, end));
        }

        public List<EmployeeCategory> GetEmployeeCategories()
        {
            return ExecuteMethod(i => i.GetEmployeeCategories(CurrentCompany.CompanyId));
        }

        public IEnumerable<Job> GetJobs()
        {
            if (CurrentDivision == null) return new List<Job>();
            return ExecuteMethod(i => i.GetJobs(CurrentDivision.Id));
        }

        public void PostEmployeeCategory(EmployeeCategory category, List<Guid> jobIds)
        {
            ExecuteMethod(i => i.PostEmployeeCategory(category, jobIds));
        }

        public List<string> GetJobUnits()
        {
            if (CurrentDivision == null) return new List<string>();
            return ExecuteMethod(i => i.GetJobUnits(CurrentDivision.Id));
        }

        public void PostJob(Job job, List<Guid> categoryIds)
        {
            ExecuteMethod(i => i.PostJob(job, categoryIds));
        }

        public string GetBaselineStatus()
        {
            if (CurrentDivision == null) return null;
            return ExecuteMethod(i => i.GetBaselineStatus(CurrentDivision.Id));
        }

        public void BaselineJobs()
        {
            ExecuteMethod(i => i.BaselineJobs(CurrentDivision.Id));
        }

        public List<Employee> GetEmployees(bool presentOnly, bool asuOnly)
        {
            if (CurrentDivision == null) return new List<Employee>();
            return ExecuteMethod(i => i.GetEmployees(CurrentDivision.Id, presentOnly, asuOnly));
        }

        public Guid PostEmployeeForm(Employee employee, Customer customer)
        {
            return ExecuteMethod(i => i.PostEmployeeForm(employee, customer));
        }

        public List<Job> GetAvailableJobs()
        {
            if (CurrentDivision == null) return new List<Job>();
            return ExecuteMethod(i => i.GetAvailableJobs(CurrentDivision.Id));
        }


        public Guid PostJobPlacement(JobPlacement placement)
        {
            return ExecuteMethod(i => i.PostJobPlacement(placement));
        }

        public JobPlacement GetJobPlacementDraft(Guid employeeId)
        {
            return ExecuteMethod(i => i.GetJobPlacementDraft(employeeId));
        }

        public Employee GetEmployeeById(Guid id)
        {
            return ExecuteMethod(i => i.GetEmployeeById(id));
        }


        public string GenerateApplyOrder(Guid id)
        {
            return ExecuteMethod(i => i.GenerateEmployeeReport(id, "GenerateApplyOrder"));
        }

        public string GenerateJobAgreement(Guid id)
        {
            return ExecuteMethod(i => i.GenerateEmployeeReport(id, "GenerateJobAgreement"));
        }

        public string GenerateJobStudyAgreement(Guid id)
        {
            return ExecuteMethod(i => i.GenerateEmployeeReport(id, "GenerateJobStudyAgreement"));
        }

        public string GenerateResponsibleAgreement(Guid id)
        {
            return ExecuteMethod(i => i.GenerateEmployeeReport(id, "GenerateResponsibleAgreement"));
        }

        public string GenerateSecretAgreement(Guid id)
        {
            return ExecuteMethod(i => i.GenerateEmployeeReport(id, "GenerateSecretAgreement"));
        }

        public string GenerateJobDescription(Guid id)
        {
            return ExecuteMethod(i => i.GenerateEmployeeReport(id, "GenerateJobDescription"));
        }

        public string GenerateEmployeeVacationOrder(Guid id)
        {
            return ExecuteMethod(i => i.GenerateEmployeeReport(id, "GenerateEmployeeVacationOrder"));
        }

        public string GetEmployeeCardStatusMessage(string cardNumber)
        {
            return ExecuteMethod(i => i.GetEmployeeCardStatusMessage(cardNumber));
        }

        public void PostEmployeeCard(Guid employeeId, string newCardNumber)
        {
            ExecuteMethod(i => i.PostEmployeeCard(employeeId, newCardNumber));
        }

        public Guid PostJobPlacementChange(JobPlacement jobPlacement)
        {
            return ExecuteMethod(i => i.PostJobPlacementChange(jobPlacement));
        }

        public string GenerateJobChangeOrder(Guid parameter)
        {
            return ExecuteMethod(i => i.GenerateEmployeeReport(parameter, "GenerateJobChangeOrder"));
        }

        public string GenerateEmployeeFireOrder(Guid parameter)
        {
            return ExecuteMethod(i => i.GenerateEmployeeReport(parameter, "GenerateEmployeeFireOrder"));
        }

        public Guid PostEmployeeFire(Guid employeeId, DateTime fireDate, string fireCause, bool cardReturned, decimal totalToPay, decimal bonus, decimal ndfl, string logMessage)
        {
            return ExecuteMethod(i => i.PostEmployeeFire(employeeId, fireDate, fireCause, cardReturned, totalToPay, bonus, ndfl, logMessage));
        }

        public Guid PostEmployeeVacation(Guid employeeId, DateTime beginDate, DateTime endDate, byte vacType, decimal totalToPay, decimal ndfl, string logMessage)
        {
            return ExecuteMethod(i => i.PostEmployeeVacation(employeeId, beginDate, endDate, vacType, totalToPay, ndfl, logMessage));
        }

        public Guid[] PostEmployeeTrip(Guid[] employeeIds, DateTime beginDate, DateTime endDate, string destination, string target, string tripBase)
        {
            return ExecuteMethod(i => i.PostEmployeeTrip(employeeIds, beginDate, endDate, destination, target, tripBase));
        }

        public string GenerateEmployeeTripOrder(Guid id)
        {
            return ExecuteMethod(i => i.GenerateEmployeeReport(id, "GenerateEmployeeTripOrder"));
        }

        public void PostLastTicketCorrection(Guid customerId, DateTime corrActivated, int corrAmount, int corrGuest, decimal paidAmt, int solCorr)
        {
            ExecuteMethod(i => i.PostLastTicketCorrection(customerId, corrActivated, corrAmount, corrGuest, paidAmt, solCorr));
        }

        public string GenerateEmployeeCategoryOrder(Guid id)
        {
            return ExecuteMethod(i => i.GenerateEmployeeReport(id, "GenerateEmployeeCategoryOrder"));
        }

        public Guid PostEmployeeCategoryChange(Guid employeeId, Guid categoryId)
        {
            return ExecuteMethod(i => i.PostEmployeeCategoryChange(employeeId, categoryId));
        }

        public List<EmployeeDocument> GetEmployeeDocuments()
        {
            if (CurrentDivision == null) return new List<EmployeeDocument>();
            return ExecuteMethod(i => i.GetEmployeeDocuments(CurrentDivision.Id));
        }

        public int GetActiveEmployeesCountForJobId(Guid jobId)
        {
            return ExecuteMethod(i => i.GetActiveEmployeesCountForJobId(jobId));
        }

        public int GetActiveEmployeesCountForCategoryId(Guid categoryId)
        {
            return ExecuteMethod(i => i.GetActiveEmployeesCountForCategoryId(categoryId));
        }

        public void HideEmployeeCategoryById(Guid categoryId)
        {
            ExecuteMethod(i => i.HideEmployeeCategoryById(categoryId));
        }

        public void HideEmployeeJobById(Guid jobId)
        {
            ExecuteMethod(i => i.HideEmployeeJobById(jobId));
        }

        public string GenerateStateScheduleReport()
        {
            return ExecuteMethod(i => i.GenerateStateScheduleReport(CurrentDivision.Id));
        }

        public Employee GetEmployeeByCard(string cardNumber)
        {
            return ExecuteMethod(i => i.GetEmployeeByCard(cardNumber));
        }

        public void PostEmployeeVisit(Guid employeeId, bool isIn)
        {
            ExecuteMethod(i => i.PostEmployeeVisit(employeeId, isIn));
        }

        public List<VacationList> GetVacationHistory()
        {
            if (CurrentDivision == null) return new List<VacationList>();
            return ExecuteMethod(i => i.GetVacationHistory(CurrentDivision.Id));
        }

        public List<VacationPreference> GetEmployeePreferences(Guid employeeId)
        {
            return ExecuteMethod(i => i.GetEmployeePreferences(employeeId));
        }

        public void PostEmployeePreference(Guid employeeId, DateTime beginDate, DateTime endDate, short prefType)
        {
            ExecuteMethod(i => i.PostEmployeePreference(employeeId, beginDate, endDate, prefType));
        }

        public List<EmployeeScheduleProposalElement> GenerateScheduleProposal(int year, int recDays)
        {
            return ExecuteMethod(i => i.GenerateScheduleProposal(CurrentDivision.Id, year, recDays));
        }

        public Guid PostEmployeeVacationsSchedule(List<EmployeeScheduleProposalElement> list)
        {
            return ExecuteMethod(i => i.PostEmployeeVacationsSchedule(list));
        }

        public List<EmployeeScheduleProposalElement> GetCurrentEmployeeVacationsSchedule()
        {
            if (CurrentDivision == null) return new List<EmployeeScheduleProposalElement>();
            return ExecuteMethod(i => i.GetCurrentEmployeeVacationsSchedule(CurrentDivision.Id));
        }

        public List<EmployeeScheduleProposalElement> GetEmployeeVacationsSchedule(Guid listId)
        {
            return ExecuteMethod(i => i.GetEmployeeVacationsSchedule(listId));
        }

        public string GenerateEmployeeVacationList(Guid id)
        {
            return ExecuteMethod(i => i.GenerateEmployeeVacationList(id));
        }

        public List<EmployeeWorkScheduleItem> GetEmployeeWorkSchedule(DateTime start, DateTime finish)
        {
            if (CurrentDivision == null) return new List<EmployeeWorkScheduleItem>();
            return ExecuteMethod(i => i.GetEmployeeWorkSchedule(CurrentDivision.Id, start, finish));
        }

        public Guid PostEmployeeSchedule(Dictionary<Guid, List<DateTime>> schedule)
        {
            return ExecuteMethod(i => i.PostEmployeeSchedule(CurrentDivision.Id, schedule));
        }

        public string GenerateEmployeeScheduleReport(Guid id)
        {
            return ExecuteMethod(i => i.GenerateEmployeeScheduleReport(id));
        }

        public List<EmployeeWorkGraph> GetWorkGraphs()
        {
            if (CurrentDivision == null) return new List<EmployeeWorkGraph>();
            return ExecuteMethod(i => i.GetWorkGraphs(CurrentDivision.Id));
        }


        public List<DateTime> GetHolidays(int year)
        {
            return ExecuteMethod(i => i.GetHolidays(year));
        }

        public void PostHoliday(DateTime date)
        {
            ExecuteMethod(i => i.PostHoliday(date));
        }

        public void DeleteHoliday(DateTime date)
        {
            ExecuteMethod(i => i.DeleteHoliday(date));
        }

        public string GetNotificationsForUser()
        {
            return ExecuteMethod(i => i.GetNotificationsForUser());
        }

        public List<SalaryScheme> GetSalarySchemes()
        {
            if (CurrentDivision == null) return new List<SalaryScheme>();
            return ExecuteMethod(i => i.GetSalarySchemes(CurrentDivision.Id));
        }

        public void PostSalaryScheme(SalaryScheme salaryScheme)
        {
            ExecuteMethod(i => i.PostSalaryScheme(CurrentDivision.Id, salaryScheme));
        }

        public decimal CalculateSalary(Guid employeeId, DateTime calcMonth)
        {
            return ExecuteMethod(i => i.CalculateSalary(employeeId, calcMonth));
        }

        public List<SalarySheet> GetSalarySheets()
        {
            if (CurrentDivision == null) return new List<SalarySheet>();
            return ExecuteMethod(i => i.GetSalarySheets(CurrentDivision.Id));
        }

        public List<SalarySheetRow> GenerateSalarySheet(DateTime genMonth)
        {
            return ExecuteMethod(i => i.GenerateSalarySheet(CurrentDivision.Id, genMonth));
        }

        public string PostSalarySheet(DateTime period, List<SalarySheetRow> lines)
        {
            return ExecuteMethod(i => i.PostSalarySheet(CurrentDivision.Id, period, lines));
        }

        public List<SalarySheetRow> GetSalarySheetLines(Guid sheetId)
        {
            return ExecuteMethod(i => i.GetSalarySheetLines(sheetId));
        }

        public string PostEmployeePayment(Guid employeeId, Guid sheetId, decimal amount)
        {
            return ExecuteMethod(i => i.PostEmployeePayment(employeeId, sheetId, amount));
        }

        public bool CheckSalarySheet(DateTime genDate)
        {
            return ExecuteMethod(i => i.CheckSalarySheet(CurrentDivision.Id, genDate));
        }

        public KeyValuePair<decimal, string> GetFireSalary(Guid employeeId, DateTime fireDate)
        {
            return ExecuteMethod(i => i.GetFireSalary(employeeId, fireDate));
        }

        public KeyValuePair<decimal, string> GetEmployeeVacationPmt(Guid employeeId, DateTime beginDate, DateTime endDate)
        {
            return ExecuteMethod(i => i.GetEmployeeVacationPmt(employeeId, beginDate, endDate));
        }

        public KeyValuePair<decimal, string> GetEmployeeIllnessPmt(Guid employeeId, DateTime beginDate, DateTime endDate)
        {
            return ExecuteMethod(i => i.GetEmployeeIllnessPmt(employeeId, beginDate, endDate));
        }

        public List<Corporate> GetCorporates()
        {
            return ExecuteMethod(i => i.GetCorporates(CurrentCompany.CompanyId));
        }

        public bool PostCorporate(Guid corpId, string name, Guid? folderId)
        {
            return ExecuteMethod(i => i.PostCorporate(CurrentCompany.CompanyId, corpId, name, folderId));
        }

        public bool DeleteCorporate(Guid corpId)
        {
            return ExecuteMethod(i => i.DeleteCorporate(corpId));
        }

        public List<TreatmentEvent> GetDivisionTreatmetnsVisits(DateTime start, DateTime finish)
        {
            if (CurrentDivision == null) return new List<TreatmentEvent>();
            return ExecuteMethod(i => i.GetDivisionTreatmetnsVisits(CurrentDivision.Id, start, finish));
        }

        public List<ScheduleProposalElement> FixSchedule(Guid customerId, Guid ticketId, List<ScheduleProposalElement> list)
        {
            return ExecuteMethod(i => i.FixSchedule(CurrentDivision.Id, customerId, ticketId, list));
        }

        public List<EmployeePayment> GetDivisionEmployeeCashflow()
        {
            if (CurrentDivision == null) return new List<EmployeePayment>();
            return ExecuteMethod(i => i.GetDivisionEmployeeCashflow(CurrentDivision.Id));
        }

        public TreatmentEvent GetTreatmentEventById(Guid treatmentEventId)
        {
            return ExecuteMethod(i => i.GetTreatmentEventById(treatmentEventId));
        }

        public void PostTreatmentEventChange(Guid treatmentEventId, DateTime newTime)
        {
            ExecuteMethod(i => i.PostTreatmentEventChange(treatmentEventId, newTime));
        }

        public void PostFilePart(Guid fileId, string fileName, byte[] part, int bytes, int? category, Guid parameter)
        {
            ExecuteMethod(i => i.PostFilePart(CurrentDivision.Id, fileId, fileName, part, bytes, category, parameter));
        }

        public List<ServiceModel.File> GetDivisionFiles()
        {
            if (CurrentDivision == null) return new List<ServiceModel.File>();
            return ExecuteMethod(i => i.GetDivisionFiles(CurrentDivision.Id));
        }

        public void UploadFile(int fileType, string fileName, Guid parameter)
        {
            FileUploader.UploadFile(this, fileName, fileType, parameter);
        }

        public byte[] GetFilePart(Guid fileId, int blockNumber)
        {
            return ExecuteMethod(i => i.GetFilePart(fileId, blockNumber));
        }

        public string DownloadFile(ServiceModel.File file)
        {
            return FileUploader.DownloadFile(this, file);
        }

        public List<SalesPlan> GetSalesPlan()
        {
            if (CurrentDivision == null) return new List<SalesPlan>();
            return ExecuteMethod(i => i.GetSalesPlanForDivsion(CurrentDivision.Id));
        }

        public void PostSalaryPlan(DateTime month, decimal amount, decimal amountCorp, DateTime oldMonth)
        {
            ExecuteMethod(i => i.PostSalesPlan(CurrentDivision.Id, month, amount, amountCorp, oldMonth));
        }

        public List<IncomingCallForm> GetCallScrenarioForms()
        {
            return ExecuteMethod(i => i.GetCallScrenarioForms());
        }

        public void PostIncomingCallForm(IncomingCallForm incomingCallForm)
        {
            ExecuteMethod(i => i.PostIncomingCallForm(incomingCallForm));
        }

        public void PostNewCall(string log, CallResult callResult, Guid? customerId, bool isIncoming, DateTime started, string goal, string result)
        {
            ExecuteMethod(i => i.PostNewCall(CurrentDivision.Id, log, callResult, customerId, isIncoming, started, goal, result));
        }

        public List<Call> GetDivisionCalls(DateTime callsStart, DateTime callsEnd)
        {
            if (CurrentDivision == null) return new List<Call>();
            return ExecuteMethod(i => i.GetDivisionCalls(CurrentDivision.Id, callsStart, callsEnd));
        }

        public void PostClubClosing(DateTime start, DateTime end, string cause)
        {
            ExecuteMethod(i => i.PostClubClosing(CurrentDivision.Id, start, end, cause));
        }

        public void PostNotificationCompletion(Guid notificationId, string comment, string result)
        {
            ExecuteMethod(i => i.PostNotificationCompletion(CurrentDivision.Id, notificationId, comment, result));
        }

        public List<Customer> GetAllCustomers()
        {
            return ExecuteMethod(i => i.GetAllCustomers());
        }

        public List<Customer> GetPotentialCustomers()
        {
            return ExecuteMethod(i => i.GetPotentialCustomers());
        }

        public List<Customer> GetInactiveCustomers()
        {
            return ExecuteMethod(i => i.GetInactiveCustomers());
        }

        public List<Customer> GetActiveCustomers()
        {
            return ExecuteMethod(i => i.GetActiveCustomers());
        }

        public List<Customer> GetCustomersByStatus(List<Guid> statIds)
        {
            return ExecuteMethod(i => i.GetCustomersByStatus(statIds));
        }

        public List<Customer> GetCustomersByManagers(List<Guid> managerIds)
        {
            return ExecuteMethod(i => i.GetCustomersByManagers(managerIds));
        }

        public List<Employee> GetEmployeesWorkingAt(DateTime date)
        {
            return ExecuteMethod(i => i.GetEmployeesWorkingAt(CurrentDivision.Id, date));
        }

        public void PostGroupCall(Guid[] customers, Guid[] employees, string comments, DateTime runDate, DateTime expityDate)
        {
            ExecuteMethod(i => i.PostGroupCall(CurrentDivision.Id, customers, employees, comments, runDate, expityDate));
        }

        public void PostReturnReject(Guid ticketId)
        {
            ExecuteMethod(i => i.PostReturnReject(ticketId));
        }

        public void PostIncorrectPhoneTask(Guid cnId, Guid customerId, string comments)
        {
            ExecuteMethod(i => i.PostIncorrectPhoneTask(CurrentDivision.Id, cnId, customerId, comments));
        }

        public void PostTaskClosing(Guid taskId, bool isCompleted, string comment, DateTime date)
        {
            ExecuteMethod(i => i.PostTaskClosing(taskId, isCompleted, comment, date));
        }

        public void PostNewTask(Guid[] employees, string subject, string comments, DateTime expiryDate, int priority)
        {
            ExecuteMethod(i => i.PostNewTask(employees, subject, comments, expiryDate, priority));
        }

        public List<Guid> GetEmployeeIdsWithPermission(string permissionName)
        {
            if (CurrentDivision == null) return new List<Guid>();
            return ExecuteMethod(i => i.GetEmployeeIdsWithPermission(CurrentDivision.Id, permissionName));
        }

        public List<OrganizerItem> GetOutboxOrganizerItems()
        {
            return ExecuteMethod(i => i.GetOutboxOrganizerItems());
        }

        public void PostTaskRecall(Guid elementId)
        {
            ExecuteMethod(i => i.PostTaskRecall(elementId));
        }

        public List<OrganizerItem> GetArchivedOrganizerItems()
        {
            return ExecuteMethod(i => i.GetArchivedOrganizerItems());
        }

        public void PostTaskReopen(Guid elementId)
        {
            ExecuteMethod(i => i.PostTaskReopen(elementId));
        }

        public List<Call> GetCustomerCalls(Guid customerId)
        {
            return ExecuteMethod(i => i.GetCustomerCalls(customerId));
        }

        public List<Division> GetDivisions()
        {
            return ExecuteMethod(i => i.GetDivisions());
        }

        public List<Role> GetRoles()
        {
            return ExecuteMethod(i => i.GetRoles());
        }

        public List<Permission> GetAllPermissions()
        {
            var res = ExecuteMethod(i => i.GetAllPermissions());
            foreach (var r in res)
            {
                if (r.ParentPermissionId.HasValue) r.Parent = res.SingleOrDefault(i => i.PermissionId == r.ParentPermissionId.Value);
                r.Permissions = res.Where(i => i.ParentPermissionId == r.PermissionId).OrderBy(i => i.PermissionName).ToList();
            }
            return res;
        }

        public void PostRole(Guid roleId, string name, Guid[] permissionIds, string cardDisc, string ticketDisc, string ticketRubDisc, Guid? folderId)
        {
            ExecuteMethod(i => i.PostRole(roleId, name, permissionIds, cardDisc, ticketDisc, ticketRubDisc, folderId));
        }

        public void DeleteRole(Guid roleId)
        {
            ExecuteMethod(i => i.DeleteRole(roleId));
        }

        public List<User> GetUsers()
        {
            return ExecuteMethod(i => i.GetUsers());
        }

        public Guid PostNewUser(Guid employeeId, string userName, string fullName, string password, string email)
        {
            return ExecuteMethod(i => i.PostNewUser(employeeId, userName, fullName, password, email));
        }

        public void PostUser(Guid userId, string fullName, bool isActive, Guid[] roleIds, string email)
        {
            ExecuteMethod(i => i.PostUser(userId, fullName, isActive, roleIds, email));
        }

        public void ResetPassword(Guid userId, string password)
        {
            ExecuteMethod(i => i.ResetPassword(userId, password));
        }

        public string ChangePassword(string oldPassword, string newPassword)
        {
            return ExecuteMethod(i => i.ChangePassword(CurrentUser.UserId, oldPassword, newPassword));
        }

        public bool NeedChangePassword { get; private set; }

        public void UpdateCompany()
        {
            Company = new Lazy<Company>(() => ExecuteMethod(i => i.GetCompnay()));
        }

        public void PostInstalmentDelete(Guid instalmentId)
        {
            ExecuteMethod(i => i.PostInstalmentDelete(instalmentId));
        }

        public void PostInstalment(Instalment instalment)
        {
            ExecuteMethod(i => i.PostInstalment(instalment));
        }

        public IEnumerable<ReportInfoInt> GetUserReportsList()
        {
            return ExecuteMethod(i => i.GetUserReportsList());
        }

        public DataTable GenerateReport(string key, IEnumerable<ReportParamInt> parameters)
        {
            var bformatter = new BinaryFormatter();
            var stream = new System.IO.Compression.GZipStream(
                new MemoryStream(
                    ExecuteMethod(ii => ii.GenerateReport(key, parameters.ToDictionary(i => i.InternalName, i => i.InstanceValue)))),
                    System.IO.Compression.CompressionMode.Decompress);
            var res = (DataTable)bformatter.Deserialize(stream);
            stream.Close();
            return res;
        }


        public List<CompanyFinance> GetCompanyFinances()
        {
            return ExecuteMethod(i => i.GetCompanyFinances(CurrentCompany.CompanyId));
        }

        public void PostCompanyFinance(DateTime period, decimal accLeft)
        {
            ExecuteMethod(i => i.PostCompanyFinance(CurrentCompany.CompanyId, period, accLeft));
        }

        public List<DivisionFinance> GetDivisionFinances()
        {
            if (CurrentDivision == null) return new List<DivisionFinance>();
            return ExecuteMethod(i => i.GetDivisionFinances(CurrentDivision.Id));
        }

        public void PostDivisionFinance(DateTime dateTime, decimal cash, decimal unsent, decimal advances, decimal loan, decimal accum)
        {
            ExecuteMethod(i => i.PostDivisionFinance(CurrentDivision.Id, dateTime, cash, unsent, advances, loan, accum));
        }

        public List<IncomeType> GetDivisionIncomeTypes()
        {
            if (CurrentDivision == null) return new List<IncomeType>();
            return ExecuteMethod(i => i.GetDivisionIncomeTypes(CurrentDivision.Id));
        }

        public void PostIncomeType(Guid id, string name)
        {
            ExecuteMethod(i => i.PostIncomeType(CurrentDivision.Id, id, name));
        }


        public List<Income> GetDivisionIncomes(DateTime start, DateTime end)
        {
            if (CurrentDivision == null) return new List<Income>();
            return ExecuteMethod(i => i.GetDivisionIncomes(CurrentDivision.Id, start, end));
        }

        public void PostIncome(Income income)
        {
            ExecuteMethod(i => i.PostIncome(income));
        }

        public void DoSync()
        {
            ExecuteMethod(i => i.DoSync());
        }


        public void DeleteTreatmentProgram(Guid programId)
        {
            ExecuteMethod(i => i.DeleteTreatmentProgram(programId));
        }

        public LocalSetting GetLocalSettings()
        {
            try
            {
                return ExecuteMethod(i => i.GetLocalSettings());
            }
            catch (Exception ex)
            {
                ExtraWindow.Alert(Localization.Resources.RegionalError, ex.Message);
                return null;
            }
        }

        public void PostLocalSettings(int keyDays, int keyPeriod, int licDays, int licPeriod, string notifyAddresses)
        {
            ExecuteMethod(i => i.PostLocalSettings(keyDays, keyPeriod, licDays, licPeriod, notifyAddresses));
        }

        public void UpdateLicenseKey()
        {
            try
            {
                ExecuteMethod(i => i.UpdateLicenseKey());
            }
            catch (Exception ex)
            {
                ExtraWindow.Alert(Localization.Resources.KeyUpdateError, ex.Message);
            }
        }

        public Dictionary<Guid, string> GetCustomerTargetTypes()
        {
            return ExecuteMethod(i => i.GetCustomerTargetTypes());
        }


        public void PostUserReport(ReportInfoInt report)
        {
            ExecuteMethod(i => i.PostUserReport(report));
        }

        public ReportInfoInt GetReportForEdit(Guid id)
        {
            var res = ExecuteMethod(i => i.GetReportForEdit(id));

            return new ReportInfoInt
            {
                BaseTypeName = res.BaseTypeName,
                Key = res.Id.ToString(),
                Name = res.Name,
                ReportComments = res.Comments,
                Type = ReportType.Configured,
                XmlClause = res.XmlClause,
                CustomFields = res.CustomFields.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                RoleIds = res.SerializedRoleIds
            };
        }

        public List<SettingsFolder> GetSettingsFolders(int categoryId, bool companyOnly)
        {
            return ExecuteMethod(i => i.GetSettingsFolders(categoryId, companyOnly));
        }

        public void PostSettingsFolder(SettingsFolder folder, Guid[] companies)
        {
            ExecuteMethod(i => i.PostSettingsFolder(folder, companies));
        }

        public void DeleteSettingsFolder(Guid folderId)
        {
            ExecuteMethod(i => i.DeleteSettingsFolder(folderId));
        }

        public void DeleteContraIndication(Guid contraId)
        {
            ExecuteMethod(i => i.DeleteContraIndication(contraId));
        }

        public List<CompanySettingsFolder> GetCompanySettingsFolders(int categoryId)
        {
            if (CurrentDivision == null) return new List<CompanySettingsFolder>();
            return ExecuteMethod(i => i.GetCompanySettingsFolders(categoryId, CurrentDivision.Id));
        }

        public void PostCompanySettingsFolder(CompanySettingsFolder folder)
        {
            ExecuteMethod(i => i.PostCompanySettingsFolder(folder, CurrentDivision.Id));
        }

        public void SetTreatmentEventColor(Guid eventId, int colorId)
        {
            ExecuteMethod(i => i.SetTreatmentEventColor(eventId, colorId));
        }

        public List<CompanyView> GetCompaniesList(Guid folderId)
        {
            return ExecuteMethod(i => i.GetCompaniesListForFolder(folderId));
        }

        public void PostPermissions(string[] auths)
        {
            ExecuteMethod(i => i.PostPermissions(auths));
        }

        public List<Company> GetCompanies()
        {
            return ExecuteMethod(i => i.GetCompanies());
        }

        public void PostNewCompany(string name, string login, string password, Guid roleId, string reportEmail, int utcCorr, string userPrefix)
        {
            ExecuteMethod(i => i.PostNewCompany(name, login, password, roleId, reportEmail, utcCorr, userPrefix));
        }

        public void PostNewDivision(Division division)
        {
            ExecuteMethod(i => i.PostNewDivision(division));
        }

        public void CreateDivision(Action closed)
        {
            var wnd = new NewDivisionWizard(this);
            wnd.ShowDialog();
            if (wnd.DialogResult ?? false) closed();
        }

        public List<Treatment> GetAllTreatments(Guid guid)
        {
            return ExecuteMethod(i => i.GetAllTreatments(guid));
        }

        public List<Solarium> GetDivisionSolariums(Guid guid)
        {
            return ExecuteMethod(i => i.GetDivisionSolariums(guid, false));
        }

        public List<Storehouse> GetStorehouses(Guid guid)
        {
            return ExecuteMethod(i => i.GetStorehouses(guid));
        }

        public Guid GetCustomerIdByPhone(string phone)
        {
            return ExecuteMethod(i => i.GetCustomerIdByPhone(phone));
        }

        public Dictionary<Guid, string> GetAllStatuses()
        {
            return ExecuteMethod(i => i.GetAllStatuses());
        }

        public Guid GetCustomerIdByTargetId(Guid targetId)
        {
            return ExecuteMethod(i => i.GetCustomerIdByTargetId(targetId));
        }

        public Guid GetCustomerIdByTreatmentEventId(Guid treatmentEventId)
        {
            return ExecuteMethod(i => i.GetCustomerIdByTreatmentEventId(treatmentEventId));
        }

        public Guid GetCustomerIdByTicketId(Guid guid)
        {
            return ExecuteMethod(i => i.GetCustomerIdByTicketId(guid));
        }

        public Guid GetCustomerByCardId(Guid guid)
        {
            return ExecuteMethod(i => i.GetCustomerByCardId(guid));
        }

        public Guid GetCustomerByGoodSale(Guid guid)
        {
            return ExecuteMethod(i => i.GetCustomerByGoodSale(guid));
        }

        public void PostParameters(string reportKey, Dictionary<string, object> parameters, string setName)
        {
            ExecuteMethod(i => i.PostParameters(reportKey, parameters, setName));
        }

        public void DeleteReport(Guid savedId, string key, ReportType reportType)
        {
            ExecuteMethod(i => i.DeleteReport(savedId, key, reportType));
        }

        public void PostMarkdown(Guid storeId, Guid goodId, string newName, decimal price, int amount, Guid provId)
        {
            ExecuteMethod(i => i.PostMarkdown(CurrentDivision.Id, storeId, goodId, newName, price, amount, provId));
        }

        public List<CustomerVisit> GetCustomerVisits(Guid customerId)
        {
            return ExecuteMethod(i => i.GetCustomerVisits(customerId));
        }

        public List<string>[] GetWorkData()
        {
            return ExecuteMethod(i => i.GetWorkData());
        }

        public void PostTicketTypeLimits(Guid ttId, KeyValuePair<Guid, int>[] lims)
        {
            ExecuteMethod(i => i.PostTicketTypeLimits(ttId, lims));
        }

        public IEnumerable<Instalment> GetAllInstalments()
        {
            return ExecuteMethod(i => i.GetAllInstalments());
        }

        public IEnumerable<Instalment> GetCompanyInstalments(bool activeOnly)
        {
            return ExecuteMethod(i => i.GetCompanyInstalments(activeOnly));
        }

        public void PostCompanyInstalmentEnable(Guid instalmentId, bool isEnabled)
        {
            ExecuteMethod(i => i.PostCompanyInstalmentEnable(instalmentId, isEnabled));
        }
        public void PostTicketCorrection(Guid ticketId, int newLength, int newUnits, int newGuest, int newSolarium, int newFreeze, string comment, DateTime? planInstDate, string newComment)
        {
            ExecuteMethod(i => i.PostTicketCorrection(ticketId, newLength, newUnits, newGuest, newSolarium, newFreeze, comment, planInstDate, newComment));
        }

        public string GenerateEmployeeDocumentsList()
        {
            return ExecuteMethod(i => i.GenerateDivisionReport("GenerateEmployeeDocumentsList", CurrentDivision.Id));
        }

        public string HideProviderOrder(Guid id)
        {
            return ExecuteMethod(i => i.HideProviderOrder(id));
        }

        public string GetMarkdownName(Guid goodId)
        {
            return ExecuteMethod(i => i.GetMarkdownName(goodId));
        }

        public void MarkTreatmentsVisited(Guid[] visited)
        {
            ExecuteMethod(i => i.MarkTreatmentsVisited(visited));
        }

        public List<UnitCharge> GetCustomerUnitCharges(Guid customerId, DateTime start, DateTime end, bool addGuest)
        {
            return ExecuteMethod(i => i.GetCustomerUnitCharges(customerId, start, end, addGuest));
        }

        public void UpdateVisitReceipt(Guid visitId, string receipt)
        {
            ExecuteMethod(i => i.UpdateVisitReceipt(visitId, receipt));
        }

        public bool IsFirstVisitEnabled(Guid customerId)
        {
            return ExecuteMethod(i => i.IsFirstVisitEnabled(customerId));
        }

        public List<AdvertGroup> GetAdvertGroups()
        {
            return ExecuteMethod(i => i.GetAdvertGroups());
        }

        public void PostAdvertGroup(Guid groupId, string name)
        {
            ExecuteMethod(i => i.PostAdvertGroup(groupId, name));
        }

        public void DeleteAdvertGroup(Guid groupId)
        {
            ExecuteMethod(i => i.DeleteAdvertGroup(groupId));
        }

        public void PostAdvertType(Guid typeId, string name, bool commentNeeded, Guid groupId)
        {
            ExecuteMethod(i => i.PostAdvertType(typeId, name, commentNeeded, groupId));
        }

        public void DeleteAdvertType(Guid typeId)
        {
            ExecuteMethod(i => i.DeleteAdvertType(typeId));
        }

        public List<ScheduleProposalElement> GetAvailableParallels(Guid treatmentEventId)
        {
            return ExecuteMethod(i => i.GetAvailableParallels(treatmentEventId));
        }

        public Guid PostParallelSigning(Guid originalEventId, Guid configId, Guid treatmentId, DateTime startTime)
        {
            return ExecuteMethod(i => i.PostParallelSigning(originalEventId, configId, treatmentId, startTime));
        }

        public string GenerateFirstRun(KeyValuePair<string, string>[] items)
        {
            return ExecuteMethod(i => i.GenerateFirstRun(items, CurrentDivision.Id));
        }

        public void AddCommentToTreatmentEvent(Guid eventId, string comment)
        {
            ExecuteMethod(i => i.AddCommentToTreatmentEvent(eventId, comment));
        }

        public byte[] GetCustomerImage(Guid customerId)
        {
            return ExecuteMethod(i => i.GetCustomerImage(customerId));
        }

        public void UpdateCustomerImage(Guid customerId, byte[] imageBytes)
        {
            ExecuteMethod(i => i.UpdateCustomerImage(customerId, imageBytes));
        }

        public void MoveTreatment(Guid treatmentId, bool isLeft)
        {
            ExecuteMethod(i => i.MoveTreatment(treatmentId, isLeft));
        }

        public int GetTotalCustomerCharged(Guid customerId)
        {
            return ExecuteMethod(i => i.GetTotalCustomerCharged(customerId));
        }

        public IEnumerable<Claim> GetClaims(DateTime start, DateTime end, bool showClosedClaims)
        {
            return ExecuteMethod(i => i.GetClaims(CurrentCompany.CompanyId, start, end, showClosedClaims));
        }

        public void PostClaim(Claim claim)
        {
            ExecuteMethod(i => i.PostClaim(claim));
        }

        public void SubmitClaim(Guid claimId, int actualScore)
        {
            ExecuteMethod(i => i.SubmitClaim(claimId, actualScore));
        }

        public List<AnketInfo> GetAnkets(DateTime start, DateTime end)
        {
            return ExecuteMethod(i => i.GetAnkets(CurrentCompany.CompanyId, start, end));
        }

        public Anket GenerateAnketDefault(Guid divisionId, DateTime period)
        {
            return ExecuteMethod(i => i.GenerateAnketDefault(divisionId, period));
        }

        public void PostAnket(Anket anket)
        {
            ExecuteMethod(i => i.PostAnket(anket));
        }

        public string GenerateAnketReport(Guid id)
        {
            return ExecuteMethod(i => i.GenerateAnketReport(id));
        }

        public Anket GetAnket(Guid anketId)
        {
            return ExecuteMethod(i => i.GetAnket(anketId));
        }

        public void DeleteAnket(Guid anketId)
        {
            ExecuteMethod(i => i.DeleteAnket(anketId));
        }

        public void ReopenClaim(Guid claimId, string message)
        {
            ExecuteMethod(i => i.ReopenClaim(claimId, message));
        }

        public bool PostTicketPayment(Guid ticketId, decimal pmtAmount)
        {
            return ExecuteMethod(i => i.PostTicketPayment(ticketId, pmtAmount));
        }

        public List<SalesData> GetUniedReportTicketSalesDynamic(bool isClub)
        {
            return ExecuteMethod(i => i.GetUniedReportTicketSalesDynamic(isClub, CurrentDivision.Id));
        }

        public List<ChannelData> GetUniedReportSalesChannels(int days)
        {
            return ExecuteMethod(i => i.GetUniedReportSalesChannels(days));
        }
        
        public List<ChannelData> GetUniedReportAmountTreatments(int days)
        {
            return ExecuteMethod(i => i.GetUniedReportAmountTreatments(days));
        }

        public List<SalesData> GetUniedReportVisitsDynamics(int kind)
        {
            return ExecuteMethod(i => i.GetUniedReportVisitsDynamics(kind));
        }

        public List<News> GetNews()
        {
            return ExecuteMethod(i => i.GetNews());
        }

        public void PostNews(News news)
        {
            ExecuteMethod(i => i.PostNews(news));
        }

        public List<SshFolder> GetSshFolders()
        {
            return ExecuteMethod(i => i.GetSshFolders());
        }

        public List<SshFile> GetSshFiles()
        {
            return ExecuteMethod(i => i.GetSshFiles());
        }

        public void EnqueueSshFile(Guid fileId)
        {
            ExecuteMethod(i => i.EnqueueSshFile(fileId));
        }

        public void PostCreditTicketPayment(Guid ticketId, decimal bankComissionRur, DateTime paymentDate)
        {
            ExecuteMethod(i => i.PostCreditTicketPayment(ticketId, bankComissionRur, paymentDate));
        }


        public void PostSpendingTypeRemove(Guid spendingTypeId)
        {
            ExecuteMethod(i => i.PostSpendingTypeRemove(spendingTypeId));
        }

        public void PostIncomeTypeRemove(Guid incomeTypeId)
        {
            ExecuteMethod(i => i.PostIncomeTypeRemove(incomeTypeId));
        }

        public List<CashInOrder> GetCashInOrders(DateTime start, DateTime end)
        {
            return ExecuteMethod(i => i.GetCashInOrders(CurrentDivision.Id, start, end));
        }

        public decimal GetCashAmount(DateTime date)
        {
            return ExecuteMethod(i => i.GetCashAmount(CurrentDivision.Id, date));
        }

        public void PostCashInOrder(Guid orderId, DateTime createdOn, string debet, decimal amount, Guid createdBy, Guid receivedBy, string reason)
        {
            ExecuteMethod(i => i.PostCashInOrder(CurrentDivision.Id, orderId, createdOn, debet, amount, createdBy, receivedBy, reason));
        }


        public List<CashOutOrder> GetCashOutOrders(DateTime start, DateTime end)
        {
            return ExecuteMethod(i => i.GetCashOutOrders(CurrentDivision.Id, start, end));
        }

        public void PostCashOutOrder(Guid orderId, DateTime createdOn, string debet, decimal amount, Guid createdBy, string receivedBy, string reason, string responsible)
        {
            ExecuteMethod(i => i.PostCashOutOrder(CurrentDivision.Id, orderId, createdOn, debet, amount, createdBy, receivedBy, reason, responsible));
        }

        public string GetNotificationsForDivision()
        {
            return ExecuteMethod(i => i.GetNotificationsForDivision(CurrentDivision.Id));
        }

        public decimal GetCashTodaysAmount()
        {
            return ExecuteMethod(i => i.GetCashTodaysAmount(CurrentDivision.Id));
        }

        public void PostCustomerStatusDelete(Guid statusId)
        {
            ExecuteMethod(i => i.PostCustomerStatusDelete(statusId));
        }

        public void PostCustomerStatus(Guid statusId, string name)
        {
            ExecuteMethod(i => i.PostCustomerStatus(statusId, name));
        }

        public List<SalesData> GetUniedReportIncomeAmount(bool isClub)
        {
            return ExecuteMethod(i => i.GetUniedReportIncomeAmount(isClub, CurrentDivision.Id));
        }

        public List<TicketsData> GetUniedReportAmountTicket(bool isClub)
        {
            return ExecuteMethod(i => i.GetUniedReportAmountTicket(isClub, CurrentDivision.Id));
        }

        public IEnumerable<TreatmentConfig> GetAllTreatmentConfigsAdmin()
        {
            return ExecuteMethod(i => i.GetAllTreatmentConfigsAdmin());
        }

        public List<Ticket> GetTicketsForPlanning(Guid customerId)
        {
            return ExecuteMethod(i => i.GetTicketsForPlanning(customerId, CurrentDivision.Id));
        }

        public List<Ticket> GetTicketsForCustomer(Guid customerId)
        {
            return ExecuteMethod(i => i.GetTicketsForCustomer(customerId));
        }

        public void SetTreatmentAsVisited(Guid treatmentId)
        {
            ExecuteMethod(i => i.SetTreatmentAsVisited(treatmentId));
        }

        public void UnmissTreatment(List<Guid> treatmentIds)
        {
            ExecuteMethod(i => i.UnmissTreatment(treatmentIds));
        }

        public SshFile GetSshFile(Guid fileId)
        {
            return ExecuteMethod(i => i.GetSshFile(fileId));
        }

        public CustomerNotificationInfo GetCustomerNotificationInfo(Guid customerId)
        {
            return ExecuteMethod(i => i.GetCustomerNotificationInfo(customerId));
        }

        public void PostEmployeeActive(Guid employeeId, bool active)
        {
            ExecuteMethod(i => i.PostEmployeeActive(employeeId, active));
        }

        public void SpreadPermission(Guid permisionId, bool maximum, bool franch, bool upravl, bool admins)
        {
            ExecuteMethod(i => i.SpreadPermission(permisionId, maximum, franch, upravl, admins));
        }

        public List<CumulativeDiscount> GetCumulativeDiscounts()
        {
            return ExecuteMethod(i => i.GetCumulativeDiscounts(CurrentCompany.CompanyId));
        }

        public void PostCumulative(CumulativeDiscount cumulative)
        {
            ExecuteMethod(i => i.PostCumulative(cumulative));
        }

        public CumulativeDiscountInfo GetCumulativeDiscountInfo(Guid customerId)
        {
            return ExecuteMethod(i => i.GetCumulativeDiscountInfo(customerId));
        }

        public List<CustomerEventView> GetCrmEvents(Guid customerId)
        {
            return ExecuteMethod(i => i.GetCRMEvents(customerId));
        }

        public Guid PostCrmEvent(Guid id, Guid customerid, DateTime date, string subject, string comment, string result)
        {
            return ExecuteMethod(i => i.PostCrmEvent(id, CurrentCompany.CompanyId, CurrentDivision.Id, customerid, date, subject, comment, result));
        }

        public List<Package> GetPackages()
        {
            return ExecuteMethod(i => i.GetPackages());
        }

        public Guid PostPackage(Package package, IEnumerable<PackageLine> packageLines)
        {
            return ExecuteMethod(i => i.PostPackage(package, packageLines));
        }

        public List<GoodReserve> GetGoodsReserve(Guid customerId)
        {
            return ExecuteMethod(i => i.GetGoodsReserve(customerId));
        }

        public bool GiveGoodToCustomer(Guid customerId, Guid goodId)
        {
            return ExecuteMethod(i => i.GiveGoodToCustomer(CurrentDivision.Id, customerId, goodId));
        }

        public List<Division> GetDivisionsForCompany(Guid companyId)
        {
            return ExecuteMethod(i => i.GetDivisionsForCompany(companyId));

        }

        public List<CustomerTargetType> GetTargetTypes()
        {
            return ExecuteMethod(i => i.GetTargetTypes());
        }

        public void PostTargetType(CustomerTargetType targetType, Guid[] recomendations)
        {
            ExecuteMethod(i => i.PostTargetType(targetType, recomendations));
        }
        public ScheduleProposalResult GetSmartProposals(Guid customerId, Guid ticketId, DateTime visitDate, Guid targetId, bool allowParallel)
        {
            if (CurrentDivision == null) return new ScheduleProposalResult { List = new List<ScheduleProposal>() };
            return ExecuteMethod(i => i.GetSmartProposals(customerId, CurrentDivision.Id, ticketId, visitDate, targetId, allowParallel));
        }

        public void HideTargetTypeById(Guid targetTypeId)
        {
            ExecuteMethod(i => i.HideTargetTypeById(targetTypeId));
        }

        public void PostTargetSet(Guid id, Guid targetId, string configs)
        {
            ExecuteMethod(i => i.PostTargetSet(id, targetId, configs));
        }

        public List<TargetTypeSet> GetTargetConfigs()
        {
            return ExecuteMethod(i => i.GetTargetConfigs());
        }

        public List<BarDiscount> GetBarDiscounts()
        {
            return ExecuteMethod(i => i.GetBarDiscounts());
        }

        public void PostBarDiscount(BarDiscount barDiscount)
        {
            ExecuteMethod(i => i.PostBarDiscount(barDiscount));
        }

        public decimal GetBarDiscountForCustomer(Guid customerId)
        {
            return ExecuteMethod(i => i.GetBarDiscountForCustomer(customerId));
        }

        public void PostRecurrentRule(ReportRecurrency reportRecurrency)
        {
            ExecuteMethod(i => i.PostRecurrentRule(reportRecurrency));
        }

        public IEnumerable<ReportRecurrency> GetRecurrentReports()
        {
            return ExecuteMethod(i => i.GetRecurrentReports(CurrentUser.UserId));
        }

        public WorkbenchInfo GetWorkbench(DateTime workbenchDate)
        {
            return ExecuteMethod(i => i.GetWorkbench(CurrentDivision.Id, workbenchDate));
        }

        public void PostBonusCorrection(Guid customerId, int amount)
        {
            ExecuteMethod(i => i.PostBonusCorrection(customerId, amount));
        }

        public void PostBonusCorrection(Guid customerId, int amount, string comment)
        {
            if (comment == null) comment = "Нет комментария";
            ExecuteMethod(i => i.PostBonusCorrectionWithComment(customerId, amount, comment));
        }

        public bool PostExtraSmart(Guid customerId, string comment)
        {
            return ExecuteMethod(i => i.PostExtraSmart(customerId, comment));
        }

        public void PostCustomerInvitor(Guid customerId, Guid? invitorId)
        {
            ExecuteMethod(i => i.PostCustomerInvitor(customerId, invitorId));
        }

        public CustomerNotification GetCustomerNotificationById(Guid notificationId)
        {
            return ExecuteMethod(i => i.GetCustomerNotificationById(notificationId));
        }

        public string GetFromSite(DateTime lastCheckTime)
        {
            return ExecuteMethod(i => i.GetFromSite(CurrentDivision.Id, lastCheckTime));
        }
    }
}
