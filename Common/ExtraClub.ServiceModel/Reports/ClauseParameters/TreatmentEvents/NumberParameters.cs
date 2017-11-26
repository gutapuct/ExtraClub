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
    [Description("Длительность")]
    public class NumberParameterEv1 : ClauseNumberParameter<TreatmentEvent>
    {
        protected override decimal? NumberFunction(TreatmentEvent entity)
        {
            return entity.TreatmentConfig.FullDuration;
        }
    }

    [DataContract]
    [ClauseRelation(typeof(TreatmentEvent))]
    [Description("Цена")]
    public class NumberParameterEv2 : ClauseNumberParameter<TreatmentEvent>
    {
        protected override decimal? NumberFunction(TreatmentEvent entity)
        {
            return entity.TreatmentConfig.Price;
        }
    }

}
