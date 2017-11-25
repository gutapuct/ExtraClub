using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace TonusClub.ServiceModel.Reports.ClauseParameters.CustomerTargets
{
    [DataContract]
    [ClauseRelation(typeof(CustomerTarget))]
    [Description("Номер карты клиента")]
    [AvailableOperators(ClauseOperator.Contains)]
    public class CustomerCardNumber : ClauseStringParameter<CustomerTarget>
    {
        protected override string StringFunction(CustomerTarget i)
        {
            i.Customer.InitActiveCard();
            if (i.Customer.ActiveCard == null) return String.Empty;
            return i.Customer.ActiveCard.CardBarcode;
        }
    }
}
