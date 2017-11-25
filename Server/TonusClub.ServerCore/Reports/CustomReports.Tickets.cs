using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using TonusClub.Entities;
using TonusClub.ServiceModel;

using System.Data.Objects;
using TonusClub.ServiceModel.Reports;
using System.Data.Objects.SqlClient;

namespace TonusClub.ServerCore
{
    partial class CustomReports
    {
        public DataTable CreditTicketsReport(DateTime start, DateTime end, Guid? divisionId)
        {
            start = start.Date;
            end = end.Date.AddDays(1);

            using(var context = new TonusEntities())
            {
                var res = new DataTable();

                res.Columns.Add("Карта");
                res.Columns.Add("ФИО");
                res.Columns.Add("Абонемент");
                res.Columns.Add("Тип абонемента");
                res.Columns.Add("Абонемент у клиента");
                res.Columns.Add("Полная стоимость", typeof(decimal));
                res.Columns.Add("Дата покупки", typeof(DateTime));
                res.Columns.Add("Первый взнос, руб.", typeof(decimal));
                res.Columns.Add("Первый взнос, %", typeof(decimal));
                res.Columns.Add("Дата отметки банка", typeof(DateTime));
                res.Columns.Add("Сумма вместе с комиссией", typeof(decimal));
                res.Columns.Add("Комиссия банка, руб.", typeof(decimal));
                res.Columns.Add("Комиссия банка, %", typeof(decimal));

                var user = UserManagement.GetUser(context);

                IEnumerable<Ticket> src = context.TicketPayments
                    .Where(i => i.CompanyId == user.CompanyId && i.PaymentDate >= start && i.PaymentDate < end)
                    .Select(i => i.Ticket)
                    .Distinct()
                    .Where(i => i.CreditInitialPayment.HasValue);
                if(divisionId.HasValue)
                {
                    src = src.Where(i => i.DivisionId == divisionId);
                }

                src = src.OrderBy(i => i.CreatedOn);

                var arr = src.Select(i => new
                {
                    Card = i.Customer.CustomerCards.Where(x => x.IsActive).Select(x => x.CardBarcode).FirstOrDefault(),
                    Fio = i.Customer.LastName + " " + i.Customer.FirstName + " " + i.Customer.MiddleName,
                    i.Number,
                    Count = i.Customer.Tickets.Count(j => j.CreatedOn <= i.CreatedOn),
                    TType = i.TicketType.Name,
                    Cost = i.Price * (1 - i.DiscountPercent),
                    i.CreatedOn,
                    i.CreditInitialPayment,
                    RetDate = (DateTime?)i.TicketPayments.OrderByDescending(j => j.PaymentDate).Select(j => j.PaymentDate).FirstOrDefault(),
                    Comis = i.CreditComission
                }).ToArray();

                foreach(var t in arr)
                {
                    res.Rows.Add(t.Card, t.Fio, t.Number, t.TType, t.Count, t.Cost, t.CreatedOn.Date, t.CreditInitialPayment,
                        t.Cost == 0 ? 0 : (t.CreditInitialPayment / t.Cost) * 100,
                        t.RetDate,
                        t.RetDate.HasValue ? t.Cost - t.CreditInitialPayment : null,
                        t.Comis,
                        t.Cost == t.CreditInitialPayment ? null : t.Comis / (t.Cost - t.CreditInitialPayment));
                }

                return res;
            }
        }
        public DataTable TicketPaymentsReport(DateTime start, DateTime end, Guid divisionId)
        {
            using(var context = new TonusEntities())
            {
                end = end.AddDays(1).AddSeconds(-1);
                var endStr = end.ToString("dd.MM.yyyy");

                var user = UserManagement.GetUser(context);

                var res = new DataTable();

                res.Columns.Add("_id", typeof(Guid));
                res.Columns.Add(ClubText, typeof(string));
                res.Columns.Add("Карта", typeof(string));
                res.Columns.Add("ФИО", typeof(string));
                res.Columns.Add("Абонемент", typeof(string));
                res.Columns.Add("Номер аб-та", typeof(string));
                res.Columns.Add("Дата продажи", typeof(DateTime));
                res.Columns.Add("Стоимость", typeof(decimal));
                res.Columns.Add("Тип рассрочки", typeof(string));
                res.Columns.Add("Оплачено на " + endStr, typeof(decimal));
                res.Columns.Add("Оплачено за период", typeof(decimal));
                res.Columns.Add("Платежи", typeof(string));
                res.Columns.Add("Задолженность на " + endStr, typeof(decimal));
                res.Columns.Add("Просроченная задолженность на " + endStr, typeof(decimal));
                res.Columns.Add("Дата возврата", typeof(DateTime));
                res.Columns.Add("Сумма возврата", typeof(decimal));
                res.Columns.Add("Способ оплаты первоначального взноса", typeof(string));
                res.Columns.Add("Планируемая дата платежа", typeof(DateTime));
                res.Columns.Add("Активен", typeof(string));
                res.Columns.Add("Комментарий", typeof(string));

                res.ExtendedProperties.Add("EntityType", typeof(Ticket));

                var ticketIds = context.Tickets
                    .Where(i => i.DivisionId == divisionId || (divisionId == Guid.Empty && i.CompanyId == user.CompanyId))
                    .Where(i => i.TicketPayments.Any(j => j.PaymentDate >= start && j.PaymentDate < end))
                    .Where(i => !i.ReturnDate.HasValue || i.ReturnDate >= end)
                    .Select(i => i.Id).ToList();
                ticketIds.AddRange(context.Tickets
                    .Where(i => i.DivisionId == divisionId || (divisionId == Guid.Empty && i.CompanyId == user.CompanyId))
                    .Where(i => i.Price * (1 - i.DiscountPercent) > i.TicketPayments.Where(j => j.PaymentDate < end).Sum(j => j.Amount))
                    .Select(i => i.Id));
                ticketIds = ticketIds.Distinct().ToList();

                var tickets = context.Tickets
                    .Where(i => ticketIds.Contains(i.Id))
                    .Select(i => new
                    {
                        Id = i.Id,
                        Division = i.Division.Name,
                        CardNumber = i.Customer.CustomerCards.FirstOrDefault(j => j.IsActive).CardBarcode,
                        Name = i.Customer.LastName + " " + i.Customer.FirstName + " " + i.Customer.MiddleName,
                        TName = i.TicketType.Name,
                        TNumber = i.Number,
                        SaleDate = i.CreatedOn,
                        Cost = i.Price * (1 - i.DiscountPercent),
                        Instalment = i.Instalment.Name,
                        PayedEnd = i.TicketPayments.Where(j => j.PaymentDate < end).Sum(j => j.Amount),
                        PayedPeriod = (decimal?)i.TicketPayments.Where(j => j.PaymentDate >= start && j.PaymentDate < end).Sum(j => j.Amount),
                        InstalmentLength = (int?)i.Instalment.Length,
                        InstalmentFirst = (decimal?)i.Instalment.ContribAmount,
                        InstalmentFirstPercent = (decimal?)i.Instalment.ContribPercent,
                        InstalmentSecondPercent = (decimal?)i.Instalment.SecondPercent,
                        InstalmentSecondLenght = (int?)i.Instalment.SecondLength,
                        CreatedOn = i.CreatedOn,
                        ReturnDate = i.ReturnDate,
                        ReturnAmount = i.ReturnCost,
                        Payments = i.TicketPayments.Where(j => j.Amount > 0).OrderBy(j => j.PaymentDate),
                        FirstOrder = context.BarOrders.FirstOrDefault(j => j.Id == i.TicketPayments.Where(k => k.Amount > 0).OrderBy(k => k.PaymentDate).Select(k => k.BarOrderId).FirstOrDefault()),
                        LoanPlanDate = i.PlanningInstalmentDay ?? i.LastInstalmentDay,
                        i.IsActive,
                        i.Comment
                    }).OrderBy(i => i.CreatedOn).ToList();
                tickets.ForEach(i => res.Rows.Add(i.Id, i.Division,
                    i.CardNumber, i.Name, i.TName, i.TNumber,
                    i.SaleDate, i.Cost, i.Instalment, i.PayedEnd, i.PayedPeriod ?? 0, GetPaymentsString(i.Payments),
                    i.Cost - i.PayedEnd, GetOverdueLoan(i, end), i.ReturnDate, i.ReturnAmount,
                    i.FirstOrder == null ? "" : i.FirstOrder.CardPayment > 0 ? "Банк.карта" : (i.FirstOrder.CashPayment > 0 ? "Наличные" : "Неизвестно"),
                    i.LoanPlanDate, i.IsActive ? "Да" : "Нет", i.Comment));
                return res;
            }
        }

        public DataTable TreatmentEventsReport(DateTime start, DateTime end, Guid divisionId)
        {
            using(var context = new TonusEntities())
            {
                end = end.AddDays(1).AddSeconds(-1);
                var endStr = end.ToString("dd.MM.yyyy");

                var user = UserManagement.GetUser(context);

                var res = new DataTable();

                res.Columns.Add("_id", typeof(Guid));
                res.Columns.Add("Карта", typeof(string));
                res.Columns.Add("Фамилия", typeof(string));
                res.Columns.Add("Имя", typeof(string));
                res.Columns.Add("Отчество", typeof(string));
                res.Columns.Add("Статус", typeof(string));
                res.Columns.Add("Телефон", typeof(string));
                res.Columns.Add("Дата рождения", typeof(string));
                res.Columns.Add("Дата", typeof(Date));
                res.Columns.Add("Время начала", typeof(string));
                res.Columns.Add("Время окончания", typeof(string));
                res.Columns.Add("Процедура", typeof(string));
                res.Columns.Add("Тренажер", typeof(string));
                res.Columns.Add("Статус записи", typeof(string));
                res.Columns.Add("Стоимость", typeof(int));
                res.Columns.Add("Списано", typeof(int));
#if BEAUTINIKA
                res.Columns.Add("Стоимость доп.", typeof(int));
                res.Columns.Add("Списано доп.", typeof(int));
#else
                res.Columns.Add("Остаток единиц", typeof(int));
                res.Columns.Add("Остаток гостевых единиц", typeof(int));
                res.Columns.Add("Дата окончания абонемента", typeof(DateTime));
                res.Columns.Add("Последнее посещение", typeof(DateTime));
#endif

                res.ExtendedProperties.Add("EntityType", typeof(TreatmentEvent));

                var src = context.TreatmentEvents.Where(i => i.DivisionId == divisionId && i.VisitDate >= start && i.VisitDate <= end)
                    .OrderBy(i => i.VisitDate)
                    .Select(i => new
                    {
                        Id = i.Id,
                        Card = i.Customer.CustomerCards.OrderByDescending(j => j.EmitDate).Where(j => j.IsActive).FirstOrDefault().CardBarcode,
                        Last = i.Customer.LastName,
                        First = i.Customer.FirstName,
                        Middle = i.Customer.MiddleName,
                        Phone = i.Customer.Phone2,
                        Birthday = i.Customer.Birthday,
                        Date = EntityFunctions.TruncateTime(i.VisitDate),
                        Time = i.VisitDate,
                        TimeDelta = i.TreatmentConfig.LengthCoeff * i.TreatmentConfig.TreatmentType.Duration,
                        TC = i.TreatmentConfig.Name,
                        T = i.Treatment.Tag ?? i.Treatment.TreatmentType.Name,
                        Stat = i.VisitStatus,

#if !BEAUTINIKA
                        Price = i.TreatmentConfig.Price,
                        Charge = (int?)context.UnitCharges.Where(j => j.EventId == i.Id).Select(j => j.Charge).FirstOrDefault(),
                        UnitsLeft = (decimal?)i.Ticket.UnitsAmount - (i.Ticket.UnitCharges.Sum(j => (int?)j.Charge) ?? 0),
                        GuestUnitsLeft = (decimal?)i.Ticket.GuestUnitsAmount - (i.Ticket.UnitCharges.Sum(j => (int?)j.GuestCharge) ?? 0),
                        TicketEnd = SqlFunctions.DateAdd("day", i.Ticket.Length, i.Ticket.StartDate),
                        LastVisit = i.Customer.CustomerVisits.Max(j => (DateTime?)j.InTime),

#else
                        Price = i.TreatmentConfig.IsMainTreatment ? i.TreatmentConfig.Price : 0,
                        PriceEx = i.TreatmentConfig.IsMainTreatment ? 0 : i.TreatmentConfig.Price,
                        Charge = (int?)context.UnitCharges.Where(j => j.EventId == i.Id).FirstOrDefault().Charge,
                        ChargeEx = (int?)context.UnitCharges.Where(j => j.EventId == i.Id).FirstOrDefault().ExtraCharge,
#endif
                        Status = i.Customer.CustomerStatuses.Select(j => j.Name)
                    })
                    .ToList();

                src.ForEach(i =>
                    {
                        res.Rows.Add(i.Id, i.Card, i.Last, i.First, i.Middle, String.Join("; ", i.Status), i.Phone, i.Birthday.HasValue ? i.Birthday.Value.ToString("d MMMM yyyy") : "",
                            (Date)i.Date, i.Time.ToString("HH:mm"), i.Time.AddMinutes(i.TimeDelta).ToString("HH:mm"), i.TC, i.T, TreatmentEvent.GetStatus(i.Stat),
                            i.Price, i.Charge
#if BEAUTINIKA
, i.PriceEx, i.ChargeEx
#else
, i.UnitsLeft, i.GuestUnitsLeft, i.TicketEnd, i.LastVisit
#endif
);
                    });


                return res;
            }
        }

        private string GetPaymentsString(IEnumerable<TicketPayment> payments)
        {
            var res = new StringBuilder();
            foreach(var p in payments)
            {
                if(res.Length > 0) res.Append("; ");
                res.AppendFormat("{0:dd.MM}, {1:c}", p.PaymentDate, p.Amount);
            }
            return res.ToString();
        }

        private decimal? GetOverdueLoan(dynamic i, DateTime end)
        {
            if(i.InstalmentLength == null) return null;
            if(i.Cost - i.PayedEnd <= 0) return null;
            var min = 0m;
            if(end > i.CreatedOn && end <= i.CreatedOn.AddDays(i.InstalmentLength))
            {
                min = (i.InstalmentFirst != null) ? i.InstalmentFirst : i.Cost * i.InstalmentFirstPercent;
            }
            else
                if(i.InstalmentSecondLenght != null && end > i.CreatedOn.AddDays(i.InstalmentLength) && end <= i.CreatedOn.AddDays(i.InstalmentLength + i.InstalmentSecondLenght))
                {
                    min = i.Cost * (1 - i.InstalmentSecondPercent);
                }
                else
                {
                    min = i.Cost;
                }
            if(i.PayedEnd < min)
            {
                return min - i.PayedEnd;
            }
            else
            {
                return null;
            }
        }


    }
}
