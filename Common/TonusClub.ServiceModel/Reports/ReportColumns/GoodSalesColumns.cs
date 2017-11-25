using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace TonusClub.ServiceModel.Reports.ReportColumns
{
    [ReportColumns(typeof(GoodSale))]
    public class GoodSalesColumns
    {
        GoodSale entity { get; set; }
        public GoodSalesColumns(GoodSale obj)
        {
            entity = obj;
        }
        public Guid Id { get { return entity.Id; } }

        [Description("Франчайзи")]
        public string f1 { get { return entity.BarOrder.Division.Company.CompanyName; } }
#if BEAUTINIKA
        [Description("Студия")]
#else
    [Description("Клуб")]
#endif
        public string f2 { get { return entity.BarOrder.Division.Name; } }
        [Description("Товар")]
        public string f3 { get { return entity.Good.Name; } }
        [Description("Количество")]
        public double f4 { get { return entity.Amount; } }
        [Description("Цена")]
        public decimal? f5 { get { return entity.PriceMoney; } }
        [Description("Цена бонус")]
        public decimal? f6 { get { return entity.PriceBonus; } }
        [Description("Дата покупки")]
        public DateTime? f6a { get { return entity.BarOrder.PurchaseDate; } }
        [Description("Возврат")]
        public DateTime? f7 { get { return entity.ReturnDate; } }
        [Description("Клиент")]
        public string f8 { get { return entity.BarOrder.Customer.FullName; } }
        [Description("Карта")]
        public string f9
        {
            get
            {
                entity.BarOrder.Customer.InitActiveCard();
                if (entity.BarOrder.Customer.ActiveCard == null) return null;
                return entity.BarOrder.Customer.ActiveCard.CardBarcode;
            }
        }
        [Description("Тип карты")]
        public string f10
        {
            get
            {
                entity.BarOrder.Customer.InitActiveCard();
                if (entity.BarOrder.Customer.ActiveCard == null) return null;
                return entity.BarOrder.Customer.ActiveCard.CustomerCardType.Name;
            }
        }
        [Description("Статусы клиента")]
        public string field11
        {
            get
            {
                if (!entity.BarOrder.Customer.CustomerStatuses.Any()) return null;
                var sb = new StringBuilder();
                entity.BarOrder.Customer.CustomerStatuses.OrderBy(i => i.Name).ToList().ForEach(i =>
                {
                    if (sb.Length > 0) sb.Append(", ");
                    sb.Append(i.Name);
                });
                return sb.ToString();
            }
        }
        [Description("Способ оплаты")]
        public string f12
        {
            get
            {
                switch (entity.BarOrder.PmtTypeId)
                {
                    case 0: return "Наличные";
                    case 1: return "Депозит";
                    case 2: return "Банк. карта";
                    case 3: return "Безнал";
                    case 4: return "Бонусы";
                }
                return null;
            }
        }
    }
}
