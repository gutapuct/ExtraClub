using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.ComponentModel;

namespace ExtraClub.ServiceModel.Reports.ClauseParameters.CustomerTargets
{
    [DataContract]
    [ClauseRelation(typeof(CustomerTarget))]
    [Description("Дата достижения цели")]
    public class CompleteDate : ClauseDateParameter<CustomerTarget>
    {
        protected override DateTime? DateFunction(CustomerTarget i)
        {
            return i.TargetDate.Date;
        }
    }
}
