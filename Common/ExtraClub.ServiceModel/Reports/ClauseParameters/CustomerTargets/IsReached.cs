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
    [Description("Достигнута ли цель")]
    [AvailableOperators(ClauseOperator.True, ClauseOperator.False, ClauseOperator.IsNull, ClauseOperator.IsNotNull)]
    public class IsReached : ClauseBoolParameter<CustomerTarget>
    {
        protected override bool? BoolFunction(CustomerTarget entity)
        {
            return entity.TargetComplete;
        }
    }
}
