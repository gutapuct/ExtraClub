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
    [Description("Дата рождения")]
    public class DateParameter1 : ClauseDateParameter<Customer>
    {
        protected override DateTime? DateFunction(Customer entity)
        {
            return entity.Birthday;
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Дата создания")]
    public class DateParameter2 : ClauseDateParameter<Customer>
    {
        protected override DateTime? DateFunction(Customer entity)
        {
            return entity.CreatedOn;
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Дата покупки первой карты")]
    [Include("CustomerCards")]
    public class DateParameter3 : ClauseDateParameter<Customer>
    {
        protected override DateTime? DateFunction(Customer entity)
        {
            if (entity.CustomerCards.Count == 0) return null;
            return entity.CustomerCards.Min(i => i.EmitDate);
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Дата последней замены карты")]
    [Include("CustomerCards")]
    public class DateParameter3a : ClauseDateParameter<Customer>
    {
        protected override DateTime? DateFunction(Customer entity)
        {
            if (entity.CustomerCards.Count < 2) return null;
            return entity.CustomerCards.Max(i => i.EmitDate);
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Последняя дата истечения аб-та*")]
    [Include("Tickets")]
    public class DateParameter4 : ClauseDateParameter<Customer>
    {
        protected override DateTime? DateFunction(Customer entity)
        {
            if (entity.Tickets.ToList().Where(i => { i.InitDetails(); return i.FinishDate.HasValue; }).Count() == 0) return null;
            return entity.Tickets.ToList().Where(i => { i.InitDetails(); return i.FinishDate.HasValue; }).Max(i => i.FinishDate);
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Дата последней активации аб-та")]
    [Include("Tickets")]
    public class DateParameter5 : ClauseDateParameter<Customer>
    {
        protected override DateTime? DateFunction(Customer entity)
        {
            if (entity.Tickets.Where(i => i.StartDate.HasValue).Count() == 0) return null;
            return entity.Tickets.Where(i => i.StartDate.HasValue).Max(i => i.StartDate);
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Дата покупки первого аб-та")]
    [Include("Tickets")]
    public class DateParameter6 : ClauseDateParameter<Customer>
    {
        protected override DateTime? DateFunction(Customer entity)
        {
            if (entity.Tickets.Count() == 0) return null;
            return entity.Tickets.Min(i => i.CreatedOn);
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Дата покупки последнего аб-та")]
    [Include("Tickets")]
    public class DateParameter7 : ClauseDateParameter<Customer>
    {
        protected override DateTime? DateFunction(Customer entity)
        {
            if (entity.Tickets.Count() == 0) return null;
            return entity.Tickets.Max(i => i.CreatedOn);
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Customer))]
    [Description("Дата последней покупки в баре")]
    [Include("BarOrders", "BarOrders.GoodSales")]
    public class DateParameter8 : ClauseDateParameter<Customer>
    {
        protected override DateTime? DateFunction(Customer entity)
        {
            if (entity.BarOrders.Where(i => i.GoodSales.Any()).Count() == 0) return null;
            return entity.BarOrders.Where(i => i.GoodSales.Any()).Max(i => i.PurchaseDate);
        }
    }
}
