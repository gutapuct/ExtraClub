using System;
using System.Collections.Generic;
using TonusClub.ServiceModel;
using TonusClub.Entities;

using System.Linq;
using Core = TonusClub.ServerCore.Core;
using TonusClub.ServerCore;
using TonusClub.ServiceModel.Organizer;

using TonusClub.ServiceModel.Reports;
using TonusClub.ServiceModel.Employees;
using TonusClub.ServiceModel.Schedule;



[ErrorHandlerBehavior(typeof(CustomErrorHandler))]
public class TonusService : ITonusService
{
    public string[] GetUserPermissions(Guid divisionId)
    {
        return Core.GetUserPermissions(divisionId);
    }

    public List<FoundCustomer> SearchCustomers(string searchKey)
    {
        return Core.SearchCustomers(searchKey);
    }

    public List<Good> GetAllGoods(Guid companyId)
    {
        return Core.GetAllGoods(companyId);
    }

    public Dictionary<string, DictionaryInfo> GetAllDictionaryInfos()
    {
        return Core.GetAllDictionaryInfos();
    }

    public Dictionary<Guid, string> GetDictionaryList(string tableName)
    {
        return Core.GetDictionaryList(tableName);
    }

    public List<Provider> GetAllProviders()
    {
        return Core.GetAllProviders();
    }

    public void PostGood(Good good)
    {
        Logger.DBLog("Добавление/обновление товара {0} c Id {1}", good.Name, good.Id);
        Core.PostEntities("Goods", new[] { good });
    }

    public List<Consignment> GetAllConsignments(Guid divisionId, DateTime start, DateTime end, bool cons)
    {
        return Core.GetAllConsignments(divisionId, start, end, cons);
    }

    public List<ProviderPayment> GetAllProviderPayments(Guid divisionId, DateTime start, DateTime end)
    {
        return Core.GetAllProviderPayments(divisionId, start, end);
    }

    public void PostConsignment(Consignment consignment)
    {
        Logger.DBLog("Добавление/обновление накладной {0} c Id {1}", consignment.Number, Core.PostConsignment(consignment));
    }

    public void PostConsignmentLines(IEnumerable<ConsignmentLine> changes)
    {
        Core.PostEntities("ConsignmentLines", changes);
    }

    public List<ConsignmentLine> GetAllConsignmentLines()
    {
        return Core.GetAllRecords<ConsignmentLine>("ConsignmentLines");
    }

    public List<GoodPrice> GetAllPrices(Guid divisionId)
    {
        return Core.GetAllPrices(divisionId);
    }

    public List<BarPointGood> GetGoodsPresence(Guid divisionId)
    {
        return Core.GetGoodsPresence(divisionId);
    }

    public List<Customer> GetPresentCustomers(Guid divisionId)
    {
        return Core.GetPresentCustomers(divisionId);
    }

    public PaymentDetails ProcessPayment(PaymentDetails details, IEnumerable<PayableItem> basket, Guid goodActionId)
    {
        var payableItems = basket as IList<PayableItem> ?? basket.ToList();
        Logger.DBLog("Отправка платежа на сумму {0}, всего позиций: {1}", details.RequestedAmount, payableItems.Count());
        return PaymentCore.ProcessPayment(details, payableItems, goodActionId);
    }

    public Customer GetCustomer(Guid customerId, bool loadDetails)
    {
        return Core.GetCustomer(customerId, loadDetails);
    }

    public User GetCurrentUser()
    {
        return Core.GetCurrentUser();
    }

    public int GetMaxPaymentNumber()
    {
        return Core.GetMaxPaymentNumber();
    }

    public Division GetDivision(Guid divisionId)
    {
        return Core.GetDivision(divisionId);
    }

    public IEnumerable<TicketType> GetTicketTypes(bool activeOnly)
    {
        return Core.GetTicketTypes(activeOnly);
    }

    public PaymentDetails ProcessReturn(PaymentDetails pmt, IEnumerable<PayableItem> items)
    {
        var payableItems = items as IList<PayableItem> ?? items.ToList();
        Logger.DBLog("Отправка возврата на сумму {0}, всего позиций: {1}", pmt.RequestedAmount, payableItems.Count());
        return PaymentCore.ProcessReturn(pmt, payableItems);
    }

    public IEnumerable<CustomerCardType> GetCustomerCardTypes(bool activeOnly)
    {
        return Core.GetCustomerCardTypes(activeOnly);
    }

    public bool RegisterCustomerVisit(Guid customerId, Guid divisionId, int shelfNumber, int safeNumber)
    {
        return Core.RegisterCustomerVisit(customerId, divisionId, shelfNumber, safeNumber);
    }

    public List<GoodAction> GetGoodActions(bool onlyActive)
    {
        var res = Core.GetAllRecords<GoodAction>("GoodActions");
        if (onlyActive) return res.Where(ga => ga.IsActive).ToList();
        return res;
    }

    public void PostGoodAction(Guid actionId, string actionName, double discount, IEnumerable<KeyValuePair<Guid, int>> goods, bool isActive)
    {
        Core.PostGoodAction(actionId, actionName, discount, goods, isActive);
    }

    public void DeleteGoodAction(Guid goodActionId)
    {
        Core.DeleteGoodAction(goodActionId);
    }

    public void SetObjectActive(string collectionName, Guid id, bool isActive)
    {
        Logger.DBLog("Задание активности для объекта группы {0}, Id = {1}, активность {2}", collectionName, id, isActive);

        Core.SetObjectActive(collectionName, id, isActive);
    }

    public void PostGoodPrice(GoodPrice goodPrice)
    {
        Core.PostGoodPrice(goodPrice);
    }

    public void PostProviderPayment(Guid orderId, DateTime date, string paymentType, decimal amount, string comment)
    {
        Logger.DBLog("Платеж поставщику по заказу {0} на сумму {1}", orderId, amount);
        Core.PostProviderPayment(orderId, date, paymentType, amount, comment);
    }

    public void ActivateTicket(Guid ticketId)
    {
        Core.ActivateTicket(ticketId);
    }

    public Guid RegisterCustomerVisitEnd(Guid customerId, Guid divisionId, bool shelfReturned, bool safeReturned)
    {
        return Core.RegisterCustomerVisitEnd(customerId, divisionId, shelfReturned, safeReturned);
    }

    public Guid PostNewDictionaryElement(Guid dictionaryId, string newElementName)
    {
        Logger.DBLog("Добавление элемента в справочник {0}, элемент {1}", dictionaryId, newElementName);
        return Core.PostNewDictionaryElement(dictionaryId, newElementName);
    }

    public void PostRenameDictionaryElement(Guid dictionaryId, Guid elementGuid, string elementName)
    {
        Logger.DBLog("Редактирование элемента в справочнике {0}, элемент {1}, новое значение {2}", dictionaryId, elementGuid, elementName);
        Core.PostRenameDictionaryElement(dictionaryId, elementGuid, elementName);
    }

    public string PostRemoveDictionaryElement(Guid dictionaryId, Guid elementGuid)
    {
        Logger.DBLog("Удаление элемента из справочника {0}, элемент {1}", dictionaryId, elementGuid);
        return Core.PostRemoveDictionaryElement(dictionaryId, elementGuid);
    }

    public Guid PostCustomer(Customer customer)
    {
        Logger.DBLog("Добавление/редактирование анкеты клиента {0}, имя {1}", customer.Id, customer.FullName);
        using (var context = new TonusEntities())
        {
            var user = UserManagement.GetUser(context);
            customer.ManagerId = user.EmployeeId;
            return Core.PostEntities("Customers", new[] { customer }).FirstOrDefault();
        }
    }

    public List<Treatment> GetAllTreatments(Guid divisionId)
    {
        return Core.GetAllTreatments(divisionId);
    }

    public Ticket GetTicketById(Guid ticketId)
    {
        var res = Core.GetOneRecord<Ticket>("TonusEntities.Tickets", "Id", ticketId, true);
        return res;
    }

    public IEnumerable<TreatmentConfig> GetAllTreatmentConfigs()
    {
        return Core.GetAllTreatmentConfigs();
    }

    public void PostTreatment(Treatment treatment)
    {
        Logger.DBLog("Добавление/редактирование тренажера {0}, наименование {1}", treatment.Id, treatment.DisplayName);

        Core.PostEntities("Treatments", new[] { treatment });
    }

    public IEnumerable<TreatmentSeqRest> GetAllTreatmentSeqRests()
    {
        return Core.GetAllRecords<TreatmentSeqRest>("TreatmentSeqRests", false);
    }


    public void PostTreatmentSeqRest(TreatmentSeqRest treatmentSeqRest)
    {
        Logger.DBLog("Добавление/редактирование ограничения {0}", treatmentSeqRest.Id);

        Core.PostEntities("TreatmentSeqRests", new[] { treatmentSeqRest });
    }

    public ScheduleProposalResult GetScheduleProposals(Guid customerId, Guid divId, Guid ticketId, DateTime visitDate, bool isParallelAllowed, List<Guid> treatments, bool isOptimalAllowed, Guid programId)
    {
        return ScheduleCore.GetScheduleProposals(customerId, divId, ticketId, visitDate, isParallelAllowed, treatments, isOptimalAllowed);
    }

    public bool PostScheduleProposal(Guid customerId, Guid divisionId, Guid ticketId, ScheduleProposal scheduleProposal)
    {
        ScheduleCore.PostScheduleProposal(customerId, divisionId, ticketId, scheduleProposal);
        return true;
    }

    public Customer GetCustomerByCard(int cardNumber, bool loadDetails)
    {
        return Core.GetCustomerById(cardNumber, loadDetails);
    }

    public List<TreatmentProgram> GetTreatmentPrograms()
    {
        return Core.GetAllRecords<TreatmentProgram>("TreatmentPrograms").Where(i => i.IsAvail).OrderBy(i => i.ProgramName).ToList();
    }

    public List<AdvertType> GetAdvertTypes()
    {
        return Core.GetAllRecords<AdvertType>("AdvertTypes", false).Where(i => i.IsAvail).OrderBy(i => i.Name).ToList();
    }

    public void UpdateCustomerForm(Customer customer)
    {
        Logger.DBLog("Обновление анкеты у клиента {0} ({1})", customer.Id, customer.FullName);
        Core.UpdateCustomerForm(customer);
    }

    public List<string>[] GetAddressLists()
    {
        return Core.GetAddressLists();
    }

    public List<decimal> GetDiscountsForCurrentUser(short discountType)
    {
        return Core.GetDiscountsForCurrentUser(discountType);
    }

    public string UpdateInvitor(Guid invitedId, Guid invitorId)
    {
        return Core.UpdateInvitor(invitedId, invitorId);
    }

    public IEnumerable<TicketType> GetActiveTicketTypesForCustomer(Guid customerId)
    {
        return Core.GetActiveTicketTypesForCustomer(customerId);
    }

    public int GetMaxGuestUnits(Guid divisionId, Guid customerId)
    {
        return Core.GetMaxGuestUnits(divisionId, customerId);
    }

    public string GenerateCardContractReport(string cardNumber, string reportKey)
    {
        return TemplatesCore.GenerateCardContractReport(cardNumber, reportKey);
    }

    public void PostCustomerAddress(Guid customerId, string metro, string index, string city, string street, string other)
    {
        Core.PostCustomerAddress(customerId, metro, index, city, street, other);
    }

    public string GenerateTicketContractReport(Guid parameter, object parameter1, string reportKey)
    {
        return TemplatesCore.GenerateTicketContractReport(parameter, parameter1, reportKey);
    }

    public Company GetCompnay()
    {
        return Core.GetCompany();
    }

    public List<TicketFreezeReason> GetTicketFreezeReasons()
    {
        return Core.GetAllRecords<TicketFreezeReason>("TicketFreezeReasons", false).Where(t => !t.IsReturnReason).OrderBy(i => i.Name).ToList();
    }

    public void CancelTreatmentEvents(List<Guid> events)
    {
        ScheduleCore.CancelTreatmentEvents(events);
    }

    public void PostCustomerCardType(CustomerCardType customerCardType)
    {
        Logger.DBLog("Добавление/изменение типа карты. {0} ({1})", customerCardType.Name, customerCardType.Id);
        Core.PostEntities("CustomerCardTypes", new[] { customerCardType });
    }

    public bool DeleteObject(string collectionName, Guid id)
    {
        Logger.DBLog("Удаление объекта. {0} ({1})", collectionName, id);

        Core.DeleteObject(collectionName, id);
        return true;
    }

    public Guid PostTicketType(TicketType ticketType)
    {
        Logger.DBLog("Добавление/изменение типа абонемента. {0} ({1})", ticketType.Name, ticketType.Id);

        var res = Core.PostEntities("TicketTypes", new[] { ticketType })[0];
        Core.MaintainTicketTypeVisibility(res);
        return res;
    }

    public void PostTicketTypeTreatmentTypes(Guid ticketTypeId, List<Guid> treatmentTypes)
    {
        Core.PostTicketTypeTreatmentTypes(ticketTypeId, treatmentTypes);
    }

    public void ClearCustomerContras(Guid customerId)
    {
        Core.ClearCustomerContras(customerId);
    }

    public List<ContraIndication> GetAllContras()
    {
        var res = Core.GetAllRecords<ContraIndication>("ContraIndications", false).Where(i => i.IsVisible).ToList();
        res.Sort((i, j) => String.Compare(i.Name, j.Name, StringComparison.Ordinal));
        return res;
    }

    public List<Guid> GetCustomerContrasIds(Guid customerId)
    {
        return Core.GetCustomerContrasIds(customerId);
    }

    public void PostContraIndications(Guid customerId, List<Guid> contraIds)
    {
        Core.PostContraIndications(customerId, contraIds);
    }

    public Guid PostContraIndication(ContraIndication contraIndication)
    {
        Logger.DBLog("Добавление/изменение противопоказания. {0} ({1})", contraIndication.Name, contraIndication.Id);

        return Core.PostEntities("ContraIndications", new[] { contraIndication })[0];
    }

    public void PostContraIndicationTreatmentTypes(Guid contraId, List<Guid> treatmentIds)
    {
        Core.PostContraIndicationTreatmentTypes(contraId, treatmentIds);
    }

    public List<CustomerTarget> GetCustomerTargets(Guid customerId)
    {
        return Core.GetCustomerTargets(customerId);
    }

    public Guid PostCustomerTarget(CustomerTarget customerTarget)
    {
        if (customerTarget.TargetText == null) customerTarget.TargetText = "";
        var res = Core.PostEntities("CustomerTargets", new[] { customerTarget })[0];
        Logger.DBLog("Добавление/изменение цели клиента. {0} ({1})", customerTarget.TargetText, res);
        return res;
    }

    public List<OrganizerItem> GetOrganizerItems()
    {
        return OrganizerCore.GetOrganizerItems(DateTime.Today.AddYears(-3), DateTime.Today.AddYears(3));
    }

    public void StartTicketReturn(Guid ticketId, string comment)
    {
        Core.StartTicketReturn(ticketId, comment);
    }

    public List<Anthropometric> GetAnthropometricsForCustomer(Guid customerId)
    {
        return Core.GetAnthropometricsForCustomer(customerId);
    }

    public Guid PostCustomerAnthropomentric(Anthropometric anthropometric)
    {
        Logger.DBLog("Добавление/изменение антропометрических данных клиенту {0}", anthropometric.CustomerId);
        return Core.PostCustomerAnthropomentric(anthropometric);
    }

    public List<DoctorVisit> GetDoctorVisitsForCustomer(Guid customerId)
    {
        return Core.GetDoctorVisitsForCustomer(customerId);
    }

    public Guid PostCustomerDoctorVisit(DoctorVisit doctorVisit)
    {
        Logger.DBLog("Добавление/изменение посещения врача клиентом {0}", doctorVisit.CustomerId);
        return Core.PostEntities("DoctorVisits", new[] { doctorVisit })[0];
    }

    public List<string> GetDoctorTemplates()
    {
        return Core.GetGetDoctorTemplates();
    }

    public List<Nutrition> GetNutritionsForCustomer(Guid customerId)
    {
        return Core.GetNutritionsForCustomer(customerId);
    }

    public Guid PostNutrition(Nutrition nutrition)
    {
        Logger.DBLog("Добавление/изменение дневника питания клиента {0}", nutrition.CustomerId);
        return Core.PostEntities("Nutritions", new[] { nutrition })[0];
    }

    public List<string>[] GetNutritionTemplates()
    {
        return Core.GetNutritionTemplates();
    }

    public List<CustomerMeasure> GetMeasuresForCustomer(Guid customerId)
    {
        return Core.GetMeasuresForCustomer(customerId);
    }

    public Guid PostCustomerMeasure(CustomerMeasure measure)
    {
        Logger.DBLog("Добавление/изменение контрольного замера клиента {0}", measure.CustomerId);
        return Core.PostMeasure(measure);
    }

    public List<TreatmentType> GetAllTreatmentTypes()
    {
        return Core.GetAllTreatmentTypes();
    }

    public Guid? GetTreatmentProgramIdForCustomer(Guid customerId)
    {
        return ScheduleCore.GetTreatmentProgramIdForCustomer(customerId);
    }

    public List<TreatmentEvent> GetCustomerEvents(Guid customerId, DateTime start, DateTime end, bool canceled)
    {
        return Core.GetCustomerEvents(customerId, start, end, canceled);
    }

    public List<TextAction> GetCurrentActions(Guid divisionId)
    {
        return Core.GetCurrentActions(divisionId);
    }

    public List<TreatmentTypeGroup> GetAllTreatmentTypeGroups()
    {
        return Core.GetAllRecords<TreatmentTypeGroup>("TreatmentTypeGroups", false).Where(i => i.IsAvail).OrderBy(i => i.Name).ToList();
    }

    public Guid PostTreatmentType(TreatmentType treatmentType)
    {
        Logger.DBLog("Добавление/изменение типа процедуры (TreatmentType) {0} ({1})", treatmentType.Name, treatmentType.Id);
        return Core.PostTreatmentType(treatmentType);
    }

    public Guid PostTreatmentConfig(TreatmentConfig treatmentConfig)
    {
        Logger.DBLog("Добавление/изменение типа процедуры (TreatmentConfig) {0} ({1})", treatmentConfig.Name, treatmentConfig.Id);
        return Core.PostTreatmentConfig(treatmentConfig);
    }

    public List<GoodSale> GetBarOrdersForCustomer(Guid customerId, DateTime startDate, DateTime endDate)
    {
        return Core.GetBarOrdersForCustomer(customerId, startDate, endDate);
    }

    public void PostCompany(Company company)
    {
        Logger.DBLog("Редактирование данных франчайзи {0}", company.CompanyName);
        Core.PostCompany(company);
    }

    public Guid PostTreatmentProgram(TreatmentProgram treatmentProgram, List<Guid> lines)
    {
        Logger.DBLog("Добавление/изменение программы занятий {0} ({1})", treatmentProgram.ProgramName, treatmentProgram.Id);
        return Core.PostTreatmentProgram(treatmentProgram, lines);
    }

    public List<TextAction> GetAllActions()
    {
        return Core.GetAllRecords<TextAction>("TextActions").OrderBy(i => i.StartDate).ToList();
    }

    public Guid PostTextAction(TextAction textAction, Guid[] divisionIds)
    {
        Logger.DBLog("Добавление/изменение информера {0} ({1})", textAction.ActionText, textAction.Id);
        var res = Core.PostEntities("TextActions", new[] { textAction })[0];
        Core.SetActionDivisions(res, divisionIds);
        return res;
    }

    public void DeleteParallelRule(TreatmentsParalleling rule)
    {
        ScheduleCore.DeleteParallelRule(rule);
    }

    public List<TreatmentsParalleling> GetParallelingRules()
    {
        return ScheduleCore.GetParallelingRules();
    }

    public void PostTreatmentsParalleling(TreatmentsParalleling old, TreatmentsParalleling rule)
    {
        Logger.DBLog("Добавление/изменение типов параллельно проводимых процедур {0}-{1}", rule.Type1Id, rule.Type2Id);
        ScheduleCore.PostTreatmentsParalleling(old, rule);
    }

    public List<Guid> GetCustomerStatusesIds(Guid customerId)
    {
        return Core.GetCustomerStatusesIds(customerId);
    }

    public void PostCustomerStatuses(Guid customerId, List<Guid> list)
    {
        Core.PostCustomerStatuses(customerId, list);
    }

    public List<ReportTemplate> GetAllTemplates()
    {
        return Core.GetAllTemplates();
    }

    public void PostReportTemplate(ReportTemplate template)
    {
        Logger.DBLog("Редактирование шаблона документов \"{0}\"", template.Name);
        Core.PostReportTemplate(template);
    }

    public void PostTreatmentBreakdown(Guid treatmentId)
    {
        ScheduleCore.PostTreatmentBreakdown(treatmentId);
    }

    public List<ChildrenRoom> GetCustomerChildren(Guid customerId)
    {
        return Core.GetCustomerChildren(customerId);
    }

    public string GenerateChildRequestReport(Guid childId, string repType)
    {
        return TemplatesCore.GenerateChildRequestReport(childId, repType);
    }

    public void PostDivision(Division division)
    {
        Logger.DBLog("Редактирование общей информации о клубе {0}", division.Name);
        Core.PostDivision(division);
    }

    public List<int> GetAvailableShelfNumbers(Guid divisionId, bool isSafe)
    {
        return Core.GetAvailableShelfNumbers(divisionId, isSafe);
    }

    public List<CustomerShelf> GetCustomerShelves(Guid customerId, bool isSafe)
    {
        return Core.GetCustomerShelves(customerId, isSafe);
    }

    public List<CustomerShelf> GetDivisionShelves(Guid divisionId, DateTime startPeriod, DateTime endPeriod, bool isSafe)
    {
        return Core.GetDivisionShelves(divisionId, startPeriod, endPeriod, isSafe);
    }

    public List<Solarium> GetDivisionSolariums(Guid divisionId, bool activeOnly)
    {
        return Core.GetDivisionSolariums(divisionId, activeOnly);
    }

    public void PostSolarium(Solarium solarium)
    {
        Logger.DBLog("Добавление/изменение солярия {0} ({1})", solarium.Name, solarium.Id);
        Core.PostEntities("Solariums", new[] { solarium });
    }

    public Dictionary<int, string> GetSolariumWarnings()
    {
        return Core.GetSolariumWarnings();
    }

    public Guid PostSolariumBooking(Guid divisionId, Guid customerId, Guid solariumId, DateTime dateTime, int amount, string comment)
    {
        return Core.PostSolariumBooking(divisionId, customerId, solariumId, dateTime, amount, comment);
    }

    public Guid PostSolariumBookingEx(Guid divisionId, Guid customerId, Guid solariumId, DateTime dateTime, int amount, string comment, Guid? ticketId)
    {
        return Core.PostSolariumBookingEx(divisionId, customerId, solariumId, dateTime, amount, comment, ticketId);
    }

    public KeyValuePair<Guid, DateTime> GetSolariumProposal(Guid divisionId, Guid customerId, DateTime dateTime, int amount, Guid selectedSolariumId, Guid toSkip)
    {
        return Core.GetSolariumProposal(divisionId, customerId, dateTime, amount, selectedSolariumId, toSkip);
    }

    public List<SolariumVisit> GetCustomerSolarium(Guid customerId)
    {
        return Core.GetCustomerSolarium(customerId);
    }

    public List<SolariumVisit> GetDivisionSolariumVisits(Guid divisionId, DateTime startDate, DateTime finishDate)
    {
        return Core.GetDivisionSolariumVisits(divisionId, startDate, finishDate);
    }

    public SolariumVisit GetSolariumVisitById(Guid visitId)
    {
        var res = Core.GetOneRecord<SolariumVisit>("TonusEntities.SolariumVisits", "Id", visitId, true);
        res.Init();
        return res;
    }

    public bool CancelSolariumEvent(Guid solVisitId, bool delete)
    {
        return Core.CancelSolariumEvent(solVisitId, delete);
    }

    public List<Ticket> GetCustomerTickets(Guid customerId)
    {
        return Core.GetCustomerTickets(customerId);
    }

    public void PostSolariumVisitStart(Guid solariumVisitId)
    {
        Logger.DBLog("Отметка начала процедуры в солярии ({0})", solariumVisitId);
        Core.PostSolariumVisitStart(solariumVisitId);
    }

    public void PostSolariumWarnings(List<KeyValuePair<int, string>> solariumWarnings)
    {
        Logger.DBLog("Добавление/изменение предупреждений о солярии ({0} шт.))", solariumWarnings.Count);
        Core.PostSolariumWarnings(solariumWarnings);
    }

    public List<CustomerCardType> GetAllCustomerCardTypes()
    {
        return Core.GetAllCustomerCardTypes();
    }

    public List<TicketType> GetAllTicketTypes()
    {
        return Core.GetAllTicketTypes();
    }

    public void PostCompanyCardTypeEnable(Guid ccardId, bool enable)
    {
        Logger.DBLog("Активация Типа карты у франчайзи {0} ({1}))", ccardId, enable);
        Core.PostCompanyCardTypeEnable(ccardId, enable);
    }

    public void PostCompanyTicketTypeEnable(Guid tTypeId, bool enable)
    {
        Logger.DBLog("Активация Типа абонемента у франчайзи {0} ({1}))", tTypeId, enable);
        Core.PostCompanyTicketTypeEnable(tTypeId, enable);
    }

    public List<ProviderFolder> GetProviderFolders()
    {
        return Core.GetProviderFolders();
    }

    public void PostProviderFolder(ProviderFolder providerFolder)
    {
        if (providerFolder.ParentFolderId == providerFolder.Id) providerFolder.ParentFolderId = null;
        Logger.DBLog("Добавление/изменение папки поставщика {0}", providerFolder.Name);
        Core.PostProviderFolder(providerFolder);
    }

    public void DeleteProviderFolder(Guid folderId)
    {
        Logger.DBLog("Удаление папки поставщика {0}", folderId);
        Core.DeleteProviderFolder(folderId);
    }

    public void PostProvider(Provider provider)
    {
        Logger.DBLog("Добавление/изменение поставщика {0}", provider.Name);
        if (provider.ProviderFolderId == Guid.Empty) provider.ProviderFolderId = null;
        Core.PostEntities("Providers", new[] { provider });
    }

    public void HideProvider(Guid providerId)
    {
        Logger.DBLog("Скрытие поставщика {0}", providerId);
        Core.HideProvider(providerId);
    }

    public void HideGood(Guid goodId)
    {
        Core.HideGood(goodId);
    }

    public List<Storehouse> GetStorehouses(Guid divisionId)
    {
        return Core.GetStorehouses(divisionId);
    }

    public void PostStorehouse(Storehouse storehouse)
    {
        Logger.DBLog("Добавление/изменение склада {0}", storehouse.Name);
        Core.PostEntities("Storehouses", new[] { storehouse });
    }

    public string GenerateConsignmentReport(Guid consId)
    {
        return TemplatesCore.GenerateConsignmentReport(consId);
    }

    public List<ChildrenRoom> GetDivisionChildren(Guid divisionId, DateTime start, DateTime end)
    {
        return Core.GetDivisionChildren(divisionId, start, end);
    }

    public List<GoodSale> GetDivisionSales(Guid divisionId, DateTime start, DateTime end)
    {
        return Core.GetDivisionSales(divisionId, start, end);
    }

    public List<GoodSale> GetCustomerSales(Guid customerId, DateTime start, DateTime end)
    {
        return Core.GetCustomerSales(customerId, start, end);
    }

    public string GenerateGoodReport(Guid goodSaleId, string reportKey)
    {
        return TemplatesCore.GenerateGoodReport(goodSaleId, reportKey);
    }

    public List<Certificate> GetDivisionCertificates(Guid divisionId)
    {
        return Core.GetDivisionCertificates(divisionId);
    }

    public void PostCertificate(Certificate certificate)
    {
        Core.PostCertificate(certificate);
    }

    public bool CancelCertificate(Guid certificateId)
    {
        Logger.DBLog("Отмена сертификата {0}", certificateId);
        return Core.CancelCertificate(certificateId);
    }

    public List<Rent> GetCustomerRent(Guid customerId)
    {
        return Core.GetCustomerRent(customerId);
    }

    public void PostRent(Rent rent)
    {
        Logger.DBLog("Добавление проката клиенту {0} на сумму {1}", rent.CustomerId, rent.Cost);
        Core.PostEntities("Rents", new[] { rent });
    }

    public string GenerateBarOrderReport(int orderId, string reportKey)
    {
        return TemplatesCore.GenerateBarOrderReport(orderId, reportKey);
    }

    public Certificate GetCertificateByNumber(int id)
    {
        return Core.GetCertificateByNumber(id);
    }

    public List<BarOrder> GetDivisionBarOrders(Guid divisionId, DateTime start, DateTime end)
    {
        return Core.GetDivisionBarOrders(divisionId, start, end);
    }

    public List<DepositAccount> GetCustomerDeposit(Guid customerId, DateTime start, DateTime end)
    {
        return Core.GetCustomerDeposit(customerId, start, end);
    }

    public void PostDepositAdd(Guid customerId, decimal amount, string description)
    {
        Core.PostDepositAdd(customerId, amount, description);
    }

    public Guid RequestDepositOut(Guid customerId, decimal amount)
    {
        return Core.RequestDepositOut(customerId, amount);
    }

    public string GenerateDepositOutStatementReport(Guid statementId)
    {
        return TemplatesCore.GenerateDepositOutStatementReport(statementId);
    }

    public void PostDepositOutDone(Guid depositOutId, string comment, bool isDone)
    {
        Core.PostDepositOutDone(depositOutId, comment, isDone);
    }

    public void FinalizeCashlessPayment(Guid orderId, string comments, bool isSuccessful)
    {
        PaymentCore.FinalizeCashlessPayment(orderId, comments, isSuccessful);
    }

    public void ProcessBankReturn(Guid orderId)
    {
        PaymentCore.ProcessBankReturn(orderId);
    }

    public List<Spending> GetDivisionSpendings(Guid divisionId, DateTime start, DateTime end)
    {
        return Core.GetDivisionSpendings(divisionId, start, end);
    }

    public List<SpendingType> GetDivisionSpendingTypes(Guid divisionId)
    {
        return Core.GetDivisionSpendingTypes(divisionId);
    }

    public void PostSpending(Spending spending)
    {
        Logger.DBLog("Добавление затраты {0}", spending.Id);
        Core.PostSpending(spending);
    }

    public void PostSpendingType(Guid divisionId, Guid typeId, string name)
    {
        Logger.DBLog("Добавление/изменение типа затрат {0} {1}", typeId, name);
        Core.PostSpendingType(divisionId, typeId, name);
    }

    public string GeneratePriceListReport(Guid divisionId)
    {
        return TemplatesCore.GeneratePriceListReport(divisionId);
    }

    public List<BonusAccount> GetCustomerBonus(Guid customerId, DateTime start, DateTime end)
    {
        return Core.GetCustomerBonus(customerId, start, end);
    }

    public List<EmployeeCategory> GetEmployeeCategories(Guid companyId)
    {
        return EmployeeCore.GetEmployeeCategories(companyId);
    }

    public IEnumerable<Job> GetJobs(Guid divisionId)
    {
        return EmployeeCore.GetJobs(divisionId, null);
    }

    public void PostEmployeeCategory(EmployeeCategory category, List<Guid> jobIds)
    {
        Logger.DBLog("Добавление/изменение категории должности {0} {1}", category.Id, category.Name);
        EmployeeCore.PostEmployeeCategory(category, jobIds);
    }

    public List<string> GetJobUnits(Guid divisionId)
    {
        return EmployeeCore.GetJobUnits(divisionId);
    }

    public void PostJob(Job job, List<Guid> categoryIds)
    {
        Logger.DBLog("Добавление/изменение должности {0} {1}", job.Id, job.Name);
        EmployeeCore.PostJob(job, categoryIds);
    }

    public string GetBaselineStatus(Guid divisionId)
    {
        return EmployeeCore.GetBaselineStatus(divisionId);
    }

    public void BaselineJobs(Guid divisionId)
    {
        Logger.DBLog("Утверждение штатного расписания в клубе {0}", divisionId);
        EmployeeCore.BaselineJobs(divisionId);
    }

    public List<Employee> GetEmployees(Guid divisionId, bool presentOnly, bool asuOnly)
    {
        return EmployeeCore.GetEmployees(divisionId, presentOnly, asuOnly);
    }

    public Guid PostEmployeeForm(Employee employee, Customer customer)
    {
        Logger.DBLog("Изменение анкетных данных сотрудника {0} {1}", employee.Id, customer.FullName);
        return EmployeeCore.PostEmployeeForm(employee, customer);
    }

    public List<Job> GetAvailableJobs(Guid divisionId)
    {
        return EmployeeCore.GetAvailableJobs(divisionId);
    }

    public Guid PostJobPlacement(JobPlacement placement)
    {
        Logger.DBLog("Оформление сотрудника {0} на должность {1}", placement.EmployeeId, placement.JobId);
        return EmployeeCore.PostJobPlacement(placement);
    }

    public JobPlacement GetJobPlacementDraft(Guid employeeId)
    {
        return EmployeeCore.GetJobPlacementDraft(employeeId);
    }

    public Employee GetEmployeeById(Guid id)
    {
        return EmployeeCore.GetEmployeeById(id);
    }

    public string GenerateEmployeeReport(Guid employeeId, string reportKey)
    {
        return TemplatesCore.GenerateEmployeeReport(employeeId, reportKey);
    }

    public string GetEmployeeCardStatusMessage(string cardNumber)
    {
        return EmployeeCore.GetEmployeeCardStatusMessage(cardNumber);
    }

    public void PostEmployeeCard(Guid employeeId, string newCardNumber)
    {
        Logger.DBLog("Выдача карты сотруднику {0} ({1})", employeeId, newCardNumber);
        EmployeeCore.PostEmployeeCard(employeeId, newCardNumber);
    }

    public Guid PostJobPlacementChange(JobPlacement jobPlacement)
    {
        Logger.DBLog("Переоформление сотрудника {0} на должность {1}", jobPlacement.EmployeeId, jobPlacement.JobId);
        return EmployeeCore.PostJobPlacementChange(jobPlacement);
    }

    public Guid PostEmployeeFire(Guid employeeId, DateTime fireDate, string fireCause, bool cardReturned, decimal totalToPay, decimal bonus, decimal ndfl, string logMessage)
    {
        Logger.DBLog("Увольнение сотрудника {0}", employeeId);
        return EmployeeCore.PostEmployeeFire(employeeId, fireDate, fireCause, cardReturned, totalToPay, bonus, ndfl, logMessage);
    }

    public Guid PostEmployeeVacation(Guid employeeId, DateTime beginDate, DateTime endDate, byte vacType, decimal totalToPay, decimal ndfl, string logMessage)
    {
        Logger.DBLog("Отпуск сотрудника {0} с {1} по {2}", employeeId, beginDate, endDate);
        return EmployeeCore.PostEmployeeVacation(employeeId, beginDate, endDate, vacType, totalToPay, ndfl, logMessage);
    }

    public Guid[] PostEmployeeTrip(Guid[] employeeIds, DateTime beginDate, DateTime endDate, string destination, string target, string tripBase)
    {
        Logger.DBLog("Командировка сотрудников ({0} чел) с {1} по {2}", employeeIds.Length, beginDate, endDate);
        return EmployeeCore.PostEmployeeTrip(employeeIds, beginDate, endDate, destination, target, tripBase);
    }


#if BEAUTINIKA
    public void PostLastTicketCorrectionBeau(Guid customerId, DateTime corrActivated, int corrAmount, int corrExtra, int corrGuest, decimal paidAmt, int solCorr)
    {
        Logger.DBLog("Коррекция последнего абонемента клиента {0}", customerId);
        Core.PostLastTicketCorrection(customerId, corrActivated, corrAmount, corrExtra, corrGuest, paidAmt, solCorr);
    }
#endif


    public void PostLastTicketCorrection(Guid customerId, DateTime corrActivated, int corrAmount, int corrGuest, decimal paidAmt, int solCorr)
    {
#if BEAUTINIKA
#else
        Core.PostLastTicketCorrection(customerId, corrActivated, corrAmount, corrGuest, paidAmt, solCorr);
#endif
    }

    public Guid PostEmployeeCategoryChange(Guid employeeId, Guid categoryId)
    {
        Logger.DBLog("Присвоение сотруднику {0} катагории {1}", employeeId, categoryId);
        return EmployeeCore.PostEmployeeCategoryChange(employeeId, categoryId);
    }

    public List<EmployeeDocument> GetEmployeeDocuments(Guid divisionId)
    {
        return EmployeeCore.GetEmployeeDocuments(divisionId);
    }

    public int GetActiveEmployeesCountForJobId(Guid jobId)
    {
        return EmployeeCore.GetActiveEmployeesCountForJobId(jobId);
    }

    public int GetActiveEmployeesCountForCategoryId(Guid categoryId)
    {
        return EmployeeCore.GetActiveEmployeesCountForCategoryId(categoryId);
    }

    public void HideEmployeeCategoryById(Guid categoryId)
    {
        Logger.DBLog("Скрытие катагории {0}", categoryId);
        EmployeeCore.HideEmployeeCategoryById(categoryId);
    }

    public void HideEmployeeJobById(Guid jobId)
    {
        Logger.DBLog("Скрытие должности {0}", jobId);
        EmployeeCore.HideEmployeeJobById(jobId);
    }

    public string GenerateStateScheduleReport(Guid divisionId)
    {
        return TemplatesCore.GenerateStateScheduleReport(divisionId);
    }

    public Employee GetEmployeeByCard(string cardNumber)
    {
        return EmployeeCore.GetEmployeeByCard(cardNumber);
    }

    public void PostEmployeeVisit(Guid employeeId, bool isIn)
    {
        EmployeeCore.PostEmployeeVisit(employeeId, isIn);
    }

    public List<VacationList> GetVacationHistory(Guid divisionId)
    {
        return EmployeeCore.GetVacationHistory(divisionId);
    }

    public List<VacationPreference> GetEmployeePreferences(Guid employeeId)
    {
        return EmployeeCore.GetEmployeePreferences(employeeId);
    }

    public void PostEmployeePreference(Guid employeeId, DateTime beginDate, DateTime endDate, short prefType)
    {
        Logger.DBLog("Предпочтения сотрудника {0} с {1} по {2}, тип {3}", employeeId, beginDate, endDate, prefType);
        EmployeeCore.PostEmployeePreference(employeeId, beginDate, endDate, prefType);
    }

    public List<EmployeeScheduleProposalElement> GenerateScheduleProposal(Guid divisionId, int year, int recDays)
    {
        return EmployeeCore.GenerateScheduleProposal(divisionId, year, recDays);
    }

    public Guid PostEmployeeVacationsSchedule(List<EmployeeScheduleProposalElement> list)
    {
        Logger.DBLog("Новое расписание отпусков");
        return EmployeeCore.PostEmployeeVacationsSchedule(list);
    }

    public List<EmployeeScheduleProposalElement> GetCurrentEmployeeVacationsSchedule(Guid divisionId)
    {
        return EmployeeCore.GetCurrentEmployeeVacationsSchedule(divisionId);
    }

    public List<EmployeeScheduleProposalElement> GetEmployeeVacationsSchedule(Guid listId)
    {
        return EmployeeCore.GetEmployeeVacationsSchedule(listId);
    }

    public string GenerateEmployeeVacationList(Guid listId)
    {
        return TemplatesCore.GenerateEmployeeVacationList(listId);
    }

    public List<EmployeeWorkScheduleItem> GetEmployeeWorkSchedule(Guid divisionId, DateTime start, DateTime finish)
    {
        return EmployeeCore.GetEmployeeWorkSchedule(divisionId, start, finish);
    }

    public Guid PostEmployeeSchedule(Guid divisionId, Dictionary<Guid, List<DateTime>> schedule)
    {
        Logger.DBLog("Новое расписание работы в клубе {0}", divisionId);
        return EmployeeCore.PostEmployeeSchedule(divisionId, schedule);
    }

    public string GenerateEmployeeScheduleReport(Guid graphId)
    {
        return TemplatesCore.GenerateEmployeeScheduleReport(graphId);
    }

    public List<EmployeeWorkGraph> GetWorkGraphs(Guid divisionId)
    {
        return EmployeeCore.GetWorkGraphs(divisionId);
    }

    public List<DateTime> GetHolidays(int year)
    {
        return EmployeeCore.GetHolidays(year);
    }

    public void PostHoliday(DateTime date)
    {
        Logger.DBLog("Гос праздник {0}", date);
        EmployeeCore.PostHoliday(date);
    }

    public void DeleteHoliday(DateTime date)
    {
        Logger.DBLog("Гос праздник (удаление) {0}", date);
        EmployeeCore.DeleteHoliday(date);
    }

    public string GetNotificationsForUser()
    {
        return OrganizerCore.GetNotificationsForUser();
    }

    public List<SalaryScheme> GetSalarySchemes(Guid divisionId)
    {
        return EmployeeCore.GetSalarySchemes(divisionId);
    }

    public void PostSalaryScheme(Guid divisionId, SalaryScheme salaryScheme)
    {
        Logger.DBLog("Добавление/редактирование схемы зарплаты {0}", salaryScheme.Name);
        EmployeeCore.PostSalaryScheme(divisionId, salaryScheme);
    }

    public decimal CalculateSalary(Guid employeeId, DateTime calcMonth)
    {
        string log;
        return EmployeeCore.CalculateSalary(employeeId, calcMonth, out log);
    }

    public List<SalarySheet> GetSalarySheets(Guid divisionId)
    {
        return EmployeeCore.GetSalarySheets(divisionId);
    }

    public List<SalarySheetRow> GenerateSalarySheet(Guid divisionId, DateTime genMonth)
    {
        return EmployeeCore.GenerateSalarySheet(divisionId, genMonth);
    }

    public string PostSalarySheet(Guid divisionId, DateTime period, List<SalarySheetRow> lines)
    {
        Logger.DBLog("Добавление ведомости на зарплату {0} по клубу {1}", period, divisionId);
        return EmployeeCore.PostSalarySheet(divisionId, period, lines);
    }

    public List<SalarySheetRow> GetSalarySheetLines(Guid sheetId)
    {
        return EmployeeCore.GetSalarySheetLines(sheetId);
    }

    public string PostEmployeePayment(Guid employeeId, Guid sheetId, decimal amount)
    {
        Logger.DBLog("Добавление выплаты зарплаты сотруднику {0} на сумму {1}", employeeId, amount);
        return EmployeeCore.PostEmployeePayment(employeeId, sheetId, amount);
    }

    public bool CheckSalarySheet(Guid divisionId, DateTime genDate)
    {
        return EmployeeCore.CheckSalarySheet(divisionId, genDate);
    }

    public KeyValuePair<decimal, string> GetFireSalary(Guid employeeId, DateTime fireDate)
    {
        return EmployeeCore.GetFireSalary(employeeId, fireDate);
    }

    public KeyValuePair<decimal, string> GetEmployeeVacationPmt(Guid employeeId, DateTime beginDate, DateTime endDate)
    {
        return EmployeeCore.GetEmployeeVacationPmt(employeeId, beginDate, endDate);
    }

    public KeyValuePair<decimal, string> GetEmployeeIllnessPmt(Guid employeeId, DateTime beginDate, DateTime endDate)
    {
        return EmployeeCore.GetEmployeeIllnessPmt(employeeId, beginDate, endDate);
    }

    public List<Corporate> GetCorporates(Guid companyId)
    {
        return Core.GetCorporates(companyId);
    }

    public bool PostCorporate(Guid companyId, Guid corpId, string name, Guid? folderId)
    {
        Logger.DBLog("Добавление/редактирование корпоративного договора {0}", name);
        return Core.PostCorporate(companyId, corpId, name, folderId);
    }

    public bool DeleteCorporate(Guid corpId)
    {
        Logger.DBLog("Удаление корпоративного договора {0}", corpId);
        return Core.DeleteCorporate(corpId);
    }

    public List<TreatmentEvent> GetDivisionTreatmetnsVisits(Guid divisionId, DateTime start, DateTime finish)
    {
        return ScheduleCore.GetDivisionTreatmetnsVisits(divisionId, start, finish);
    }

    public List<ScheduleProposalElement> FixSchedule(Guid divisionId, Guid customerId, Guid ticketId, List<ScheduleProposalElement> list)
    {
        return ScheduleCore.FixSchedule(divisionId, customerId, ticketId, list);
    }

    public List<EmployeePayment> GetDivisionEmployeeCashflow(Guid divisionId)
    {
        return EmployeeCore.GetDivisionEmployeeCashflow(divisionId);
    }

    public TreatmentEvent GetTreatmentEventById(Guid treatmentEventId)
    {
        return ScheduleCore.GetTreatmentEventById(treatmentEventId);
    }

    public void PostTreatmentEventChange(Guid treatmentEventId, DateTime newTime)
    {
        ScheduleCore.PostTreatmentEventChange(treatmentEventId, newTime);
    }

    public void PostFilePart(Guid divisionId, Guid fileId, string fileName, byte[] part, int bytes, int? category, Guid parameter)
    {
        FileCore.PostFilePart(divisionId, fileId, fileName, part, bytes, category, parameter);
    }

    public List<File> GetDivisionFiles(Guid divisionId)
    {
        return FileCore.GetDivisionFiles(divisionId);
    }

    public byte[] GetFilePart(Guid fileId, int blockNumber)
    {
        return FileCore.GetFilePart(fileId, blockNumber);
    }

    public void PostSalesPlan(Guid divisionId, DateTime month, decimal amount, decimal amountCorp, DateTime oldMonth)
    {
        Logger.DBLog("Добавление/редактирование плана продаж клуба {0} на месяц {1}", divisionId, month);
        EmployeeCore.PostSalesPlan(divisionId, month, amount, amountCorp, oldMonth);
    }

    public List<SalesPlan> GetSalesPlanForDivsion(Guid divisionId)
    {
        return EmployeeCore.GetSalesPlanForDivsion(divisionId);
    }

    public List<IncomingCallForm> GetCallScrenarioForms()
    {
        return Core.GetAllRecords<IncomingCallForm>("IncomingCallForms", false).OrderBy(i => i.Header).ToList();
    }

    public void PostIncomingCallForm(IncomingCallForm incomingCallForm)
    {
        Logger.DBLog("Редактирование формы сценария звонка {0}", incomingCallForm.Header);
        Core.PostIncomingCallForm(incomingCallForm);
    }

    public void PostNewCall(Guid divisionId, string log, CallResult callResult, Guid? customerId, bool isIncoming, DateTime started, string goal, string result)
    {
        Logger.DBLog("Регистрация звонка {0}", started);
        OrganizerCore.PostNewCall(divisionId, log, callResult, customerId, isIncoming, started, goal, result);
    }

    public List<Call> GetDivisionCalls(Guid divisionId, DateTime callsStart, DateTime callsEnd)
    {
        return OrganizerCore.GetDivisionCalls(divisionId, callsStart, callsEnd);
    }

    public void PostClubClosing(Guid divisionId, DateTime start, DateTime end, string cause)
    {
        Logger.DBLog("Задача на закрытие клуба {0} c {1} по {2}", divisionId, start, end);
        ScheduleCore.PostClubClosing(divisionId, start, end, cause);
    }

    public void PostNotificationCompletion(Guid divisionId, Guid notificationId, string comment, string result)
    {
        Logger.DBLog("Регистрация завершения оповещения {0}", notificationId);
        OrganizerCore.PostNotificationCompletion(divisionId, notificationId, comment, result);
    }

    public List<Customer> GetAllCustomers()
    {
        return Core.GetCustomers(t => true);
    }

    public List<Customer> GetPotentialCustomers()
    {
        return Core.GetCustomers(i => !i.CustomerCards.Any() || (i.CustomerCards.Count == 1 && i.CustomerCards.Any(c => c.CustomerCardType.IsGuest || c.CustomerCardType.IsVisit)));
    }

    public List<Customer> GetInactiveCustomers()
    {
        return Core.GetCustomers(i => !i.Tickets.Any(j => j.IsActive) && i.CustomerCards.Any());
    }

    public List<Customer> GetActiveCustomers()
    {
        return Core.GetCustomers(i => i.Tickets.Any(j => j.IsActive));
    }

    public List<Customer> GetCustomersByStatus(List<Guid> statIds)
    {
        return Core.GetCustomersByStatus(statIds);
    }

    public List<Customer> GetCustomersByManagers(List<Guid> managerIds)
    {
        return Core.GetCustomersByManagers(managerIds);
    }

    public List<Employee> GetEmployeesWorkingAt(Guid divisionId, DateTime date)
    {
        return EmployeeCore.GetEmployeesWorkingAt(divisionId, date);
    }

    public void PostGroupCall(Guid divisionId, Guid[] customers, Guid[] employees, string comments, DateTime runDate, DateTime expityDate)
    {
        Logger.DBLog("Добавление задачи на групповой звонок {0} клиентам", customers.Length);
        OrganizerCore.PostGroupCall(divisionId, customers, employees, comments, runDate, expityDate);
    }

    public void PostReturnReject(Guid ticketId)
    {
        Core.PostReturnReject(ticketId);
    }

    public void PostIncorrectPhoneTask(Guid divisionId, Guid cnId, Guid customerId, string comments)
    {
        OrganizerCore.PostIncorrectPhoneTask(divisionId, cnId, customerId, comments);
    }

    public void PostTaskClosing(Guid taskId, bool isCompleted, string comment, DateTime date)
    {
        Logger.DBLog("Регистрация завершения задачи {0}", taskId);
        OrganizerCore.PostTaskClosing(taskId, isCompleted, comment, date);
    }

    public void PostNewTask(Guid[] employees, string subject, string comments, DateTime expiryDate, int priority)
    {
        Logger.DBLog("Добавление задачи {0}", subject);
        OrganizerCore.PostNewTask(employees, subject, comments, expiryDate, priority);
    }

    public List<Guid> GetEmployeeIdsWithPermission(Guid divisionId, string permissionName)
    {
        return Core.GetEmployeeIdsWithPermission(divisionId, permissionName);
    }

    public List<OrganizerItem> GetOutboxOrganizerItems()
    {
        return OrganizerCore.GetOutboxOrganizerItems();
    }

    public void PostTaskRecall(Guid elementId)
    {
        Logger.DBLog("Отзыв задачи {0}", elementId);
        OrganizerCore.PostTaskRecall(elementId);
    }

    public List<OrganizerItem> GetArchivedOrganizerItems()
    {
        return OrganizerCore.GetArchivedOrganizerItems();
    }

    public void PostTaskReopen(Guid elementId)
    {
        Logger.DBLog("Возобновление задачи {0}", elementId);
        OrganizerCore.PostTaskReopen(elementId);
    }

    public List<Call> GetCustomerCalls(Guid customerId)
    {
        return OrganizerCore.GetCustomerCalls(customerId);
    }

    public List<Division> GetDivisions()
    {
        return Core.GetDivisions();
    }

    public List<Role> GetRoles()
    {
        return AdminCore.GetRoles();
    }

    public List<Permission> GetAllPermissions()
    {
        return AdminCore.GetAllPermissions();
    }

    public void PostRole(Guid roleId, string name, Guid[] permissionIds, string cardDisc, string ticketDisc, string ticketRubDisc, Guid? folderId)
    {
        Logger.DBLog("Добавление/редактирование роли {0} {1}", roleId, name);
        AdminCore.PostRole(roleId, name, permissionIds, cardDisc, ticketDisc, ticketRubDisc, folderId);
    }

    public void DeleteRole(Guid roleId)
    {
        Logger.DBLog("Удаление роли {0}", roleId);
        AdminCore.DeleteRole(roleId);
    }

    public List<User> GetUsers()
    {
        return AdminCore.GetUsers();
    }

    public Guid PostNewUser(Guid employeeId, string userName, string fullName, string password, string email)
    {
        Logger.DBLog("Добавление пользователя для сотрудника {0} ({1})", employeeId, userName);
        return AdminCore.PostNewUser(employeeId, userName, fullName, password, email);
    }

    public void PostUser(Guid userId, string fullName, bool isActive, Guid[] roleIds, string email)
    {
        Logger.DBLog("Редактирование пользователя {0}", userId);
        AdminCore.PostUser(userId, fullName, isActive, roleIds, email);
    }

    public void ResetPassword(Guid userId, string password)
    {
        Logger.DBLog("Сброс пароля для пользователя {0}", userId);
        AdminCore.ResetPassword(userId, password);
    }

    public string ChangePassword(Guid userId, string oldPassword, string newPassword)
    {
        Logger.DBLog("Смена пароля для пользователя {0}", userId);
        return AdminCore.ChangePassword(userId, oldPassword, newPassword);
    }

    public void PostInstalmentDelete(Guid instalmentId)
    {
        Logger.DBLog("Скрытие типа рассрочки {0}", instalmentId);
        Core.PostInstalmentDelete(instalmentId);
    }

    public void PostInstalment(Instalment instalment)
    {
        Logger.DBLog("Добавление/редактирование типа рассрочки {0}", instalment.Name);
        Core.PostInstalment(instalment);
    }

    public IEnumerable<ReportInfoInt> GetUserReportsList()
    {
        return ReportsCore.GetUserReportsList();
    }

    public byte[] GenerateReport(string key, Dictionary<string, object> parameters)
    {
        return ReportsCore.GenerateReport(key, parameters);
    }

    public List<CompanyFinance> GetCompanyFinances(Guid companyId)
    {
        return Core.GetCompanyFinances(companyId);
    }

    public void PostCompanyFinance(Guid companyId, DateTime period, decimal accLeft)
    {
        Logger.DBLog("Добавление/редактирование финансовой информации компании {0} за период {1}", companyId, period);
        Core.PostCompanyFinance(companyId, period, accLeft);
    }

    public List<DivisionFinance> GetDivisionFinances(Guid divisionId)
    {
        return Core.GetDivisionFinances(divisionId);
    }

    public void PostDivisionFinance(Guid divisionId, DateTime dateTime, decimal cash, decimal unsent, decimal advances, decimal loan, decimal accumulated)
    {
        Logger.DBLog("Добавление/редактирование финансовой информации клуба {0} за период {1}", divisionId, dateTime);
        Core.PostDivisionFinance(divisionId, dateTime, cash, unsent, advances, loan, accumulated);
    }

    public List<IncomeType> GetDivisionIncomeTypes(Guid divisionId)
    {
        return Core.GetDivisionIncomeTypes(divisionId);
    }

    public void PostIncomeType(Guid divisionId, Guid id, string name)
    {
        Logger.DBLog("Добавление/редактирование типа доходов {0}", name);
        Core.PostIncomeType(divisionId, id, name);
    }

    public void PostIncome(Income income)
    {
        Logger.DBLog("Добавление доходa {0}", income.Amount);
        Core.PostIncome(income);
    }

    public List<Income> GetDivisionIncomes(Guid divisionId, DateTime start, DateTime end)
    {
        return Core.GetDivisionIncomes(divisionId, start, end);
    }

    public void DoSync()
    {
        Logger.DBLog("Принудительная синхронизация");
        SyncCore.Syncronize();
    }

    public void DeleteTreatmentProgram(Guid programId)
    {
        Logger.DBLog("Удаление программы занятий {0}", programId);
        Core.DeleteTreatmentProgram(programId);
    }

    public LocalSetting GetLocalSettings()
    {
        return Core.GetLocalSetting();
    }

    public void PostLocalSettings(int keyDays, int keyPeriod, int licDays, int licPeriod, string notifyAddresses)
    {
        Logger.DBLog("Сохранение настроек промежуточного сервера");
        Core.PostLocalSettings(keyDays, keyPeriod, licDays, licPeriod, notifyAddresses);
    }

    public void UpdateLicenseKey()
    {
        Logger.DBLog("Запрос лицензионного ключа");
        SyncCore.UpdateLicenseKey();
    }

    public Dictionary<Guid, string> GetCustomerTargetTypes()
    {
        return new TonusEntities().CustomerTargetTypes.Where(i => i.IsAvail).ToDictionary(i => i.Id, i => i.Name);
    }

    public void PostUserReport(ReportInfoInt report)
    {
        Logger.DBLog("Добавление/редактирование пользовательского отчета {0}", report.Name);
        ReportsCore.PostUserReport(report);
    }

    public CustomReport GetReportForEdit(Guid id)
    {
        return ReportsCore.GetReportForEdit(id);
    }

    public List<SettingsFolder> GetSettingsFolders(int categoryId, bool companyOnly)
    {
        return AdminCore.GetSettingsFolders(categoryId, companyOnly);
    }

    public void PostSettingsFolder(SettingsFolder folder, Guid[] companies)
    {
        Logger.DBLog("Добавление/редактирование папки адмики {0}", folder.Name);
        Core.PostSettingsFolder(folder, companies);
    }

    public void DeleteSettingsFolder(Guid folderId)
    {
        Logger.DBLog("Удаление папки адмики {0}", folderId);
        Core.DeleteSettingsFolder(folderId);
    }

    public void DeleteContraIndication(Guid contraId)
    {
        Logger.DBLog("Удаление противопоказания {0}", contraId);
        Core.DeleteContraIndication(contraId);
    }

    public List<CompanySettingsFolder> GetCompanySettingsFolders(int categoryId, Guid divId)
    {
        return AdminCore.GetCompanySettingsFolders(categoryId, divId);
    }

    public void PostCompanySettingsFolder(CompanySettingsFolder folder, Guid divId)
    {
        Logger.DBLog("Добавление/редактирование папки адмики уровня франчайзи {0}", folder.Name);
        Core.PostCompanySettingsFolder(folder, divId);
    }

    public void SetTreatmentEventColor(Guid eventId, int colorId)
    {
        Logger.DBLog("Раскрашивание процедуры {0}", eventId);
        ScheduleCore.SetTreatmentEventColor(eventId, colorId);
    }

    public List<CompanyView> GetCompaniesListForFolder(Guid folderId)
    {
        return AdminCore.GetCompaniesListForFolder(folderId);
    }

    public void PostPermissions(string[] auths)
    {
        Logger.DBLog("Обновление списка пермишенов");
        AdminCore.PostPermissions(auths);
    }

    public List<Company> GetCompanies()
    {
        return AdminCore.GetCompanies();
    }

    public void PostNewCompany(string name, string login, string password, Guid roleId, string reportEmail, int utcCorr, string userPrefix)
    {
        Logger.DBLog("Добавление новой компании {0}", name);
        AdminCore.PostNewCompany(name, login, password, roleId, reportEmail, utcCorr, userPrefix);
    }

    public void PostNewDivision(Division division)
    {
        Logger.DBLog("Добавление нового клуба {0}", division.Name);
        AdminCore.PostNewDivision(division);
    }

    public Guid GetCustomerIdByPhone(string phone)
    {
        return Core.GetCustomerIdByPhone(phone);
    }

    public Dictionary<Guid, string> GetAllStatuses()
    {
        return Core.GetAllStatuses();
    }

    public Guid GetCustomerIdByTargetId(Guid targetId)
    {
        return Core.GetCustomerIdByTargetId(targetId);
    }

    public Guid GetCustomerIdByTreatmentEventId(Guid treatmentEventId)
    {
        return Core.GetCustomerIdByTreatmentEventId(treatmentEventId);
    }

    public Guid GetCustomerIdByTicketId(Guid guid)
    {
        return Core.GetCustomerIdByTicketId(guid);
    }

    public Guid GetCustomerByCardId(Guid guid)
    {
        return Core.GetCustomerByCardId(guid);
    }

    public Guid GetCustomerByGoodSale(Guid guid)
    {
        return Core.GetCustomerByGoodSale(guid);
    }

    public void PostParameters(string reportKey, Dictionary<string, object> parameters, string setName)
    {
        ReportsCore.PostParameters(reportKey, parameters, setName);
    }

    public void DeleteReport(Guid savedId, string key, ReportType reportType)
    {
        Logger.DBLog("Удаление отчета {0} {1}", key, savedId);
        ReportsCore.DeleteReport(savedId, key, reportType);
    }

    public void PostMarkdown(Guid divisionId, Guid storeId, Guid goodId, string newName, decimal price, int amount, Guid provId)
    {
        Core.PostMarkdown(divisionId, storeId, goodId, newName, price, amount, provId);
    }

    public List<CustomerVisit> GetCustomerVisits(Guid customerId)
    {
        return Core.GetCustomerVisits(customerId);
    }

    public List<string>[] GetWorkData()
    {
        return Core.GetWorkData();
    }

    public void PostTicketTypeLimits(Guid ttId, KeyValuePair<Guid, int>[] lims)
    {
        Core.PostTicketTypeLimits(ttId, lims);
    }

    public IEnumerable<Instalment> GetAllInstalments()
    {
        return Core.GetAllRecords<Instalment>("Instalments", false).Where(i => i.IsActive).OrderBy(i => i.Name).ToList();
    }

    public IEnumerable<Instalment> GetCompanyInstalments(bool activeOnly)
    {
        return Core.GetCompanyInstalments(activeOnly);
    }

    public void PostCompanyInstalmentEnable(Guid instalmentId, bool isEnabled)
    {
        Core.PostCompanyInstalmentEnable(instalmentId, isEnabled);
    }
#if BEAUTINIKA
    public void PostTicketCorrection(Guid guid, int NewLength, int NewUnits, int newExtra, int NewGuest, int NewSolarium, int NewFreeze, string comment, DateTime? planInstDate)
    {
        Core.PostTicketCorrection(guid, NewLength, NewUnits, newExtra, NewGuest, NewSolarium, NewFreeze, comment, planInstDate);
    }
#else
    public void PostTicketCorrection(Guid guid, int newLength, int newUnits, int newGuest, int newSolarium, int newFreeze, string comment, DateTime? planInstDate, string newComment)
    {
        Core.PostTicketCorrection(guid, newLength, newUnits, newGuest, newSolarium, newFreeze, comment, planInstDate, newComment);
    }
#endif

    public string GenerateDivisionReport(string key, Guid divisionId)
    {
        return TemplatesCore.GenerateCardContractReport(key, divisionId);
    }

    public string HideProviderOrder(Guid id)
    {
        return Core.HideProviderOrder(id);
    }

    public string GetMarkdownName(Guid goodId)
    {
        return Core.GetMarkdownName(goodId);
    }

    public void MarkTreatmentsVisited(Guid[] visited)
    {
        ScheduleCore.MarkTreatmentsVisited(visited);
    }

    public List<UnitCharge> GetCustomerUnitCharges(Guid customerId, DateTime start, DateTime end, bool addGuest)
    {
        return Core.GetCustomerUnitCharges(customerId, start, end, addGuest);
    }

    public void UpdateVisitReceipt(Guid visitId, string receipt)
    {
        Core.UpdateVisitReceipt(visitId, receipt);
    }

    public bool IsFirstVisitEnabled(Guid customerId)
    {
        return Core.IsFirstVisitEnabled(customerId);
    }

    public List<AdvertGroup> GetAdvertGroups()
    {
        return AdminCore.GetAdvertGroups();
    }

    public void PostAdvertGroup(Guid groupId, string name)
    {
        AdminCore.PostAdvertGroup(groupId, name);
    }

    public void DeleteAdvertGroup(Guid groupId)
    {
        AdminCore.DeleteAdvertGroup(groupId);
    }

    public void PostAdvertType(Guid typeId, string name, bool commentNeeded, Guid groupId)
    {
        AdminCore.PostAdvertType(typeId, name, commentNeeded, groupId);
    }

    public void DeleteAdvertType(Guid typeId)
    {
        AdminCore.DeleteAdvertType(typeId);
    }

    public List<ScheduleProposalElement> GetAvailableParallels(Guid treatmentEventId)
    {
        return ScheduleCore.GetAvailableParallels(treatmentEventId);
    }

    public Guid PostParallelSigning(Guid originalEventId, Guid configId, Guid treatmentId, DateTime startTime)
    {
        return ScheduleCore.PostParallelSigning(originalEventId, configId, treatmentId, startTime);
    }

    public string GenerateFirstRun(KeyValuePair<string, string>[] parameters, Guid divisionId)
    {
        return TemplatesCore.GenerateFirstRun(parameters, divisionId);
    }

    public Treatment GetTreatmentByMac(string macAddress)
    {
        return Core.GetTreatmentByMac(macAddress);
    }

    public TreatmentEvent GetCustomerPlanningForTreatment(Guid customerId, Guid treatmentId)
    {
        return ScheduleCore.GetCustomerPlanningForTreatment(customerId, treatmentId);
    }

    public void PostTreatmentStart(Guid treatmentId, DateTime newTime)
    {
        ScheduleCore.PostTreatmentStart(treatmentId, newTime);
    }

    public HWProposal GetHWProposal(Guid treatmentId, Guid customerId)
    {
        return ScheduleCore.GetHWProposal(treatmentId, customerId);
    }

    public TreatmentEvent PostNewTreatmentEvent(Guid ticketId, Guid treatmentId, DateTime visitStart)
    {
        return ScheduleCore.PostNewTreatmentEvent(ticketId, treatmentId, visitStart);
    }

    public void SetTreatmentOnline(Guid treatmnetId, bool isOnline)
    {
        Core.SetTreatmentOnline(treatmnetId, isOnline);
    }

    public TreatmentEvent GetCurrentTreatmentEvent(Guid treatmentId)
    {
        return ScheduleCore.GetCurrentTreatmentEvent(treatmentId);
    }

    public string GenerateTicketReceiptReport(Guid pmtId, string reportKey)
    {
        return TemplatesCore.GenerateTicketReceiptReport(pmtId, reportKey);
    }

    public void AddCommentToTreatmentEvent(Guid eventId, string comment)
    {
        Core.AddCommentToTreatmentEvent(eventId, comment);
    }

    public byte[] GetCustomerImage(Guid customerId)
    {
        return Core.GetCustomerImage(customerId);
    }

    public void UpdateCustomerImage(Guid customerId, byte[] imageBytes)
    {
        Core.UpdateCustomerImage(customerId, imageBytes);
    }

    public void MoveTreatment(Guid treatmentId, bool isLeft)
    {
        Core.MoveTreatment(treatmentId, isLeft);
    }

    public int CorrectAvailableTreatmentLength(Guid eventId)
    {
        return ScheduleCore.CorrectAvailableTreatmentLength(eventId);
    }


    public List<OrganizerItem> GetOrganizerItemsEx(DateTime start, DateTime end)
    {
        return OrganizerCore.GetOrganizerItems(start, end);
    }

    public int GetTotalCustomerCharged(Guid customerId)
    {
        return Core.GetTotalCustomerCharged(customerId);
    }

    public List<Claim> GetClaims(Guid companyId, DateTime start, DateTime end, bool showClosedClaims)
    {
        return OrganizerCore.GetClaims(companyId, start, end, showClosedClaims);
    }

    public void PostClaim(Claim claim)
    {
        OrganizerCore.PostClaim(claim);
    }

    public void SubmitClaim(Guid claimId, int ActualScore)
    {
        OrganizerCore.SubmitClaim(claimId, ActualScore);
    }

    public List<AnketInfo> GetAnkets(Guid companyId, DateTime start, DateTime end)
    {
        return OrganizerCore.GetAnkets(companyId, start, end);
    }

    public Anket GenerateAnketDefault(Guid divisionId, DateTime period)
    {
        return OrganizerCore.GenerateAnketDefault(divisionId, period);
    }

    public void PostAnket(Anket anket)
    {
        OrganizerCore.PostAnket(anket);
    }

    public string GenerateAnketReport(Guid anketId)
    {
        return TemplatesCore.GenerateAnketReport(anketId);
    }

    public Anket GetAnket(Guid anketId)
    {
        return OrganizerCore.GetAnket(anketId);
    }

    public void DeleteAnket(Guid anketId)
    {
        OrganizerCore.DeleteAnket(anketId);
    }

    public void ReopenClaim(Guid claimId, string message)
    {
        OrganizerCore.ReopenClaim(claimId, message);
    }

    public bool PostTicketPayment(Guid ticketId, decimal pmtAmount)
    {
        Core.PostTicketPayment(ticketId, pmtAmount);
        return true;
    }

    public List<SalesData> GetUniedReportTicketSalesDynamic(bool isClub, Guid divisionId)
    {
        return UniedReportCore.GetUniedReportTicketSalesDynamic(isClub, divisionId);
    }


    public List<ChannelData> GetUniedReportSalesChannels(int days)
    {
        return UniedReportCore.GetUniedReportSalesChannels(days);
    }

    public List<ChannelData> GetUniedReportAmountTreatments(int days)
    {
        return UniedReportCore.GetUniedReportAmountTreatments(days);
    }

    public List<SalesData> GetUniedReportVisitsDynamics(int kind)
    {
        return UniedReportCore.GetUniedReportVisitsDynamics(kind);
    }

    public List<News> GetNews()
    {
        return Core.GetNews();
    }


    public void PostNews(News news)
    {
        Core.PostEntities("News", new[] { news });
    }

    public List<TonusClub.ServiceModel.Ssh.SshFolder> GetSshFolders()
    {
        return Core.GetSshFolders();
    }

    public List<SshFile> GetSshFiles()
    {
        return Core.GetSshFiles();
    }

    public void EnqueueSshFile(Guid fileId)
    {
        Core.EnqueueSshFile(fileId);
    }

    public void PostCreditTicketPayment(Guid ticketId, decimal bankComissionRur, DateTime paymentDate)
    {
        PaymentCore.PostCreditTicketPayment(ticketId, bankComissionRur, paymentDate);
    }


    public void PostSpendingTypeRemove(Guid spendingTypeId)
    {
        Core.PostSpendingTypeRemove(spendingTypeId);
    }

    public void PostIncomeTypeRemove(Guid incomeTypeId)
    {
        Core.PostIncomeTypeRemove(incomeTypeId);
    }

    public List<CashInOrder> GetCashInOrders(Guid divisionId, DateTime start, DateTime end)
    {
        return FinancesCore.GetCashInOrders(divisionId, start, end);
    }

    public List<CashOutOrder> GetCashOutOrders(Guid divisionId, DateTime start, DateTime end)
    {
        return FinancesCore.GetCashOutOrders(divisionId, start, end);
    }

    public decimal GetCashAmount(Guid divisionId, DateTime dateTime)
    {
        return FinancesCore.GetCashAmount(divisionId, dateTime);
    }

    public void PostCashInOrder(Guid divisionId, Guid orderId, DateTime createdOn, string debet, decimal amount, Guid createdBy, Guid receivedBy, string reason)
    {
        FinancesCore.PostCashInOrder(divisionId, orderId, createdOn, debet, amount, createdBy, receivedBy, reason);
    }

    public void PostCashOutOrder(Guid divisionId, Guid orderId, DateTime createdOn, string debet, decimal amount, Guid createdBy, string receivedBy, string reason, string responsible)
    {
        FinancesCore.PostCashOutOrder(divisionId, orderId, createdOn, debet, amount, createdBy, receivedBy, reason, responsible);
    }

    public string GeneratePKOReport(Guid cashInOrderId)
    {
        return TemplatesCore.GeneratePKOReport(cashInOrderId);
    }
    public string GenerateRKOReport(Guid cashOutOrderId)
    {
        return TemplatesCore.GenerateRKOReport(cashOutOrderId);
    }

    public string GenerateCashierPageReport(Guid divisionId, DateTime date)
    {
        return TemplatesCore.GenerateCashierPageReport(divisionId, date);
    }

    public string GetNotificationsForDivision(Guid divisionId)
    {
        return OrganizerCore.GetNotificationsForDivision(divisionId);
    }

    public decimal GetCashTodaysAmount(Guid divisionId)
    {
        return FinancesCore.GetCashTodaysAmount(divisionId);
    }

    public void PostCustomerStatus(Guid statusId, string name)
    {
        Core.PostCustomerStatus(statusId, name);
    }

    public void PostCustomerStatusDelete(Guid statusId)
    {
        Core.PostCustomerStatusDelete(statusId);
    }

    public List<SalesData> GetUniedReportIncomeAmount(bool isClub, Guid divisionId)
    {
        return UniedReportCore.GetUniedReportIncomeAmount(isClub, divisionId);
    }

    public List<TicketsData> GetUniedReportAmountTicket(bool isClub, Guid divisionId)
    {
        return UniedReportCore.GetUniedReportAmountTicket(isClub, divisionId);
    }

    public IEnumerable<TreatmentConfig> GetAllTreatmentConfigsAdmin()
    {
        return Core.GetAllTreatmentConfigsAdmin();
    }


    public List<Ticket> GetTicketsForPlanning(Guid customerId, Guid divisionId)
    {
        return ScheduleCore.GetTicketsForPlanning(customerId, divisionId);
    }

    public List<Ticket> GetTicketsForCustomer(Guid customerId)
    {
        return Core.GetTicketsForCustomer(customerId);
    }

    public void SetTreatmentAsVisited(Guid eventId)
    {
        ScheduleCore.SetTreatmentAsVisited(eventId);
    }

#if BEAUTINIKA
    public List<Room> GetRooms(Guid divisionId)
    {
        return BeautinikaCore.GetRooms(divisionId);
    }

    public Guid PostRoom(Guid roomId, Guid divisionId, string roomName, Guid[] availTreatments)
    {
        return BeautinikaCore.PostRoom(roomId, divisionId, roomName, availTreatments);
    }

    public BeautinikaProposal GetBeautinikaProposal(Guid divisionId, Guid customerId, Guid ticketId, DateTime date, Guid mainTreatmentId, Guid mainRoomId, Guid mainEmployeeId, Guid extraTreatmentId, Guid extraRoomId, Guid extraEmployeeId)
    {
        return BeautinikaCore.GetBeautinikaProposal(divisionId, customerId, ticketId, date, mainTreatmentId, mainRoomId, mainEmployeeId, extraTreatmentId, extraRoomId, extraEmployeeId);
    }

    public void PostBeautinikaProposal(Guid divId, BeautinikaProposal proposal)
    {
        BeautinikaCore.PostBeautinikaProposal(divId, proposal);
    }

    public List<UpcomingEventInfo> GetUpcomingEvents(Guid divisionId)
    {
        return BeautinikaCore.GetUpcomingEvents(divisionId);
    }

    public void PostTreatmentConfigGoods(Guid treatmentConfigId, List<TreatmentConfigGood> amounts)
    {
        BeautinikaCore.PostTreatmentConfigGoods(treatmentConfigId, amounts);
    }

    public List<TreatmentConfigGood> GetGoodsForTC(Guid treatmentConfigId)
    {
        return BeautinikaCore.GetGoodsForTC(treatmentConfigId);
    }

    public List<TonusClub.ServiceModel.Turnover.MaterialsReportLine> GetCurrentMaterialsAmount(Guid divisionId)
    {
        return BeautinikaCore.GetCurrentMaterialsAmount(divisionId);
    }

    public List<Recommendation> GetRecommendationsForCustomer(Guid customerId)
    {
        return BeautinikaCore.GetRecommendationsForCustomer(customerId);
    }

    public Guid PostRecommendation(Recommendation recommendation)
    {
        return BeautinikaCore.PostRecommendation(recommendation);
    }

    public List<CustomerTicketInfo> GetTicketsForCard(Guid companyId, string cardBarcode)
    {
        return BeautinikaCore.GetTicketsForCard(companyId, cardBarcode);
    }

    public void PostConsultation(Customer customer, DateTime dateTime, Guid employeeId, Guid roomId, Guid treatmentId)
    {
        BeautinikaCore.PostConsultation(customer, dateTime, employeeId, roomId, treatmentId);
    }

    public void PostMaterialCharges(Guid treatmentEventId, List<Tuple<Guid, decimal>> goods)
    {
        BeautinikaCore.PostMaterialCharges(treatmentEventId, goods);
    }

    public void PostTreatmentCosmetologist(Guid treatmentEventId, Guid employeeId)
    {
        BeautinikaCore.PostTreatmentCosmetologist(treatmentEventId, employeeId);
    }

    public void PostTreatmentConfigChange(Guid id, Guid configId, Guid treatmentId)
    {
        BeautinikaCore.PostTreatmentConfigChange(id, configId, treatmentId);
    }

#endif

    public void UnmissTreatment(List<Guid> treatmentIds)
    {
#if BEAUTINIKA
        BeautinikaCore.UnmissTreatment(treatmentIds);
#else
        treatmentIds.ForEach(ScheduleCore.SetTreatmentAsVisited);
#endif
    }

    public SshFile GetSshFile(Guid fileId)
    {
        return Core.GetSshFile(fileId);
    }

    public CustomerNotificationInfo GetCustomerNotificationInfo(Guid customerId)
    {
        return OrganizerCore.GetCustomerNotificationInfo(customerId);
    }

    public void PostTicketTypeCustomerCardTypes(Guid ttId, Guid[] ccTypes)
    {
        Core.PostTicketTypeCustomerCardTypes(ttId, ccTypes);
    }

    public void PostEmployeeActive(Guid employeeId, bool active)
    {
        EmployeeCore.PostEmployeeActive(employeeId, active);
    }

    public void SpreadPermission(Guid permisionId, bool maximum, bool franch, bool upravl, bool admins)
    {
        AdminCore.SpreadPermission(permisionId, maximum, franch, upravl, admins);
    }

    public List<CumulativeDiscount> GetCumulativeDiscounts(Guid companyId)
    {
        return Core.GetCumulativeDiscounts(companyId);
    }

    public void PostCumulative(CumulativeDiscount cumulative)
    {
        Core.PostEntities("CumulativeDiscounts", new[] { cumulative });
    }

    public CumulativeDiscountInfo GetCumulativeDiscountInfo(Guid customerId)
    {
        return Core.GetCumulativeDiscountInfo(customerId);
    }

    public List<CustomerEventView> GetCRMEvents(Guid customerId)
    {
        return Core.GetCRMEvents(customerId);
    }

    public Guid PostCrmEvent(Guid id, Guid companyId, Guid divisionId, Guid customerid, DateTime date, string subject, string comment, string result)
    {
        var user = UserManagement.GetUser();
        return Core.PostEntities("CustomerCrmEvents", new[] { new CustomerCrmEvent {
                Comment = comment,
                CompanyId = companyId,
                CreatedBy = user.UserId,
                CreatedOn = DateTime.Now,
                CustomerId = customerid,
                DivisionId = divisionId,
                EventDate = date,
                Id = id,
                Result = result,
                Subject = subject
            } })[0];
    }

    public List<Package> GetPackages()
    {
        return Core.GetPackages();
    }

    public Guid PostPackage(Package package, IEnumerable<PackageLine> packageLines)
    {
        return Core.PostPackage(package, packageLines);
    }

    public List<GoodReserve> GetGoodsReserve(Guid customerId)
    {
        return Core.GetGoodsReserve(customerId);
    }

    public bool GiveGoodToCustomer(Guid divisionId, Guid customerId, Guid goodId)
    {
        return Core.GiveGoodToCustomer(divisionId, customerId, goodId);
    }

    public List<Division> GetDivisionsForCompany(Guid companyId)
    {
        return Core.GetDivisionsForCompany(companyId);
    }

#if !BEAUTINIKA
    public List<CustomerTargetType> GetTargetTypes()
    {
        return Core.GetTargetTypes();
    }

    public void PostTargetType(CustomerTargetType targetType, Guid[] recomendations)
    {
        var id = Core.PostEntities("CustomerTargetTypes", new[] { targetType })[0];
        Core.SetTargetTypeRecs(id, recomendations);
    }

    public ScheduleProposalResult GetSmartProposals(Guid customerId, Guid divisionId, Guid ticketId, DateTime visitDate, Guid targetId, bool allowParallel)
    {
        return SmartCore.GetSmartProposals(customerId, divisionId, ticketId, visitDate, targetId, allowParallel);
    }
    public void HideTargetTypeById(Guid targetTypeId)
    {
        Core.HideTargetTypeById(targetTypeId);
    }

    public List<TargetTypeSet> GetTargetConfigs()
    {
        return Core.GetTargetConfigs();
    }

    public void PostTargetSet(Guid id, Guid targetId, string configs)
    {
        Core.PostEntities("TargetTypeSets", new[] { new TargetTypeSet { Id = id, TargetTypeId = targetId, TreatmentConfigIds = configs } });
    }

#endif

    public List<BarDiscount> GetBarDiscounts()
    {
        return Core.GetBarDiscounts();
    }

    public void PostBarDiscount(BarDiscount barDiscount)
    {
        Core.PostEntities("BarDiscounts", new[] { barDiscount });
    }

    public decimal GetBarDiscountForCustomer(Guid customerId)
    {
        return Core.GetBarDiscountForCustomer(customerId);
    }


    public void PostRecurrentRule(ReportRecurrency reportRecurrency)
    {
        Core.PostEntities("ReportRecurrencies", Enumerable.Repeat(reportRecurrency, 1));
    }

    public IEnumerable<ReportRecurrency> GetRecurrentReports(Guid userId)
    {
        return ReportsCore.GetRecurrentReports(userId);
    }

    public WorkbenchInfo GetWorkbench(Guid divisionId, DateTime workbenchDate)
    {
        return UniedReportCore.GetWorkbench(divisionId, workbenchDate);
    }

    public void PostBonusCorrection(Guid customerId, int amount)
    {
        Core.PostBonusCorrection(customerId, amount);
    }

    public void PostCustomerInvitor(Guid customerId, Guid? invitorId)
    {
        Core.PostCustomerInvitor(customerId, invitorId);
    }


    public void PostBonusCorrectionWithComment(Guid customerId, int amount, string comment)
    {
        Core.PostBonusCorrection(customerId, amount, comment);
    }

    public bool PostExtraSmart(Guid customerId, string comment)
    {
        return Core.PostExtraSmart(customerId, comment);
    }

    public CustomerNotification GetCustomerNotificationById(Guid notificationId)
    {
        return Core.GetCustomerNotificationById(notificationId);
    }

    public string GetLastVisitText(Guid customerId)
    {
        return ScheduleCore.GetLastVisitText(customerId);
    }

    public string GetFromSite(Guid divisionId, DateTime fromTime)
    {
        return Core.GetFromSite(divisionId, fromTime);
    }

    public IEnumerable<Tuple<string, List<string>>> GetTreatmentTypesForCustomerGoals(Guid divisionId, Guid customerId)
    {
        return Core.GetTreatmentTypesForCustomerGoals(divisionId, customerId);
    }
}