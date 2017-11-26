using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace ExtraClub.ServiceModel.Reports.ClauseParameters.Tickets
{
    [DataContract]
    [ClauseRelation(typeof(Ticket))]
    [Description("Закончился ли")]
    [AvailableOperators(ClauseOperator.True, ClauseOperator.False)]
    [Include("UnitCharges", "Division", "TicketPayments", "TicketFreezes", "TicketFreezes.TicketFreezeReason", "Successors", "MinutesCharges", "Division.Company", "SolariumVisits")]
    [InitAttribude]
    public class BoolParameterTi1 : ClauseBoolParameter<Ticket>
    {
        protected override bool? BoolFunction(Ticket entity)
        {
            return entity.Status == TicketStatus.RunOut || entity.Status == TicketStatus.Expiried || entity.Status == TicketStatus.Rebilled || entity.Status == TicketStatus.Returned;
        }
    }
    [DataContract]
    [ClauseRelation(typeof(Ticket))]
    [Description("Заморожен ли")]
    [AvailableOperators(ClauseOperator.True, ClauseOperator.False)]
    [Include("TicketFreezes")]
    public class BoolParameterTi2 : ClauseBoolParameter<Ticket>
    {
        protected override bool? BoolFunction(Ticket entity)
        {
            return entity.TicketFreezes.Any(i => i.StartDate <= DateTime.Now && i.FinishDate >= DateTime.Now);
        }
    }
    [DataContract]
    [ClauseRelation(typeof(Ticket))]
    [Description("Акционный ли")]
    [AvailableOperators(ClauseOperator.True, ClauseOperator.False)]
    [Include("TicketType")]
    public class BoolParameterTi3 : ClauseBoolParameter<Ticket>
    {
        protected override bool? BoolFunction(Ticket entity)
        {
            return entity.TicketType.IsAction;
        }
    }
}
