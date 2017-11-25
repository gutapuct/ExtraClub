using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace TonusClub.ServiceModel.Reports.ClauseParameters.Tickets
{
    [DataContract]
    [ClauseRelation(typeof(Ticket))]
    [Description("Дата покупки")]
    public class DateParameterTi1 : ClauseDateParameter<Ticket>
    {
        protected override DateTime? DateFunction(Ticket entity)
        {
            return entity.CreatedOn;
        }
    }
    [DataContract]
    [ClauseRelation(typeof(Ticket))]
    [Description("Дата активации")]
    public class DateParameterTi2 : ClauseDateParameter<Ticket>
    {
        protected override DateTime? DateFunction(Ticket entity)
        {
            return entity.StartDate;
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Ticket))]
    [Description("План внесения рассрочки")]
    public class DateParameterTi3 : ClauseDateParameter<Ticket>
    {
        protected override DateTime? DateFunction(Ticket entity)
        {
            return entity.PlanningInstalmentDay;
        }
    }
}
