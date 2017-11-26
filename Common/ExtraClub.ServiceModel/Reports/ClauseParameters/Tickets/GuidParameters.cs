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
    [Description("Тип оплаты первого взноса")]
    [AvailableOperators(ClauseOperator.Equals)]
    public class GuidParameterTi1 : ClauseGuidParameter<Ticket>
    {
        public override Func<IExtraService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<IExtraService, Dictionary<object, string>>(_ =>
                {
                    var res = new Dictionary<object, string>();
                    res.Add(new Guid(0, 0, 0, new byte[8]), "Наличные");
                    res.Add(new Guid(0, 1, 0, new byte[8]), "Депозит");
                    res.Add(new Guid(0, 2, 0, new byte[8]), "Банк. карта");
                    res.Add(new Guid(0, 3, 0, new byte[8]), "Безнал");
                    return res;
                });
            }
        }

        protected override Guid? GuidFunction(Ticket entity)
        {
            return new Guid(0, (short)entity.FirstPmtTypeId, 0, new byte[8]);
        }

        protected override IEnumerable<Guid> GuidsFunction(Ticket entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Ticket))]
    [Description("Тип")]
    [AvailableOperators(ClauseOperator.Equals, ClauseOperator.NotEquals)]
    public class GuidParameterTi2 : ClauseGuidParameter<Ticket>
    {
        public override Func<IExtraService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<IExtraService, Dictionary<object, string>>(i => i.GetAllTicketTypes().ToDictionary(j => (object)j.Id, j => j.Name));
            }
        }

        protected override Guid? GuidFunction(Ticket entity)
        {
            return entity.TicketTypeId;
        }

        protected override IEnumerable<Guid> GuidsFunction(Ticket entity)
        {
            throw new NotImplementedException();
        }
    }


    [DataContract]
    [ClauseRelation(typeof(Ticket))]
    [Description("Франчайзи")]
    [AvailableOperators(ClauseOperator.Equals)]
    public class GuidParameterTi3 : ClauseGuidParameter<Ticket>
    {
        public override Func<IExtraService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<IExtraService, Dictionary<object, string>>(i => i.GetCompanies().ToDictionary(j => (object)j.CompanyId, j => j.CompanyName));
            }
        }

        protected override Guid? GuidFunction(Ticket entity)
        {
            return entity.CompanyId;
        }

        protected override IEnumerable<Guid> GuidsFunction(Ticket entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Ticket))]
    [Description("Клуб")]
    [AvailableOperators(ClauseOperator.Equals)]
    public class GuidParameterTi4 : ClauseGuidParameter<Ticket>
    {
        public override Func<IExtraService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<IExtraService, Dictionary<object, string>>(i => i.GetDivisions().ToDictionary(j => (object)j.Id, j => j.Name));
            }
        }

        protected override Guid? GuidFunction(Ticket entity)
        {
            return entity.DivisionId;
        }

        protected override IEnumerable<Guid> GuidsFunction(Ticket entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Ticket))]
    [Description("Продал")]
    [AvailableOperators(ClauseOperator.Equals, ClauseOperator.NotEquals)]
    public class GuidParameterTi5 : ClauseGuidParameter<Ticket>
    {
        public override Func<IExtraService, Dictionary<object, string>> GetValuesFunction
        {
            get { return (context) => context.GetUsers().ToDictionary(i => (object)i.UserId, i => i.FullName); }
        }

        protected override Guid? GuidFunction(Ticket entity)
        {
            return entity.AuthorId;
        }

        protected override IEnumerable<Guid> GuidsFunction(Ticket entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Ticket))]
    [Description("Статусы клиента")]
    [AvailableOperators(ClauseOperator.Contains, ClauseOperator.NotContains)]
    [Include("Customer", "Customer.CustomerStatuses")]
    public class GuidParameterTi6 : ClauseGuidParameter<Ticket>
    {
        public override Func<IExtraService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<IExtraService, Dictionary<object, string>>(c => c.GetAllStatuses().ToDictionary(i => (object)i.Key, i => i.Value));
            }
        }

        protected override Guid? GuidFunction(Ticket entity)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<Guid> GuidsFunction(Ticket entity)
        {
            return entity.Customer.CustomerStatuses.Select(i => i.Id);
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Ticket))]
    [Description("Тип карты клиента")]
    [AvailableOperators(ClauseOperator.Equals, ClauseOperator.NotEquals)]
    [Include("Customer", "Customer.CustomerCards", "Customer.CustomerCards.CustomerCardType")]
    public class GuidParameterTi7 : ClauseGuidParameter<Ticket>
    {
        public override Func<IExtraService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<IExtraService, Dictionary<object, string>>(i => i.GetAllCustomerCardTypes().ToDictionary(j => (object)j.Id, j => j.Name));
            }
        }

        protected override Guid? GuidFunction(Ticket entity)
        {
            entity.Customer.InitActiveCard();
            if (entity.Customer.ActiveCard == null) return null;
            return entity.Customer.ActiveCard.Id;
        }

        protected override IEnumerable<Guid> GuidsFunction(Ticket entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Ticket))]
    [Description("Статус абонемента*")]
    [AvailableOperators(ClauseOperator.Equals, ClauseOperator.NotEquals)]
    [Include("UnitCharges", "Division", "TicketPayments", "TicketFreezes", "TicketFreezes.TicketFreezeReason", "Successors", "MinutesCharges", "Division.Company", "SolariumVisits")]
    [InitAttribude]
    public class GuidParameterTi8 : ClauseGuidParameter<Ticket>
    {
        public override Func<IExtraService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<IExtraService, Dictionary<object, string>>(i => Enum.GetValues(typeof(TicketStatus)).Cast<TicketStatus>().ToDictionary(j => (object)(new Guid(0, (byte)j, 0, new byte[8])), j => Ticket.GetStatusName(j)));
            }
        }

        protected override Guid? GuidFunction(Ticket entity)
        {
            return new Guid(0, (byte)entity.Status, 0, new byte[8]);
        }

        protected override IEnumerable<Guid> GuidsFunction(Ticket entity)
        {
            throw new NotImplementedException();
        }
    }

}
