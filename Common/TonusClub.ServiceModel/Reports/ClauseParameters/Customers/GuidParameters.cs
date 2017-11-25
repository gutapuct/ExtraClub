using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace TonusClub.ServiceModel.Reports.ClauseParameters.Customers
{
    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Франчайзи")]
    [AvailableOperators(ClauseOperator.Equals)]
    public class GuidParameter1 : ClauseGuidParameter<Customer>
    {
        public override Func<ITonusService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<ITonusService, Dictionary<object, string>>(i => i.GetCompanies().ToDictionary(j => (object)j.CompanyId, j => j.CompanyName));
            }
        }

        protected override Guid? GuidFunction(Customer entity)
        {
            return entity.CompanyId;
        }

        protected override IEnumerable<Guid> GuidsFunction(Customer entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Где куплен последний аб-т")]
    [AvailableOperators(ClauseOperator.Equals, ClauseOperator.NotEquals)]
    [Include("Tickets")]
    public class GuidParameter2 : ClauseGuidParameter<Customer>
    {
        public override Func<ITonusService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<ITonusService, Dictionary<object, string>>(i => i.GetDivisions().ToDictionary(j => (object)j.Id, j => j.Name));
            }
        }

        protected override Guid? GuidFunction(Customer entity)
        {
            var ticket = entity.Tickets.OrderByDescending(i => i.CreatedOn).FirstOrDefault();
            if (ticket == null) return null;
            return ticket.DivisionId;
        }

        protected override IEnumerable<Guid> GuidsFunction(Customer entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Рекламный канал")]
    [AvailableOperators(ClauseOperator.Equals, ClauseOperator.NotEquals)]
    public class GuidParameter3 : ClauseGuidParameter<Customer>
    {
        public override Func<ITonusService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<ITonusService, Dictionary<object, string>>(i => i.GetAdvertTypes().ToDictionary(j => (object)j.Id, j => j.Name));
            }
        }

        protected override Guid? GuidFunction(Customer entity)
        {
            return entity.AdvertTypeId;
        }

        protected override IEnumerable<Guid> GuidsFunction(Customer entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Рекламная группа")]
    [AvailableOperators(ClauseOperator.Equals, ClauseOperator.NotEquals)]
    [Include("AdvertType")]
    public class GuidParameter3a : ClauseGuidParameter<Customer>
    {
        public override Func<ITonusService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<ITonusService, Dictionary<object, string>>(i => i.GetAdvertGroups().ToDictionary(j => (object)j.Id, j => j.Name));
            }
        }

        protected override Guid? GuidFunction(Customer entity)
        {
            if (!entity.AdvertTypeId.HasValue) return null;
            return entity.AdvertType.AdvertGroupId;
        }

        protected override IEnumerable<Guid> GuidsFunction(Customer entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Купленные абонементы")]
    [AvailableOperators(ClauseOperator.Contains, ClauseOperator.NotContains)]
    [Include("Tickets")]
    public class GuidParameter4 : ClauseGuidParameter<Customer>
    {
        public override Func<ITonusService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<ITonusService, Dictionary<object, string>>(i => i.GetTicketTypes(false).ToDictionary(j => (object)j.Id, j => j.Name));
            }
        }

        protected override Guid? GuidFunction(Customer entity)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<Guid> GuidsFunction(Customer entity)
        {
            return entity.Tickets.Select(i => i.TicketTypeId).Distinct();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Тип последнего приобретенного аб-та")]
    [AvailableOperators(ClauseOperator.Equals, ClauseOperator.NotEquals, ClauseOperator.IsNull, ClauseOperator.IsNotNull)]
    [Include("Tickets")]
    public class GuidParameter5 : ClauseGuidParameter<Customer>
    {
        public override Func<ITonusService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<ITonusService, Dictionary<object, string>>(i => i.GetTicketTypes(false).ToDictionary(j => (object)j.Id, j => j.Name));
            }
        }

        protected override Guid? GuidFunction(Customer entity)
        {
            if (!entity.Tickets.Any()) return null;
            return entity.Tickets.OrderByDescending(i => i.CreatedOn).First().TicketTypeId;
        }

        protected override IEnumerable<Guid> GuidsFunction(Customer entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Тип последнего активированного аб-та")]
    [AvailableOperators(ClauseOperator.Equals, ClauseOperator.NotEquals, ClauseOperator.IsNull, ClauseOperator.IsNotNull)]
    [Include("Tickets")]
    public class GuidParameter6 : ClauseGuidParameter<Customer>
    {
        public override Func<ITonusService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<ITonusService, Dictionary<object, string>>(i => i.GetTicketTypes(false).ToDictionary(j => (object)j.Id, j => j.Name));
            }
        }

        protected override Guid? GuidFunction(Customer entity)
        {
            if (!entity.Tickets.Any(i => i.StartDate.HasValue)) return null;
            return entity.Tickets.Where(i => i.StartDate.HasValue).OrderByDescending(i => i.StartDate).First().TicketTypeId;
        }

        protected override IEnumerable<Guid> GuidsFunction(Customer entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Что куплено в баре")]
    [AvailableOperators(ClauseOperator.Contains, ClauseOperator.NotContains)]
    [Include("BarOrders", "BarOrders.GoodSales")]
    public class GuidParameter7 : ClauseGuidParameter<Customer>
    {
        public override Func<ITonusService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<ITonusService, Dictionary<object, string>>(i => i.GetAllGoods(Guid.Empty).ToDictionary(j => (object)j.Id, j => j.Name));
            }
        }

        protected override Guid? GuidFunction(Customer entity)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<Guid> GuidsFunction(Customer entity)
        {
            return entity.BarOrders.SelectMany(i => i.GoodSales).Select(i => i.GoodId).Distinct();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Противопоказания")]
    [AvailableOperators(ClauseOperator.Contains, ClauseOperator.NotContains)]
    [Include("ContraIndications")]
    public class GuidParameter8 : ClauseGuidParameter<Customer>
    {
        public override Func<ITonusService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<ITonusService, Dictionary<object, string>>(i => i.GetAllContras().ToDictionary(j => (object)j.Id, j => j.Name));
            }
        }

        protected override Guid? GuidFunction(Customer entity)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<Guid> GuidsFunction(Customer entity)
        {
            return entity.ContraIndications.Select(i => i.Id);
        }
    }


    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Любимая процедура**")]
    [AvailableOperators(ClauseOperator.Equals, ClauseOperator.NotEquals, ClauseOperator.IsNull, ClauseOperator.IsNotNull)]
    [InitAttribude]
    public class GuidParameter9 : ClauseGuidParameter<Customer>
    {
        public override Func<ITonusService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<ITonusService, Dictionary<object, string>>(i => i.GetAllTreatmentTypes().Where(j => j.IsActive).ToList().ToDictionary(j => (object)j.Id, j => j.Name));
            }
        }

        protected override Guid? GuidFunction(Customer entity)
        {
            entity.Init();
            return entity.LikedTreatmentId;
        }

        protected override IEnumerable<Guid> GuidsFunction(Customer entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Любимая программа**")]
    [AvailableOperators(ClauseOperator.Equals, ClauseOperator.NotEquals, ClauseOperator.IsNull, ClauseOperator.IsNotNull)]
    [InitAttribude]
    public class GuidParameter10 : ClauseGuidParameter<Customer>
    {
        public override Func<ITonusService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<ITonusService, Dictionary<object, string>>(i => i.GetTreatmentPrograms().ToDictionary(j => (object)j.Id, j => j.ProgramName));
            }
        }

        protected override Guid? GuidFunction(Customer entity)
        {
            entity.Init();
            return entity.LikedProgramId;
        }

        protected override IEnumerable<Guid> GuidsFunction(Customer entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Любимый товар**")]
    [AvailableOperators(ClauseOperator.Equals, ClauseOperator.NotEquals, ClauseOperator.IsNull, ClauseOperator.IsNotNull)]
    [InitAttribude]
    public class GuidParameter11 : ClauseGuidParameter<Customer>
    {
        public override Func<ITonusService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<ITonusService, Dictionary<object, string>>(i => i.GetAllGoods(Guid.Empty).ToDictionary(j => (object)j.Id, j => j.Name));
            }
        }

        protected override Guid? GuidFunction(Customer entity)
        {
            entity.Init();
            return entity.LikedGoodId;
        }

        protected override IEnumerable<Guid> GuidsFunction(Customer entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Поставил последнюю цель")]
    [AvailableOperators(ClauseOperator.Equals)]
    [Include("CustomerTargets")]
    public class GuidParameter13 : ClauseGuidParameter<Customer>
    {
        public override Func<ITonusService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<ITonusService, Dictionary<object, string>>(i => i.GetUsers().ToDictionary(j => (object)j.UserId, j => j.FullName));
            }
        }

        protected override Guid? GuidFunction(Customer entity)
        {
            if (!entity.CustomerTargets.Any()) return null;
            return entity.CustomerTargets.OrderByDescending(i => i.CreatedOn).First().AuthorId;
        }

        protected override IEnumerable<Guid> GuidsFunction(Customer entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Снимал последнюю антропометрию")]
    [AvailableOperators(ClauseOperator.Equals)]
    [Include("Anthropometrics")]
    public class GuidParameter14 : ClauseGuidParameter<Customer>
    {
        public override Func<ITonusService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<ITonusService, Dictionary<object, string>>(i => i.GetUsers().ToDictionary(j => (object)j.UserId, j => j.FullName));
            }
        }

        protected override Guid? GuidFunction(Customer entity)
        {
            if (!entity.Anthropometrics.Any()) return null;
            return entity.Anthropometrics.OrderByDescending(i => i.CreatedOn).First().AuthorId;
        }

        protected override IEnumerable<Guid> GuidsFunction(Customer entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Записал последний в дневник питания")]
    [AvailableOperators(ClauseOperator.Equals)]
    [Include("Nutritions")]
    public class GuidParameter15 : ClauseGuidParameter<Customer>
    {
        public override Func<ITonusService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<ITonusService, Dictionary<object, string>>(i => i.GetUsers().ToDictionary(j => (object)j.UserId, j => j.FullName));
            }
        }

        protected override Guid? GuidFunction(Customer entity)
        {
            if (!entity.Nutritions.Any()) return null;
            return entity.Nutritions.OrderByDescending(i => i.CreatedOn).First().AuthorId;
        }

        protected override IEnumerable<Guid> GuidsFunction(Customer entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Снимал последний контрольный замер")]
    [AvailableOperators(ClauseOperator.Equals)]
    [Include("CustomerMeasures")]
    public class GuidParameter16 : ClauseGuidParameter<Customer>
    {
        public override Func<ITonusService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<ITonusService, Dictionary<object, string>>(i => i.GetUsers().ToDictionary(j => (object)j.UserId, j => j.FullName));
            }
        }

        protected override Guid? GuidFunction(Customer entity)
        {
            if (!entity.CustomerMeasures.Any()) return null;
            return entity.CustomerMeasures.OrderByDescending(i => i.CreatedOn).First().AuthorId;
        }

        protected override IEnumerable<Guid> GuidsFunction(Customer entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Статус клиента")]
    [AvailableOperators(ClauseOperator.Contains, ClauseOperator.NotContains)]
    [Include("CustomerStatuses")]
    public class GuidParameter17 : ClauseGuidParameter<Customer>
    {
        public override Func<ITonusService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<ITonusService, Dictionary<object, string>>(i => i.GetAllStatuses().ToDictionary(j => (object)j.Key, j => j.Value));
            }
        }

        protected override Guid? GuidFunction(Customer entity)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<Guid> GuidsFunction(Customer entity)
        {
            return entity.CustomerStatuses.Select(i => i.Id);
        }
    }
}
