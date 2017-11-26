using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Collections;
using System.Data;
using System.Runtime.Serialization;
using System.Net.Security;
using ExtraClub.ServiceModel.Turnover;
using ExtraClub.ServiceModel.Employees;
using ExtraClub.ServiceModel.Organizer;
using ExtraClub.ServiceModel.Reports;
using ExtraClub.ServiceModel.Schedule;
using ExtraClub.ServiceModel.Ssh;

namespace ExtraClub.ServiceModel
{
    [ServiceContract]
    public interface IExtraService
    {
        [OperationContract]
        string[] GetUserPermissions(Guid divisionId);

        [OperationContract]
        [ApplyDataContractResolver]
        List<FoundCustomer> SearchCustomers(string searchKey);

        [OperationContract]
        [ApplyDataContractResolver]
        List<Good> GetAllGoods(Guid companyId);

        [OperationContract]
        [ApplyDataContractResolver]
        Dictionary<string, DictionaryInfo> GetAllDictionaryInfos();

        [OperationContract]
        Dictionary<Guid, string> GetDictionaryList(string tableName);

        [OperationContract]
        [ApplyDataContractResolver]
        List<Provider> GetAllProviders();

        [OperationContract]
        [ApplyDataContractResolver]
        void PostGood(Good good);

        [OperationContract]
        [ApplyDataContractResolver]
        List<ProviderPayment> GetAllProviderPayments(Guid divisionId, DateTime start, DateTime end);

        [OperationContract]
        [ApplyDataContractResolver]
        List<Consignment> GetAllConsignments(Guid divisionId, DateTime start, DateTime end, bool cons);

        [OperationContract]
        [ApplyDataContractResolver]
        void PostConsignment(Consignment consignment);

        [OperationContract]
        [ApplyDataContractResolver]
        void PostConsignmentLines(IEnumerable<ConsignmentLine> changes);

        [OperationContract]
        [ApplyDataContractResolver]
        List<ConsignmentLine> GetAllConsignmentLines();

        [OperationContract]
        [ApplyDataContractResolver]
        List<GoodPrice> GetAllPrices(Guid divisionId);

        /// <summary>
        /// Возвращает остатки товара по конкретному подразделению.
        /// Для получения остатков по филиалу необходимо послать Guid.Empty
        /// </summary>
        /// <param name="token"></param>
        /// <param name="divisionId"></param>
        /// <returns></returns>
        [OperationContract]
        List<BarPointGood> GetGoodsPresence(Guid divisionId);

        [OperationContract]
        [ApplyDataContractResolver]
        List<Customer> GetPresentCustomers(Guid divisionId);

        [ServiceKnownType(typeof(TicketPaymentPosition))]
        [ServiceKnownType(typeof(BarPointGood))]
        [ServiceKnownType(typeof(PayableItem))]
        [ServiceKnownType(typeof(PaymentDetails))]
        [ServiceKnownType(typeof(CustomerCardGood))]
        [ServiceKnownType(typeof(TicketGood))]
        [ServiceKnownType(typeof(TicketChangeGood))]
        [ServiceKnownType(typeof(TicketRebillGood))]
        [ServiceKnownType(typeof(TicketFreezeGood))]
        [ServiceKnownType(typeof(ChildrenRoomGood))]
        [ServiceKnownType(typeof(SolariumGood))]
        [ServiceKnownType(typeof(RentPayment))]
        [ServiceKnownType(typeof(CloseRentPayment))]
        [ServiceKnownType(typeof(DepositGood))]
        [OperationContract]
        PaymentDetails ProcessPayment(PaymentDetails details, IEnumerable<PayableItem> basket, Guid goodActionId);

        [OperationContract]
        [ApplyDataContractResolver]
        Customer GetCustomer(Guid customerId, bool loadDetails);

        [OperationContract]
        [ApplyDataContractResolver]
        User GetCurrentUser();

        [OperationContract]
        int GetMaxPaymentNumber();

        [OperationContract]
        [ApplyDataContractResolver]
        Division GetDivision(Guid divisionId);

        [OperationContract]
        [ApplyDataContractResolver]
        IEnumerable<TicketType> GetTicketTypes(bool activeOnly);

        [OperationContract]
        [ApplyDataContractResolver]
        IEnumerable<CustomerCardType> GetCustomerCardTypes(bool activeOnly);

        [OperationContract]
        [FaultContract(typeof(string))]
        [ApplyDataContractResolver]
        bool RegisterCustomerVisit(Guid customerId, Guid divisionId, int shelfNumber, int safeNumber);

        [OperationContract]
        [ApplyDataContractResolver]
        List<GoodAction> GetGoodActions(bool onlyActive);

        [OperationContract]
        void PostGoodAction(Guid actionId, string actionName, double discount, IEnumerable<KeyValuePair<Guid, int>> goods, bool isActive);

        [OperationContract]
        void DeleteGoodAction(Guid goodActionId);

        [OperationContract]
        void SetObjectActive(string collectionName, Guid id, bool isActive);

        [OperationContract]
        [ApplyDataContractResolver]
        void PostGoodPrice(GoodPrice GoodPrice);

        [OperationContract]
        void PostProviderPayment(Guid orderId, DateTime date, string paymentType, decimal amount, string comment);

        [OperationContract]
        void ActivateTicket(Guid ticketId);

        [OperationContract]
        Guid RegisterCustomerVisitEnd(Guid customerId, Guid divisionId, bool shelfReturned, bool safeReturned);

        [OperationContract]
        Guid PostNewDictionaryElement(Guid dictionaryId, string newElementName);

        [OperationContract]
        void PostRenameDictionaryElement(Guid dictionaryId, Guid elementGuid, string elementName);

        [OperationContract]
        string PostRemoveDictionaryElement(Guid dictionaryId, Guid elementGuid);

        [OperationContract]
        Guid PostCustomer(Customer customer);

        [OperationContract]
        [ApplyDataContractResolver]
        List<Treatment> GetAllTreatments(Guid divisionId);

        [OperationContract]
        [ApplyDataContractResolver]
        Ticket GetTicketById(Guid ticketId);

        [OperationContract]
        [ApplyDataContractResolver]
        IEnumerable<TreatmentConfig> GetAllTreatmentConfigs();

        [OperationContract]
        [ApplyDataContractResolver]
        void PostTreatment(Treatment Treatment);

        [OperationContract]
        [ApplyDataContractResolver]
        IEnumerable<TreatmentSeqRest> GetAllTreatmentSeqRests();

        [OperationContract]
        [ApplyDataContractResolver]
        void PostTreatmentSeqRest(TreatmentSeqRest treatmentSR);

        [OperationContract]
        [FaultContract(typeof(string))]
        [ApplyDataContractResolver]
        ScheduleProposalResult GetScheduleProposals(Guid customerId, Guid divId, Guid ticketId, DateTime visitDate, bool isParallelAllowed, List<Guid> treatments, bool isOptimalAllowed, Guid programId);

        [OperationContract]
        [FaultContract(typeof(string))]
        [ApplyDataContractResolver]
        bool PostScheduleProposal(Guid customerId, Guid divisionId, Guid ticketId, ScheduleProposal scheduleProposal);

        [OperationContract]
        [ApplyDataContractResolver]
        Customer GetCustomerByCard(int cardNumber, bool loadDetails);

        [OperationContract]
        [ApplyDataContractResolver]
        List<TreatmentProgram> GetTreatmentPrograms();

        [OperationContract]
        [ApplyDataContractResolver]
        List<AdvertType> GetAdvertTypes();

        [OperationContract]
        [ApplyDataContractResolver]
        void UpdateCustomerForm(Customer customer);

        [OperationContract]
        List<string>[] GetAddressLists();

        [OperationContract]
        List<decimal> GetDiscountsForCurrentUser(short discountType);

        [OperationContract]
        string UpdateInvitor(Guid invitedId, Guid invitorId);

        [OperationContract]
        [ApplyDataContractResolver]
        IEnumerable<TicketType> GetActiveTicketTypesForCustomer(Guid customerId);

        [OperationContract]
        int GetMaxGuestUnits(Guid divisionId, Guid customerId);

        [OperationContract]
        string GenerateCardContractReport(string cardNumber, string reportKey);

        [OperationContract]
        string GeneratePKOReport(Guid cashInOrderId);

        [OperationContract]
        string GenerateCashierPageReport(Guid divisionId, DateTime date);

        [OperationContract]
        string GenerateRKOReport(Guid cashOutOrderId);

        [OperationContract]
        string GenerateTicketReceiptReport(Guid pmtId, string reportKey);

        [OperationContract]
        void PostCustomerAddress(Guid customerId, string metro, string index, string city, string street, string other);

        [OperationContract]
        [ServiceKnownType(typeof(TicketFreeze))]
        string GenerateTicketContractReport(Guid parameter, object parameter1, string reportKey);

        [OperationContract]
        [ApplyDataContractResolver]
        Company GetCompnay();

        [OperationContract]
        [ApplyDataContractResolver]
        List<TicketFreezeReason> GetTicketFreezeReasons();

        [OperationContract]
        void CancelTreatmentEvents(List<Guid> events);

        [OperationContract]
        [ApplyDataContractResolver]
        void PostCustomerCardType(CustomerCardType customerCardType);

        [OperationContract]
        [ApplyDataContractResolver]
        Guid PostTicketType(TicketType ticketType);

        [OperationContract]
        [FaultContract(typeof(string))]
        bool DeleteObject(string collectionName, Guid id);

        [OperationContract]
        void PostTicketTypeTreatmentTypes(Guid ticketTypeId, List<Guid> treatmentTypes);

        [OperationContract]
        void ClearCustomerContras(Guid customerId);

        [OperationContract]
        [ApplyDataContractResolver]
        List<ContraIndication> GetAllContras();

        [OperationContract]
        List<Guid> GetCustomerContrasIds(Guid customerId);

        [OperationContract]
        void PostContraIndications(Guid customerId, List<Guid> contraIds);

        [OperationContract]
        [ApplyDataContractResolver]
        Guid PostContraIndication(ContraIndication contraIndication);

        [OperationContract]
        void PostContraIndicationTreatmentTypes(Guid contraId, List<Guid> treatmentIds);

        [OperationContract]
        [ApplyDataContractResolver]
        List<CustomerTarget> GetCustomerTargets(Guid customerId);

        [OperationContract]
        [ApplyDataContractResolver]
        Guid PostCustomerTarget(CustomerTarget customerTarget);

        [OperationContract]
        [ApplyDataContractResolver]
        [ServiceKnownType(typeof(CustomerTarget))]
        [ServiceKnownType(typeof(Ticket))]
        [ServiceKnownType(typeof(DepositOut))]
        [ServiceKnownType(typeof(BarOrder))]
        [ServiceKnownType(typeof(CustomerNotification))]
        [ServiceKnownType(typeof(Task))]
        List<OrganizerItem> GetOrganizerItems();

        [OperationContract]
        [ApplyDataContractResolver]
        [ServiceKnownType(typeof(CustomerTarget))]
        [ServiceKnownType(typeof(Ticket))]
        [ServiceKnownType(typeof(DepositOut))]
        [ServiceKnownType(typeof(BarOrder))]
        [ServiceKnownType(typeof(CustomerNotification))]
        [ServiceKnownType(typeof(Task))]
        List<OrganizerItem> GetOrganizerItemsEx(DateTime start, DateTime end);

        [OperationContract]
        void StartTicketReturn(Guid ticketId, string comment);

        [OperationContract]
        [ApplyDataContractResolver]
        List<Anthropometric> GetAnthropometricsForCustomer(Guid customerId);

        [OperationContract]
        [ApplyDataContractResolver]
        Guid PostCustomerAnthropomentric(Anthropometric anthropometric);

        [OperationContract]
        [ApplyDataContractResolver]
        List<DoctorVisit> GetDoctorVisitsForCustomer(Guid customerId);

        [OperationContract]
        [ApplyDataContractResolver]
        Guid PostCustomerDoctorVisit(DoctorVisit doctorVisit);

        [OperationContract]
        List<string> GetDoctorTemplates();

        [OperationContract]
        [ApplyDataContractResolver]
        List<Nutrition> GetNutritionsForCustomer(Guid customerId);

        [OperationContract]
        [ApplyDataContractResolver]
        Guid PostNutrition(Nutrition nutrition);

        [OperationContract]
        List<string>[] GetNutritionTemplates();

        [OperationContract]
        [ApplyDataContractResolver]
        List<CustomerMeasure> GetMeasuresForCustomer(Guid customerId);

        [OperationContract]
        [ApplyDataContractResolver]
        Guid PostCustomerMeasure(CustomerMeasure measure);

        [OperationContract]
        [ApplyDataContractResolver]
        List<TreatmentType> GetAllTreatmentTypes();

        [OperationContract]
        Guid? GetTreatmentProgramIdForCustomer(Guid customerId);

        [OperationContract]
        [ApplyDataContractResolver]
        List<TreatmentEvent> GetCustomerEvents(Guid customerId, DateTime start, DateTime end, bool canceled);

        [OperationContract]
        [ApplyDataContractResolver]
        List<TextAction> GetCurrentActions(Guid divisionId);

        [OperationContract]
        [ApplyDataContractResolver]
        List<TreatmentTypeGroup> GetAllTreatmentTypeGroups();

        [OperationContract]
        [ApplyDataContractResolver]
        [FaultContract(typeof(string))]
        Guid PostTreatmentType(TreatmentType TreatmentType);

        [OperationContract]
        [ApplyDataContractResolver]
        [FaultContract(typeof(string))]
        Guid PostTreatmentConfig(TreatmentConfig treatmentConfig);

        [OperationContract]
        [ApplyDataContractResolver]
        List<GoodSale> GetBarOrdersForCustomer(Guid customerId, DateTime startDate, DateTime endDate);

        [OperationContract]
        [ApplyDataContractResolver]
        void PostCompany(Company Company);

        [OperationContract]
        [ApplyDataContractResolver]
        Guid PostTreatmentProgram(TreatmentProgram treatmentProgram, List<Guid> lines);

        [OperationContract]
        [ApplyDataContractResolver]
        List<TextAction> GetAllActions();

        [OperationContract]
        [ApplyDataContractResolver]
        Guid PostTextAction(TextAction textAction, Guid[] divisionIds);

        [OperationContract]
        [ApplyDataContractResolver]
        void DeleteParallelRule(TreatmentsParalleling rule);

        [OperationContract]
        [ApplyDataContractResolver]
        List<TreatmentsParalleling> GetParallelingRules();

        [OperationContract]
        [ApplyDataContractResolver]
        void PostTreatmentsParalleling(TreatmentsParalleling old, TreatmentsParalleling rule);

        [OperationContract]
        List<Guid> GetCustomerStatusesIds(Guid customerId);

        [OperationContract]
        void PostCustomerStatuses(Guid customerId, List<Guid> list);

        [OperationContract]
        [ApplyDataContractResolver]
        List<ReportTemplate> GetAllTemplates();

        [OperationContract]
        [ApplyDataContractResolver]
        void PostReportTemplate(ReportTemplate template);

        [OperationContract]
        void PostTreatmentBreakdown(Guid treatmentId);

        [OperationContract]
        [ApplyDataContractResolver]
        List<ChildrenRoom> GetCustomerChildren(Guid customerId);

        [OperationContract]
        string GenerateChildRequestReport(Guid childId, string repType);

        [OperationContract]
        [ApplyDataContractResolver]
        void PostDivision(Division division);

        [OperationContract]
        List<int> GetAvailableShelfNumbers(Guid divisionId, bool isSafe);

        [OperationContract]
        [ApplyDataContractResolver]
        List<CustomerShelf> GetCustomerShelves(Guid customerId, bool isSafe);

        [OperationContract]
        [ApplyDataContractResolver]
        List<CustomerShelf> GetDivisionShelves(Guid divisionId, DateTime startPeriod, DateTime endPeriod, bool isSafe);

        [OperationContract]
        [ApplyDataContractResolver]
        List<Solarium> GetDivisionSolariums(Guid divisionId, bool activeOnly);

        [OperationContract]
        [ApplyDataContractResolver]
        void PostSolarium(Solarium solarium);

        [OperationContract]
        Dictionary<int, string> GetSolariumWarnings();

        [OperationContract]
        Guid PostSolariumBooking(Guid divisionId, Guid customerId, Guid solariumId, DateTime dateTime, int amount, string comment);

        [OperationContract]
        Guid PostSolariumBookingEx(Guid divisionId, Guid customerId, Guid solariumId, DateTime dateTime, int amount, string comment, Guid? ticketId);

        [OperationContract]
        [FaultContract(typeof(string))]
        KeyValuePair<Guid, DateTime> GetSolariumProposal(Guid divisionId, Guid customerId, DateTime dateTime, int amount, Guid selectedSolariumId, Guid toSkip);

        [OperationContract]
        [ApplyDataContractResolver]
        List<SolariumVisit> GetCustomerSolarium(Guid customerId);

        [OperationContract]
        [ApplyDataContractResolver]
        List<GoodSale> GetCustomerSales(Guid customerId, DateTime start, DateTime end);

        [OperationContract]
        [ApplyDataContractResolver]
        List<SolariumVisit> GetDivisionSolariumVisits(Guid divisionId, DateTime startDate, DateTime finishDate);

        [OperationContract]
        [ApplyDataContractResolver]
        SolariumVisit GetSolariumVisitById(Guid visitId);

        [OperationContract]
        [ApplyDataContractResolver]
        bool CancelSolariumEvent(Guid solVisitId, bool delete);

        [OperationContract]
        [ApplyDataContractResolver]
        List<Ticket> GetCustomerTickets(Guid customerId);

        [OperationContract]
        void PostSolariumVisitStart(Guid solariumVisitId);

        [OperationContract]
        void PostSolariumWarnings(List<KeyValuePair<int, string>> solariumWarnings);

        [OperationContract]
        [ApplyDataContractResolver]
        List<CustomerCardType> GetAllCustomerCardTypes();

        [OperationContract]
        [ApplyDataContractResolver]
        List<TicketType> GetAllTicketTypes();

        [OperationContract]
        void PostCompanyCardTypeEnable(Guid ccardId, bool enable);

        [OperationContract]
        void PostCompanyTicketTypeEnable(Guid tTypeId, bool enable);

        [OperationContract]
        [ApplyDataContractResolver]
        List<ProviderFolder> GetProviderFolders();

        [OperationContract]
        [ApplyDataContractResolver]
        void PostProviderFolder(ProviderFolder providerFolder);

        [OperationContract]
        void DeleteProviderFolder(Guid folderId);

        [OperationContract]
        [ApplyDataContractResolver]
        void PostProvider(Provider provider);

        [OperationContract]
        void HideProvider(Guid providerId);

        [OperationContract]
        void HideGood(Guid goodId);

        [OperationContract]
        [ApplyDataContractResolver]
        List<Storehouse> GetStorehouses(Guid divisionId);

        [OperationContract]
        [ApplyDataContractResolver]
        void PostStorehouse(Storehouse storehouse);

        [OperationContract]
        [ApplyDataContractResolver]
        string GenerateConsignmentReport(Guid consId);

        [OperationContract]
        [ApplyDataContractResolver]
        List<ChildrenRoom> GetDivisionChildren(Guid divisionId, DateTime start, DateTime end);

        [OperationContract]
        [ApplyDataContractResolver]
        List<GoodSale> GetDivisionSales(Guid divisionId, DateTime start, DateTime end);

        [OperationContract]
        string GenerateGoodReport(Guid goodSaleId, string reportKey);

        [OperationContract]
        string GenerateEmployeeReport(Guid employee, string reportKey);

        [ServiceKnownType(typeof(PaymentDetails))]
        [ServiceKnownType(typeof(TicketReturnPosition))]
        [ServiceKnownType(typeof(GoodSaleReturnPosition))]
        [OperationContract]
        PaymentDetails ProcessReturn(PaymentDetails pmt, IEnumerable<PayableItem> items);

        [OperationContract]
        [ApplyDataContractResolver]
        List<Certificate> GetDivisionCertificates(Guid divisionId);

        [OperationContract]
        [ApplyDataContractResolver]
        void PostCertificate(Certificate certificate);

        [OperationContract]
        bool CancelCertificate(Guid certificateId);

        [OperationContract]
        [ApplyDataContractResolver]
        List<Rent> GetCustomerRent(Guid customerId);

        [OperationContract]
        [ApplyDataContractResolver]
        void PostRent(Rent rent);

        [OperationContract]
        string GenerateBarOrderReport(int orderId, string reportKey);

        [OperationContract]
        [ApplyDataContractResolver]
        Certificate GetCertificateByNumber(int id);

        [OperationContract]
        [ApplyDataContractResolver]
        List<BarOrder> GetDivisionBarOrders(Guid divisionId, DateTime start, DateTime end);

        [OperationContract]
        [ApplyDataContractResolver]
        List<DepositAccount> GetCustomerDeposit(Guid customerId, DateTime start, DateTime end);

        [OperationContract]
        void PostDepositAdd(Guid customerId, decimal amount, string description);

        [OperationContract]
        Guid RequestDepositOut(Guid customerId, decimal amount);

        [OperationContract]
        string GenerateDepositOutStatementReport(Guid statementId);

        [OperationContract]
        void PostDepositOutDone(Guid depositOutId, string comment, bool isDone);

        [OperationContract]
        void FinalizeCashlessPayment(Guid orderId, string comments, bool isSuccessful);

        [OperationContract]
        void ProcessBankReturn(Guid orderId);

        [OperationContract]
        [ApplyDataContractResolver]
        List<Spending> GetDivisionSpendings(Guid divisionId, DateTime start, DateTime end);

        [OperationContract]
        [ApplyDataContractResolver]
        List<SpendingType> GetDivisionSpendingTypes(Guid divisionId);

        [OperationContract]
        [ApplyDataContractResolver]
        void PostSpending(Spending spending);

        [OperationContract]
        void PostSpendingType(Guid divisionId, Guid typeId, string name);

        [OperationContract]
        string GeneratePriceListReport(Guid divisionId);

        [OperationContract]
        [ApplyDataContractResolver]
        List<BonusAccount> GetCustomerBonus(Guid customerId, DateTime start, DateTime end);

        [OperationContract]
        [ApplyDataContractResolver]
        List<EmployeeCategory> GetEmployeeCategories(Guid companyId);

        [OperationContract]
        [ApplyDataContractResolver]
        IEnumerable<Job> GetJobs(Guid divisionId);

        [OperationContract]
        [ApplyDataContractResolver]
        void PostEmployeeCategory(EmployeeCategory category, List<Guid> jobIds);

        [OperationContract]
        List<string> GetJobUnits(Guid divisionId);

        [OperationContract]
        [ApplyDataContractResolver]
        void PostJob(Job job, List<Guid> categoryIds);

        [OperationContract]
        string GetBaselineStatus(Guid divisionId);

        [OperationContract]
        void BaselineJobs(Guid divisionId);

        [OperationContract]
        [ApplyDataContractResolver]
        List<Employee> GetEmployees(Guid divisionId, bool presentOnly, bool asuOnly);

        [OperationContract]
        [ApplyDataContractResolver]
        Guid PostEmployeeForm(Employee employee, Customer customer);

        [OperationContract]
        [ApplyDataContractResolver]
        List<Job> GetAvailableJobs(Guid divisionId);

        [OperationContract]
        [ApplyDataContractResolver]
        Guid PostJobPlacement(JobPlacement placement);

        [OperationContract]
        [ApplyDataContractResolver]
        JobPlacement GetJobPlacementDraft(Guid employeeId);

        [OperationContract]
        [ApplyDataContractResolver]
        Employee GetEmployeeById(Guid id);

        [OperationContract]
        string GetEmployeeCardStatusMessage(string cardNumber);

        [OperationContract]
        void PostEmployeeCard(Guid employeeId, string newCardNumber);

        [OperationContract]
        [ApplyDataContractResolver]
        Guid PostJobPlacementChange(JobPlacement jobPlacement);

        [OperationContract]
        Guid PostEmployeeFire(Guid employeeId, DateTime fireDate, string fireCause, bool cardReturned, decimal totalToPay, decimal bonus, decimal ndfl, string logMessage);

        [OperationContract]
        [FaultContract(typeof(string))]
        Guid PostEmployeeVacation(Guid employeeId, DateTime beginDate, DateTime endDate, byte vacType, decimal totalToPay, decimal ndfl, string logMessage);

        [OperationContract]
        Guid[] PostEmployeeTrip(Guid[] employeeIds, DateTime beginDate, DateTime endDate, string destination, string target, string tripBase);

        [OperationContract]
        void PostLastTicketCorrection(Guid customerId, DateTime corrActivated, int corrAmount, int corrGuest, decimal paidAmt, int solCorr);

        [OperationContract]
        Guid PostEmployeeCategoryChange(Guid employeeId, Guid categoryId);

        [OperationContract]
        [ApplyDataContractResolver]
        List<EmployeeDocument> GetEmployeeDocuments(Guid divisionId);

        [OperationContract]
        int GetActiveEmployeesCountForJobId(Guid jobId);

        [OperationContract]
        int GetActiveEmployeesCountForCategoryId(Guid categoryId);

        [OperationContract]
        void HideEmployeeCategoryById(Guid categoryId);

        [OperationContract]
        void HideEmployeeJobById(Guid jobId);

        [OperationContract]
        string GenerateStateScheduleReport(Guid divisionId);

        [OperationContract]
        [ApplyDataContractResolver]
        Employee GetEmployeeByCard(string cardNumber);

        [OperationContract]
        void PostEmployeeVisit(Guid employeeId, bool isIn);

        [OperationContract]
        [ApplyDataContractResolver]
        List<VacationList> GetVacationHistory(Guid divisionId);

        [OperationContract]
        [ApplyDataContractResolver]
        List<VacationPreference> GetEmployeePreferences(Guid employeeId);

        [OperationContract]
        void PostEmployeePreference(Guid employeeId, DateTime beginDate, DateTime endDate, short prefType);

        [OperationContract]
        [ApplyDataContractResolver]
        List<EmployeeScheduleProposalElement> GenerateScheduleProposal(Guid divisionId, int year, int recDays);

        [OperationContract]
        [ApplyDataContractResolver]
        Guid PostEmployeeVacationsSchedule(List<EmployeeScheduleProposalElement> list);

        [OperationContract]
        [ApplyDataContractResolver]
        List<EmployeeScheduleProposalElement> GetCurrentEmployeeVacationsSchedule(Guid divisionId);

        [OperationContract]
        [ApplyDataContractResolver]
        List<EmployeeScheduleProposalElement> GetEmployeeVacationsSchedule(Guid listId);

        [OperationContract]
        string GenerateEmployeeVacationList(Guid listId);

        [OperationContract]
        [ApplyDataContractResolver]
        List<EmployeeWorkScheduleItem> GetEmployeeWorkSchedule(Guid divisionId, DateTime start, DateTime finish);

        [OperationContract]
        Guid PostEmployeeSchedule(Guid divisionId, Dictionary<Guid, List<DateTime>> schedule);

        [OperationContract]
        string GenerateEmployeeScheduleReport(Guid graphId);

        [OperationContract]
        [ApplyDataContractResolver]
        List<EmployeeWorkGraph> GetWorkGraphs(Guid divisionId);

        [OperationContract]
        List<DateTime> GetHolidays(int year);

        [OperationContract]
        void PostHoliday(DateTime date);

        [OperationContract]
        void DeleteHoliday(DateTime date);

        [OperationContract]
        string GetNotificationsForUser();///n

        [OperationContract]
        [ApplyDataContractResolver]
        List<SalaryScheme> GetSalarySchemes(Guid divisionId);

        [OperationContract]
        [ApplyDataContractResolver]
        void PostSalaryScheme(Guid divisionId, SalaryScheme salaryScheme);

        [OperationContract]
        decimal CalculateSalary(Guid employeeId, DateTime calcMonth);

        [OperationContract]
        [ApplyDataContractResolver]
        List<SalarySheet> GetSalarySheets(Guid divisionId);

        [OperationContract]
        [ApplyDataContractResolver]
        List<SalarySheetRow> GenerateSalarySheet(Guid divisionId, DateTime genMonth);

        [OperationContract]
        [ApplyDataContractResolver]
        string PostSalarySheet(Guid divisionId, DateTime period, List<SalarySheetRow> lines);

        [OperationContract]
        [ApplyDataContractResolver]
        List<SalarySheetRow> GetSalarySheetLines(Guid sheetId);

        [OperationContract]
        string PostEmployeePayment(Guid employeeId, Guid sheetId, decimal amount);

        [OperationContract]
        bool CheckSalarySheet(Guid divisionId, DateTime genDate);

        [OperationContract]
        KeyValuePair<decimal, string> GetFireSalary(Guid employeeId, DateTime fireDate);

        [OperationContract]
        KeyValuePair<decimal, string> GetEmployeeVacationPmt(Guid employeeId, DateTime beginDate, DateTime endDate);

        [OperationContract]
        KeyValuePair<decimal, string> GetEmployeeIllnessPmt(Guid employeeId, DateTime beginDate, DateTime endDate);

        [OperationContract]
        [ApplyDataContractResolver]
        List<Corporate> GetCorporates(Guid companyId);

        [OperationContract]
        bool PostCorporate(Guid companyId, Guid corpId, string name, Guid? folderId);

        [OperationContract]
        [ApplyDataContractResolver]
        List<TreatmentEvent> GetDivisionTreatmetnsVisits(Guid divisionId, DateTime start, DateTime finish);

        [OperationContract]
        [ApplyDataContractResolver]
        [FaultContract(typeof(string))]
        List<ScheduleProposalElement> FixSchedule(Guid divisionId, Guid customerId, Guid ticketId, List<ScheduleProposalElement> list);

        [OperationContract]
        [ApplyDataContractResolver]
        List<EmployeePayment> GetDivisionEmployeeCashflow(Guid divisionId);

        [OperationContract]
        [ApplyDataContractResolver]
        TreatmentEvent GetTreatmentEventById(Guid treatmentEventId);

        [OperationContract]
        void PostTreatmentEventChange(Guid treatmentEventId, DateTime newTime);

        [OperationContract]
        void PostFilePart(Guid divisionId, Guid fileId, string fileName, byte[] part, int bytes, int? category, Guid parameter);

        [OperationContract]
        [ApplyDataContractResolver]
        List<File> GetDivisionFiles(Guid divisionId);

        [OperationContract]
        byte[] GetFilePart(Guid fileId, int blockNumber);

        [OperationContract]
        void PostSalesPlan(Guid divisionId, DateTime month, decimal amount, decimal amountCorp, DateTime oldMonth);

        [OperationContract]
        [ApplyDataContractResolver]
        List<SalesPlan> GetSalesPlanForDivsion(Guid divisionId);

        [OperationContract]
        [ApplyDataContractResolver]
        List<IncomingCallForm> GetCallScrenarioForms();

        [OperationContract]
        [ApplyDataContractResolver]
        void PostIncomingCallForm(IncomingCallForm incomingCallForm);

        [OperationContract]
        void PostNewCall(Guid divisionId, string log, CallResult callResult, Guid? customerId, bool isIncoming, DateTime started, string goal, string result);

        [OperationContract]
        [ApplyDataContractResolver]
        List<Call> GetDivisionCalls(Guid divisionId, DateTime callsStart, DateTime callsEnd);

        [OperationContract]
        void PostClubClosing(Guid divisionId, DateTime start, DateTime end, string cause);

        [OperationContract]
        void PostNotificationCompletion(Guid divsionId, Guid notificationId, string comment, string result);

        [OperationContract]
        [ApplyDataContractResolver]
        List<Customer> GetAllCustomers();

        [OperationContract]
        [ApplyDataContractResolver]
        List<Customer> GetPotentialCustomers();

        [OperationContract]
        [ApplyDataContractResolver]
        List<Customer> GetInactiveCustomers();

        [OperationContract]
        [ApplyDataContractResolver]
        List<Customer> GetActiveCustomers();

        [OperationContract]
        [ApplyDataContractResolver]
        List<Customer> GetCustomersByStatus(List<Guid> statIds);

        [OperationContract]
        [ApplyDataContractResolver]
        List<Customer> GetCustomersByManagers(List<Guid> managerIds);

        [OperationContract]
        [ApplyDataContractResolver]
        List<Employee> GetEmployeesWorkingAt(Guid divisionId, DateTime date);

        [OperationContract]
        void PostGroupCall(Guid divisionId, Guid[] customers, Guid[] employees, string comments, DateTime runDate, DateTime expityDate);

        [OperationContract]
        void PostReturnReject(Guid ticketId);

        [OperationContract]
        void PostIncorrectPhoneTask(Guid divisionId, Guid cnId, Guid customerId, string comments);

        [OperationContract]
        void PostTaskClosing(Guid taskId, bool isCompleted, string comment, DateTime date);

        [OperationContract]
        void PostNewTask(Guid[] employees, string subject, string comments, DateTime expiryDate, int priority);

        [OperationContract]
        List<Guid> GetEmployeeIdsWithPermission(Guid divisionId, string permissionName);

        [OperationContract]
        [ApplyDataContractResolver]
        List<OrganizerItem> GetOutboxOrganizerItems();

        [OperationContract]
        void PostTaskRecall(Guid elementId);

        [OperationContract]
        [ApplyDataContractResolver]
        List<OrganizerItem> GetArchivedOrganizerItems();

        [OperationContract]
        void PostTaskReopen(Guid elementId);

        [OperationContract]
        [ApplyDataContractResolver]
        List<Call> GetCustomerCalls(Guid customerId);

        [OperationContract]
        [ApplyDataContractResolver]
        List<Division> GetDivisions();

        [OperationContract]
        [ApplyDataContractResolver]
        List<Role> GetRoles();

        [OperationContract]
        [ApplyDataContractResolver]
        List<Permission> GetAllPermissions();

        [OperationContract]
        void PostRole(Guid roleId, string name, Guid[] permissionIds, string cardDisc, string ticketDisc, string ticketRubDisc, Guid? folderId);

        [OperationContract]
        void DeleteRole(Guid roleId);

        [OperationContract]
        [ApplyDataContractResolver]
        List<User> GetUsers();

        [OperationContract]
        [FaultContract(typeof(string))]
        Guid PostNewUser(Guid employeeId, string userName, string fullName, string password, string email);

        [OperationContract]
        void PostUser(Guid userId, string fullName, bool isActive, Guid[] roleIds, string email);

        [OperationContract]
        void ResetPassword(Guid userId, string password);

        [OperationContract]
        string ChangePassword(Guid userId, string oldPassword, string newPassword);

        [OperationContract]
        void PostInstalmentDelete(Guid instalmentId);

        [OperationContract]
        [ApplyDataContractResolver]
        void PostInstalment(Instalment instalment);

        [OperationContract]
        [ApplyDataContractResolver]
        IEnumerable<ReportInfoInt> GetUserReportsList();

        [OperationContract]
        [ApplyDataContractResolver]
        byte[] GenerateReport(string key, Dictionary<string, object> parameters);

        [OperationContract]
        [ApplyDataContractResolver]
        List<CompanyFinance> GetCompanyFinances(Guid companyId);

        [OperationContract]
        void PostCompanyFinance(Guid companyId, DateTime period, decimal accLeft);

        [OperationContract]
        [ApplyDataContractResolver]
        List<DivisionFinance> GetDivisionFinances(Guid divisionId);

        [OperationContract]
        void PostDivisionFinance(Guid divisionId, DateTime dateTime, decimal cash, decimal unsent, decimal advances, decimal loan, decimal accumulated);

        [OperationContract]
        [ApplyDataContractResolver]
        List<IncomeType> GetDivisionIncomeTypes(Guid divisionId);

        [OperationContract]
        void PostIncomeType(Guid divisionId, Guid id, string name);

        [OperationContract]
        [ApplyDataContractResolver]
        void PostIncome(Income income);

        [OperationContract]
        [ApplyDataContractResolver]
        List<Income> GetDivisionIncomes(Guid divisionId, DateTime start, DateTime end);

        [OperationContract]
        void DoSync();

        [OperationContract]
        void DeleteTreatmentProgram(Guid programId);

        [OperationContract]
        [ApplyDataContractResolver]
        LocalSetting GetLocalSettings();

        [OperationContract]
        void PostLocalSettings(int keyDays, int keyPeriod, int licDays, int licPeriod, string notifyAddresses);

        [OperationContract]
        void UpdateLicenseKey();

        [OperationContract]
        Dictionary<Guid, string> GetCustomerTargetTypes();

        [OperationContract]
        [ApplyDataContractResolver]
        void PostUserReport(ReportInfoInt report);

        [OperationContract]
        [ApplyDataContractResolver]
        CustomReport GetReportForEdit(Guid id);

        [OperationContract]
        [ApplyDataContractResolver]
        List<SettingsFolder> GetSettingsFolders(int categoryId, bool companyOnly);

        [OperationContract]
        [ApplyDataContractResolver]
        void PostSettingsFolder(SettingsFolder folder, Guid[] companies);

        [OperationContract]
        void DeleteSettingsFolder(Guid folderId);

        [OperationContract]
        void DeleteContraIndication(Guid contraId);

        [OperationContract]
        bool DeleteCorporate(Guid corpId);

        [OperationContract]
        [ApplyDataContractResolver]
        List<CompanySettingsFolder> GetCompanySettingsFolders(int categoryId, Guid divId);

        [OperationContract]
        [ApplyDataContractResolver]
        void PostCompanySettingsFolder(CompanySettingsFolder folder, Guid divId);

        [OperationContract]
        void SetTreatmentEventColor(Guid eventId, int colorId);

        [OperationContract]
        [ApplyDataContractResolver]
        List<CompanyView> GetCompaniesListForFolder(Guid folderId);

        [OperationContract]
        void PostPermissions(string[] auths);

        [OperationContract]
        [ApplyDataContractResolver]
        List<Company> GetCompanies();

        [OperationContract]
        [FaultContract(typeof(string))]
        void PostNewCompany(string name, string login, string password, Guid roleId, string reportEmail, int utcCorr, string userPrefix);

        [OperationContract]
        [FaultContract(typeof(string))]
        void PostNewDivision(Division division);

        [OperationContract]
        Guid GetCustomerIdByPhone(string phone);

        [OperationContract]
        Dictionary<Guid, string> GetAllStatuses();

        [OperationContract]
        Guid GetCustomerIdByTargetId(Guid targetId);

        [OperationContract]
        Guid GetCustomerIdByTreatmentEventId(Guid treatmentEventId);

        [OperationContract]
        Guid GetCustomerIdByTicketId(Guid guid);

        [OperationContract]
        Guid GetCustomerByCardId(Guid guid);

        [OperationContract]
        Guid GetCustomerByGoodSale(Guid guid);

        [OperationContract]
        void PostParameters(string reportKey, Dictionary<string, object> parameters, string setName);

        [OperationContract]
        [ApplyDataContractResolver]
        void DeleteReport(Guid savedId, string key, ReportType reportType);

        [OperationContract]
        [FaultContract(typeof(string))]
        void PostMarkdown(Guid divisionId, Guid storeId, Guid goodId, string newName, decimal price, int amount, Guid provId);

        [OperationContract]
        [ApplyDataContractResolver]
        List<CustomerVisit> GetCustomerVisits(Guid customerId);

        [OperationContract]
        List<string>[] GetWorkData();

        [OperationContract]
        void PostTicketTypeLimits(Guid ttId, KeyValuePair<Guid, int>[] lims);

        [OperationContract]
        [ApplyDataContractResolver]
        IEnumerable<Instalment> GetAllInstalments();

        [OperationContract]
        [ApplyDataContractResolver]
        IEnumerable<Instalment> GetCompanyInstalments(bool activeOnly);

        [OperationContract]
        void PostCompanyInstalmentEnable(Guid instalmentId, bool isEnabled);

        [OperationContract]
        void PostTicketCorrection(Guid guid, int newLength, int newUnits, int newGuest, int newSolarium, int newFreeze, string comment, DateTime? planInstDate, string newComment);

        [OperationContract]
        string GenerateDivisionReport(string key, Guid divisionId);

        [OperationContract]
        string HideProviderOrder(Guid id);

        [OperationContract]
        string GetMarkdownName(Guid goodId);

        [OperationContract]
        void MarkTreatmentsVisited(Guid[] visited);

        [OperationContract]
        [ApplyDataContractResolver]
        List<UnitCharge> GetCustomerUnitCharges(Guid customerId, DateTime start, DateTime end, bool addGuest);

        [OperationContract]
        void UpdateVisitReceipt(Guid visitId, string receipt);

        [OperationContract]
        bool IsFirstVisitEnabled(Guid customerId);

        [OperationContract]
        [ApplyDataContractResolver]
        List<AdvertGroup> GetAdvertGroups();

        [OperationContract]
        [FaultContract(typeof(string))]
        void PostAdvertGroup(Guid groupId, string name);

        [OperationContract]
        void DeleteAdvertGroup(Guid groupId);

        [OperationContract]
        [FaultContract(typeof(string))]
        void PostAdvertType(Guid typeId, string name, bool commentNeeded, Guid groupId);

        [OperationContract]
        void DeleteAdvertType(Guid typeId);

        [OperationContract]
        [ApplyDataContractResolver]
        List<ScheduleProposalElement> GetAvailableParallels(Guid treatmentEventId);

        [OperationContract]
        [FaultContract(typeof(string))]
        Guid PostParallelSigning(Guid originalEventId, Guid configId, Guid treatmentId, DateTime startTime);

        [OperationContract]
        string GenerateFirstRun(KeyValuePair<string, string>[] parameters, Guid divisionId);

        [OperationContract]
        [ApplyDataContractResolver]
        Treatment GetTreatmentByMac(string macAddress);

        [OperationContract]
        [ApplyDataContractResolver]
        TreatmentEvent GetCustomerPlanningForTreatment(Guid customerId, Guid treatmentId);

        [OperationContract]
        void PostTreatmentStart(Guid treatmentId, DateTime newTime);

        [OperationContract]
        [ApplyDataContractResolver]
        HWProposal GetHWProposal(Guid treatmentId, Guid customerId);

        [OperationContract]
        [ApplyDataContractResolver]
        TreatmentEvent PostNewTreatmentEvent(Guid ticketId, Guid treatmentId, DateTime visitStart);

        [OperationContract]
        void SetTreatmentOnline(Guid treatmnetId, bool isOnline);

        [OperationContract]
        [ApplyDataContractResolver]
        TreatmentEvent GetCurrentTreatmentEvent(Guid treatmentId);

        [OperationContract]
        void AddCommentToTreatmentEvent(Guid eventId, string comment);

        [OperationContract]
        byte[] GetCustomerImage(Guid customerId);

        [OperationContract]
        void UpdateCustomerImage(Guid customerId, byte[] imageBytes);

        [OperationContract]
        void MoveTreatment(Guid treatmentId, bool isLeft);

        [OperationContract]
        int CorrectAvailableTreatmentLength(Guid eventId);

        [OperationContract]
        int GetTotalCustomerCharged(Guid customerId);

        [OperationContract]
        [ApplyDataContractResolver]
        List<Claim> GetClaims(Guid companyId, DateTime start, DateTime end, bool showClosedClaims);

        [OperationContract]
        [ApplyDataContractResolver]
        void PostClaim(Claim claim);

        [OperationContract]
        void SubmitClaim(Guid claimId, int ActualScore);

        [OperationContract]
        [ApplyDataContractResolver]
        [ServiceKnownType(typeof(AnketInfo))]
        List<AnketInfo> GetAnkets(Guid companyId, DateTime start, DateTime end);

        [OperationContract]
        [ApplyDataContractResolver]
        Anket GenerateAnketDefault(Guid divisionId, DateTime period);

        [OperationContract]
        [ApplyDataContractResolver]
        void PostAnket(Anket anket);

        [OperationContract]
        string GenerateAnketReport(Guid anketId);

        [OperationContract]
        [ApplyDataContractResolver]
        Anket GetAnket(Guid anketId);

        [OperationContract]
        void DeleteAnket(Guid anketId);

        [OperationContract]
        void ReopenClaim(Guid claimId, string message);

        [OperationContract]
        bool PostTicketPayment(Guid ticketId, decimal pmtAmount);

        [OperationContract]
        List<SalesData> GetUniedReportTicketSalesDynamic(bool isClub, Guid divisionId);

        [OperationContract]
        [ApplyDataContractResolver]
        List<ChannelData> GetUniedReportSalesChannels(int days);
        
        [OperationContract]
        [ApplyDataContractResolver]
        List<ChannelData> GetUniedReportAmountTreatments(int days);
        [OperationContract]
        [ApplyDataContractResolver]
        List<SalesData> GetUniedReportVisitsDynamics(int kind);

        [OperationContract]
        [ApplyDataContractResolver]
        List<News> GetNews();

        [OperationContract]
        [ApplyDataContractResolver]
        void PostNews(News news);

        [OperationContract]
        [ApplyDataContractResolver]
        List<SshFolder> GetSshFolders();

        [OperationContract]
        [ApplyDataContractResolver]
        List<SshFile> GetSshFiles();

        [OperationContract]
        void EnqueueSshFile(Guid fileId);

        [OperationContract]
        void PostCreditTicketPayment(Guid ticketId, decimal bankComissionRur, DateTime paymentDate);

        [OperationContract]
        void PostSpendingTypeRemove(Guid spendingTypeId);
        [OperationContract]
        void PostIncomeTypeRemove(Guid incomeTypeId);

        [OperationContract]
        [ApplyDataContractResolver]
        List<CashInOrder> GetCashInOrders(Guid divisionId, DateTime start, DateTime end);

        [OperationContract]
        decimal GetCashAmount(Guid divisionId, DateTime dateTime);

        [OperationContract]
        void PostCashInOrder(Guid divisionId, Guid orderId, DateTime createdOn, string debet, decimal amount, Guid createdBy, Guid receivedBy, string reason);

        [OperationContract]
        [ApplyDataContractResolver]
        List<CashOutOrder> GetCashOutOrders(Guid divisionId, DateTime start, DateTime end);

        [OperationContract]
        void PostCashOutOrder(Guid divisionId, Guid orderId, DateTime createdOn, string debet, decimal amount, Guid createdBy, string receivedBy, string reason, string responsible);

        [OperationContract]
        string GetNotificationsForDivision(Guid divisionId);

        [OperationContract]
        decimal GetCashTodaysAmount(Guid divisionId);

        [OperationContract]
        void PostCustomerStatus(Guid statusId, string name);

        [OperationContract]
        void PostCustomerStatusDelete(Guid statusId);

        [OperationContract]
        List<SalesData> GetUniedReportIncomeAmount(bool isClub, Guid divisionId);

        [OperationContract]
        List<TicketsData> GetUniedReportAmountTicket(bool isClub, Guid divisionId);

        [OperationContract]
        [ApplyDataContractResolver]
        IEnumerable<TreatmentConfig> GetAllTreatmentConfigsAdmin();

        [OperationContract]
        [ApplyDataContractResolver]
        List<Ticket> GetTicketsForPlanning(Guid customerId, Guid divisionId);

        [OperationContract]
        [ApplyDataContractResolver]
        List<Ticket> GetTicketsForCustomer(Guid customerId);

        [OperationContract]
        void SetTreatmentAsVisited(Guid eventId);

        [OperationContract]
        void UnmissTreatment(List<Guid> treatmentIds);

        [OperationContract]
        [ApplyDataContractResolver]
        SshFile GetSshFile(Guid fileId);

        [OperationContract]
        [ApplyDataContractResolver]
        CustomerNotificationInfo GetCustomerNotificationInfo(Guid customerId);

        [OperationContract]
        [ApplyDataContractResolver]
        void PostTicketTypeCustomerCardTypes(Guid ttId, Guid[] ccTypes);

        [OperationContract]
        void PostEmployeeActive(Guid employeeId, bool active);

        [OperationContract]
        void SpreadPermission(Guid permisionId, bool maximum, bool franch, bool upravl, bool admins);

        [OperationContract]
        [ApplyDataContractResolver]
        List<CumulativeDiscount> GetCumulativeDiscounts(Guid companyId);

        [OperationContract]
        [ApplyDataContractResolver]
        void PostCumulative(CumulativeDiscount cumulative);

        [OperationContract]
        [ApplyDataContractResolver]
        CumulativeDiscountInfo GetCumulativeDiscountInfo(Guid customerId);

        [OperationContract]
        [ApplyDataContractResolver]
        List<CustomerEventView> GetCRMEvents(Guid customerId);

        [OperationContract]
        Guid PostCrmEvent(Guid id, Guid comapnyId, Guid divisionId, Guid customerid, DateTime date, string subject, string comment, string result);

        [OperationContract]
        [ApplyDataContractResolver]
        List<Package> GetPackages();

        [OperationContract]
        [ApplyDataContractResolver]
        Guid PostPackage(Package package, IEnumerable<PackageLine> packageLines);

        [OperationContract]
        [ApplyDataContractResolver]
        List<GoodReserve> GetGoodsReserve(Guid customerId);

        [OperationContract]
        bool GiveGoodToCustomer(Guid divisionId, Guid customerId, Guid goodId);

        [OperationContract]
        [ApplyDataContractResolver]
        List<Division> GetDivisionsForCompany(Guid companyId);

        [OperationContract]
        [ApplyDataContractResolver]
        List<CustomerTargetType> GetTargetTypes();///t

        [OperationContract]
        [ApplyDataContractResolver]
        void PostTargetType(CustomerTargetType targetType, Guid[] recomendations);///t

        [OperationContract]
        [ApplyDataContractResolver]
        ScheduleProposalResult GetSmartProposals(Guid customerId, Guid divisionId, Guid ticketId, DateTime visitDate, Guid targetId, bool allowParallel);///t

        [OperationContract]
        [ApplyDataContractResolver]
        void HideTargetTypeById(Guid targetTypeId);///t

        [OperationContract]
        [ApplyDataContractResolver]
        List<TargetTypeSet> GetTargetConfigs();///t

        [OperationContract]
        void PostTargetSet(Guid id, Guid targetId, string configs);///t

        [OperationContract]
        [ApplyDataContractResolver]
        List<BarDiscount> GetBarDiscounts();

        [OperationContract]
        [ApplyDataContractResolver]
        void PostBarDiscount(BarDiscount barDiscount);

        [OperationContract]
        [ApplyDataContractResolver]
        decimal GetBarDiscountForCustomer(Guid customerId);

        [OperationContract]
        [ApplyDataContractResolver]
        void PostRecurrentRule(ReportRecurrency reportRecurrency);

        [OperationContract]
        [ApplyDataContractResolver]
        IEnumerable<ReportRecurrency> GetRecurrentReports(Guid userId);

        [OperationContract]
        [ApplyDataContractResolver]
        [ServiceKnownType(typeof(CustomerTarget))]
        [ServiceKnownType(typeof(Ticket))]
        [ServiceKnownType(typeof(DepositOut))]
        [ServiceKnownType(typeof(BarOrder))]
        [ServiceKnownType(typeof(CustomerNotification))]
        [ServiceKnownType(typeof(Task))]
        WorkbenchInfo GetWorkbench(Guid divisionId, DateTime workbenchDate);

        [OperationContract]
        void PostBonusCorrection(Guid customerId, int amount);

        [OperationContract]
        void PostBonusCorrectionWithComment(Guid customerId, int amount, string comment);

        [OperationContract]
        void PostCustomerInvitor(Guid customerId, Guid? invitorId);

        [OperationContract]
        bool PostExtraSmart(Guid customerId, string comment);

        [OperationContract]
        [ApplyDataContractResolver]
        CustomerNotification GetCustomerNotificationById(Guid notificationId);
        
        [OperationContract]
        string GetLastVisitText(Guid customerId);

        [OperationContract]
        [FaultContract(typeof(string))]
        string GetFromSite(Guid divisionId, DateTime fromTime);

        [OperationContract]
        [ApplyDataContractResolver]
        IEnumerable<Tuple<string, List<string>>> GetTreatmentTypesForCustomerGoals(Guid divisionId, Guid customerId);
    }
}