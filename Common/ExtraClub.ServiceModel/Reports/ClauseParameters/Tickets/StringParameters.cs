using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace ExtraClub.ServiceModel.Reports.ClauseParameters.Tickets
{

    [DataContract]
    [ClauseRelation(typeof(Ticket))]
    [Description("ФИО клиента")]
    [AvailableOperators(ClauseOperator.Contains)]
    [Include("Customer")]
    public class StringParamTi1 : ClauseStringParameter<Ticket>
    {
        protected override string StringFunction(Ticket i)
        {
            return i.Customer.FullName ?? "";
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Ticket))]
    [Description("Номер карты клиента")]
    [AvailableOperators(ClauseOperator.Contains)]
    [Include("Customer", "Customer.CustomerCards", "Customer.CustomerCards.CustomerCardType")]
    public class StringParamTi2 : ClauseStringParameter<Ticket>
    {
        protected override string StringFunction(Ticket i)
        {
            i.Customer.InitActiveCard();
            if (i.Customer.ActiveCard == null) return String.Empty;
            return i.Customer.ActiveCard.CardBarcode;
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Ticket))]
    [Description("Номер абонемента")]
    [AvailableOperators(ClauseOperator.Contains)]
    public class StringParamTi3 : ClauseStringParameter<Ticket>
    {
        protected override string StringFunction(Ticket i)
        {
            return i.Number;
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Ticket))]
    [Description("Название типа")]
    [AvailableOperators(ClauseOperator.Contains)]
    [Include("TicketType")]
    public class StringParamTi4 : ClauseStringParameter<Ticket>
    {
        protected override string StringFunction(Ticket i)
        {
            return i.TicketType.Name;
        }
    }

}
