using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace ExtraClub.ServiceModel.Reports.ClauseParameters.CustomerCards
{
    [DataContract]
    [ClauseRelation(typeof(CustomerCard))]
    [Description("ФИО клиента")]
    [AvailableOperators(ClauseOperator.Contains)]
    public class StringParamC1 : ClauseStringParameter<CustomerCard>
    {
        protected override string StringFunction(CustomerCard i)
        {
            return i.Customer.FullName ?? "";
        }
    }
}
