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
    [Description("ФИО клиента")]
    [AvailableOperators(ClauseOperator.Contains, ClauseOperator.NotContains)]
    public class CustomerName : ClauseStringParameter<CustomerTarget>
    {
        protected override string StringFunction(CustomerTarget i)
        {
            return i.Customer.FullName.ToLower();
        }
    }
}
