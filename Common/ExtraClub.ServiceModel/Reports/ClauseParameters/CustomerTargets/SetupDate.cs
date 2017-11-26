using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace ExtraClub.ServiceModel.Reports.ClauseParameters.CustomerTargets
{
    [DataContract]
    [ClauseRelation(typeof(CustomerTarget))]
    [Description("Дата постановки цели")]
    public class SetupDate : ClauseDateParameter<CustomerTarget>
    {
        protected override DateTime? DateFunction(CustomerTarget entity)
        {
            return entity.CreatedOn.Date;
        }
    }
}
