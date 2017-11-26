using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ExtraClub.ServiceModel.Reports.ReportColumns
{
    [ReportColumns(typeof(Ticket))]
    public class TicketColumns
    {
        public Ticket entity { get; set; }

        public TicketColumns(Ticket value)
        {
            entity = value;
        }
        public Guid Id { get { return entity.Id; } }

        [Description("Номер")]
        public string fi1 { get { return entity.Number; } }
        [Description("Цена")]
        public decimal fi2 { get { return entity.Price; } }
        [Description("Стоимость")]
        public decimal fi3 { get { return entity.Cost; } }
        [Description("Размер скидки")]
        public decimal fi4 { get { return entity.DiscountPercent * entity.Price; } }
        [Description("Процент скидки")]
        public decimal fi5 { get { return entity.DiscountPercent * 100; } }

        [Description("Тип оплаты первого взноса")]
        public string fi6
        {
            get
            {
                switch (entity.FirstPmtTypeId)
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

        [Description("Количество единиц")]
        public int fi7 { get { return (int)entity.UnitsAmount; } }
        [Description("Остаток единиц")]
        [Include("UnitCharges")]
        public int fi8 { get { entity.InitUnitsOut(); return (int)entity.UnitsLeft; } }
        [Description("Потрачено единиц")]
        [Include("UnitCharges")]
        public int fi9 { get { entity.InitUnitsOut(); return (int)entity.UnitsOut; } }

        [Description("Списано за штрафы")]
        [Include("UnitCharges")]
        public int fi10
        {
            get
            {
                if (!entity.UnitCharges.Any(i => i.Reason.Contains("Штраф"))) return 0;
                return entity.UnitCharges.Where(i => i.Reason.Contains("Штраф")).Sum(i => i.Charge);
            }
        }
        [Description("Количество гостевых единиц")]
        public int fi11 { get { return (int)entity.GuestUnitsAmount; } }
        [Description("Остаток гостевых единиц")]
        [Include("UnitCharges")]
        public int fi12 { get { entity.InitUnitsOut(); return (int)entity.GuestUnitsLeft; } }
        [Description("Потрачено гостевых единиц")]
        [Include("UnitCharges")]
        public int fi13 { get { entity.InitUnitsOut(); return (int)entity.GuestUnitsOut; } }
        [Description("Длительность")]
        public int fi14 { get { return entity.Length; } }
        [Description("Закончился ли*")]

        [Include("UnitCharges", "Division", "TicketPayments", "TicketFreezes", "TicketFreezes.TicketFreezeReason", "Successors", "MinutesCharges", "Division.Company", "SolariumVisits")]
        [InitAttribude]
        public string fi15
        {
            get
            {
                return (entity.Status == TicketStatus.RunOut || entity.Status == TicketStatus.Expiried || entity.Status == TicketStatus.Rebilled || entity.Status == TicketStatus.Returned) ? "Да" : "Нет";
            }
        }
        [Description("Дней до окончания*")]
        [Include("UnitCharges", "Division", "TicketPayments", "TicketFreezes", "TicketFreezes.TicketFreezeReason", "Successors", "MinutesCharges", "Division.Company", "SolariumVisits")]
        [InitAttribude]
        public int? fi16
        {
            get
            {
                if (!entity.FinishDate.HasValue) return null;
                return (int)(entity.FinishDate.Value - DateTime.Now).TotalDays;
            }
        }
        [Description("Дата покупки")]
        public DateTime f17
        {
            get
            {
                return entity.CreatedOn;
            }
        }
        [Description("Дата активации")]
        public DateTime? f18
        {
            get
            {
                return entity.StartDate;
            }
        }
        [Description("Активирован ли")]
        public string f19
        {
            get
            {
                return entity.StartDate.HasValue ? "Да" : "Нет";
            }
        }
        [Description("Дней между продажей и активацией")]
        public int? f20
        {
            get
            {
                if (!entity.StartDate.HasValue) return null;
                return (int)(entity.StartDate.Value - entity.CreatedOn).TotalDays;
            }
        }
        [Description("Заморожен ли")]
        [Include("TicketFreezes")]
        public string f21
        {
            get
            {
                return entity.TicketFreezes.Any(i => i.StartDate <= DateTime.Now && i.FinishDate >= DateTime.Now) ? "Да" : "Нет";
            }
        }
        [Description("Дней заморозки всего")]
        public int f22 { get { return entity.FreezesAmount; } }
        [Description("Дней заморозки потрачено")]
        [Include("TicketFreezes")]
        public int f23 { get { return entity.FreezesAmount - entity.FreezesLeft; } }
        [Description("Минут солярия")]
        public decimal f24 { get { return entity.SolariumMinutes; } }
        [Description("Минут солярия потрачено*")]
        [Include("UnitCharges", "Division", "TicketPayments", "TicketFreezes", "TicketFreezes.TicketFreezeReason", "Successors", "MinutesCharges", "Division.Company", "SolariumVisits")]
        [InitAttribude]
        public decimal f25
        {
            get
            {
                return entity.SolariumMinutes - entity.SolariumMinutesLeft;
            }
        }
        [Description("Тип абонемента")]
        [Include("TicketType")]
        public string f27 { get { return entity.TicketType.Name; } }
        [Description("Акционный ли")]
        [Include("TicketType")]
        public string f28
        {
            get
            {
                return entity.TicketType.IsAction ? "Да" : "Нет";
            }
        }
        [Description("Франчайзи")]
        [Include("Division", "Division.Company")]
        public string f29 { get { return entity.Division.Company.CompanyName; } }

        [Description("Клуб")]
        [Include("Division")]
        public string f30 { get { return entity.Division.Name; } }
        [Description("ФИО продавшего")]
        [Include("CreatedBy")]
        public string f31 { get { return entity.CreatedBy.FullName; } }
        [Description("ФИО клиента")]
        [Include("Customer")]
        public string f32 { get { return entity.Customer.FullName; } }
        [Description("Карта клиента")]
        [Include("Customer", "Customer.CustomerCards", "Customer.CustomerCards.CustomerCardType")]
        public string f33 { get { entity.Customer.InitActiveCard(); if (entity.Customer.ActiveCard == null) return null; return entity.Customer.ActiveCard.CardBarcode; } }
        [Description("Тип карты клиента")]
        [Include("Customer", "Customer.CustomerCards", "Customer.CustomerCards.CustomerCardType")]
        public string f34 { get { entity.Customer.InitActiveCard(); if (entity.Customer.ActiveCard == null) return null; return entity.Customer.ActiveCard.CustomerCardType.Name; } }
        [Description("Статусы клиента")]
        [Include("Customer", "Customer.CustomerStatuses")]
        public string field35
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
        [Description("План внесения рассрочки")]
        public DateTime? f36 { get { return entity.PlanningInstalmentDay; } }

        [Description("Телефон клиента")]
        [Include("Customer")]
        public string f37 { get { return entity.Customer.Phone2; } }

        [Description("Дата возврата")]
        public DateTime? f38 { get { return entity.ReturnDate; } }

        [Description("Сумма возврата")]
        public decimal? f39 { get { return entity.ReturnCost; } }

    }
}