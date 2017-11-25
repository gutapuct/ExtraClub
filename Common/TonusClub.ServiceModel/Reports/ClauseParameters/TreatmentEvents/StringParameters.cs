using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace TonusClub.ServiceModel.Reports.ClauseParameters.TreatmentEvents
{
    [DataContract]
    [ClauseRelation(typeof(TreatmentEvent))]
    [Description("Номер карты клиента")]
    [AvailableOperators(ClauseOperator.Contains)]
    public class StringParamEv1 : ClauseStringParameter<TreatmentEvent>
    {
        protected override string StringFunction(TreatmentEvent i)
        {
            i.Customer.InitActiveCard();
            if (i.Customer.ActiveCard == null) return String.Empty;
            return i.Customer.ActiveCard.CardBarcode;
        }
    }

    [DataContract]
    [ClauseRelation(typeof(TreatmentEvent))]
    [Description("Номер абонемента")]
    [AvailableOperators(ClauseOperator.Contains)]
    public class StringParamEv2 : ClauseStringParameter<TreatmentEvent>
    {
        protected override string StringFunction(TreatmentEvent i)
        {
            if (i.Ticket == null) return String.Empty;
            return i.Ticket.Number;
        }
    }

    [DataContract]
    [ClauseRelation(typeof(TreatmentEvent))]
    [Description("ФИО клиента")]
    [AvailableOperators(ClauseOperator.Contains)]
    public class StringParamEv3 : ClauseStringParameter<TreatmentEvent>
    {
        protected override string StringFunction(TreatmentEvent i)
        {
            return i.Customer.FullName ?? "";
        }
    }
}
