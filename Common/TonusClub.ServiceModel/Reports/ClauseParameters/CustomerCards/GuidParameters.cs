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
    [Description("Тип")]
    [AvailableOperators(ClauseOperator.Equals, ClauseOperator.NotEquals)]
    public class GuidParameterC1 : ClauseGuidParameter<CustomerCard>
    {
        public override Func<ITonusService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<ITonusService, Dictionary<object, string>>(i => i.GetAllCustomerCardTypes().ToDictionary(j => (object)j.Id, j => j.Name));
            }
        }

        protected override Guid? GuidFunction(CustomerCard entity)
        {
            return entity.CustomerCardTypeId;
        }

        protected override IEnumerable<Guid> GuidsFunction(CustomerCard entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(CustomerCard))]
    [Description("Тип оплаты")]
    [AvailableOperators(ClauseOperator.Equals)]
    public class GuidParameterC2 : ClauseGuidParameter<CustomerCard>
    {
        public override Func<ITonusService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<ITonusService, Dictionary<object, string>>(_ =>
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

        protected override Guid? GuidFunction(CustomerCard entity)
        {
            return new Guid(0, (short)entity.PmtTypeId, 0, new byte[8]);
        }

        protected override IEnumerable<Guid> GuidsFunction(CustomerCard entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(CustomerCard))]
    [Description("Статусы клиента")]
    [AvailableOperators(ClauseOperator.Contains, ClauseOperator.NotContains)]
    public class GuidParameterC3 : ClauseGuidParameter<CustomerCard>
    {
        public override Func<ITonusService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<ITonusService, Dictionary<object, string>>(c => c.GetAllStatuses().ToDictionary(i => (object)i.Key, i => i.Value));
            }
        }

        protected override Guid? GuidFunction(CustomerCard entity)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<Guid> GuidsFunction(CustomerCard entity)
        {
            return entity.Customer.CustomerStatuses.Select(i => i.Id);
        }
    }

    [DataContract]
    [ClauseRelation(typeof(CustomerCard))]
    [Description("Тип предыдущей карты")]
    [AvailableOperators(ClauseOperator.Equals, ClauseOperator.NotEquals)]
    public class GuidParameterC4 : ClauseGuidParameter<CustomerCard>
    {
        public override Func<ITonusService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<ITonusService, Dictionary<object, string>>(i => i.GetAllCustomerCardTypes().ToDictionary(j => (object)j.Id, j => j.Name));
            }
        }

        protected override Guid? GuidFunction(CustomerCard entity)
        {
            var prev = entity.Customer.CustomerCards.Where(i => i.EmitDate < entity.EmitDate).OrderByDescending(i => i.EmitDate).FirstOrDefault();
            if (prev == null) return null;
            return prev.CustomerCardTypeId;
        }

        protected override IEnumerable<Guid> GuidsFunction(CustomerCard entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(CustomerCard))]
    [Description("Тип следующей карты")]
    [AvailableOperators(ClauseOperator.Equals, ClauseOperator.NotEquals)]
    public class GuidParameterC5 : ClauseGuidParameter<CustomerCard>
    {
        public override Func<ITonusService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<ITonusService, Dictionary<object, string>>(i => i.GetAllCustomerCardTypes().ToDictionary(j => (object)j.Id, j => j.Name));
            }
        }

        protected override Guid? GuidFunction(CustomerCard entity)
        {
            var next = entity.Customer.CustomerCards.Where(i => i.EmitDate > entity.EmitDate).OrderBy(i => i.EmitDate).FirstOrDefault();
            if (next == null) return null;
            return next.CustomerCardTypeId;
        }

        protected override IEnumerable<Guid> GuidsFunction(CustomerCard entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(CustomerCard))]
    [Description("Купленные абонементы клиента")]
    [AvailableOperators(ClauseOperator.Contains, ClauseOperator.NotContains)]
    public class GuidParameterC6 : ClauseGuidParameter<CustomerCard>
    {
        public override Func<ITonusService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<ITonusService, Dictionary<object, string>>(i => i.GetAllTicketTypes().ToDictionary(j => (object)j.Id, j => j.Name));
            }
        }

        protected override Guid? GuidFunction(CustomerCard entity)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<Guid> GuidsFunction(CustomerCard entity)
        {
            return entity.Customer.Tickets.Select(i => i.TicketTypeId);
        }
    }

    [DataContract]
    [ClauseRelation(typeof(CustomerCard))]
    [Description("Активные абонементы клиента")]
    [AvailableOperators(ClauseOperator.Contains, ClauseOperator.NotContains)]
    public class GuidParameterC7 : ClauseGuidParameter<CustomerCard>
    {
        public override Func<ITonusService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<ITonusService, Dictionary<object, string>>(i => i.GetAllTicketTypes().ToDictionary(j => (object)j.Id, j => j.Name));
            }
        }

        protected override Guid? GuidFunction(CustomerCard entity)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<Guid> GuidsFunction(CustomerCard entity)
        {
            return entity.Customer.Tickets.Where(i => i.IsActive).Select(i => i.TicketTypeId);
        }
    }


    [DataContract]
    [ClauseRelation(typeof(CustomerCard))]
#if BEAUTINIKA
    [Description("Студия")]
#else
    [Description("Клуб")]
#endif
    [AvailableOperators(ClauseOperator.Equals)]
    public class GuidParameterC8 : ClauseGuidParameter<CustomerCard>
    {
        public override Func<ITonusService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<ITonusService, Dictionary<object, string>>(i => i.GetDivisions().ToDictionary(j => (object)j.Id, j => j.Name));
            }
        }

        protected override Guid? GuidFunction(CustomerCard entity)
        {
            return entity.DivisionId;
        }

        protected override IEnumerable<Guid> GuidsFunction(CustomerCard entity)
        {
            throw new NotImplementedException();
        }
    }

}
