using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ExtraClub.Entities;
using ExtraClub.ServiceModel;

namespace ExtraClub.ServerCore
{
    internal class SalesFunnelReport
    {
        private Guid CompanyId { get; }
        private Guid? DivisionId { get; }
        private DateTime End { get; }
        private DateTime Start { get; }

        public SalesFunnelReport(DateTime start, DateTime end, Guid? divisionId)
        {
            Start = start.Date;
            End = end.Date.AddDays(1).AddTicks(-1);
            DivisionId = divisionId != Guid.Empty ? divisionId : null;
            using(var context = new ExtraEntities())
            {
                CompanyId = UserManagement.GetCompanyIdOrDefaultId(context);
            }
        }

        internal DataTable Execute()
        {
            using (var context = new ExtraEntities())
            {
                var advertGroups = context.Customers.Select(i => i.AdvertType.AdvertGroup).Distinct().Where(i => i != null).OrderBy(i => i.Name).ToList();

                var res = new DataTable();

                res.Columns.Add("Параметр", typeof(string));
                foreach (var ag in advertGroups)
                {
                    res.Columns.Add(ag.Name, typeof(decimal));
                }
                res.Columns.Add("Итого", typeof(decimal));

                var consultationTreatmentTypeId = Guid.Parse("73EE31C6-379E-49C3-8ADE-EB1F46282523");

            
                var selector = context.Customers.Where(i => i.CompanyId == CompanyId);
                var selectorConsultationsWithPeriod = context.TreatmentEvents
                    .Where(i => i.CompanyId == CompanyId)
                    .Where(i => i.VisitDate >= Start && i.VisitDate <= End && (i.VisitStatus == 2 || i.VisitStatus == 3) && i.TreatmentConfig.TreatmentTypeId == consultationTreatmentTypeId)
                    .Where(i => !DivisionId.HasValue || i.DivisionId == DivisionId)
                    .Select(i => new { i.VisitStatus, Customer = i.Customer });

                if (DivisionId.HasValue)
                {
                    selector = selector.Where(i => i.ClubId == DivisionId);
                }

                var selectorWithPeriod = selector.Where(i => i.CreatedOn >= Start && i.CreatedOn <= End);

                var grouper = new Func<IQueryable<Customer>, List<AggResult>>(x => x.Where(i => i.AdvertTypeId != null).GroupBy(i => i.AdvertType.AdvertGroup).Select(i => new AggResult { Key = i.Key, Count = i.Count() }).ToList());

                var totalNewCustomers = grouper(selectorWithPeriod);
                AddString(res, "Количество новых клиентов за период", totalNewCustomers, advertGroups);

                var consultationTotalAmount = grouper(selectorConsultationsWithPeriod.Select(i => i.Customer));
                AddString(res, "Количество консультаций", consultationTotalAmount, advertGroups);

                AddStringConversion(res, "Конверсия", consultationTotalAmount, totalNewCustomers, advertGroups);

                var consultationVisitedAmount = grouper(selectorConsultationsWithPeriod.Where(i => i.VisitStatus == 2).Select(i => i.Customer));
                AddString(res, "Количество посещенных консультаций", consultationVisitedAmount, advertGroups);

                AddStringConversion(res, "Конверсия", consultationVisitedAmount, consultationTotalAmount, advertGroups);

                var newCustomersWithTicketes = grouper(selectorWithPeriod.Where(i => i.Tickets.Any(t => !t.TicketType.IsGuest && !t.TicketType.IsVisit)));
                AddString(res, "Количество клиентов за период, купившие абонемент (не ДОД, не Гостевой)", newCustomersWithTicketes, advertGroups);

                AddStringConversion(res, "Конверсия", newCustomersWithTicketes, consultationVisitedAmount, advertGroups);

                var eventsCount = selectorWithPeriod
                    .Select(i => new { i.AdvertType.AdvertGroup, Count = i.Calls.Count() + i.CustomerCrmEvents.Count })
                    .GroupBy(i => i.AdvertGroup)
                    .Where(i => i.Key != null)
                    .Select(i => new AggResult { Key = i.Key, Count = i.Sum(j => j.Count) })
                    .ToList();
                AddString(res, "Количество событий по клиентам за период", eventsCount, advertGroups);

                var newCustomersWithMoreTicketes = grouper(selectorWithPeriod.Where(i => i.Tickets.Count(t => !t.TicketType.IsGuest && !t.TicketType.IsVisit) > 1));
                AddString(res, "Количество клиентов, купившие два абонемента или больше за период", newCustomersWithMoreTicketes, advertGroups);

                var newCustomersWithMoreTicketesNoPeriod = grouper(selector
                    .Where(i => i.Tickets.Count(t => !t.TicketType.IsGuest && !t.TicketType.IsVisit) > 1));
                AddString(res, "Количество клиентов, купившие два абонемента или больше за все время", newCustomersWithMoreTicketesNoPeriod, advertGroups);

                AverageTickets(res, "Среднее количество купленных абонементов клиентами за период", selectorWithPeriod, advertGroups);

                AverageTickets(res, "Среднее количество купленных абонементов всеми клиентами за все время", selector, advertGroups);

                var totalRecomendations = grouper(selectorWithPeriod.Where(i => i.InvitorId.HasValue));
                AddString(res, "Количество клиентов, которые пришли по рекомендации подруг за период", totalRecomendations, advertGroups);

                var totalRecomendationsNoPeriod = grouper(selector.Where(i => i.InvitorId.HasValue));
                AddString(res, "Количество клиентов, которые пришли по рекомендации подруг за все время", totalRecomendationsNoPeriod, advertGroups);

                GoodsSpendings(res, "Общая сумма потраченная на товары клиентами за период", selectorWithPeriod, advertGroups);
                GoodsSpendings(res, "Общая сумма потраченная на товары всеми клиентами за все время", selector, advertGroups);

                TicketsSpendings(res, "Общая сумма потраченная на абонементы клиентами за период", selectorWithPeriod, advertGroups);
                TicketsSpendings(res, "Общая сумма потраченная на абонементы всеми клиентами  за все время", selector, advertGroups);

                return res;
            }
        }

        private static void AverageTickets(DataTable res, string category, IQueryable<Customer> selector, List<AdvertGroup> advertGroups)
        {
            var src = selector
                .Select(i => new { i.AdvertType.AdvertGroup, Count = i.Tickets.Count(t => !t.ReturnDate.HasValue && !t.TicketType.IsGuest && !t.TicketType.IsVisit) })
                .Where(i => i.AdvertGroup != null)
                .GroupBy(i => i.AdvertGroup)
                .Select(i => new AggResult { Key = i.Key, Count = i.Where(j => j.AdvertGroup == i.Key).Count() == 0 ? 0 : i.Where(j => j.AdvertGroup == i.Key).Sum(j => (decimal?)j.Count ?? 0) / i.Where(j => j.AdvertGroup == i.Key).Count() })
                .ToList();

            AddString(res, category, src, advertGroups, true);
        }

        private static void GoodsSpendings(DataTable res, string category, IQueryable<Customer> selector, List<AdvertGroup> advertGroups)
        {
            var src = selector
                .Select(i => new { AdvertGroup = i.AdvertType.AdvertGroup, Sum = i.BarOrders.Where(b => b.GoodSales.Any()).Sum(b => b.CashPayment + b.CardPayment) })
                .GroupBy(i => i.AdvertGroup)
                .Where(i => i.Key != null)
                .Select(i => new AggResult { Key = i.Key, Count = (decimal?)i.Sum(j => j.Sum) ?? 0 }).ToList();

            AddString(res, category, src, advertGroups);
        }

        private static void TicketsSpendings(DataTable res, string category, IQueryable<Customer> selector, List<AdvertGroup> advertGroups)
        {
            var src = selector
                .Select(i => new { i.AdvertType.AdvertGroup, Sum = i.Tickets.Where(t => !t.ReturnDate.HasValue).SelectMany(t => t.TicketPayments).Sum(b => b.Amount) })
                .GroupBy(i => i.AdvertGroup)
                .Where(i => i.Key != null)
                .Select(i => new AggResult { Key = i.Key, Count = (decimal?)i.Sum(j => j.Sum) ?? 0 }).ToList();

            AddString(res, category, src, advertGroups);
        }

        private class AggResult
        {
            public AdvertGroup Key { get; set; }
            public decimal Count { get; set; }
        }

        private static void AddString(DataTable res, string argument, List<AggResult> data, List<AdvertGroup> advertGroups, bool isAvg = false)
        {
            var strArr = new List<object>();
            strArr.Add(argument);
            advertGroups.ForEach(ag => strArr.Add((decimal)(data.Where(i => i.Key.Id == ag.Id).FirstOrDefault()?.Count ?? 0)));
            if (!isAvg)
            {
                strArr.Add(data.Select(i => i.Count).Sum());
            }
            res.Rows.Add(strArr.ToArray());
        }

        private static void AddStringConversion(DataTable res, string argument, List<AggResult> data1, List<AggResult> data2, List<AdvertGroup> advertGroups)
        {
            var strArr = new List<object>();
            strArr.Add(argument);
            advertGroups.ForEach(ag =>
                strArr.Add(
                    ((decimal)(data1.Where(i => i.Key.Id == ag.Id).FirstOrDefault()?.Count ?? 0)
                    / (decimal)(data2.Where(i => i.Key.Id == ag.Id).FirstOrDefault()?.Count ?? 1))
                    * 100
                )
            );
            strArr.Add((data2.Select(i => i.Count).Sum() == 0 ? 0 : data1.Select(i => i.Count).Sum() / data2.Select(i => i.Count).Sum()) * 100);
            res.Rows.Add(strArr.ToArray());
        }
    }
}