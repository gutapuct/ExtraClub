using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace ExtraClub.ServiceModel.Reports.ClauseParameters.TreatmentEvents
{
    [DataContract]
    [ClauseRelation(typeof(TreatmentEvent))]
    [Description("Несколько занимающихся")]
    [AvailableOperators(ClauseOperator.True, ClauseOperator.False)]
    public class BoolParameterEv1 : ClauseBoolParameter<TreatmentEvent>
    {
        protected override bool? BoolFunction(TreatmentEvent entity)
        {
            return entity.TreatmentConfig.TreatmentType.AllowsMultiple;
        }
    }

    [DataContract]
    [ClauseRelation(typeof(TreatmentEvent))]
    [Description("В рамках программы")]
    [AvailableOperators(ClauseOperator.True, ClauseOperator.False)]
    public class BoolParameterEv2 : ClauseBoolParameter<TreatmentEvent>
    {
        protected override bool? BoolFunction(TreatmentEvent entity)
        {
            return entity.ProgramId.HasValue;
        }
    }
}
