using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace ExtraClub.ServiceModel.Reports.ClauseParameters.Customers
{
    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Возраст")]
    public class NumberParameter1 : ClauseNumberParameter<Customer>
    {
        protected override decimal? NumberFunction(Customer entity)
        {
            if (!entity.Birthday.HasValue) return null;
            return (int)Math.Floor((DateTime.Today - entity.Birthday.Value).TotalDays / 365.25);
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Дней до окончания последнего проданного аб-та")]
    [Include("Tickets")]
    public class NumberParameter2 : ClauseNumberParameter<Customer>
    {
        protected override decimal? NumberFunction(Customer entity)
        {
            if (!entity.Tickets.Any()) return null;
            var ticket = entity.Tickets.OrderByDescending(i => i.CreatedOn).First();
            ticket.InitDetails();
            return (int)(((ticket.FinishDate ?? (ticket.CreatedOn.AddDays((ticket.TicketType.AutoActivate ?? 0) + ticket.Length))) - DateTime.Today).TotalDays);
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Единиц на последнем проданном активном аб-те")]
    [Include("Tickets")]
    public class NumberParameter3 : ClauseNumberParameter<Customer>
    {
        protected override decimal? NumberFunction(Customer entity)
        {
            if (!entity.Tickets.Any(i => i.IsActive)) return null;
            var ticket = entity.Tickets.Where(i => i.IsActive).OrderByDescending(i => i.CreatedOn).First();
            ticket.InitDetails();
            return ticket.UnitsLeft;
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Коэффициент активности*")]
    [Include("TreatmentEvents", "TreatmentEvents.TreatmentConfig", "TreatmentEvents.TreatmentConfig.TreatmentType", "TreatmentEvents.TreatmentConfig.TreatmentType.TreatmentTypeGroup")]
    public class NumberParameter4 : ClauseNumberParameter<Customer>
    {
        protected override decimal? NumberFunction(Customer entity)
        {
            if (entity.TreatmentEvents.Where(i => i.VisitStatus == 2).Count() == 0) return null;
            var actives = entity.TreatmentEvents.Where(i => i.VisitStatus == 2).Where(e => e.TreatmentConfig.TreatmentType.TreatmentTypeGroupId.HasValue && e.TreatmentConfig.TreatmentType.TreatmentTypeGroup.Name == "Активный");
            if (actives.Count() == 0) return 0;
            return (decimal)actives.Count() / (decimal)entity.TreatmentEvents.Where(i => i.VisitStatus == 2).Count();
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Средний чек")]
    [Include("BarOrders", "BarOrders.GoodSales")]
    public class NumberParameter5 : ClauseNumberParameter<Customer>
    {
        protected override decimal? NumberFunction(Customer entity)
        {
            var sales = entity.BarOrders.SelectMany(i => i.GoodSales);
            if (sales.Count() == 0) return null;
            var days = entity.BarOrders.Where(i => i.GoodSales.Any()).Select(i => i.PurchaseDate.Date).Distinct().Count();
            if (days == 0) return null;

            return sales.Sum(i => i.Cost) / (decimal)days;
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Средний чек за последний месяц")]
    [Include("BarOrders", "BarOrders.GoodSales")]
    public class NumberParameter6 : ClauseNumberParameter<Customer>
    {
        protected override decimal? NumberFunction(Customer entity)
        {
            var date = DateTime.Now.AddMonths(-1);
            var sales = entity.BarOrders.Where(i => i.PurchaseDate >= date).SelectMany(i => i.GoodSales);
            if (sales.Count() == 0) return null;
            var days = entity.BarOrders.Where(i => i.PurchaseDate >= date).Where(i => i.GoodSales.Any()).Select(i => i.PurchaseDate.Date).Distinct().Count();
            if (days == 0) return null;

            return sales.Sum(i => i.Cost) / (decimal)days;
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Средний чек за последний год")]
    [Include("BarOrders", "BarOrders.GoodSales")]
    public class NumberParameter7 : ClauseNumberParameter<Customer>
    {
        protected override decimal? NumberFunction(Customer entity)
        {
            var date = DateTime.Now.AddYears(-1);
            var sales = entity.BarOrders.Where(i => i.PurchaseDate >= date).SelectMany(i => i.GoodSales);
            if (sales.Count() == 0) return null;
            var days = entity.BarOrders.Where(i => i.PurchaseDate >= date).Where(i => i.GoodSales.Any()).Select(i => i.PurchaseDate.Date).Distinct().Count();
            if (days == 0) return null;

            return sales.Sum(i => i.Cost) / (decimal)days;
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Средний чек с момента активации последнего аб-та")]
    [Include("BarOrders", "BarOrders.GoodSales", "Tickets")]
    public class NumberParameter8 : ClauseNumberParameter<Customer>
    {
        protected override decimal? NumberFunction(Customer entity)
        {
            var ticks = entity.Tickets.Where(i => i.StartDate.HasValue);
            if (ticks.Count() == 0) return null;
            var date = ticks.Max(i => i.StartDate);

            var sales = entity.BarOrders.Where(i => i.PurchaseDate >= date).SelectMany(i => i.GoodSales);
            if (sales.Count() == 0) return null;
            var days = entity.BarOrders.Where(i => i.PurchaseDate >= date).Where(i => i.GoodSales.Any()).Select(i => i.PurchaseDate.Date).Distinct().Count();
            if (days == 0) return null;

            return sales.Sum(i => i.Cost) / (decimal)days;
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Бонусный счет")]
    [Include("BonusAccounts", "DepositAccounts")]
    public class NumberParameter9 : ClauseNumberParameter<Customer>
    {
        protected override decimal? NumberFunction(Customer entity)
        {
            entity.InitDepositValues();
            return entity.BonusDepositValue;
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Размер депозита")]
    [Include("BonusAccounts", "DepositAccounts")]
    public class NumberParameter10 : ClauseNumberParameter<Customer>
    {
        protected override decimal? NumberFunction(Customer entity)
        {
            entity.InitDepositValues();
            return entity.RurDepositValue;
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Всего бонусов потрачено")]
    [Include("BonusAccounts")]
    public class NumberParameter11 : ClauseNumberParameter<Customer>
    {
        protected override decimal? NumberFunction(Customer entity)
        {
            if (entity.BonusAccounts.Where(i => i.Amount < 0).Count() == 0) return 0;
            return entity.BonusAccounts.Where(i => i.Amount < 0).Sum(i => i.Amount);
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Всего депозита потрачено")]
    [Include("DepositAccounts")]
    public class NumberParameter12 : ClauseNumberParameter<Customer>
    {
        protected override decimal? NumberFunction(Customer entity)
        {
            if (entity.DepositAccounts.Where(i => i.Amount < 0).Count() == 0) return 0;
            return entity.DepositAccounts.Where(i => i.Amount < 0).Sum(i => i.Amount);
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Число дней с последнего посещения")]
    [Include("CustomerVisits")]
    public class NumberParameter13 : ClauseNumberParameter<Customer>
    {
        protected override decimal? NumberFunction(Customer entity)
        {
            if (!entity.CustomerVisits.Any()) return null;
            var lv = entity.CustomerVisits.OrderByDescending(i => i.InTime).First();
            if (lv == null) return null;
            return (decimal)(DateTime.Today - lv.InTime.Date).TotalDays;
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Количество дней до дня рождения")]
    public class NumberParameter14 : ClauseNumberParameter<Customer>
    {
        protected override decimal? NumberFunction(Customer entity)
        {
            if (!entity.Birthday.HasValue) return null;
            try
            {
                try
                {
                    var bd = new DateTime(DateTime.Today.Year, entity.Birthday.Value.Month, entity.Birthday.Value.Day);
                    var days = (int)((bd - DateTime.Today).TotalDays);
                    if (days < 0) return days + 365;
                    return days;
                }
                catch (ArgumentOutOfRangeException)
                {
                    return null;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Всего единиц на активных аб-х*")]
    [Include("Tickets", "Tickets.UnitCharges")]
    public class NumberParameter15 : ClauseNumberParameter<Customer>
    {
        protected override decimal? NumberFunction(Customer entity)
        {
            var ticks = entity.Tickets.Where(i => i.IsActive || !i.StartDate.HasValue).ToList();
            if (!ticks.Any()) return 0;
            ticks.ForEach(i => i.InitUnitsOut());
            return ticks.Sum(i => (int)i.UnitsLeft);
        }
    }

}
