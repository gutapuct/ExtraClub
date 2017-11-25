using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace TonusClub.ServiceModel.Reports.ClauseParameters.Customers
{
    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Ближайшая станция метро")]
    [AvailableOperators(ClauseOperator.Contains, ClauseOperator.NotContains)]
    public class StringParameter1 : ClauseStringParameter<Customer>
    {
        protected override string StringFunction(Customer entity)
        {
            return entity.AddrMetro ?? "";
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("ФИО последнего врача")]
    [AvailableOperators(ClauseOperator.Contains, ClauseOperator.NotContains)]
    [Include("DoctorVisits")]
    public class StringParameter2 : ClauseStringParameter<Customer>
    {
        protected override string StringFunction(Customer entity)
        {
            if (!entity.DoctorVisits.Any()) return String.Empty;
            return entity.DoctorVisits.OrderBy(i => i.Date ?? i.CreatedOn).First().Doctor ?? "";
        }
    }

}
