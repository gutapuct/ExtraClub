using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using TonusClub.Entities;


namespace TonusClub.ServerCore
{
    partial class CustomReports
    {
#if BEAUTINIKA
        static string ClubText = "Студия";
        static string ClubTextR = "Студию";
        static string ClubTextV = "Студии";
        static string ClubTextU = "Студии";
        static string ClubTextUs = "Студиям";
#else
        static string ClubText="Клуб";
        static string ClubTextR="Клуб";
        static string ClubTextV = "Клуба";
        static string ClubTextU = "Клубу";
        static string ClubTextUs = "Клубам";
#endif
        public DataTable AdvertCustomers(DateTime start, DateTime end, Guid? divisionId, bool daily, bool weekly, bool years, bool groups, bool cost)
        {
            end = end.AddDays(1).AddSeconds(-1);

            if (weekly && daily)
            {
                throw new Exception("Детализация одновременно по дням и по неделям невозможна!");
            }
            if (!divisionId.HasValue)
            {
                throw new Exception("Необходимо указать "+ClubTextR+"!");
            }
            using (var context = new TonusEntities())
            {
                var div = context.Divisions.Single(i => i.Id == divisionId.Value);

                var res = new DataTable();
                res.ExtendedProperties.Add("Detailed", true);

                res.Columns.Add("Рекламная группа", typeof(string));
                res.Columns.Add("Рекламный канал", typeof(string));
                res.Columns.Add("Параметр", typeof(string));
                AddDatesColumns(res, start, end, daily, weekly, typeof(decimal), years);
                bool add, add1, add2, add3;

                var user = UserManagement.GetUser(context);

                if (!groups)
                    foreach (var adv in context.AdvertTypes.Where(i => i.IsAvail && (i.CompanyId == user.CompanyId || !i.CompanyId.HasValue))
                        .OrderBy(i => i.AdvertGroup.Name)
                        .ThenBy(i => i.Name))
                    {
                        var src = context.Tickets
                            .Where(i => (!i.TicketType.IsGuest && !i.TicketType.IsVisit)
                                && i.Customer.AdvertTypeId == adv.Id
                                && i.DivisionId == divisionId
                                && i.CreatedOn >= start
                                && i.CreatedOn < end
                                && i.Customer.Tickets
                                    .Where(j => (!j.TicketType.IsGuest && !j.TicketType.IsVisit)).OrderBy(j => j.CreatedOn)
                                    .FirstOrDefault().Id == i.Id)
                            .Select(i => new { CreatedOn = i.CreatedOn, CustomerId = i.CustomerId })
                            .ToList();
                        var row = CalcValue(new object[] { adv.AdvertGroup.Name, adv.Name },
                            "Количество новых клиентов (равно кол-ву абонементов)",
                            src,
                            start, end,
                            i => i.CreatedOn, daily, weekly, out add,
                            years, (i, _, __) => i.Select(j => j.CustomerId).Distinct().Count());

                        var src1 = context.Customers
                            .Where(c => c.AdvertTypeId == adv.Id
                                && c.Calls.Any(i => i.DivisionId == divisionId))
                            .Select(c => c.Calls.Where(i => i.DivisionId == divisionId).OrderBy(ca => ca.StartAt).FirstOrDefault())
                            .Where(i => i.StartAt >= start && i.StartAt < end)
                            .Select(i => i.StartAt)
                            .ToList();
                        var row1 = CalcValue(new object[] { adv.AdvertGroup.Name, adv.Name }, "Количество первых звонков",
                            src1,
                            start, end, i => i, daily, weekly, out add1, years, (i, _, __) => i.Count());

                        List<object> row2;
                        if (!cost)
                        {
                            var src2 = context.Tickets
                                .Where(i => (!i.TicketType.IsGuest && !i.TicketType.IsVisit)
                                    && i.Customer.AdvertTypeId == adv.Id
                                    && i.DivisionId == divisionId
                                    && i.Customer.Tickets.Where(j => (!j.TicketType.IsGuest && !j.TicketType.IsVisit)).OrderBy(j => j.CreatedOn).FirstOrDefault().Id == i.Id)
                                .Select(i => i.TicketPayments.FirstOrDefault())
                                .Where(i => i != null && i.PaymentDate >= start && i.PaymentDate < end)
                                .Select(i => new { PaymentDate = i.PaymentDate, CreatedOn = i.Ticket.CreatedOn, Amount = i.Amount })
                                .ToList();
                            row2 = CalcValue(new object[] { adv.AdvertGroup.Name, adv.Name },
                                "Первый взнос за 1й абонемент",
                                src2,
                                start, end,
                                i => i.PaymentDate, daily, weekly, out add2, years,
                                (i, s, f) => i.Where(j => j.CreatedOn >= s && j.CreatedOn < f).Sum(j => j.Amount));
                        }
                        else
                        {
                            var src2 = context.Tickets
                                .Where(i => (!i.TicketType.IsGuest && !i.TicketType.IsVisit)
                                    && i.Customer.AdvertTypeId == adv.Id
                                    && i.DivisionId == divisionId
                                    && i.Customer.Tickets.Where(j => (!j.TicketType.IsGuest && !j.TicketType.IsVisit)).OrderBy(j => j.CreatedOn).FirstOrDefault().Id == i.Id)
                                .Select(i => new { CreatedOn = i.CreatedOn, Amount = i.Price * (1 - i.DiscountPercent) })
                                .ToList();
                            row2 = CalcValue(new object[] { adv.AdvertGroup.Name, adv.Name },
                                "Стоимость первого абонемента",
                                src2,
                                start, end,
                                i => i.CreatedOn, daily, weekly, out add2, years,
                                (i, s, f) => i.Sum(j => j.Amount));
                        }

                        var src3 = context.Customers
                            .Where(i => i.AdvertTypeId == adv.Id
                                && !i.Calls.Any(j => j.StartAt < i.CustomerVisits.Min(k => k.InTime))
                                && i.ClubId == divisionId)
                                .Where(i => i.CreatedOn >= start && i.CreatedOn < end)
                                .Select(i => new { i.CreatedOn })
                            .ToList();
                        var row3 = CalcValue(new object[] { adv.AdvertGroup.Name, adv.Name }, "Количество первых приходов",
                            src3,
                            start, end,
                            i => i.CreatedOn, daily, weekly, out add3, years,
                            (i, _, __) => i.Count());


                        if (add || add1 || add2 || add3)
                        {
                            res.Rows.Add(row.ToArray());
                            res.Rows.Add(row1.ToArray());
                            res.Rows.Add(row2.ToArray());
                            res.Rows.Add(row3.ToArray());
                        }
                    }
                else
                {
                    foreach (var adv in context.AdvertGroups.Where(i => i.IsActive).OrderBy(i => i.Name))
                    {
                        var src = context.Tickets.Where(i => i.Customer.AdvertTypeId.HasValue)
                            .Where(i => (!i.TicketType.IsGuest && !i.TicketType.IsVisit)
                                && i.Customer.AdvertType.AdvertGroupId == adv.Id
                                && i.DivisionId == divisionId
                                && i.CreatedOn >= start
                                && i.CreatedOn < end
                                && i.Customer.Tickets.Where(j => (!j.TicketType.IsGuest && !j.TicketType.IsVisit)).OrderBy(j => j.CreatedOn).FirstOrDefault().Id == i.Id)
                            .Select(i => new { CreatedOn = i.CreatedOn, CustomerId = i.CustomerId })
                            .ToList();
                        var row = CalcValue(new object[] { adv.Name, "" }, "Количество новых клиентов (равно кол-ву абонементов)",
                            src,
                            start, end, i => i.CreatedOn, daily, weekly, out add, years,
                            (i, _, __) => i.Select(j => j.CustomerId).Distinct().Count());

                        var src1 = context.Customers
                            .Where(i => i.AdvertTypeId.HasValue)
                            .Where(c => c.AdvertType.AdvertGroupId == adv.Id
                                && c.Calls.Any(i => i.DivisionId == divisionId))
                            .Select(c => c.Calls.Where(i => i.DivisionId == divisionId).OrderBy(ca => ca.StartAt).FirstOrDefault())
                            .Where(i => i.StartAt >= start && i.StartAt < end)
                            .Select(i => i.StartAt)
                            .ToList();
                        var row1 = CalcValue(new object[] { adv.Name, "" }, "Количество первых звонков",
                            src1,
                            start, end, i => i, daily, weekly, out add1, years, (i, _, __) => i.Count());

                        List<object> row2;
                        if (!cost)
                        {
                            var src2 = context.Tickets
                                .Where(i => (!i.TicketType.IsGuest && !i.TicketType.IsVisit)
                                    && i.Customer.AdvertType.AdvertGroupId == adv.Id
                                    && i.DivisionId == divisionId
                                    && i.Customer.Tickets.Where(j => (!j.TicketType.IsGuest && !j.TicketType.IsVisit)).OrderBy(j => j.CreatedOn).FirstOrDefault().Id == i.Id)
                                .Select(i => i.TicketPayments.FirstOrDefault())
                                .Where(i => i != null && i.PaymentDate >= start && i.PaymentDate < end)
                                .Select(i => new { PaymentDate = i.PaymentDate, CreatedOn = i.Ticket.CreatedOn, Amount = i.Amount })
                                .ToList();
                            row2 = CalcValue(new object[] { adv.Name, "" },
                                "Первый взнос за 1й абонемент",
                                src2,
                                start, end,
                                i => i.PaymentDate, daily, weekly, out add2, years,
                                (i, s, f) => i.Where(j => j.CreatedOn >= s && j.CreatedOn < f).Sum(j => j.Amount));
                        }
                        else
                        {
                            var src2 = context.Tickets
                                .Where(i => (!i.TicketType.IsGuest && !i.TicketType.IsVisit)
                                    && i.Customer.AdvertType.AdvertGroupId == adv.Id
                                    && i.DivisionId == divisionId
                                    && i.Customer.Tickets.Where(j => (!j.TicketType.IsGuest && !j.TicketType.IsVisit)).OrderBy(j => j.CreatedOn).FirstOrDefault().Id == i.Id)
                                .Select(i => new { CreatedOn = i.CreatedOn, Amount = i.Price * (1 - i.DiscountPercent) })
                                .ToList();
                            row2 = CalcValue(new object[] { adv.Name, "" },
                                "Стоимость первого абонемента",
                                src2,
                                start, end,
                                i => i.CreatedOn, daily, weekly, out add2, years,
                                (i, s, f) => i.Sum(j => j.Amount));
                        }

                        var src3 = context.Customers
                            .Where(i => i.AdvertTypeId.HasValue)
                            .Where(i => i.AdvertType.AdvertGroupId == adv.Id
                                && !i.Calls.Any(j => j.StartAt < i.CustomerVisits.Min(k => k.InTime))
                                && i.ClubId == divisionId)
                                .Where(i => i.CreatedOn >= start && i.CreatedOn < end)
                                .Select(i => new { i.CreatedOn }).ToList();
                        var row3 = CalcValue(new object[] { adv.Name, "" }, "Количество первых приходов",
                            src3,
                            start, end, i => i.CreatedOn, daily, weekly, out add3, years, (i, _, __) => i.Count());


                        if (add || add1 || add2 || add3)
                        {
                            res.Rows.Add(row.ToArray());
                            res.Rows.Add(row1.ToArray());
                            res.Rows.Add(row2.ToArray());
                            res.Rows.Add(row3.ToArray());
                        }
                    }
                }

                return res;
            }
        }
    }
}
