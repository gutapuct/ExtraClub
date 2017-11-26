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
    [Description("Тип услуги")]
    [AvailableOperators(ClauseOperator.Equals, ClauseOperator.NotEquals)]
    public class GuidParameterEv1 : ClauseGuidParameter<TreatmentEvent>
    {
        public override Func<IExtraService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<IExtraService, Dictionary<object, string>>(i => i.GetAllTreatmentTypes().Where(j => j.IsActive).ToList().ToDictionary(j => (object)j.Id, j => j.Name));
            }
        }

        protected override Guid? GuidFunction(TreatmentEvent entity)
        {
            return entity.TreatmentConfig.TreatmentTypeId;
        }

        protected override IEnumerable<Guid> GuidsFunction(TreatmentEvent entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(TreatmentEvent))]
    [Description("Группа")]
    [AvailableOperators(ClauseOperator.Equals, ClauseOperator.NotEquals, ClauseOperator.IsNull, ClauseOperator.IsNotNull)]
    public class GuidParameterEv2 : ClauseGuidParameter<TreatmentEvent>
    {
        public override Func<IExtraService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<IExtraService, Dictionary<object, string>>(i => i.GetAllTreatmentTypeGroups().ToDictionary(j => (object)j.Id, j => j.Name));
            }
        }

        protected override Guid? GuidFunction(TreatmentEvent entity)
        {
            return entity.TreatmentConfig.TreatmentType.TreatmentTypeGroupId;
        }

        protected override IEnumerable<Guid> GuidsFunction(TreatmentEvent entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(TreatmentEvent))]
    [Description("Название услуги")]
    [AvailableOperators(ClauseOperator.Equals, ClauseOperator.NotEquals)]
    public class GuidParameterEv3 : ClauseGuidParameter<TreatmentEvent>
    {
        public override Func<IExtraService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<IExtraService, Dictionary<object, string>>(i => i.GetAllTreatmentConfigs().Where(j => j.IsActive).ToDictionary(j => (object)j.Id, j => j.Name));
            }
        }

        protected override Guid? GuidFunction(TreatmentEvent entity)
        {
            return entity.TreatmentConfigId;
        }

        protected override IEnumerable<Guid> GuidsFunction(TreatmentEvent entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(TreatmentEvent))]
    [Description("Франчайзи")]
    [AvailableOperators(ClauseOperator.Equals)]
    public class GuidParameterEv4 : ClauseGuidParameter<TreatmentEvent>
    {
        public override Func<IExtraService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<IExtraService, Dictionary<object, string>>(i => i.GetCompanies().ToDictionary(j => (object)j.CompanyId, j => j.CompanyName));
            }
        }

        protected override Guid? GuidFunction(TreatmentEvent entity)
        {
            return entity.CompanyId;
        }

        protected override IEnumerable<Guid> GuidsFunction(TreatmentEvent entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(TreatmentEvent))]
    [Description("Клуб")]
    [AvailableOperators(ClauseOperator.Equals)]
    public class GuidParameterEv5 : ClauseGuidParameter<TreatmentEvent>
    {
        public override Func<IExtraService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<IExtraService, Dictionary<object, string>>(i => i.GetDivisions().ToDictionary(j => (object)j.Id, j => j.Name));
            }
        }

        protected override Guid? GuidFunction(TreatmentEvent entity)
        {
            return entity.DivisionId;
        }

        protected override IEnumerable<Guid> GuidsFunction(TreatmentEvent entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(TreatmentEvent))]
    [Description("Оборудование")]
    [AvailableOperators(ClauseOperator.Equals)]
    public class GuidParameterEv6 : ClauseGuidParameter<TreatmentEvent>
    {
        public override Func<IExtraService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<IExtraService, Dictionary<object, string>>(i => i.GetAllTreatments(Guid.Empty).ToDictionary(j => (object)j.Id, j => j.DisplayName));
            }
        }

        protected override Guid? GuidFunction(TreatmentEvent entity)
        {
            return entity.TreatmentId;
        }

        protected override IEnumerable<Guid> GuidsFunction(TreatmentEvent entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(TreatmentEvent))]
    [Description("Статус")]
    [AvailableOperators(ClauseOperator.Equals)]
    public class GuidParameterEv7 : ClauseGuidParameter<TreatmentEvent>
    {
        public override Func<IExtraService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<IExtraService, Dictionary<object, string>>(_ =>
                {
                    var res = new Dictionary<object, string>();
                    res.Add(new Guid(0, 0, 0, new byte[8]), "Запланирована");
                    res.Add(new Guid(0, 1, 0, new byte[8]), "Отменена");
                    res.Add(new Guid(0, 2, 0, new byte[8]), "Завершена");
                    res.Add(new Guid(0, 3, 0, new byte[8]), "Прогуляна");
                    return res;
                });
            }
        }

        protected override Guid? GuidFunction(TreatmentEvent entity)
        {
            return new Guid(0, entity.VisitStatus, 0, new byte[8]);
        }

        protected override IEnumerable<Guid> GuidsFunction(TreatmentEvent entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(TreatmentEvent))]
    [Description("Тип карты")]
    [AvailableOperators(ClauseOperator.Equals, ClauseOperator.NotEquals)]
    public class GuidParameterEv8 : ClauseGuidParameter<TreatmentEvent>
    {
        public override Func<IExtraService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<IExtraService, Dictionary<object, string>>(i => i.GetAllCustomerCardTypes().ToDictionary(j => (object)j.Id, j => j.Name));
            }
        }

        protected override Guid? GuidFunction(TreatmentEvent entity)
        {
            entity.Customer.InitActiveCard();
            if (entity.Customer.ActiveCard == null) return null;
            return entity.Customer.ActiveCard.Id;
        }

        protected override IEnumerable<Guid> GuidsFunction(TreatmentEvent entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(TreatmentEvent))]
    [Description("Тип абонемента")]
    [AvailableOperators(ClauseOperator.Equals, ClauseOperator.NotEquals)]
    public class GuidParameterEv9 : ClauseGuidParameter<TreatmentEvent>
    {
        public override Func<IExtraService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<IExtraService, Dictionary<object, string>>(i => i.GetAllTicketTypes().ToDictionary(j => (object)j.Id, j => j.Name));
            }
        }

        protected override Guid? GuidFunction(TreatmentEvent entity)
        {
            if (!entity.TicketId.HasValue) return null;
            return entity.Ticket.TicketTypeId;
        }

        protected override IEnumerable<Guid> GuidsFunction(TreatmentEvent entity)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(TreatmentEvent))]
    [Description("Статусы")]
    [AvailableOperators(ClauseOperator.Contains, ClauseOperator.NotContains)]
    public class GuidParameterEv10 : ClauseGuidParameter<TreatmentEvent>
    {
        public override Func<IExtraService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<IExtraService, Dictionary<object, string>>(c => c.GetAllStatuses().ToDictionary(i => (object)i.Key, i => i.Value));
            }
        }

        protected override Guid? GuidFunction(TreatmentEvent entity)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<Guid> GuidsFunction(TreatmentEvent entity)
        {
            return entity.Customer.CustomerStatuses.Select(i => i.Id);
        }
    }

    [DataContract]
    [ClauseRelation(typeof(TreatmentEvent))]
    [Description("Программа")]
    [AvailableOperators(ClauseOperator.Equals, ClauseOperator.NotEquals, ClauseOperator.IsNull, ClauseOperator.IsNotNull)]
    public class GuidParameterEv11 : ClauseGuidParameter<TreatmentEvent>
    {
        public override Func<IExtraService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return new Func<IExtraService, Dictionary<object, string>>(c => c.GetTreatmentPrograms().ToDictionary(i => (object)i.Id, i => i.ProgramName));
            }
        }

        protected override Guid? GuidFunction(TreatmentEvent entity)
        {
            return entity.ProgramId;
        }

        protected override IEnumerable<Guid> GuidsFunction(TreatmentEvent entity)
        {
            throw new NotImplementedException();
        }
    }

}
