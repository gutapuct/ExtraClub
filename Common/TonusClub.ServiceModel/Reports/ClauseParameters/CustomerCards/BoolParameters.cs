using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace TonusClub.ServiceModel.Reports.ClauseParameters.CustomerCards
{
    [DataContract]
    [ClauseRelation(typeof(CustomerCard))]
    [Description("Гостевая ли")]
    [AvailableOperators(ClauseOperator.True, ClauseOperator.False)]
    public class BoolParameterC1 : ClauseBoolParameter<CustomerCard>
    {
        protected override bool? BoolFunction(CustomerCard entity)
        {
            return entity.CustomerCardType.IsGuest;
        }
    }
    [DataContract]
    [ClauseRelation(typeof(CustomerCard))]
    [Description("Визитера ли")]
    [AvailableOperators(ClauseOperator.True, ClauseOperator.False)]
    public class BoolParameterC2 : ClauseBoolParameter<CustomerCard>
    {
        protected override bool? BoolFunction(CustomerCard entity)
        {
            return entity.CustomerCardType.IsVisit;
        }
    }
    [DataContract]
    [ClauseRelation(typeof(CustomerCard))]
    [Description("Активна ли")]
    [AvailableOperators(ClauseOperator.True, ClauseOperator.False)]
    public class BoolParameterC3 : ClauseBoolParameter<CustomerCard>
    {
        protected override bool? BoolFunction(CustomerCard entity)
        {
            return entity.CustomerCardType.IsActive;
        }
    }

}
