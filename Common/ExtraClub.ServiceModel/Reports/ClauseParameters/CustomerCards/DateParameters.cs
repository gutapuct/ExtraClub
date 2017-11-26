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
    [Description("Дата покупки")]
    public class DateParameterC1 : ClauseDateParameter<CustomerCard>
    {
        protected override DateTime? DateFunction(CustomerCard entity)
        {
            return entity.EmitDate;
        }
    }

}
