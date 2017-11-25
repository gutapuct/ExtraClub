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
    [Description("Цена")]
    public class NumberParameterTi1 : ClauseNumberParameter<Ticket>
    {
        protected override decimal? NumberFunction(Ticket entity)
        {
            return entity.Price;
        }
    }
    [DataContract]
    [ClauseRelation(typeof(Ticket))]
    [Description("Стоимость")]
    public class NumberParameterTi2 : ClauseNumberParameter<Ticket>
    {
        protected override decimal? NumberFunction(Ticket entity)
        {
            return entity.Cost;
        }
    }
    [DataContract]
    [ClauseRelation(typeof(Ticket))]
    [Description("Скидка, руб.")]
    public class NumberParameterTi3 : ClauseNumberParameter<Ticket>
    {
        protected override decimal? NumberFunction(Ticket entity)
        {
            return entity.DiscountPercent * entity.Price;
        }
    }
    [DataContract]
    [ClauseRelation(typeof(Ticket))]
    [Description("Скидка, %")]
    public class NumberParameterTi4 : ClauseNumberParameter<Ticket>
    {
        protected override decimal? NumberFunction(Ticket entity)
        {
            return entity.DiscountPercent * 100;
        }
    }
    [DataContract]
    [ClauseRelation(typeof(Ticket))]
    [Description("Начальное к-во единиц")]
    public class NumberParameterTi5 : ClauseNumberParameter<Ticket>
    {
        protected override decimal? NumberFunction(Ticket entity)
        {
            return entity.UnitsAmount;
        }
    }
    [DataContract]
    [ClauseRelation(typeof(Ticket))]
    [Description("Остаток единиц")]
    [Include("UnitCharges")]
    public class NumberParameterTi6 : ClauseNumberParameter<Ticket>
    {
        protected override decimal? NumberFunction(Ticket entity)
        {
            entity.InitUnitsOut();
            return entity.UnitsLeft;
        }
    }
    [DataContract]
    [ClauseRelation(typeof(Ticket))]
    [Description("Потрачено единиц")]
    [Include("UnitCharges")]
    public class NumberParameterTi7 : ClauseNumberParameter<Ticket>
    {
        protected override decimal? NumberFunction(Ticket entity)
        {
            entity.InitUnitsOut();
            return entity.UnitsOut;
        }
    }
    [DataContract]
    [ClauseRelation(typeof(Ticket))]
    [Description("Списано за штрафы")]
    [Include("UnitCharges")]
    public class NumberParameterTi8 : ClauseNumberParameter<Ticket>
    {
        protected override decimal? NumberFunction(Ticket entity)
        {
            if (!entity.UnitCharges.Any(i=>i.Reason.Contains("Штраф"))) return 0;
            return entity.UnitCharges.Where(i => i.Reason.Contains("Штраф")).Sum(i => i.Charge);
        }
    }
#if BEAUTINIKA
    [DataContract]
    [ClauseRelation(typeof(Ticket))]
    [Description("Начальное к-во доп. единиц")]
    public class NumberParameterTi9a : ClauseNumberParameter<Ticket>
    {
        protected override decimal? NumberFunction(Ticket entity)
        {
            return entity.ExtraUnitsAmount;
        }
    }
    [DataContract]
    [ClauseRelation(typeof(Ticket))]
    [Description("Остаток доп. единиц")]
    [Include("UnitCharges")]
    public class NumberParameterTi10a : ClauseNumberParameter<Ticket>
    {
        protected override decimal? NumberFunction(Ticket entity)
        {
            entity.InitUnitsOut();
            return entity.ExtraUnitsLeft;
        }
    }
    [DataContract]
    [ClauseRelation(typeof(Ticket))]
    [Description("Потрачено доп. единиц")]
        [Include("UnitCharges")]
public class NumberParameterTi11a : ClauseNumberParameter<Ticket>
    {
        protected override decimal? NumberFunction(Ticket entity)
        {
            entity.InitUnitsOut();
            return entity.ExtraUnitsOut;
        }
    }
#endif
    [DataContract]
    [ClauseRelation(typeof(Ticket))]
    [Description("Начальное к-во гост. единиц")]
    public class NumberParameterTi9 : ClauseNumberParameter<Ticket>
    {
        protected override decimal? NumberFunction(Ticket entity)
        {
            return entity.GuestUnitsAmount;
        }
    }
    [DataContract]
    [ClauseRelation(typeof(Ticket))]
    [Description("Остаток гост. единиц")]
    [Include("UnitCharges")]
    public class NumberParameterTi10 : ClauseNumberParameter<Ticket>
    {
        protected override decimal? NumberFunction(Ticket entity)
        {
            entity.InitUnitsOut();
            return entity.GuestUnitsLeft;
        }
    }
    [DataContract]
    [ClauseRelation(typeof(Ticket))]
    [Description("Потрачено гост. единиц")]
    [Include("UnitCharges")]
    public class NumberParameterTi11 : ClauseNumberParameter<Ticket>
    {
        protected override decimal? NumberFunction(Ticket entity)
        {
            entity.InitUnitsOut();
            return entity.GuestUnitsOut;
        }
    }
    [DataContract]
    [ClauseRelation(typeof(Ticket))]
    [Description("Длительность")]
    public class NumberParameterTi12 : ClauseNumberParameter<Ticket>
    {
        protected override decimal? NumberFunction(Ticket entity)
        {
            return entity.Length;
        }
    }
    [DataContract]
    [ClauseRelation(typeof(Ticket))]
    [Description("Дней до окончания")]
    [Include("TicketFreezes")]
    public class NumberParameterTi13 : ClauseNumberParameter<Ticket>
    {
        protected override decimal? NumberFunction(Ticket entity)
        {
            entity.InitFinishDate();
            if (!entity.FinishDate.HasValue) return null;
            return (int)(entity.FinishDate.Value - DateTime.Now).TotalDays;
        }
    }
    [DataContract]
    [ClauseRelation(typeof(Ticket))]
    [Description("Дней между продажей и активацией")]
    public class NumberParameterTi14 : ClauseNumberParameter<Ticket>
    {
        protected override decimal? NumberFunction(Ticket entity)
        {
            if (!entity.StartDate.HasValue) return null;
            return (int)(entity.StartDate.Value - entity.CreatedOn).TotalDays;
        }
    }
    [DataContract]
    [ClauseRelation(typeof(Ticket))]
    [Description("Дней заморозки всего")]
    public class NumberParameterTi15 : ClauseNumberParameter<Ticket>
    {
        protected override decimal? NumberFunction(Ticket entity)
        {
            return entity.FreezesAmount;
        }
    }
    [DataContract]
    [ClauseRelation(typeof(Ticket))]
    [Description("Дней заморозки потрачено")]
    [Include("TicketFreezes")]
    public class NumberParameterTi16 : ClauseNumberParameter<Ticket>
    {
        protected override decimal? NumberFunction(Ticket entity)
        {
            return entity.FreezesAmount - entity.FreezesLeft;
        }
    }
    [DataContract]
    [ClauseRelation(typeof(Ticket))]
    [Description("Минут солярия всего")]
    public class NumberParameterTi17 : ClauseNumberParameter<Ticket>
    {
        protected override decimal? NumberFunction(Ticket entity)
        {
            return entity.SolariumMinutes;
        }
    }
    [DataContract]
    [ClauseRelation(typeof(Ticket))]
    [Description("Минут солярия потрачено*")]
    [Include("UnitCharges", "Division", "TicketPayments", "TicketFreezes", "TicketFreezes.TicketFreezeReason", "Successors", "MinutesCharges", "Division.Company", "SolariumVisits")]
    [InitAttribude]
    public class NumberParameterTi18 : ClauseNumberParameter<Ticket>
    {
        protected override decimal? NumberFunction(Ticket entity)
        {
            entity.InitDetails();
            return entity.SolariumMinutes-entity.SolariumMinutesLeft;
        }
    }
}
