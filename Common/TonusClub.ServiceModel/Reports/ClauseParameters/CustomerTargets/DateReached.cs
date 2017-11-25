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
    [Description("Прошла ли дата цели")]
    [AvailableOperators(ClauseOperator.True, ClauseOperator.False)]
    public class DateReached : ClauseBoolParameter<CustomerTarget>
    {
        protected override bool? BoolFunction(CustomerTarget entity)
        {
            return entity.TargetDate.Date <= DateTime.Now;
        }
    }
}
