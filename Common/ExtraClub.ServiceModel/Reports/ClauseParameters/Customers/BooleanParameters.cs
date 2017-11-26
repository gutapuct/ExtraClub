using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace ExtraClub.ServiceModel.Reports.ClauseParameters.Customers
{
    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Заполнено ли поле - Фамилия")]
    [AvailableOperators(ClauseOperator.True, ClauseOperator.False)]
    public class BoolParameter1 : ClauseBoolParameter<Customer>
    {
        protected override bool? BoolFunction(Customer entity)
        {
            return !String.IsNullOrWhiteSpace(entity.LastName);
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Заполнено ли поле - Имя")]
    [AvailableOperators(ClauseOperator.True, ClauseOperator.False)]
    public class BoolParameter2 : ClauseBoolParameter<Customer>
    {
        protected override bool? BoolFunction(Customer entity)
        {
            return !String.IsNullOrWhiteSpace(entity.FirstName);
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Заполнено ли поле - Отчество")]
    [AvailableOperators(ClauseOperator.True, ClauseOperator.False)]
    public class BoolParameter3 : ClauseBoolParameter<Customer>
    {
        protected override bool? BoolFunction(Customer entity)
        {
            return !String.IsNullOrWhiteSpace(entity.MiddleName);
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Заполнено ли поле - Дата рождения")]
    [AvailableOperators(ClauseOperator.True, ClauseOperator.False)]
    public class BoolParameter4 : ClauseBoolParameter<Customer>
    {
        protected override bool? BoolFunction(Customer entity)
        {
            return entity.Birthday.HasValue;
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Заполнено ли поле - Серия и номер паспорта")]
    [AvailableOperators(ClauseOperator.True, ClauseOperator.False)]
    public class BoolParameter5 : ClauseBoolParameter<Customer>
    {
        protected override bool? BoolFunction(Customer entity)
        {
            return !String.IsNullOrWhiteSpace(entity.PasspNumber);
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Заполнено ли поле - Кем выдан паспорт")]
    [AvailableOperators(ClauseOperator.True, ClauseOperator.False)]
    public class BoolParameter6 : ClauseBoolParameter<Customer>
    {
        protected override bool? BoolFunction(Customer entity)
        {
            return !String.IsNullOrWhiteSpace(entity.PasspEmitPlace);
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Заполнено ли поле - Дата выдачи паспорта")]
    [AvailableOperators(ClauseOperator.True, ClauseOperator.False)]
    public class BoolParameter7 : ClauseBoolParameter<Customer>
    {
        protected override bool? BoolFunction(Customer entity)
        {
            return entity.PasspEmitDate.HasValue;
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Заполнено ли поле - Домашний телефон")]
    [AvailableOperators(ClauseOperator.True, ClauseOperator.False)]
    public class BoolParameter8 : ClauseBoolParameter<Customer>
    {
        protected override bool? BoolFunction(Customer entity)
        {
            return !String.IsNullOrWhiteSpace(entity.Phone1) && entity.Phone1.Length>2;
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Заполнено ли поле - Электронная почта")]
    [AvailableOperators(ClauseOperator.True, ClauseOperator.False)]
    public class BoolParameter9 : ClauseBoolParameter<Customer>
    {
        protected override bool? BoolFunction(Customer entity)
        {
            return !String.IsNullOrWhiteSpace(entity.Email);
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Заполнено ли поле - Мобильный телефон")]
    [AvailableOperators(ClauseOperator.True, ClauseOperator.False)]
    public class BoolParameter10 : ClauseBoolParameter<Customer>
    {
        protected override bool? BoolFunction(Customer entity)
        {
            return !String.IsNullOrWhiteSpace(entity.Phone2) || (entity.Phone2 ?? "").Length > 4;
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Заполнено ли поле - Индекс")]
    [AvailableOperators(ClauseOperator.True, ClauseOperator.False)]
    public class BoolParameter11 : ClauseBoolParameter<Customer>
    {
        protected override bool? BoolFunction(Customer entity)
        {
            return !String.IsNullOrWhiteSpace(entity.AddrIndex);
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Заполнено ли поле - Город")]
    [AvailableOperators(ClauseOperator.True, ClauseOperator.False)]
    public class BoolParameter12 : ClauseBoolParameter<Customer>
    {
        protected override bool? BoolFunction(Customer entity)
        {
            return !String.IsNullOrWhiteSpace(entity.AddrCity);
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Заполнено ли поле - Улица")]
    [AvailableOperators(ClauseOperator.True, ClauseOperator.False)]
    public class BoolParameter13 : ClauseBoolParameter<Customer>
    {
        protected override bool? BoolFunction(Customer entity)
        {
            return !String.IsNullOrWhiteSpace(entity.AddrStreet);
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Заполнено ли поле - Дом, квартира")]
    [AvailableOperators(ClauseOperator.True, ClauseOperator.False)]
    public class BoolParameter14 : ClauseBoolParameter<Customer>
    {
        protected override bool? BoolFunction(Customer entity)
        {
            return !String.IsNullOrWhiteSpace(entity.AddrOther);
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Заполнено ли поле - Метро")]
    [AvailableOperators(ClauseOperator.True, ClauseOperator.False)]
    public class BoolParameter15 : ClauseBoolParameter<Customer>
    {
        protected override bool? BoolFunction(Customer entity)
        {
            return !String.IsNullOrWhiteSpace(entity.AddrMetro);
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Есть ли хоть один абонемент")]
    [AvailableOperators(ClauseOperator.True, ClauseOperator.False)]
    [Include("Tickets")]
    public class BoolParameter16 : ClauseBoolParameter<Customer>
    {
        protected override bool? BoolFunction(Customer entity)
        {
            return entity.Tickets.Any();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Наличие активного аб-та")]
    [AvailableOperators(ClauseOperator.True, ClauseOperator.False)]
    [Include("Tickets")]
    public class BoolParameter17 : ClauseBoolParameter<Customer>
    {
        protected override bool? BoolFunction(Customer entity)
        {
            return entity.Tickets.Any(i => i.IsActive);
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Действует ли заморозка")]
    [AvailableOperators(ClauseOperator.True, ClauseOperator.False)]
    [Include("Tickets", "Tickets.TicketFreezes")]
    public class BoolParameter18 : ClauseBoolParameter<Customer>
    {
        protected override bool? BoolFunction(Customer entity)
        {
            return entity.Tickets.SelectMany(i => i.TicketFreezes).Any(i => i.StartDate <= DateTime.Now && i.FinishDate >= DateTime.Now);
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Заполнены противопоказания")]
    [AvailableOperators(ClauseOperator.True, ClauseOperator.False)]
    public class BoolParameter19 : ClauseBoolParameter<Customer>
    {
        protected override bool? BoolFunction(Customer entity)
        {
            return entity.NoContraIndications.HasValue || entity.ContraIndications.Any();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Ставились ли цели")]
    [AvailableOperators(ClauseOperator.True, ClauseOperator.False)]
    [Include("CustomerTargets")]
    public class BoolParameter20 : ClauseBoolParameter<Customer>
    {
        protected override bool? BoolFunction(Customer entity)
        {
            return entity.CustomerTargets.Any();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Если ли достигнутые цели")]
    [AvailableOperators(ClauseOperator.True, ClauseOperator.False)]
    [Include("CustomerTargets")]
    public class BoolParameter21 : ClauseBoolParameter<Customer>
    {
        protected override bool? BoolFunction(Customer entity)
        {
            return entity.CustomerTargets.Any(i => i.TargetComplete ?? false);
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Если ли текущая цель")]
    [AvailableOperators(ClauseOperator.True, ClauseOperator.False)]
    [Include("CustomerTargets")]
    public class BoolParameter22 : ClauseBoolParameter<Customer>
    {
        protected override bool? BoolFunction(Customer entity)
        {
            return entity.CustomerTargets.Any(i => i.CreatedOn <= DateTime.Now && i.TargetDate >= DateTime.Now);
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Если ли антропометрия")]
    [AvailableOperators(ClauseOperator.True, ClauseOperator.False)]
    [Include("Anthropometrics")]
    public class BoolParameter23 : ClauseBoolParameter<Customer>
    {
        protected override bool? BoolFunction(Customer entity)
        {
            return entity.Anthropometrics.Any();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Если ли посещения врача")]
    [AvailableOperators(ClauseOperator.True, ClauseOperator.False)]
    [Include("DoctorVisits")]
    public class BoolParameter24 : ClauseBoolParameter<Customer>
    {
        protected override bool? BoolFunction(Customer entity)
        {
            return entity.DoctorVisits.Any();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Если ли дневник питания")]
    [AvailableOperators(ClauseOperator.True, ClauseOperator.False)]
    [Include("Nutritions")]
    public class BoolParameter25 : ClauseBoolParameter<Customer>
    {
        protected override bool? BoolFunction(Customer entity)
        {
            return entity.Nutritions.Any();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Если ли контрольный замер")]
    [AvailableOperators(ClauseOperator.True, ClauseOperator.False)]
    [Include("CustomerMeasures")]
    public class BoolParameter26 : ClauseBoolParameter<Customer>
    {
        protected override bool? BoolFunction(Customer entity)
        {
            return entity.CustomerMeasures.Any();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Смс-рассылка")]
    [AvailableOperators(ClauseOperator.True, ClauseOperator.False)]
    public class BoolParameter27 : ClauseBoolParameter<Customer>
    {
        protected override bool? BoolFunction(Customer entity)
        {
            return entity.SmsList;
        }
    }
}
