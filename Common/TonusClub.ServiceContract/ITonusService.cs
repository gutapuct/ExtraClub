using System;
using System.Collections.Generic;
using System.ServiceModel;
using TonusClub.ServiceModel;
using TonusClub.Entities;
using System.Collections;
using System.Data;
using System.Runtime.Serialization;
using System.Net.Security;

namespace TonusClub.ServiceContract
{

    [ServiceContract]
    public interface ITonusService
    {
        /// <summary>
        /// Validates user in system
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="sha1"></param>
        /// <returns>Returns current session token</returns>
        [OperationContract]
        Guid AuthorizeUser(string userName, string sha1);

        /// <summary>
        /// Gets user's permission key array by token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [OperationContract]
        string[] GetUserPermissions(Guid token);

        [OperationContract]
        [ApplyDataContractResolver]
        List<Customer> SearchCustomers(Guid token, string searchKey);

        [OperationContract]
        [ApplyDataContractResolver]
        List<Good> GetAllGoods(Guid token);

        [OperationContract]
        [ApplyDataContractResolver]
        Dictionary<string, DictionaryInfo> GetAllDictionaryInfos(Guid token);

        [OperationContract]
        Dictionary<Guid, string> GetDictionaryList(Guid token, string tableName);

        [OperationContract]
        [ApplyDataContractResolver]
        List<Provider> GetAllProviders(Guid token);

        [OperationContract]
        [ApplyDataContractResolver]
        void PostGoods(Guid token, IEnumerable<Good> changes);

        [OperationContract]
        [ApplyDataContractResolver]
        void PostProviders(Guid token, IEnumerable<Provider> changes);

        [OperationContract]
        [ApplyDataContractResolver]
        List<ProviderPayment> GetAllProviderPayments(Guid token);

        [OperationContract]
        [ApplyDataContractResolver]
        List<Consignment> GetAllConsignments(Guid token);

        [OperationContract]
        [ApplyDataContractResolver]
        void PostConsignments(Guid token, IEnumerable<Consignment> changes);

        [OperationContract]
        [ApplyDataContractResolver]
        void PostConsignmentLines(Guid _token, IEnumerable<ConsignmentLine> changes);

        [OperationContract]
        [ApplyDataContractResolver]
        List<ConsignmentLine> GetAllConsignmentLines(Guid token);

        [OperationContract]
        [ApplyDataContractResolver]
        List<GoodPrice> GetAllPrices(Guid token, Guid companyId);

        //[OperationContract]
        //[ApplyDataContractResolver]
        //void PostGoodPrices(Guid token, IEnumerable<GoodPrice> changes);

        /// <summary>
        /// Возвращает остатки товара по конкретному подразделению.
        /// Для получения остатков по филиалу необходимо послать Guid.Empty
        /// </summary>
        /// <param name="token"></param>
        /// <param name="divisionId"></param>
        /// <returns></returns>
        [OperationContract]
        List<BarPointGood> GetGoodsPresence(Guid token, Guid divisionId);

        [OperationContract]
        [ApplyDataContractResolver]
        List<Customer> GetPresentCustomers(Guid token, Guid divisionId);

        [ServiceKnownType(typeof(TicketPaymentPosition))]
        [ServiceKnownType(typeof(BarPointGood))]
        [ServiceKnownType(typeof(PayableItem))]
        [ServiceKnownType(typeof(PaymentDetails))]
        [ServiceKnownType(typeof(CustomerCardGood))]
        [ServiceKnownType(typeof(TicketGood))]
        [OperationContract]
        PaymentDetails ProcessPayment(Guid _token, PaymentDetails details, IEnumerable<PayableItem> basket);

        [OperationContract]
        [ApplyDataContractResolver]
        Customer GetCustomer(Guid token, Guid customerId, bool loadDetails);

        [OperationContract]
        [ApplyDataContractResolver]
        User GetCurrentUser(Guid token);

        [OperationContract]
        int GetMaxPaymentNumber(Guid token);

        [OperationContract]
        [ApplyDataContractResolver]
        Division GetDivision(Guid token, Guid guid);

        [OperationContract]
        [ApplyDataContractResolver]
        IEnumerable<TicketType> GetActiveTicketTypes(Guid token);

        [OperationContract]
        [ApplyDataContractResolver]
        Guid PostTicket(Guid token, Ticket ticket);

        [OperationContract]
        [ApplyDataContractResolver]
        void PostTicketFreeze(Guid token, TicketFreeze ticketFreeze);

        [ServiceKnownType(typeof(PaymentDetails))]
        [ServiceKnownType(typeof(TicketReturnPosition))]
        [OperationContract]
        PaymentDetails ProcessReturn(Guid token, PaymentDetails pmt, IEnumerable<PayableItem> items);

        [OperationContract]
        [ApplyDataContractResolver]
        Ticket PostTicketRebill(Guid token, Guid ticketId, Guid newCustomerId);

        [OperationContract]
        [ApplyDataContractResolver]
        Ticket PostTicketChange(Guid token, Guid oldTicketId, Guid newTicketType);

        [OperationContract]
        [ApplyDataContractResolver]
        IEnumerable<CustomerCardType> GetActiveCustomerCardTypes(Guid token);

        [OperationContract]
        [ApplyDataContractResolver]
        bool RegisterCustomerVisit(Guid token, Guid ticketId);

        [OperationContract]
        [ApplyDataContractResolver]
        List<GoodAction> GetGoodActions(Guid token, bool onlyActive);

        [OperationContract]
        void PostGoodAction(Guid token, Guid actionId, string actionName, double discount, IEnumerable<KeyValuePair<Guid, int>> goods, bool isActive);

        [OperationContract]
        void DeleteGoodAction(Guid token, Guid goodActionId);

        [OperationContract]
        void SetGoodActionEnabled(Guid token, Guid goodActionId, bool isEnabled);

        [OperationContract]
        [ApplyDataContractResolver]
        void PostGoodPrice(Guid _token, GoodPrice GoodPrice);

        [OperationContract]
        void PostProviderPayment(Guid token, Guid divisionId, Guid providerId, double amount, string comment, string number);

        [OperationContract]
        void ActivateTicket(Guid token, Guid ticketId);

        [OperationContract]
        bool RegisterCustomerVisitEnd(Guid token, Guid customerId, Guid divisionId);

        [OperationContract]
        Guid PostNewDictionaryElement(Guid token, Guid dictionaryId, string newElementName);

        [OperationContract]
        void PostRenameDictionaryElement(Guid token, Guid dictionaryId, Guid elementGuid, string elementName);

        [OperationContract]
        string PostRemoveDictionaryElement(Guid token, Guid dictionaryId, Guid elementGuid);

        [OperationContract]
        Guid PostCustomer(Guid token, Customer customer);

        [OperationContract]
        [ApplyDataContractResolver]
        List<Treatment> GetAllTreatments(Guid token);

        [OperationContract]
        [ApplyDataContractResolver]
        Ticket GetTicketById(Guid token, Guid ticketId);

        [OperationContract]
        [ApplyDataContractResolver]
        IEnumerable<TreatmentType> GetAllTreatmentTypes(Guid token);

        [OperationContract]
        [ApplyDataContractResolver]
        void PostTreatment(Guid token, Treatment Treatment);

        [OperationContract]
        [ApplyDataContractResolver]
        IEnumerable<TreatmentSeqRest> GetAllTreatmentSeqRests(Guid token);

        [OperationContract]
        [ApplyDataContractResolver]
        void PostTreatmentSeqRest(Guid token, TreatmentSeqRest treatmentSR);

        [OperationContract]
        [FaultContract(typeof(string))]
        [ApplyDataContractResolver]
        List<ScheduleProposal> GetScheduleProposals(Guid token, Guid customerId, Guid ticketId, DateTime visitDate, bool isFixedTime, List<Guid> treatments);

        [OperationContract]
        [FaultContract(typeof(string))]
        [ApplyDataContractResolver]
        bool PostScheduleProposal(Guid token,Guid ticketId, ScheduleProposal scheduleProposal);

        [OperationContract]
        [ApplyDataContractResolver]
        Customer GetCustomerByCard(Guid token, int cardNumber);

        [OperationContract]
        [ApplyDataContractResolver]
        List<TreatmentProgram> GetTreatmentPrograms(Guid token);

        [OperationContract]
        [ApplyDataContractResolver]
        List<AdvertType> GetAdvertTypes(Guid token);

        [OperationContract]
        [ApplyDataContractResolver]
        void UpdateCustomerForm(Guid token, Customer customer);

        [OperationContract]
        List<string>[] GetAddressLists(Guid token);

        [OperationContract]
        List<decimal> GetDiscountsForCurrentUser(Guid token, short discountType);

        [OperationContract]
        void UpdateInvitor(Guid token, Guid invitedId, Guid invitorId);

        [OperationContract]
        [ApplyDataContractResolver]
        IEnumerable<TicketType> GetActiveTicketTypesForCustomer(Guid token, Guid customerId);

        [OperationContract]
        int GetMaxGuestUnits(Guid token, Guid divisionId, Guid customerId);

        [OperationContract]
        [ApplyDataContractResolver]
        IEnumerable<Instalment> GetActiveInstalments(Guid token);
    }
}
