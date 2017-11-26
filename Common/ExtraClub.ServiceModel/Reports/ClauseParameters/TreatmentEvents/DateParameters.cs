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
    [Description("Дата создания")]
    public class DateParameterEv1 : ClauseDateParameter<TreatmentEvent>
    {
        protected override DateTime? DateFunction(TreatmentEvent entity)
        {
            return entity.CreatedOn;
        }
    }

    [DataContract]
    [ClauseRelation(typeof(TreatmentEvent))]
    [Description("Дата проведения")]
    public class DateParameterEv2 : ClauseDateParameter<TreatmentEvent>
    {
        protected override DateTime? DateFunction(TreatmentEvent entity)
        {
            return entity.VisitDate;
        }
    }
}