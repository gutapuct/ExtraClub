using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ExtraClub.ServiceModel.Reports.ReportColumns
{
    [ReportColumns(typeof(CustomerCard))]
    public class CustomerCardsColumns
    {
        public CustomerCard entity { get; set; }
        public CustomerCardsColumns(CustomerCard value)
        {
            entity = value;
        }
        public Guid Id { get { return entity.Id; } }

        [Description("Тип")]
        public string f1 { get { return entity.CustomerCardType.Name; } }
        [Description("Гостевая ли")]
        public string f2 { get { return entity.CustomerCardType.IsGuest ? "Да" : "Нет"; } }
        [Description("Визитера ли")]
        public string f3 { get { return entity.CustomerCardType.IsVisit ? "Да" : "Нет"; } }
        [Description("Цена")]
        public decimal f4 { get { return entity.Price; } }
        [Description("Тип оплаты")]
        public string f5
        {
            get
            {
                switch (entity.PmtTypeId)
                {
                    case 0:
                        return "Наличные";
                    case 1:
                        return "Депозит";
                    case 2:
                        return "Банк. карта";
                    default:
                        return "Безнал";
                }
            }
        }
        [Description("ФИО клиента")]
        public string f6 { get { return entity.Customer.FullName; } }
        [Description("Статусы клиента")]
        public string f7
        {
            get
            {
                if (!entity.Customer.CustomerStatuses.Any()) return null;
                var sb = new StringBuilder();
                entity.Customer.CustomerStatuses.OrderBy(i => i.Name).ToList().ForEach(i =>
                {
                    if (sb.Length > 0) sb.Append(", ");
                    sb.Append(i.Name);
                });
                return sb.ToString();
            }
        }
        [Description("Дата покупки")]
        public DateTime f8 { get { return entity.EmitDate; } }
        [Description("Клуб")]
        public string f8a { get { return entity.Division.Name; } }
        [Description("Активна ли")]
        public string f9 { get { return entity.IsActive ? "Да" : "Нет"; } }
        [Description("Тип предыдущей карты")]
        public string f10
        {
            get
            {
                var prev = entity.Customer.CustomerCards.Where(i => i.EmitDate < entity.EmitDate).OrderByDescending(i => i.EmitDate).FirstOrDefault();
                if (prev == null) return null;
                return prev.CustomerCardType.Name;

            }
        }
        [Description("Тип следующей карты")]
        public string f11
        {
            get
            {
                var next = entity.Customer.CustomerCards.Where(i => i.EmitDate > entity.EmitDate).OrderBy(i => i.EmitDate).FirstOrDefault();
                if (next == null) return null;
                return next.CustomerCardType.Name;
            }
        }
        [Description("Типы купленных аб-в")]
        public string f12
        {
            get
            {
                if (!entity.Customer.Tickets.Any()) return null;
                var sb = new StringBuilder();
                entity.Customer.Tickets.Select(i => i.TicketType).Distinct().OrderBy(i => i.Name).ToList().ForEach(i =>
                {
                    if (sb.Length > 0) sb.Append(", ");
                    sb.Append(i.Name);
                });
                return sb.ToString();

            }
        }
        [Description("Типы активных аб-в")]
        public string f13
        {
            get
            {
                if (!entity.Customer.Tickets.Any(i=>i.IsActive)) return null;
                var sb = new StringBuilder();
                entity.Customer.Tickets.Where(i => i.IsActive).Select(i => i.TicketType).Distinct().OrderBy(i => i.Name).ToList().ForEach(i =>
                {
                    if (sb.Length > 0) sb.Append(", ");
                    sb.Append(i.Name);
                });
                return sb.ToString();

            }
        }
        [Description("Номер")]
        public string f14
        {
            get
            {
                return entity.CardBarcode;
            }
        }

    }
}