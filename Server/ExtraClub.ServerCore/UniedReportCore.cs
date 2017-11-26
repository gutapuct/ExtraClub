using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.Objects.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using ExtraClub.Entities;

using ExtraClub.ServiceModel.Reports;

namespace ExtraClub.ServerCore
{
    public static class UniedReportCore
    {
        public static List<SalesData> GetUniedReportTicketSalesDynamic(bool isClub, Guid divisionId)
        {
            var res = new List<SalesData>();
            var mfi = CultureInfo.CurrentCulture.DateTimeFormat;
            for(int i = 1; i < 13; i++)
            {
                res.Add(new SalesData { MonthName = mfi.GetMonthName(i).ToString(), Amount = new List<decimal?>() });
            }
            using(var context = new ExtraEntities())
            {
                var cId = UserManagement.GetUser(context).CompanyId;
                var sales = context.Tickets
                    .Where(i => !isClub || divisionId == i.DivisionId)
                    .Where(i => i.CompanyId == cId && i.TicketPayments.Any())
                    .Select(i => new { Date = i.TicketPayments.Min(j => j.PaymentDate), Ticket = i })
                    .GroupBy(i => new { i.Date.Month, i.Date.Year })
                    .Select(i => new { Year = i.Key.Year, Month = i.Key.Month, Amount = i.Sum(j => j.Ticket.Price * (1 - j.Ticket.DiscountPercent)) })
                    .ToList();

                for(var i = DateTime.Today.Year; i > DateTime.Today.Year - 5; i--)
                {
                    if(i != DateTime.Today.Year && !sales.Any(j => j.Year == i && j.Amount > 0))
                    {
                        break;
                    }
                    for(var j = 1; j < 13; j++)
                    {
                        var item = sales.FirstOrDefault(k => k.Year == i && k.Month == j);
                        if(item == null)
                        {
                            res[j - 1].Amount.Add(null);
                        }
                        else
                        {
                            res[j - 1].Amount.Add(item.Amount / 1000);
                        }
                    }
                }
            }
            return res;
        }

        public static List<ChannelData> GetUniedReportSalesChannels(int days)
        {
            using(var context = new ExtraEntities())
            {
                var cId = UserManagement.GetUser(context).CompanyId;
                var start = DateTime.Today.AddDays(-days);
                return context.Customers.Where(i => i.CreatedOn >= start && i.AdvertTypeId.HasValue && i.CompanyId == cId).GroupBy(i => i.AdvertType.AdvertGroup)
                    .Select(i => new ChannelData { CatName = i.Key.Name, Value = i.Count() }).ToList();

            }
        }

        public static List<ChannelData> GetUniedReportAmountTreatments(int days)
        {
            using (var context = new ExtraEntities())
            {
                var cId = UserManagement.GetUser(context).CompanyId;
                var start = DateTime.Today.AddDays(-days);

                var result = context.TreatmentEvents
                    .Where(i => i.CompanyId == cId && i.VisitDate >= start && (i.VisitStatus == 2 || i.VisitStatus == 3))
                    .GroupBy(i => i.Treatment.TreatmentType.Name)
                    .OrderByDescending(i => i.Count())
                    .Select(i => new ChannelData
                    {
                        CatName = i.Key,
                        Value = i.Count()
                    }).ToList();

                result.ForEach(t =>
                    t.CatName = t.CatName.Substring(0, (t.CatName.Contains("(")) ? t.CatName.LastIndexOf('(') : t.CatName.Length - 1)
                );

                return result;
            }
        }

        /// <summary>
        /// kind: 0=Custs
        /// 1=procs
        /// 2=units
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        public static List<SalesData> GetUniedReportVisitsDynamics(int kind)
        {
            var res = new List<SalesData>();
            var mfi = CultureInfo.CurrentCulture.DateTimeFormat;
            for(int i = 1; i < 7; i++)
            {
                res.Add(new SalesData { MonthName = mfi.GetDayName((DayOfWeek)i).ToString(), Amount = new List<decimal?>() });
            }
            res.Add(new SalesData { MonthName = mfi.GetDayName(DayOfWeek.Sunday).ToString(), Amount = new List<decimal?>() });
            using(var context = new ExtraEntities())
            {
                var firstSunday = new DateTime(1753, 1, 7);
                var cId = UserManagement.GetUser(context).CompanyId;

                var start = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);
                if(DateTime.Today.DayOfWeek == DayOfWeek.Sunday) start = start.AddDays(-7);

                for(int t = 0; t < 3; t++)
                {
                    var end = start.AddDays(7);

                    List<DayAmount> visits;
                    if(kind == 0)
                    {
                        visits = context.CustomerVisits.Where(i => i.CompanyId == cId && i.InTime >= start && i.InTime < end)
                            .GroupBy(i => EntityFunctions.DiffDays(firstSunday, i.InTime) % 7).Select(i => new DayAmount { Day = i.Key, Amount = i.Count() }).ToList();
                    }
                    else if(kind == 1)
                    {
                        visits = context.TreatmentEvents.Where(i => i.CompanyId == cId && i.VisitDate >= start && i.VisitDate < end && i.VisitStatus == 2)
                            .GroupBy(i => EntityFunctions.DiffDays(firstSunday, i.VisitDate) % 7).Select(i => new DayAmount { Day = i.Key, Amount = i.Count() }).ToList();
                    }
                    else
                    {
                        visits = context.UnitCharges.Where(i => i.CompanyId == cId && i.Date >= start && i.Date < end)
                            .GroupBy(i => new { i.Ticket.CustomerId, Date = EntityFunctions.TruncateTime(i.Date) })
                            .Select(i => new { Count = Math.Ceiling(i.Sum(j => (decimal)j.Charge) / 8), i.Key.Date })
                            .GroupBy(i => EntityFunctions.DiffDays(firstSunday, i.Date) % 7).Select(i => new DayAmount { Day = i.Key, Amount = i.Sum(j => (int?)j.Count) }).ToList();
                    }
                    for(int i = 1; i < 8; i++)
                    {
                        var x = visits.FirstOrDefault(j => (j.Day.Value == 0 ? 7 : (j.Day.Value)) == i);
                        if(x == null)
                        {
                            res[i - 1].Amount.Add(null);
                        }
                        else
                        {
                            res[i - 1].Amount.Add(x.Amount);
                        }
                    }
                    start = start.AddDays(-7);
                }
            }
            return res;
        }

        public class DayAmount
        {
            public int? Day { get; set; }
            public int? Amount { get; set; }
        }

        public static List<SalesData> GetUniedReportIncomeAmount(bool isClub, Guid divisionId)
        {
            using(var context = new ExtraEntities())
            {
                var res = new List<SalesData>();

                var cId = UserManagement.GetCompanyIdOrDefaultId(context);
                var dateFrom = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddYears(-1).AddMonths(1);
                var dateTo = dateFrom.AddYears(1);
                var mfi = CultureInfo.CurrentCulture.DateTimeFormat;
                var data1_1 = context.BarOrders.Where(i => i.CompanyId == cId && i.PurchaseDate >= dateFrom && i.PurchaseDate < dateTo)
                    .Where(i => !isClub || divisionId == i.DivisionId)
                    .GroupBy(i => SqlFunctions.DatePart("month", i.PurchaseDate))
                    .Select(i => new { i.Key, Cash = i.Sum(x => (decimal?)x.CashPayment), Bank = i.Sum(x => (decimal?)x.CardPayment) }).ToArray();
                //возвраты абонементов
                var data1_2 = context.TicketPayments.Where(i => i.CompanyId == cId && i.PaymentDate >= dateFrom && i.PaymentDate < dateTo)
                    .Where(i => !isClub || divisionId == i.Ticket.DivisionId)
                    .Where(i => i.Ticket.ReturnDate.HasValue)
                    .GroupBy(i => SqlFunctions.DatePart("month", i.PaymentDate))
                    .Select(i => new { i.Key, Cash = i.Sum(x => (decimal?)x.Ticket.ReturnCost) }).ToArray();
                //возвраты товаров
                var data1_3 = context.GoodSales.Where(i => i.CompanyId == cId && i.ReturnDate.HasValue && i.ReturnDate >= dateFrom && i.ReturnDate < dateTo)
                    .Where(i => !isClub || divisionId == i.BarOrder.DivisionId)
                    .GroupBy(i => SqlFunctions.DatePart("month", i.ReturnDate))
                    .Select(i => new { i.Key, Cash = i.Sum(x => (decimal?)x.BarOrder.CashPayment), Bank = i.Sum(x => (decimal?)x.BarOrder.CardPayment) }).ToArray();
                var data2 = context.TicketPayments
                    .Where(i => i.CompanyId == cId && i.PaymentDate >= dateFrom && i.PaymentDate < dateTo && i.Ticket.CreditInitialPayment.HasValue && !i.Ticket.ReturnDate.HasValue)
                    .Where(i => !isClub || divisionId == i.Ticket.DivisionId)
                    .Where(i => !i.BarOrderId.HasValue)
                    .GroupBy(i => SqlFunctions.DatePart("month", i.PaymentDate))
                    .Select(i => new { i.Key, Bank = i.Sum(x => (decimal?)x.Amount) }).ToArray();
                for (int i = dateFrom.Month - 1; i < dateFrom.Month + 11; i++)
                {
                    res.Add(new SalesData
                    {
                        MonthName = mfi.GetMonthName(i % 12 + 1),
                        Nal = (data1_1.Where(j => j.Key == i % 12 + 1).Select(j => j.Cash).FirstOrDefault()
                            - (data1_2.Where(j => j.Key == i % 12 + 1).Select(j => j.Cash).FirstOrDefault() ?? 0)
                            - (data1_3.Where(j => j.Key == i % 12 + 1).Select(j => j.Cash).FirstOrDefault() ?? 0)) / 1000,
                        Beznal = ((data1_1.Where(j => j.Key == i % 12 + 1).Select(j => j.Bank).FirstOrDefault() ?? 0)
                                + (data2.Where(j => j.Key == i % 12 + 1).Select(j => j.Bank).FirstOrDefault() ?? 0)
                                - (data1_3.Where(j => j.Key == i % 12 + 1).Select(j => j.Bank).FirstOrDefault() ?? 0)) / 1000,
                    });
                }
                res.ForEach(i =>
                {
                    if(i.Beznal == 0) i.Beznal = null;
                    if(i.Nal == 0) i.Nal = null;
                });
                return res;
            }
        }

        public static List<TicketsData> GetUniedReportAmountTicket(bool isClub, Guid divisionId)
        {
            using (var context = new ExtraEntities())
            {
                var result = new List<TicketsData>();

                var cId = UserManagement.GetCompanyIdOrDefaultId(context);
                var dateFrom = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddYears(-1).AddMonths(1);
                var dateTo = dateFrom.AddYears(1);
                var mfi = CultureInfo.CurrentCulture.DateTimeFormat;

                var allTickets = context.Tickets
                    .Where(i => i.CompanyId == cId)
                    .Where(i => !isClub || divisionId == i.DivisionId)
                    .Where(i => !i.ReturnDate.HasValue && !i.TicketType.IsGuest && !i.TicketType.IsVisit)
                    .Select(i => new { i.Id, i.CustomerId, i.CreatedOn })
                    .ToList();

                var tickets = allTickets
                    .Where(i => i.CreatedOn >= dateFrom && i.CreatedOn < dateTo)
                    .ToList();

                var data_1 = tickets
                    .Where(i => !allTickets.Where(k => k.CreatedOn < i.CreatedOn).Select(k => k.CustomerId).Contains(i.CustomerId))
                    .GroupBy(i => i.CreatedOn.Month)
                    .Select(i => new { i.Key, Count = i.Count() })
                    .ToList();

                var data_2 = tickets
                    .Where(i => allTickets.Where(k => k.CreatedOn < i.CreatedOn).Select(k => k.CustomerId).Contains(i.CustomerId))
                    .GroupBy(i => i.CreatedOn.Month)
                    .Select(i => new { i.Key, Count = i.Count() })
                    .ToList();

                for (int i = dateFrom.Month-1; i < dateFrom.Month + 11; i++)
                {
                    result.Add(new TicketsData
                    {
                        MonthName = mfi.GetMonthName(i%12+1),
                        NewCustomers = data_1.Where(j => j.Key == i % 12 + 1).Select(j => j.Count).FirstOrDefault(),
                        OldCustomers = data_2.Where(j => j.Key == i % 12 + 1).Select(j => j.Count).FirstOrDefault()
                    });
                }

                return result;
            }
        }

        public static WorkbenchInfo GetWorkbench(Guid divisionId, DateTime workbenchDate)
        {
            using(var context = new ExtraEntities())
            {
                var res = new WorkbenchInfo();
                var month = new DateTime(workbenchDate.Year, workbenchDate.Month, 1);
                var plan = context.SalesPlans.FirstOrDefault(i => i.DivisionId == divisionId && i.Month == month);
                if(plan != null)
                {
                    res.SalesPlan = plan.Value + plan.CorpValue;
                }
                else
                {
                    res.SalesPlan = 0;
                }

                var data = context.BarOrders.Where(i => i.DivisionId == divisionId && i.PurchaseDate >= month)
                    .Sum(x => (decimal?)(x.CashPayment + x.CardPayment)) ?? 0;
                var data1 = context.TicketPayments.Where(i => divisionId == i.Ticket.DivisionId && i.PaymentDate >= month && !i.BarOrderId.HasValue && i.Ticket.CreditInitialPayment.HasValue)
                    .Sum(i => (decimal?)i.Amount) ?? 0;
                var returns = context.GoodSales.Where(i => i.ReturnDate >= month && i.BarOrder.DivisionId == divisionId).Sum(i => i.PriceMoney) ?? 0;
                var returns2 = context.Tickets.Where(i => i.ReturnDate >= month && i.DivisionId == divisionId).Sum(i => i.ReturnCost) ?? 0;
                res.TotalSales = data + data1 - returns - returns2;

                res.CustomerVisits = context.TreatmentEvents.Where(i => i.VisitStatus == 0 && EntityFunctions.TruncateTime(i.VisitDate) == workbenchDate && i.DivisionId == divisionId)
                    .GroupBy(i => i.Customer)
                    .Select(i => new
                    {
                        CustomerId = i.Key.Id,
                        FullName = (i.Key.LastName ?? "") + " " + (i.Key.FirstName ?? "") + " " + (i.Key.MiddleName ?? ""),
                        Card = i.Key.CustomerCards.OrderByDescending(j => j.EmitDate).Select(j => j.CardBarcode).FirstOrDefault(),
                        IsInClub = i.Key.CustomerVisits.Any(j => !j.OutTime.HasValue),
                        Phone = i.Key.Phone2,
                        VisitStart = i.Min(j => j.VisitDate),
                        VisitEnd = i.Max(j => j.VisitDate),
                        VisitLen = i.OrderByDescending(j => j.VisitDate)
                            .Select(j => new { j.TreatmentConfig.LengthCoeff, j.TreatmentConfig.TreatmentType.Duration })
                            .Select(j => j.Duration * j.LengthCoeff).FirstOrDefault(),
                        VisitTexts = i.OrderBy(j => j.VisitDate).Select(j => j.TreatmentConfig.Name)
                    })
                    .ToList()
                    .OrderBy(i => i.VisitStart)
                    .Select(i => new VisitInfo
                    {
                        CustomerId = i.CustomerId,
                        FullName = "[" + (i.Card ?? "без карты") + "] " + i.FullName,
                        IsInClub = i.IsInClub,
                        Phone = "+" + i.Phone,
                        VisitTime = String.Format("{0:HH:mm} - {1:HH:mm}", i.VisitStart, i.VisitEnd.AddMinutes(i.VisitLen)),
                        TreatmnetNames = String.Join(", ", i.VisitTexts)
                    }).ToList();

                var user = UserManagement.GetUser(context);
                var empId = user.EmployeeId;
                var fromWorkbenchDate = workbenchDate.AddDays(-14);
                if (empId != null)
                {
                    res.CustomerCalls = context.CustomerNotifications
                        .Where(i => (i.Employees.Any(j => j.Id == empId) || !i.Employees.Any())
                            && !i.CompletedOn.HasValue
                            && i.Customer.ClubId == divisionId
                            && EntityFunctions.TruncateTime(i.ExpiryDate) == workbenchDate)
                        .OrderBy(i => i.ExpiryDate).ToList()
                        .Concat(context.CustomerNotifications
                            .Where(i => (i.Employees.Any(j => j.Id == empId) || !i.Employees.Any())
                                && !i.CompletedOn.HasValue
                                && i.Customer.ClubId == divisionId
                                && EntityFunctions.TruncateTime(i.ExpiryDate) < workbenchDate
                                && EntityFunctions.TruncateTime(i.ExpiryDate) >= fromWorkbenchDate)
                            .OrderBy(i => i.ExpiryDate).ToList())
                        .Select(i => new CallInfo
                        {
                            CustomerId = i.CustomerId,
                            Deadline = i.ExpiryDate,
                            Description = i.Message,
                            FullName = (i.Customer.LastName ?? "") + " " + (i.Customer.FirstName ?? "") + " " + (i.Customer.MiddleName ?? ""),
                            Phone = i.Customer.Phone2,
                            Id = i.Id
                        })
                        .ToList();
                }

                res.CustomerTasks = OrganizerCore.GetOrganizerItems(workbenchDate, workbenchDate, false).Where(i => i.ExpiryDate <= workbenchDate.AddDays(7)).ToList();

                var str1 = "Выполнение замеров по клиенту ";
                var str2 = ". Месяц назад были произведены замеры, необходимо провести повторные.";

                var values = res.CustomerTasks.Where(i => i.Text.StartsWith(str1)).Select(i => i.Text.Replace(str1, "").Replace(str2, "")).ToArray();

                var match = context.Customers.Where(i => i.CompanyId == user.CompanyId && i.ClubId == divisionId && values.Contains(i.LastName + ((" " + i.FirstName) ?? "") + ((" " + i.MiddleName) ?? "")))
                    .Select(i => i.LastName + ((" " + i.FirstName) ?? "") + ((" " + i.MiddleName) ?? "")).ToArray();

                res.CustomerTasks = res.CustomerTasks.Where(i => !i.Text.StartsWith(str1) || match.Any(j => i.Text.Contains(j))).ToList();
                return res;
            }
        }
    }
}
