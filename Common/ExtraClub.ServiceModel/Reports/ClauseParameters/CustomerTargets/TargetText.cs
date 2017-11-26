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
    [Description("Название цели")]
    [AvailableOperators(ClauseOperator.Contains, ClauseOperator.NotContains)]
    public class TargetText : ClauseStringParameter<CustomerTarget>
    {
        protected override string StringFunction(CustomerTarget i)
        {
            return i.TargetText.ToLower();
        }
    }
}
