using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using TonusClub.Entities;
using TonusClub.ServiceModel;
using System.Data.Objects;

namespace TonusClub.ServerCore
{
    partial class CustomReports
    {
        public DataTable GetFinanceReport(DateTime start, DateTime end, Guid? divisionId, bool weekly, bool daily)
        {
            start = new DateTime(start.Year, start.Month, 1);
            end = new DateTime(end.Year, end.Month, 1).AddMonths(1).AddMilliseconds(-1);

            if (weekly && daily)
            {
                throw new Exception("Детализация одновременно по дням и по неделям невозможна!");
            }
            using (var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                var res = new DataTable();

                res.ExtendedProperties.Add("Detailed", true);
                res.Columns.Add("_color", typeof(int));
                res.Columns.Add(ClubText, typeof(string));
                res.Columns.Add("Категория", typeof(string));
                res.Columns.Add("Подкатегория", typeof(string));
                res.Columns.Add("Детали", typeof(string));

                AddDatesColumns(res, start, end, daily, weekly, typeof(decimal));

                foreach (var div in context.Divisions.Where(i => i.Id == divisionId || (divisionId == null && i.CompanyId == user.CompanyId)))
                {

                    //План
                    #region План
                    var plancells = new List<object>();
                    var corpcells = new List<object>();
                    var planperc = new List<object>();
                    var corpperc = new List<object>();

                    plancells.AddRange(new object[] { null, div.Name });
                    plancells.Add("План");
                    plancells.Add("План");
                    plancells.Add("План " + ClubTextV);
                    planperc.AddRange(new object[] { 0, div.Name });
                    planperc.Add("План");
                    planperc.Add("План");
                    planperc.Add("План выполнен, %");
                    corpcells.AddRange(new object[] { null, div.Name });
                    corpcells.Add("План");
                    corpcells.Add("План");
                    corpcells.Add("Корпоративные продажи");
                    corpperc.AddRange(new object[] { 0, div.Name });
                    corpperc.Add("План");
                    corpperc.Add("План");
                    corpperc.Add("Корп. план выполнен, %");
                    for (var i = start; i <= end; i = i.AddMonths(1))
                    {
                        var plan = context.SalesPlans.FirstOrDefault(j => (j.DivisionId == div.Id) && (j.Month == i));
                        if (plan != null)
                        {
                            plancells.Add(plan.Value);
                            corpcells.Add(plan.CorpValue);
                            planperc.Add(SalaryCalculation.Get01TotalSalesPercent(context, div.Id, i, DateTime.MinValue));
                            corpperc.Add(SalaryCalculation.Get01aTotalCorporateSalesPercent(context, div.Id, i, DateTime.MinValue));
                            if (daily)
                            {
                                for (int j = 0; j < DateTime.DaysInMonth(i.Year, i.Month); j++)
                                {
                                    planperc.Add(SalaryCalculation.Get01TotalSalesPercent(context, div.Id, i.AddDays(j), i.AddDays(j + 1)));
                                    corpperc.Add(SalaryCalculation.Get01aTotalCorporateSalesPercent(context, div.Id, i.AddDays(j), i.AddDays(j + 1)));
                                }
                            }
                            if (weekly)
                            {
                                foreach (var j in GetWeeksInMonth(i))
                                {
                                    planperc.Add(SalaryCalculation.Get01TotalSalesPercent(context, div.Id, j[0], j[1].AddDays(1)));
                                    corpperc.Add(SalaryCalculation.Get01aTotalCorporateSalesPercent(context, div.Id, j[0], j[1].AddDays(1)));
                                }
                            }

                        }
                        else
                        {
                            plancells.Add(0);
                            corpcells.Add(0);
                            planperc.Add(0);
                            corpperc.Add(0);
                            if (daily)
                            {
                                for (int j = 0; j < DateTime.DaysInMonth(i.Year, i.Month); j++)
                                {
                                    planperc.Add(0);
                                    corpperc.Add(0);
                                }
                            }
                            if (weekly)
                            {
                                foreach (var j in GetWeeksInMonth(i))
                                {
                                    planperc.Add(0);
                                    corpperc.Add(0);
                                }
                            }


                        }

                    }
                    res.Rows.Add(plancells.ToArray());
                    res.Rows.Add(corpcells.ToArray());
                    res.Rows.Add(planperc.ToArray());
                    res.Rows.Add(corpperc.ToArray());
                    #endregion
                    //Доходы
                    #region карты обычные
                    var inc = new List<object>();
                    inc.AddRange(new object[] { null, div.Name, "Доходы", "Выручка от продажи карт" });
                    var src = context.CustomerCards.Where(j => j.Customer.Tickets.Any() && j.Customer.Tickets.FirstOrDefault().DivisionId == div.Id
                                        && j.EmitDate >= start && j.EmitDate < end).Select(i => new { i.Customer.CorporateId, i.CustomerCardTypeId, i.EmitDate, i.Price }).ToArray();
                    foreach (var ct in context.CustomerCardTypes.Select(i => new { Id = i.Id, Name = i.Name }).ToArray())
                    {
                        bool flag;
                        var tinc = CalcValue(inc, ct.Name, src.Where(j => !j.CorporateId.HasValue && j.CustomerCardTypeId == ct.Id),
                                        start, end,
                                        j => j.EmitDate,
                                        daily,
                                        weekly,
                                        out flag,
                                        false,
                                        (i, _, __) => i.Sum(j => j.Price));
                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }
                    #endregion
                    #region карты корпоративные
                    inc = new List<object>();
                    inc.AddRange(new object[] { null, div.Name, "Доходы корпоративные", "Выручка от продажи карт" });
                    foreach (var ct in context.CustomerCardTypes)
                    {

                        bool flag;
                        var tinc = CalcValue(inc, ct.Name, src.Where(j => j.CorporateId.HasValue && j.CustomerCardTypeId == ct.Id), start, end,
                                        j => j.EmitDate,
                                        daily, weekly,
                                        out flag, false,
                                        (i, _, __) => i.Max(j => j.Price));
                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }
                    #endregion
                    #region абонементы

                    var src2 = context.TicketPayments.Where(j => j.Ticket.DivisionId == div.Id
                                        && j.PaymentDate >= start && j.PaymentDate < end
                                        && (context.BarOrders.Where(k => k.Id == j.BarOrderId).FirstOrDefault() == null || context.BarOrders.Where(k => k.Id == j.BarOrderId).Select(k => k.DepositPayment).FirstOrDefault() == 0))
                        //&& !j.Ticket.InheritedTicketId.HasValue)
                                    .Where(j => !j.BarOrderId.HasValue || !context.BarOrders.Any(k => k.Id == j.BarOrderId && k.CertificateId.HasValue))
                                    .Select(i => new { i.Ticket.Customer.CorporateId, i.Ticket.TicketTypeId, i.PaymentDate, i.Amount }).ToArray();

                    inc = new List<object>();
                    inc.AddRange(new object[] { null, div.Name, "Доходы", "Выручка от продажи абонементов" });
                    foreach (var tt in context.TicketTypes.Where(i => !i.IsAction && (i.SolariumMinutes == 0)))
                    {
                        bool flag;
                        var tinc = CalcValue(inc, tt.Name, src2.Where(j => !j.CorporateId.HasValue
                                        && j.TicketTypeId == tt.Id), start, end,
                                        j => j.PaymentDate,
                                        daily, weekly,
                                        out flag, false,
                                        (i, _, __) => i.Sum(j => j.Amount));
                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }
                    #endregion
                    #region абонементы корпоративные
                    inc = new List<object>();
                    inc.AddRange(new object[] { null, div.Name, "Доходы корпоративные", "Выручка от продажи абонементов" });
                    foreach (var tt in context.TicketTypes.Where(i => !i.IsAction && (i.SolariumMinutes == 0)))
                    {
                        bool flag;
                        var tinc = CalcValue(inc, tt.Name, src2.Where(j => j.CorporateId.HasValue
                                        && j.TicketTypeId == tt.Id), start, end,
                                        j => j.PaymentDate, daily, weekly,
                                        out flag, false,
                                        (i, _, __) => i.Sum(j => j.Amount));
                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }
                    #endregion
                    #region товары

                    var src3 = context.GoodSales.Where(x => x.BarOrder.DivisionId == div.Id
                        && x.BarOrder.DepositPayment == 0
                        && (x.BarOrder.PaymentDate ?? x.BarOrder.PurchaseDate) >= start
                        && (x.BarOrder.PaymentDate ?? x.BarOrder.PurchaseDate) < end
                        && (!x.BarOrder.Kind1C.HasValue || x.BarOrder.Kind1C != 10))
                        .Select(i => new { i.GoodId, i.BarOrder.PaymentDate, i.BarOrder.PurchaseDate, i.Amount, i.PriceMoney }).ToArray();

                    inc = new List<object>();
                    inc.AddRange(new object[] { null, div.Name, "Доходы", "Выручка от продажи товаров" });
                    foreach (var g in context.Goods)
                    {
                        bool flag;
                        var tinc = CalcValue(inc, g.Name, src3.Where(x => x.GoodId == g.Id),
                                    start, end,
                                    j => j.PaymentDate ?? j.PurchaseDate,
                                    daily, weekly,
                                    out flag, false,
                                    (i, _, __) => i.Sum(j => (decimal)j.Amount * (j.PriceMoney ?? 0)));
                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }
                    #endregion

                    var srcCertUsage = context.Certificates.Where(i => i.DivisionId == div.Id && i.UsedOrderId.HasValue
                        && i.UsedInOrder.PurchaseDate >= start && i.UsedInOrder.PurchaseDate < end).Select(i => new { i.UsedInOrder.PurchaseDate, Amount = -i.Amount }).ToList();

                    {
                        bool flag;
                        var tinc = CalcValue(inc, "Корректировка оплат сертификатами", srcCertUsage,
                                    start, end,
                                    j => j.PurchaseDate,
                                    daily, weekly,
                                    out flag, false,
                                    (i, _, __) => i.Sum(j => (decimal)j.Amount));
                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }

                    #region Пакеты товаров

                    inc = new List<object>();
                    inc.AddRange(new object[] { null, div.Name, "Доходы", "Выручка от продажи пакетов товаров" });
                    var goodPackages = context.BarOrders.Where(i => i.DivisionId == div.Id && i.Kind1C == 10).ToList()
                        .Select(i => new { Date = i.PurchaseDate, Lines = i.GetContent() }).ToList()
                        .SelectMany(i => i.Lines.Select(j => new { Date = i.Date.Date, Line = j }))
                        .GroupBy(i => i.Line.Name)
                        .ToArray();
                    foreach (var gp in goodPackages)
                    {
                        bool flag;

                        var tinc = CalcValue(inc, gp.Key, gp,
                                    start, end,
                                    j => j.Date,
                                    daily, weekly,
                                    out flag, false,
                                    (i, _, __) => i.Sum(j => j.Line.Cost));

                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }
                    #endregion

                    #region Сертификаты
                    inc = new List<object>();
                    inc.AddRange(new object[] { null, div.Name, "Доходы", "Выручка от продажи сертификатов" });

                    var certificates = context.Certificates.Where(i => i.SellDate.HasValue && i.DivisionId == div.Id).ToList();
                    foreach (var cert in certificates)
                    {
                        bool flag;

                        var tinc = CalcValue(inc, cert.Name ?? "Без названия", Enumerable.Repeat(cert, 1),
                                    start, end,
                                    j => j.SellDate.Value,
                                    daily, weekly,
                                    out flag, false,
                                    (i, _, __) => i.Sum(j =>
                                    {
                                        var item = j.SellBarOrder.GetContent().FirstOrDefault(k => k.Price == cert.PriceMoney);
                                        if (item == null)
                                        {
                                            return 0;
                                        }
                                        return item.Cost;
                                        //j.SellBarOrder.CardPayment + j.SellBarOrder.CashPayment
                                    }));

                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }
                    #endregion




                    #region Доп.услуги
                    inc = new List<object>();
                    inc.AddRange(new object[] { null, div.Name, "Доходы", "Выручка от продажи доп. услуг" });
                    //Детская комната
                    {
                        var src10 = context.ChildrenRooms.Where(j => j.DivisionId == div.Id).Where(i => i.CreatedOn >= start && i.CreatedOn < end).Select(i => new { i.CreatedOn, i.Cost }).ToArray();
                        bool flag;
                        var tinc = CalcValue(inc, "Детская комната", src10, start, end,
                                        j => j.CreatedOn,
                                        daily, weekly,
                                        out flag, false,
                                        (i, _, __) => i.Sum(j => j.Cost));
                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }
                    //Прокат
                    var src11 = context.Rents.Where(i => i.Storehouse.DivisionId == div.Id && i.CreatedOn >= start && i.CreatedOn < end)
                        .ToArray();
                    foreach (var g in context.Goods)
                    {
                        bool flag;
                        var tinc = CalcValue(inc, "Прокат - " + g.Name, src11, start, end,
                            j => j.GoodId == g.Id,
                                        j => j.CreatedOn,
                                        j => j.IsPayed ? j.Cost + (j.LostFine ?? 0m) + (j.OverdueFine ?? 0m) : 0, daily, weekly,
                                        out flag, false);
                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }
                    #endregion
                    #region абонементы акционные
                    inc = new List<object>();
                    inc.AddRange(new object[] { null, div.Name, "Доходы", "Выручка от продажи абонементов (не солярий, акционные)" });
                    foreach (var tt in context.TicketTypes.Where(i => i.IsAction && (i.SolariumMinutes == 0)))
                    {
                        bool flag;
                        var tinc = CalcValue(inc, tt.Name, src2, start, end,
                            j => !j.CorporateId.HasValue
                                    && j.TicketTypeId == tt.Id,
                                        j => j.PaymentDate,
                                        j => j.Amount, daily, weekly,
                                        out flag, false);
                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }
                    #endregion
                    #region абонементы акционные корпоративные
                    inc = new List<object>();
                    inc.AddRange(new object[] { null, div.Name, "Доходы  корпоративные", "Выручка от продажи абонементов (не солярий, акционные)" });
                    foreach (var tt in context.TicketTypes.Where(i => i.IsAction && (i.SolariumMinutes == 0)))
                    {
                        bool flag;
                        var tinc = CalcValue(inc, tt.Name, src2, start, end,
                            j => j.CorporateId.HasValue
                                    && j.TicketTypeId == tt.Id,
                                        j => j.PaymentDate,
                                        j => j.Amount, daily, weekly,
                                        out flag, false);
                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }
                    #endregion
                    #region абонементы солярий
                    inc = new List<object>();
                    inc.AddRange(new object[] { null, div.Name, "Доходы", "Выручка от продажи абонементов (солярий)" });
                    foreach (var tt in context.TicketTypes.Where(i => i.Units == 0 && i.GuestUnits == 0 && i.SolariumMinutes > 0))
                    {
                        bool flag;
                        var tinc = CalcValue(inc, tt.Name, src2, start, end,
                            j => !j.CorporateId.HasValue
                                    && j.TicketTypeId == tt.Id,
                                        j => j.PaymentDate,
                                        j => j.Amount, daily, weekly,
                                        out flag, false);
                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }
                    #endregion
                    #region абонементы солярий корпоративные
                    inc = new List<object>();
                    inc.AddRange(new object[] { null, div.Name, "Доходы корпоративные", "Выручка от продажи абонементов (солярий)" });
                    foreach (var tt in context.TicketTypes.Where(i => i.Units == 0 && i.GuestUnits == 0 && i.SolariumMinutes > 0))
                    {
                        bool flag;
                        var tinc = CalcValue(inc, tt.Name, src2, start, end,
                            j => j.CorporateId.HasValue
                                    && j.TicketTypeId == tt.Id,
                                        j => j.PaymentDate,
                                        j => j.Amount, daily, weekly,
                                        out flag, false);
                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }
                    #endregion
                    #region солярий
                    inc = new List<object>();
                    inc.AddRange(new object[] { null, div.Name, "Доходы", "Выручка от продажи минут солярия" });
                    {
                        var src4 = context.SolariumVisits.Where(i => i.Cost.HasValue && i.DivisionId == div.Id && i.VisitDate >= start && i.VisitDate < end).Select(i => new { i.VisitDate, i.Cost }).ToArray();
                        bool flag;
                        var tinc = CalcValue(inc, "Выручка от продажи минут солярия", src4, start, end,
                            j => true,
                                        j => j.VisitDate,
                                        j => j.Cost, daily, weekly,
                                        out flag, false);
                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }
                    #endregion
                    #region пополнение депозита
                    var src8 = context.BarOrders.Where(i => i.Kind1C == 9 && i.DivisionId == div.Id && i.PurchaseDate >= start && i.PurchaseDate <= end)
                        .Select(i => new { i.Customer.CorporateId, Customer = i.Customer.LastName + " " + i.Customer.FirstName + " " + i.Customer.MiddleName, CreatedOn = i.PurchaseDate, Amount = i.CashPayment + i.CardPayment }).ToArray();

                    inc = new List<object>();
                    inc.AddRange(new object[] { null, div.Name, "Доходы", "Пополнение депозита клиентами" });
                    {
                        bool flag;
                        var tinc = CalcValue(inc, "Пополнение депозита клиентами", src8, start, end,
                            j => !j.CorporateId.HasValue,
                                        j => j.CreatedOn,
                                        j => j.Amount, daily, weekly,
                                        out flag, false);
                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }
                    #endregion
                    #region пополнение депозита корпоративными
                    inc = new List<object>();
                    inc.AddRange(new object[] { null, div.Name, "Доходы корпоративные", "Пополнение депозита клиентами" });
                    {
                        bool flag;
                        var tinc = CalcValue(inc, "Пополнение депозита клиентами", src8, start, end,
                            j => j.CorporateId.HasValue,
                                        j => j.CreatedOn,
                                        j => j.Amount, daily, weekly,
                                        out flag, false);
                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }
                    #endregion
                    #region доплата за обмен абонемента
                    inc = new List<object>();
                    inc.AddRange(new object[] { null, div.Name, "Доходы", "Доплата за абонемент при обмене" });

                    var src5 = context.TicketPayments.Where(j => j.Ticket.DivisionId == div.Id
                                        && !j.Ticket.InheritedTicketId.HasValue //чтобы никогда не выполнилось
                                        && j.Ticket.InheritedFrom.CustomerId == j.Ticket.CustomerId)
                                        .Select(i => new { i.Ticket.TicketTypeId, i.PaymentDate, i.Amount }).ToArray();

                    foreach (var tt in context.TicketTypes)
                    {
                        bool flag;
                        var tinc = CalcValue(inc, tt.Name, src5.Where(j => j.TicketTypeId == tt.Id), start, end,
                                        j => j.PaymentDate,
                                        daily, weekly,
                                        out flag, false,
                                        (i, _, __) => i.Sum(j => j.Amount));
                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }
                    #endregion
                    #region доходы прочие
                    var src7 = context.Incomes.Where(j => j.DivisionId == div.Id && j.CreatedOn >= start && j.CreatedOn < end).Select(i => new { i.IncomeTypeId, i.CreatedOn, i.Amount }).ToArray();
                    inc = new List<object>();
                    inc.AddRange(new object[] { null, div.Name, "Доходы", "Прочее" });
                    foreach (var it in context.IncomeTypes.Where(i => i.DivisionId == div.Id))
                    {
                        bool flag;
                        var tinc = CalcValue(inc, it.Name, src7, start, end,
                            j => j.IncomeTypeId == it.Id,
                                        j => j.CreatedOn,
                                        j => j.Amount, daily, weekly,
                                        out flag, false);
                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }
                    #endregion


                    #region возвраты
                    //var goodsRet = context.GoodSales.Where(i => i.BarOrder.DivisionId == div.Id)
                    //    .Where(i => i.ReturnDate.HasValue && EntityFunctions.TruncateTime(i.ReturnDate) == EntityFunctions.TruncateTime(i.BarOrder.PurchaseDate))
                    //    .Select(i => new { Date = i.BarOrder.PurchaseDate, Price = (i.PriceMoney ?? 0), Amount = i.Amount })
                    //    .ToList();
                    {
                        bool flag;
                        var srcX = context.Spendings.Where(i => i.DivisionId == div.Id
                                && !i.IsInvestment
                                && i.CreatedOn >= start && i.CreatedOn < end
                                && (i.SpendingType.Name == Localization.Resources.GoodRefund || i.SpendingType.Name == "Возврат абонемента"))
                            .Select(i => new { TypeId = i.SpendingTypeId, PaymentType = i.PaymentType, Amount = i.Amount, CreatedOn = i.CreatedOn })
                            .ToList();
                        var tinc = CalcValue(new List<object> { null, div.Name, "Доходы", "Прочее" },
                                "Возвраты",
                                srcX,
                                start, end,
                                j => j.CreatedOn,
                                daily, weekly,
                                out flag,
                                false,
                                (i, s, e) => -(i.Sum(j => j.Amount)/* - goodsRet.Where(j => j.Date >= s && j.Date < e).Sum(j => (decimal)j.Amount * j.Price)*/));
                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }

                    }
                    #endregion


                    var src6 = context.Spendings.Where(j => j.DivisionId == div.Id && j.CreatedOn >= start && j.CreatedOn < end)
                        .Select(i => new { i.SpendingTypeId, i.CreatedOn, i.Amount, i.PaymentType }).ToArray();

                    //Расходы
                    #region расходы нал
                    inc = new List<object>();
                    inc.AddRange(new object[] { null, div.Name, "Расходы", "Расходы (нал)" });
                    foreach (var st in context.SpendingTypes.Where(i => (i.DivisionId == div.Id || !i.DivisionId.HasValue)
                        && i.Name != Localization.Resources.GoodRefund && i.Name != "Возврат абонемента"))
                    {
                        bool flag;
                        var tinc = CalcValue(inc, st.Name, src6, start, end,
                            j => j.SpendingTypeId == st.Id
                                && !j.PaymentType.ToLower().Contains("безнал"),
                                        j => j.CreatedOn,
                                        j => j.Amount, daily, weekly,
                                        out flag, false);
                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }


                    inc = new List<object>();
                    inc.AddRange(new object[] { null, div.Name, "Расходы", "Вывод средств с депозита" });

                    var srcDO = context.DepositAccounts.Where(i => (i.Customer.ClubId ?? i.Customer.CustomerCards.FirstOrDefault().DivisionId) == div.Id && i.CreatedOn >= start && i.CreatedOn <= end && i.Description.StartsWith("Вывод средств по заявлению"))
                        .Select(i => new { Customer = i.Customer.LastName + " " + i.Customer.FirstName + " " + i.Customer.MiddleName, i.CreatedOn, Amount = -i.Amount }).ToArray();

                    foreach (var st in srcDO.GroupBy(i => i.Customer))
                    {
                        bool flag;
                        var tinc = CalcValue(inc, "Вывод средств с депозита " + st.Key, srcDO, start, end,
                            j => j.Customer == st.Key,
                                        j => j.CreatedOn,
                                        j => j.Amount, daily, weekly,
                                        out flag, false);
                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }




                    #endregion
                    #region расходы безнал
                    inc = new List<object>();
                    inc.AddRange(new object[] { null, div.Name, "Расходы", "Расходы (безнал)" });

                    foreach (var st in context.SpendingTypes.Where(i => (i.DivisionId == div.Id || !i.DivisionId.HasValue)
                        && i.Name != Localization.Resources.GoodRefund && i.Name != "Возврат абонемента"))
                    {
                        bool flag;
                        var tinc = CalcValue(inc, st.Name, src6, start, end,
                            j => j.SpendingTypeId == st.Id && j.PaymentType.ToLower().Contains("безнал"),
                                        j => j.CreatedOn,
                                        j => j.Amount, daily, weekly,
                                        out flag, false);
                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }
                    #endregion
                    //Итоги
                    #region итоги
                    {
                        var income = new List<object> { null, div.Name, "Итоги", "Итоги", "Выручка" };
                        var clearincome = new List<object> { null, div.Name, "Итоги", "Итоги", "Чистая прибыль" };
                        var accumulated = new List<object> { null, div.Name, "Итоги", "Итоги", "Накопленный итог" };
                        var kkmleft = new List<object> { null, div.Name, "Итоги", "Итоги", "Остаток в кассе на 1 число" };
                        var unsent = new List<object> { null, div.Name, "Итоги", "Итоги", "Несданная выручка на 1 число" };
                        var advances = new List<object> { null, div.Name, "Итоги", "Итоги", "Невыданные авансы на 1 число" };
                        var terminal = new List<object> { null, div.Name, "Итоги", "Итоги", "Не поступившие оплаты по терминалу на 1 число" };

                        var n = 5;
                        var start1 = start.AddMonths(-1);
                        var prevdfin = context.DivisionFinances.FirstOrDefault(f => f.DivisionId == div.Id && f.Period == start1);
                        var prevaccum = 0m;
                        var unclearincome = 0m;
                        if (prevdfin != null)
                        {
                            prevaccum = prevdfin.Accum;
                        }

                        var totalIncome = 0m;
                        var totalClearIncome = 0m;

                        for (var i = start; i <= end; i = i.AddMonths(1))
                        {
                            unclearincome = 0;
                            var subres = 0m;
                            for (var j = 0; j < res.Rows.Count; j++)
                            {
                                if ((string)res.Rows[j][1] != div.Name) continue;
                                if (res.Rows[j][2].ToString() == "Доходы" || res.Rows[j][2].ToString() == "Доходы корпоративные")
                                {
                                    unclearincome += (decimal)res.Rows[j][n];
                                    subres += (decimal)res.Rows[j][n];
                                }
                                else if (res.Rows[j][2].ToString() == "Расходы") subres -= (decimal)res.Rows[j][n];
                            }

                            prevaccum += subres;

                            var dfin = context.DivisionFinances.FirstOrDefault(f => f.DivisionId == div.Id && f.Period == i) ?? new DivisionFinance();

                            kkmleft.Add(dfin.CashLeft);
                            unsent.Add(dfin.Unsent);
                            advances.Add(dfin.Advances);
                            terminal.Add(dfin.TerminalLoan);

                            accumulated.Add(prevaccum);
                            income.Add(unclearincome);
                            clearincome.Add(subres);
                            n++;

                            totalIncome += unclearincome;
                            totalClearIncome += subres;

                            if (daily)
                            {
                                for (int k = 0; k < DateTime.DaysInMonth(i.Year, i.Month); k++)
                                {
                                    unclearincome = 0;
                                    subres = 0m;
                                    for (var j = 0; j < res.Rows.Count; j++)
                                    {
                                        if ((string)res.Rows[j][1] != div.Name) continue;
                                        if (res.Rows[j][2].ToString() == "Доходы" || res.Rows[j][2].ToString() == "Доходы корпоративные")
                                        {
                                            unclearincome += (decimal)res.Rows[j][n];
                                            subres += (decimal)res.Rows[j][n];
                                        }
                                        else if (res.Rows[j][2].ToString() == "Расходы") subres -= (decimal)res.Rows[j][n];
                                    }
                                    kkmleft.Add(null);
                                    unsent.Add(null);
                                    advances.Add(null);
                                    terminal.Add(null);

                                    accumulated.Add(null);
                                    income.Add(unclearincome);
                                    clearincome.Add(subres);

                                    n++;
                                }
                            }
                            if (weekly)
                            {
                                foreach (var k in GetWeeksInMonth(i))
                                {
                                    unclearincome = 0;
                                    subres = 0m;
                                    for (var j = 0; j < res.Rows.Count; j++)
                                    {
                                        if ((string)res.Rows[j][1] != div.Name) continue;
                                        if (res.Rows[j][2].ToString() == "Доходы" || res.Rows[j][2].ToString() == "Доходы корпоративные")
                                        {
                                            unclearincome += (decimal)res.Rows[j][n];
                                            subres += (decimal)res.Rows[j][n];
                                        }
                                        else if (res.Rows[j][2].ToString() == "Расходы") subres -= (decimal)res.Rows[j][n];
                                    }
                                    kkmleft.Add(null);
                                    unsent.Add(null);
                                    advances.Add(null);
                                    terminal.Add(null);

                                    accumulated.Add(null);
                                    income.Add(unclearincome);
                                    clearincome.Add(subres);

                                    n++;
                                }
                            }
                        }

                        income.Add(totalIncome);
                        res.Rows.Add(income.ToArray());
                        clearincome.Add(totalClearIncome);
                        res.Rows.Add(clearincome.ToArray());
                        res.Rows.Add(accumulated.ToArray());
                        res.Rows.Add(kkmleft.ToArray());
                        res.Rows.Add(unsent.ToArray());
                        res.Rows.Add(advances.ToArray());
                        res.Rows.Add(terminal.ToArray());

                        var outs = new List<object> { null, div.Name, "Итоги", "Итоги" };
                        bool flag2;
                        outs = CalcValue(outs, "Расходы", context.Spendings, start, end,
                                j => j.DivisionId == div.Id,
                                            j => j.CreatedOn,
                                            j => j.Amount, daily, weekly,
                                            out flag2, false);
                        res.Rows.Add(outs.ToArray());
                    }
                    #endregion

                }
                #region Итого по франчайзи
                if (divisionId == null)
                {
                    var income = new List<object> { null, "Итого по франчайзи", "Итоги", "Итоги", "Выручка" };
                    var clearincome = new List<object> { null, "Итого по франчайзи", "Итоги", "Итоги", "Чистая прибыль" };
                    var accumulated = new List<object> { null, "Итого по франчайзи", "Итоги", "Итоги", "Накопленный итог" };
                    var cashleft = new List<object> { null, "Итого по франчайзи", "Итоги", "Итоги", "Остаток на р/счете на 1 число" };
                    var kkmleft = new List<object> { null, "Итого по франчайзи", "Итоги", "Итоги", "Остаток в кассе на 1 число" };
                    var unsent = new List<object> { null, "Итого по франчайзи", "Итоги", "Итоги", "Несданная выручка на 1 число" };
                    var advances = new List<object> { null, "Итого по франчайзи", "Итоги", "Итоги", "Невыданные авансы на 1 число" };
                    var terminal = new List<object> { null, "Итого по франчайзи", "Итоги", "Итоги", "Не поступившие оплаты по терминалу на 1 число" };
                    var realaccumulated = new List<object> { null, "Итого по франчайзи", "Итоги", "Итоги", "Реальный накопленный итог" };
                    var delta = new List<object> { null, "Итого по франчайзи", "Итоги", "Итоги", "Расхождение" };

                    var n = 5;
                    var start1 = start.AddMonths(-1);
                    var prevdfins = context.DivisionFinances.Where(f => f.CompanyId == user.CompanyId && f.Period == start1);
                    var prevaccum = prevdfins.SafeSum(x => x.Accum);
                    for (var i = start; i <= end; i = i.AddMonths(1))
                    {
                        var subres = 0m;
                        var unclearincome = 0m;
                        for (var j = 0; j < res.Rows.Count; j++)
                        {
                            if (res.Rows[j][2].ToString() == "Доходы" || res.Rows[j][2].ToString() == "Доходы корпоративные")
                            {
                                unclearincome += (decimal)res.Rows[j][n];
                                subres += (decimal)res.Rows[j][n];
                            }
                            else if (res.Rows[j][2].ToString() == "Расходы") subres -= (decimal)res.Rows[j][n];
                        }

                        prevaccum += subres;


                        var cfin = context.CompanyFinances.FirstOrDefault(c => c.CompanyId == user.CompanyId && c.Period == i) ?? new CompanyFinance();
                        var dfins = context.DivisionFinances.Where(f => f.CompanyId == user.CompanyId && f.Period == i);

                        cashleft.Add(cfin.AccountLeft);
                        kkmleft.Add(dfins.SafeSum(x => x.CashLeft));
                        unsent.Add(dfins.SafeSum(x => x.Unsent));
                        advances.Add(dfins.SafeSum(x => x.Advances));
                        terminal.Add(dfins.SafeSum(x => x.TerminalLoan));
                        realaccumulated.Add(cfin.AccountLeft + dfins.SafeSum(x => x.CashLeft) + dfins.SafeSum(x => x.Unsent) + dfins.SafeSum(x => x.Advances) + dfins.SafeSum(x => x.TerminalLoan));

                        delta.Add(cfin.AccountLeft + dfins.SafeSum(x => x.CashLeft) + dfins.SafeSum(x => x.Unsent) + dfins.SafeSum(x => x.Advances) + dfins.SafeSum(x => x.TerminalLoan) - prevaccum);

                        accumulated.Add(prevaccum);
                        income.Add(unclearincome);
                        clearincome.Add(subres);
                        n++;
                        if (daily)
                        {
                            for (int k = 0; k < DateTime.DaysInMonth(i.Year, i.Month); k++)
                            {
                                subres = 0m;
                                for (var j = 0; j < res.Rows.Count; j++)
                                {
                                    if (res.Rows[j][2].ToString() == "Доходы" || res.Rows[j][2].ToString() == "Доходы корпоративные")
                                    {
                                        unclearincome += (decimal)res.Rows[j][n];
                                        subres += (decimal)res.Rows[j][n];
                                    }
                                    else if (res.Rows[j][2].ToString() == "Расходы") subres -= (decimal)res.Rows[j][n];
                                }
                                cashleft.Add(null);
                                kkmleft.Add(null);
                                unsent.Add(null);
                                advances.Add(null);
                                terminal.Add(null);
                                realaccumulated.Add(null);

                                delta.Add(null);

                                accumulated.Add(null);
                                income.Add(unclearincome);
                                clearincome.Add(subres);

                                n++;
                            }
                        }
                        if (weekly)
                        {
                            foreach (var k in GetWeeksInMonth(i))
                            {
                                subres = 0m;
                                for (var j = 0; j < res.Rows.Count; j++)
                                {
                                    if (res.Rows[j][2].ToString() == "Доходы" || res.Rows[j][2].ToString() == "Доходы корпоративные")
                                    {
                                        unclearincome += (decimal)res.Rows[j][n];
                                        subres += (decimal)res.Rows[j][n];
                                    }
                                    else if (res.Rows[j][2].ToString() == "Расходы") subres -= (decimal)res.Rows[j][n];
                                }
                                cashleft.Add(null);
                                kkmleft.Add(null);
                                unsent.Add(null);
                                advances.Add(null);
                                terminal.Add(null);
                                realaccumulated.Add(null);

                                delta.Add(null);

                                accumulated.Add(null);
                                income.Add(unclearincome);
                                clearincome.Add(subres);

                                n++;
                            }
                        }
                    }
                    res.Rows.Add(income.ToArray());
                    res.Rows.Add(clearincome.ToArray());
                    res.Rows.Add(accumulated.ToArray());
                    res.Rows.Add(cashleft.ToArray());
                    res.Rows.Add(kkmleft.ToArray());
                    res.Rows.Add(unsent.ToArray());
                    res.Rows.Add(advances.ToArray());
                    res.Rows.Add(terminal.ToArray());
                    res.Rows.Add(realaccumulated.ToArray());
                    res.Rows.Add(delta.ToArray());

                    #region План
                    var plancells = new List<object> { null };
                    var corpcells = new List<object> { null };
                    var planperc = new List<object> { 0 };
                    var corpperc = new List<object> { 0 };

                    var corpsales = new List<object> { null, "Итого по франчайзи", "Итоги", "Итоги", "Выручка копроративные" };

                    plancells.Add("Итого по франчайзи");
                    plancells.Add("План");
                    plancells.Add("План");
                    plancells.Add("План по всем " + ClubTextUs);
                    planperc.Add("Итого по франчайзи");
                    planperc.Add("План");
                    planperc.Add("План");
                    planperc.Add("План выполнен, %");
                    corpcells.Add("Итого по франчайзи");
                    corpcells.Add("План");
                    corpcells.Add("План");
                    corpcells.Add("Корпоративные продажи");
                    corpperc.Add("Итого по франчайзи");
                    corpperc.Add("План");
                    corpperc.Add("План");
                    corpperc.Add("Корп. план выполнен, %");
                    for (var i = start; i <= end; i = i.AddMonths(1))
                    {
                        var plans = context.SalesPlans.Where(j => (j.CompanyId == user.CompanyId) && (j.Month == i));
                        if (plans != null && plans.Count() > 0)
                        {
                            plancells.Add(plans.Sum(j => j.Value));
                            var cv = plans.Sum(j => j.CorpValue);
                            corpcells.Add(cv);
                            planperc.Add(SalaryCalculation.Get01TotalSalesPercent(context, null, i, DateTime.MinValue, user.CompanyId));
                            var cp = SalaryCalculation.Get01aTotalCorporateSalesPercent(context, null, i, DateTime.MinValue, user.CompanyId);
                            corpperc.Add(cp);
                            corpsales.Add(cp * cv / 100);
                            if (daily)
                            {
                                for (int j = 0; j < DateTime.DaysInMonth(i.Year, i.Month); j++)
                                {
                                    planperc.Add(SalaryCalculation.Get01TotalSalesPercent(context, null, i.AddDays(j), i.AddDays(j + 1), user.CompanyId));
                                    var cpd = SalaryCalculation.Get01aTotalCorporateSalesPercent(context, null, i.AddDays(j), i.AddDays(j + 1), user.CompanyId);
                                    corpperc.Add(cpd);
                                    corpsales.Add(cv * cpd / 100);
                                }
                            }
                            if (weekly)
                            {
                                foreach (var j in GetWeeksInMonth(i))
                                {
                                    planperc.Add(SalaryCalculation.Get01TotalSalesPercent(context, null, j[0], j[1].AddDays(1), user.CompanyId));
                                    var cpw = SalaryCalculation.Get01aTotalCorporateSalesPercent(context, null, j[0], j[1].AddDays(1), user.CompanyId);
                                    corpperc.Add(cpw);
                                    corpsales.Add(cv * cpw / 100);
                                }
                            }

                        }
                        else
                        {
                            plancells.Add(0);
                            corpcells.Add(0);
                            planperc.Add(0);
                            corpperc.Add(0);
                            corpsales.Add(0);
                            if (daily)
                            {
                                for (int j = 0; j < DateTime.DaysInMonth(i.Year, i.Month); j++)
                                {
                                    planperc.Add(0);
                                    corpperc.Add(0);
                                    corpsales.Add(0);
                                }
                            }
                            if (weekly)
                            {
                                foreach (var j in GetWeeksInMonth(i))
                                {
                                    planperc.Add(0);
                                    corpperc.Add(0);
                                    corpsales.Add(0);
                                }
                            }


                        }

                    }
                    res.Rows.Add(plancells.ToArray());
                    res.Rows.Add(corpcells.ToArray());
                    res.Rows.Add(planperc.ToArray());
                    res.Rows.Add(corpperc.ToArray());
                    res.Rows.Add(corpsales.ToArray());
                    #endregion


                }
                #region расходы
                var totout = new List<object> { null };
                totout.AddRange(new object[] { "Итого по франчайзи", "Итоги", "Итоги" });
                bool flag1;
                totout = CalcValue(totout, "Расходы", context.Spendings, start, end,
                        j => j.CompanyId == user.CompanyId,
                                    j => j.CreatedOn,
                                    j => j.Amount, daily, weekly,
                                    out flag1, false);
                res.Rows.Add(totout.ToArray());
                #endregion
                #endregion
                var res1 = res.Clone();
                res1.Rows.Clear();

                foreach (var r in res.Select("Категория='План'"))
                {
                    AddRow(res1, r);
                }
                foreach (var r in res.Select("Категория='Итоги'"))
                {
                    AddRow(res1, r);
                }
                foreach (var r in res.Select("Категория='Доходы'"))
                {
                    AddRow(res1, r);
                }
                foreach (var r in res.Select("Категория='Расходы'"))
                {
                    AddRow(res1, r);
                }
                return res1;
            }
        }

        public DataTable GetFinanceReportCompact(DateTime start, DateTime end, Guid? divisionId, bool weekly, bool daily)
        {
            end = end.AddMonths(1).AddMilliseconds(-1);

            if (weekly && daily)
            {
                throw new Exception("Детализация одновременно по дням и по неделям невозможна!");
            }
            using (var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                var res = new DataTable();
                res.ExtendedProperties.Add("Detailed", true);
                res.Columns.Add(ClubText, typeof(string));
                res.Columns.Add("Категория", typeof(string));
                res.Columns.Add("Подкатегория", typeof(string));

                AddDatesColumns(res, start, end, daily, weekly, typeof(decimal));

                foreach (var div in context.Divisions.Where(i => i.Id == divisionId || (divisionId == null && i.CompanyId == user.CompanyId)))
                {

                    //План
                    #region План
                    var plancells = new List<object>();
                    var corpcells = new List<object>();
                    var planperc = new List<object>();
                    var corpperc = new List<object>();

                    plancells.Add(div.Name);
                    plancells.Add("План");
                    plancells.Add("План " + ClubTextV);
                    planperc.Add(div.Name);
                    planperc.Add("План");
                    planperc.Add("План выполнен, %");
                    corpcells.Add(div.Name);
                    corpcells.Add("План");
                    corpcells.Add("Корпоративные продажи");
                    corpperc.Add(div.Name);
                    corpperc.Add("План");
                    corpperc.Add("Корп. план выполнен, %");
                    for (var i = start; i <= end; i = i.AddMonths(1))
                    {
                        var plan = context.SalesPlans.FirstOrDefault(j => (j.DivisionId == div.Id) && (j.Month == i));
                        if (plan != null)
                        {
                            plancells.Add(plan.Value);
                            corpcells.Add(plan.CorpValue);
                            planperc.Add(SalaryCalculation.Get01TotalSalesPercent(context, div.Id, i, DateTime.MinValue));
                            corpperc.Add(SalaryCalculation.Get01aTotalCorporateSalesPercent(context, div.Id, i, DateTime.MinValue));
                            if (daily)
                            {
                                for (int j = 0; j < DateTime.DaysInMonth(i.Year, i.Month); j++)
                                {
                                    planperc.Add(SalaryCalculation.Get01TotalSalesPercent(context, div.Id, i.AddDays(j), i.AddDays(j + 1)));
                                    corpperc.Add(SalaryCalculation.Get01aTotalCorporateSalesPercent(context, div.Id, i.AddDays(j), i.AddDays(j + 1)));
                                }
                            }
                            if (weekly)
                            {
                                foreach (var j in GetWeeksInMonth(i))
                                {
                                    planperc.Add(SalaryCalculation.Get01TotalSalesPercent(context, div.Id, j[0], j[1].AddDays(1)));
                                    corpperc.Add(SalaryCalculation.Get01aTotalCorporateSalesPercent(context, div.Id, j[0], j[1].AddDays(1)));
                                }
                            }

                        }
                        else
                        {
                            plancells.Add(0);
                            corpcells.Add(0);
                            planperc.Add(0);
                            corpperc.Add(0);
                            if (daily)
                            {
                                for (int j = 0; j < DateTime.DaysInMonth(i.Year, i.Month); j++)
                                {
                                    planperc.Add(0);
                                    corpperc.Add(0);
                                }
                            }
                            if (weekly)
                            {
                                foreach (var j in GetWeeksInMonth(i))
                                {
                                    planperc.Add(0);
                                    corpperc.Add(0);
                                }
                            }


                        }

                    }
                    res.Rows.Add(plancells.ToArray());
                    res.Rows.Add(corpcells.ToArray());
                    res.Rows.Add(planperc.ToArray());
                    res.Rows.Add(corpperc.ToArray());
                    #endregion
                    //Доходы
                    #region карты обычные
                    var inc = new List<object>();
                    inc.AddRange(new object[] { div.Name, "Доходы" });
                    {
                        bool flag;

                        var src = context.CustomerCards.Where(i => i.Customer.Tickets.Any()
                                && i.Customer.Tickets.FirstOrDefault().DivisionId == div.Id
                                && !i.Customer.CorporateId.HasValue
                                && i.EmitDate >= start
                                && i.EmitDate < end)
                            .Select(i => new { i.EmitDate, i.Price }).ToArray();

                        var tinc = CalcValue(inc, "Выручка от продажи карт", src, start, end,
                            j => true,
                                        j => j.EmitDate,
                                        j => j.Price,
                                        daily,
                                        weekly,
                                        out flag, false);
                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }
                    #endregion
                    #region карты корпоративные
                    inc = new List<object>();
                    inc.AddRange(new object[] { div.Name, "Доходы корпоративные" });
                    {

                        bool flag;
                        var src2 = context.CustomerCards.Where(i => i.DivisionId == div.Id && i.EmitDate >= start && i.EmitDate <= end)
                            .Where(i => i.Customer.CorporateId.HasValue)
                            .Where(j => j.Customer.Tickets.Any() && j.Customer.Tickets.FirstOrDefault().DivisionId == div.Id)
                            .Select(i => new
                            {
                                i.EmitDate,
                                i.Price
                            })
                            .ToArray();
                        var tinc = CalcValue(inc, "Выручка от продажи карт", src2, start, end,
                            j => true,
                                        j => j.EmitDate,
                                        j => j.Price, daily, weekly,
                                        out flag, false);
                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }
                    #endregion
                    #region абонементы
                    inc = new List<object>();
                    inc.AddRange(new object[] { div.Name, "Доходы" });
                    //foreach (var tt in context.TicketTypes.Where(i => !i.IsAction && (i.Units > 0 || i.GuestUnits > 0)))
                    {
                        var src2 = context.TicketPayments
                            .Where(j => j.Ticket.DivisionId == div.Id
                                && j.PaymentDate >= start && j.PaymentDate <= end
                                && !j.Ticket.Customer.CorporateId.HasValue
                                        && !j.Ticket.TicketType.IsAction
                                        && !j.Ticket.InheritedTicketId.HasValue
                                         && (j.Ticket.TicketType.Units > 0 || j.Ticket.TicketType.GuestUnits > 0)
)
                                    .Where(j => !j.BarOrderId.HasValue || !context.BarOrders.Any(k => k.Id == j.BarOrderId && k.CertificateId.HasValue))

 .Select(i => new { i.PaymentDate, i.Amount }).ToArray();

                        bool flag;
                        var tinc = CalcValue(inc, "Выручка от продажи абонементов", src2, start, end,
                            j => true,

 j => j.PaymentDate,
                                        j => j.Amount, daily, weekly,
                                        out flag, false);
                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }
                    #endregion
                    #region абонементы корпоративные
                    inc = new List<object>();
                    inc.AddRange(new object[] { div.Name, "Доходы корпоративные" });
                    ///foreach (var tt in context.TicketTypes.Where(i => !i.IsAction && (i.Units > 0 || i.GuestUnits > 0)))
                    {
                        bool flag;
                        var src2 = context.TicketPayments.Where(j => j.Ticket.DivisionId == div.Id
                                && j.PaymentDate >= start && j.PaymentDate <= end
                                        && j.Ticket.Customer.CorporateId.HasValue
                                        && !j.Ticket.TicketType.IsAction
                                        && !j.Ticket.InheritedTicketId.HasValue
                                         && (j.Ticket.TicketType.Units > 0 || j.Ticket.TicketType.GuestUnits > 0))
                                    .Where(j => !j.BarOrderId.HasValue || !context.BarOrders.Any(k => k.Id == j.BarOrderId && k.CertificateId.HasValue))
                                    .Select(i => new { i.PaymentDate, i.Amount }).ToArray();
                        var tinc = CalcValue(inc, "Выручка от продажи абонементов", src2, start, end,
                            j => true, j => j.PaymentDate,
                                        j => j.Amount, daily, weekly,
                                        out flag, false);
                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }
                    #endregion
                    #region товары
                    inc = new List<object>();
                    inc.AddRange(new object[] { div.Name, "Доходы", });
                    //foreach (var g in context.Goods)
                    {
                        bool flag;
                        var src2 = context.GoodSales.Where(j => j.BarOrder.DivisionId == div.Id
                            && (j.BarOrder.PaymentDate ?? j.BarOrder.PurchaseDate) >= start
                            && (j.BarOrder.PaymentDate ?? j.BarOrder.PurchaseDate) <= end
                            && j.BarOrder.DepositPayment == 0)
                            .Select(i => new { i.Amount, i.PriceMoney, i.BarOrder.PaymentDate, i.BarOrder.PurchaseDate })
                            .ToArray();
                        var tinc = CalcValue(inc, "Выручка от продажи товаров", src2, start, end,
                            j => true,
                                        j => j.PaymentDate ?? j.PurchaseDate,
                                        j => (decimal)j.Amount * (j.PriceMoney ?? 0), daily, weekly,
                                        out flag, false);
                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }
                    #endregion

                    var srcCertUsage = context.Certificates.Where(i => i.DivisionId == div.Id && i.UsedOrderId.HasValue
                        && i.UsedInOrder.PurchaseDate >= start && i.UsedInOrder.PurchaseDate < end).Select(i => new { i.UsedInOrder.PurchaseDate, Amount = -i.Amount }).ToList();

                    {
                        bool flag;
                        var tinc = CalcValue(inc, "Корректировка оплат сертификатами", srcCertUsage,
                                    start, end,
                                    j => j.PurchaseDate,
                                    daily, weekly,
                                    out flag, false,
                                    (i, _, __) => i.Sum(j => (decimal)j.Amount));
                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }


                    #region Пакеты товаров

                    inc = new List<object>();
                    inc.AddRange(new object[] { div.Name, "Доходы" });
                    var goodPackages = context.BarOrders.Where(i => i.DivisionId == div.Id && i.Kind1C == 10).ToList()
                        .Select(i => new { Date = i.PurchaseDate, Lines = i.GetContent() }).ToList()
                        .SelectMany(i => i.Lines.Select(j => new { Date = i.Date.Date, Line = j }))
                        .ToArray();
                    {
                        bool flag;

                        var tinc = CalcValue(inc, "Выручка от продажи пакетов товаров", goodPackages,
                                    start, end,
                                    j => j.Date,
                                    daily, weekly,
                                    out flag, false,
                                    (i, _, __) => i.Sum(j => j.Line.Cost));

                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }
                    #endregion


                    #region Сертификаты

                    inc = new List<object>();
                    inc.AddRange(new object[] { div.Name, "Доходы" });
                    var certificates = context.Certificates.Where(i => i.SellDate.HasValue && i.DivisionId == div.Id).ToList();
                    {
                        bool flag;

                        var tinc = CalcValue(inc, "Выручка от продажи сертификатов", certificates,
                                    start, end,
                                    j => j.SellDate.Value,
                                    daily, weekly,
                                    out flag, false,
                                    (i, _, __) => i.Sum(j =>
                                    {
                                        var item = j.SellBarOrder.GetContent().FirstOrDefault(k => k.Price == j.PriceMoney);
                                        if (item == null)
                                        {
                                            return 0;
                                        }
                                        return item.Cost;
                                        //j.SellBarOrder.CardPayment + j.SellBarOrder.CashPayment
                                    }));

                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }
                    #endregion

                    #region Доп.услуги
                    inc = new List<object>();
                    inc.AddRange(new object[] { div.Name, "Доходы" });
                    //Детская комната
                    {
                        bool flag;
                        var tinc = CalcValue(inc, "Детская комната", context.ChildrenRooms.Where(i => i.DivisionId == div.Id && i.CreatedOn >= start && i.CreatedOn <= end).ToArray(), start, end,
                            j => true,
                                        j => j.CreatedOn,
                                        j => j.Cost, daily, weekly,
                                        out flag, false);
                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }
                    //Прокат
                    //foreach (var g in context.Goods)
                    {
                        bool flag;
                        var tinc = CalcValue(inc, "Прокат", context.Rents.Where(j => j.Storehouse.DivisionId == div.Id && j.CreatedOn >= start && j.CreatedOn <= end).ToArray(), start, end,
                            j => true,
                                        j => j.CreatedOn,
                                        j => j.IsPayed ? j.Cost + (j.LostFine ?? 0m) + (j.OverdueFine ?? 0m) : 0, daily, weekly,
                                        out flag, false);
                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }
                    #endregion
                    #region абонементы акционные
                    inc = new List<object>();
                    inc.AddRange(new object[] { div.Name, "Доходы" });
                    {
                        var src2 = context.TicketPayments.Where(j => j.Ticket.DivisionId == div.Id
                            && j.PaymentDate >= start && j.PaymentDate <= end
                                    && !j.Ticket.Customer.CorporateId.HasValue
                                    && j.Ticket.TicketType.IsAction
                                    && !j.Ticket.InheritedTicketId.HasValue
 && (j.Ticket.TicketType.SolariumMinutes == 0)
).Select(i => new { i.Amount, i.PaymentDate }).ToArray();
                        bool flag;
                        var tinc = CalcValue(inc, "Выручка от продажи абонементов (не солярий, акционные)", src2, start, end,
                            j => true,
 j => j.PaymentDate,
                                        j => j.Amount, daily, weekly,
                                        out flag, false);
                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }
                    #endregion
                    #region абонементы акционные корпоративные
                    inc = new List<object>();
                    inc.AddRange(new object[] { div.Name, "Доходы  корпоративные" });
                    //foreach (var tt in context.TicketTypes.Where(i => i.IsAction && (i.Units > 0 || i.GuestUnits > 0)))
                    {
                        var src2 = context.TicketPayments.Where(j => j.Ticket.DivisionId == div.Id
                            && j.PaymentDate >= start && j.PaymentDate <= end
                                    && j.Ticket.Customer.CorporateId.HasValue
                                    && j.Ticket.TicketType.IsAction
                                    && !j.Ticket.InheritedTicketId.HasValue
 && (j.Ticket.TicketType.SolariumMinutes == 0)
).Select(i => new { i.Amount, i.PaymentDate }).ToArray();
                        bool flag;
                        var tinc = CalcValue(inc, "Выручка от продажи абонементов (не солярий, акционные)", src2, start, end,
                            j => true,
 j => j.PaymentDate,
                                        j => j.Amount, daily, weekly,
                                        out flag, false);
                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }
                    #endregion
                    #region абонементы солярий
                    inc = new List<object>();
                    inc.AddRange(new object[] { div.Name, "Доходы" });
                    //foreach (var tt in context.TicketTypes.Where(i => i.Units == 0 && i.GuestUnits == 0 && i.SolariumMinutes > 0))
                    {
                        var src2 = context.TicketPayments.Where(j => j.Ticket.DivisionId == div.Id
                                    && !j.Ticket.Customer.CorporateId.HasValue
                                    && j.PaymentDate >= start && j.PaymentDate <= end
                                    && j.Ticket.TicketType.Units == 0
 && j.Ticket.TicketType.GuestUnits == 0
 && !j.Ticket.InheritedTicketId.HasValue
                                    && j.Ticket.TicketType.SolariumMinutes > 0).Select(i => new { i.PaymentDate, i.Amount }).ToArray();
                        bool flag;
                        var tinc = CalcValue(inc, "Выручка от продажи абонементов (солярий)", src2, start, end,
                            j => true,
                                        j => j.PaymentDate,
                                        j => j.Amount, daily, weekly,
                                        out flag, false);
                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }
                    #endregion
                    #region абонементы солярий корпоративные
                    inc = new List<object>();
                    inc.AddRange(new object[] { div.Name, "Доходы корпоративные" });
                    //foreach (var tt in context.TicketTypes.Where(i => i.Units == 0 && i.GuestUnits == 0 && i.SolariumMinutes > 0))
                    {
                        var src2 = context.TicketPayments.Where(j => j.Ticket.DivisionId == div.Id
                                    && j.Ticket.Customer.CorporateId.HasValue
                                    && j.PaymentDate >= start && j.PaymentDate <= end
                                    && j.Ticket.TicketType.Units == 0
                                    && !j.Ticket.InheritedTicketId.HasValue
 && j.Ticket.TicketType.GuestUnits == 0
 && j.Ticket.TicketType.SolariumMinutes > 0).Select(i => new { i.PaymentDate, i.Amount }).ToArray();
                        bool flag;
                        var tinc = CalcValue(inc, "Выручка от продажи абонементов (солярий)", src2, start, end,
                            j => true,
                                        j => j.PaymentDate,
                                        j => j.Amount, daily, weekly,
                                        out flag, false);
                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }
                    #endregion
                    #region солярий
                    inc = new List<object>();
                    inc.AddRange(new object[] { div.Name, "Доходы" });
                    {
                        var src2 = context.SolariumVisits.Where(j => j.Cost.HasValue && j.DivisionId == div.Id
                            && j.VisitDate >= start && j.VisitDate <= end)
                            .Select(i => new { i.VisitDate, i.Cost })
                            .ToArray();
                        bool flag;
                        var tinc = CalcValue(inc, "Выручка от продажи минут солярия", src2, start, end,
                            j => true,
                                        j => j.VisitDate,
                                        j => j.Cost, daily, weekly,
                                        out flag, false);
                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }
                    #endregion
                    #region пополнение депозита
                    inc = new List<object>();
                    inc.AddRange(new object[] { div.Name, "Доходы" });
                    {
                        bool flag;
                        var src = context.BarOrders.Where(i => i.Kind1C == 9 && i.DivisionId == div.Id && i.PurchaseDate >= start && i.PurchaseDate <= end && !i.Customer.CorporateId.HasValue)
                        .Select(i => new { Customer = i.Customer.LastName + " " + i.Customer.FirstName + " " + i.Customer.MiddleName, CreatedOn = i.PurchaseDate, Amount = i.CashPayment + i.CardPayment }).ToArray();
                        var tinc = CalcValue(inc, "Пополнение депозита клиентами", src, start, end,
                            j => true,
                                        j => j.CreatedOn,
                                        j => j.Amount, daily, weekly,
                                        out flag, false);
                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }
                    #endregion
                    #region пополнение депозита корпоративными
                    inc = new List<object>();
                    inc.AddRange(new object[] { div.Name, "Доходы корпоративные" });
                    {
                        bool flag;
                        var src = context.BarOrders.Where(i => i.Kind1C == 9 && i.DivisionId == div.Id && i.PurchaseDate >= start && i.PurchaseDate <= end && i.Customer.CorporateId.HasValue)
                        .Select(i => new { Customer = i.Customer.LastName + " " + i.Customer.FirstName + " " + i.Customer.MiddleName, CreatedOn = i.PurchaseDate, Amount = i.CashPayment }).ToArray();
                        var tinc = CalcValue(inc, "Пополнение депозита клиентами", src, start, end,
                            j => true,
                                        j => j.CreatedOn,
                                        j => j.Amount, daily, weekly,
                                        out flag, false);
                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }
                    #endregion
                    #region доплата за обмен абонемента
                    inc = new List<object>();
                    inc.AddRange(new object[] { div.Name, "Доходы" });
                    //foreach (var tt in context.TicketTypes)
                    {
                        bool flag;
                        var src = context.TicketPayments.Where(j => j.Ticket.DivisionId == div.Id
                                        && j.Ticket.InheritedTicketId.HasValue
                                        && j.PaymentDate >= start && j.PaymentDate <= end
                                        && j.Ticket.InheritedFrom.CustomerId == j.Ticket.CustomerId).Select(i => new { i.PaymentDate, i.Amount }).ToArray();
                        var tinc = CalcValue(inc, "Доплата за абонемент при обмене", src,
                                        start, end,
                                        j => j.PaymentDate,
                                        daily, weekly,
                                        out flag, false,
                                        (i, _, __) => i.Sum(j => j.Amount));
                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }
                    #endregion
                    #region доходы прочие
                    inc = new List<object>();
                    inc.AddRange(new object[] { div.Name, "Доходы" });
                    var src4 = context.Incomes.Where(j => j.DivisionId == div.Id && j.CreatedOn >= start && j.CreatedOn <= end).Select(i => new { i.Amount, i.CreatedOn, i.IncomeTypeId }).ToArray();
                    foreach (var it in context.IncomeTypes.Where(i => i.DivisionId == div.Id))
                    {
                        bool flag;
                        var tinc = CalcValue(inc, it.Name, context.Incomes, start, end,
                            j => j.IncomeTypeId == it.Id,
                                        j => j.CreatedOn,
                                        j => j.Amount, daily, weekly,
                                        out flag, false);
                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }
                    #endregion

                    #region возвраты
                    //var goodsRet = context.GoodSales.Where(i => i.BarOrder.DivisionId == div.Id)
                    //    .Where(i => i.ReturnDate.HasValue && EntityFunctions.TruncateTime(i.ReturnDate) == EntityFunctions.TruncateTime(i.BarOrder.PurchaseDate))
                    //    .Select(i => new { Date = i.BarOrder.PurchaseDate, Price = (i.PriceMoney ?? 0), Amount = i.Amount })
                    //    .ToList();
                    {
                        bool flag;
                        var srcX = context.Spendings.Where(i => i.DivisionId == div.Id
                                && !i.IsInvestment
                                && i.CreatedOn >= start && i.CreatedOn < end
                                && (i.SpendingType.Name == Localization.Resources.GoodRefund || i.SpendingType.Name == "Возврат абонемента"))
                            .Select(i => new { TypeId = i.SpendingTypeId, PaymentType = i.PaymentType, Amount = i.Amount, CreatedOn = i.CreatedOn })
                            .ToList();
                        var tinc = CalcValue(new List<object> { div.Name, "Доходы" },
                                "Возвраты",
                                srcX,
                                start, end,
                                j => j.CreatedOn,
                                daily, weekly,
                                out flag,
                                false,
                                (i, s, e) => -(i.Sum(j => j.Amount)/* - goodsRet.Where(j => j.Date >= s && j.Date < e).Sum(j => (decimal)j.Amount * j.Price)*/));
                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }

                    }
                    #endregion


                    //Расходы
                    #region расходы нал
                    inc = new List<object>();
                    inc.AddRange(new object[] { div.Name, "Расходы" });

                    var src3 = context.Spendings.Where(j => j.CreatedOn >= start && j.CreatedOn <= end && j.DivisionId == div.Id)
                                .Select(i => new { Beznal = i.PaymentType.ToLower().Contains("безнал"), i.SpendingTypeId, i.CreatedOn, i.Amount })
                                .ToArray();

                    foreach (var st in context.SpendingTypes.Where(i => (i.DivisionId == div.Id || !i.DivisionId.HasValue)
                        && i.Name != Localization.Resources.GoodRefund && i.Name != "Возврат абонемента"))
                    {
                        bool flag;
                        var tinc = CalcValue(inc, st.Name, src3, start, end,
                            j => j.SpendingTypeId == st.Id,
                                        j => j.CreatedOn,
                                        j => j.Amount, daily, weekly,
                                        out flag, false);
                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }



                    inc = new List<object>();
                    inc.AddRange(new object[] { div.Name, "Расходы" });

                    var srcDO = context.DepositAccounts.Where(i => (i.Customer.ClubId ?? i.Customer.CustomerCards.FirstOrDefault().DivisionId) == div.Id && i.CreatedOn >= start && i.CreatedOn <= end && i.Description.StartsWith("Вывод средств по заявлению"))
                        .Select(i => new { i.CreatedOn, Amount = -i.Amount }).ToArray();
                    {
                        bool flag;
                        var tinc = CalcValue(inc, "Вывод средств с депозита", srcDO, start, end,
                            j => true,
                                        j => j.CreatedOn,
                                        j => j.Amount, daily, weekly,
                                        out flag, false);
                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }

                    #endregion
                    #region расходы безнал
                    //inc = new List<object>();
                    //inc.AddRange(new object[] { div.Name, "Расходы" });
                    //foreach (var st in context.SpendingTypes.Where(i => (i.DivisionId == div.Id || !i.DivisionId.HasValue)
                    //    && i.Name != Localization.Resources.GoodRefund && i.Name != "Возврат абонемента"))
                    //{
                    //    bool flag;
                    //    var tinc = CalcValue(inc, st.Name + " (безнал)", src3, start, end,
                    //        j => j.SpendingTypeId == st.Id
                    //            && j.Beznal,
                    //                    j => j.CreatedOn,
                    //                    j => j.Amount, daily, weekly,
                    //                    out flag, false);
                    //    if (flag)
                    //    {
                    //        res.Rows.Add(tinc.ToArray());
                    //    }
                    //}
                    #endregion
                    //Итоги
                    #region итоги
                    {
                        var income = new List<object> { div.Name, "Итоги", "Выручка" };
                        var clearincome = new List<object> { div.Name, "Итоги", "Чистая прибыль" };
                        var accumulated = new List<object> { div.Name, "Итоги", "Накопленный итог" };
                        var kkmleft = new List<object> { div.Name, "Итоги", "Остаток в кассе на 1 число" };
                        var unsent = new List<object> { div.Name, "Итоги", "Несданная выручка на 1 число" };
                        var advances = new List<object> { div.Name, "Итоги", "Невыданные авансы на 1 число" };
                        var terminal = new List<object> { div.Name, "Итоги", "Не поступившие оплаты по терминалу на 1 число" };

                        var n = 3;
                        var start1 = start.AddMonths(-1);
                        var prevdfin = context.DivisionFinances.FirstOrDefault(f => f.DivisionId == div.Id && f.Period == start1);
                        var prevaccum = 0m;
                        if (prevdfin != null)
                        {
                            prevaccum = prevdfin.Accum;
                        }
                        for (var i = start; i <= end; i = i.AddMonths(1))
                        {
                            var subres = 0m;
                            var unclearincome = 0m;
                            for (var j = 0; j < res.Rows.Count; j++)
                            {
                                if ((string)res.Rows[j][0] != div.Name) continue;
                                if (res.Rows[j][1].ToString() == "Доходы" || res.Rows[j][1].ToString() == "Доходы корпоративные")
                                {
                                    unclearincome += (decimal)res.Rows[j][n];
                                    subres += (decimal)res.Rows[j][n];
                                }
                                else if (res.Rows[j][1].ToString() == "Расходы") subres -= (decimal)res.Rows[j][n];
                            }

                            prevaccum += subres;

                            var dfin = context.DivisionFinances.FirstOrDefault(f => f.DivisionId == div.Id && f.Period == i) ?? new DivisionFinance();

                            kkmleft.Add(dfin.CashLeft);
                            unsent.Add(dfin.Unsent);
                            advances.Add(dfin.Advances);
                            terminal.Add(dfin.TerminalLoan);

                            accumulated.Add(prevaccum);
                            income.Add(unclearincome);
                            clearincome.Add(subres);
                            n++;
                            if (daily)
                            {
                                for (int k = 0; k < DateTime.DaysInMonth(i.Year, i.Month); k++)
                                {
                                    unclearincome = 0;
                                    subres = 0m;
                                    for (var j = 0; j < res.Rows.Count; j++)
                                    {
                                        if ((string)res.Rows[j][0] != div.Name) continue;
                                        if (res.Rows[j][1].ToString() == "Доходы" || res.Rows[j][1].ToString() == "Доходы корпоративные")
                                        {
                                            unclearincome += (decimal)res.Rows[j][n];
                                            subres += (decimal)res.Rows[j][n];
                                        }
                                        else if (res.Rows[j][1].ToString() == "Расходы") subres -= (decimal)res.Rows[j][n];
                                    }
                                    kkmleft.Add(null);
                                    unsent.Add(null);
                                    advances.Add(null);
                                    terminal.Add(null);

                                    accumulated.Add(null);
                                    income.Add(unclearincome);
                                    clearincome.Add(subres);

                                    n++;
                                }
                            }
                            if (weekly)
                            {
                                foreach (var k in GetWeeksInMonth(i))
                                {
                                    unclearincome = 0;
                                    subres = 0m;
                                    for (var j = 0; j < res.Rows.Count; j++)
                                    {
                                        if ((string)res.Rows[j][0] != div.Name) continue;
                                        if (res.Rows[j][1].ToString() == "Доходы" || res.Rows[j][1].ToString() == "Доходы корпоративные")
                                        {
                                            unclearincome += (decimal)res.Rows[j][n];
                                            subres += (decimal)res.Rows[j][n];
                                        }
                                        else if (res.Rows[j][1].ToString() == "Расходы") subres -= (decimal)res.Rows[j][n];
                                    }
                                    kkmleft.Add(null);
                                    unsent.Add(null);
                                    advances.Add(null);
                                    terminal.Add(null);

                                    accumulated.Add(null);
                                    income.Add(unclearincome);
                                    clearincome.Add(subres);

                                    n++;
                                }
                            }
                        }
                        res.Rows.Add(income.ToArray());
                        res.Rows.Add(clearincome.ToArray());
                        res.Rows.Add(accumulated.ToArray());
                        res.Rows.Add(kkmleft.ToArray());
                        res.Rows.Add(unsent.ToArray());
                        res.Rows.Add(advances.ToArray());
                        res.Rows.Add(terminal.ToArray());
                    }
                    #endregion

                }
                #region Итого по франчайзи
                if (divisionId == null)
                {
                    var income = new List<object> { "Итого по франчайзи", "Итоги", "Выручка" };
                    var clearincome = new List<object> { "Итого по франчайзи", "Итоги", "Чистая прибыль" };
                    var accumulated = new List<object> { "Итого по франчайзи", "Итоги", "Накопленный итог" };
                    var cashleft = new List<object> { "Итого по франчайзи", "Итоги", "Остаток на р/счете на 1 число" };
                    var kkmleft = new List<object> { "Итого по франчайзи", "Итоги", "Остаток в кассе на 1 число" };
                    var unsent = new List<object> { "Итого по франчайзи", "Итоги", "Несданная выручка на 1 число" };
                    var advances = new List<object> { "Итого по франчайзи", "Итоги", "Невыданные авансы на 1 число" };
                    var terminal = new List<object> { "Итого по франчайзи", "Итоги", "Не поступившие оплаты по терминалу на 1 число" };
                    var realaccumulated = new List<object> { "Итого по франчайзи", "Итоги", "Реальный накопленный итог" };
                    var delta = new List<object> { "Итого по франчайзи", "Итоги", "Расхождение" };

                    var n = 3;
                    var start1 = start.AddMonths(-1);
                    var prevdfins = context.DivisionFinances.Where(f => f.CompanyId == user.CompanyId && f.Period == start1);
                    var prevaccum = prevdfins.SafeSum(x => x.Accum);
                    for (var i = start; i <= end; i = i.AddMonths(1))
                    {
                        var subres = 0m;
                        var unclearincome = 0m;
                        for (var j = 0; j < res.Rows.Count; j++)
                        {
                            if (res.Rows[j][1].ToString() == "Доходы" || res.Rows[j][1].ToString() == "Доходы корпоративные")
                            {
                                unclearincome += (decimal)res.Rows[j][n];
                                subres += (decimal)res.Rows[j][n];
                            }
                            else if (res.Rows[j][1].ToString() == "Расходы") subres -= (decimal)res.Rows[j][n];
                        }

                        prevaccum += subres;


                        var cfin = context.CompanyFinances.FirstOrDefault(c => c.CompanyId == user.CompanyId && c.Period == i) ?? new CompanyFinance();
                        var dfins = context.DivisionFinances.Where(f => f.CompanyId == user.CompanyId && f.Period == i);

                        cashleft.Add(cfin.AccountLeft);
                        kkmleft.Add(dfins.SafeSum(x => x.CashLeft));
                        unsent.Add(dfins.SafeSum(x => x.Unsent));
                        advances.Add(dfins.SafeSum(x => x.Advances));
                        terminal.Add(dfins.SafeSum(x => x.TerminalLoan));
                        realaccumulated.Add(cfin.AccountLeft + dfins.SafeSum(x => x.CashLeft) + dfins.SafeSum(x => x.Unsent) + dfins.SafeSum(x => x.Advances) + dfins.SafeSum(x => x.TerminalLoan));

                        delta.Add(cfin.AccountLeft + dfins.SafeSum(x => x.CashLeft) + dfins.SafeSum(x => x.Unsent) + dfins.SafeSum(x => x.Advances) + dfins.SafeSum(x => x.TerminalLoan) - prevaccum);

                        accumulated.Add(prevaccum);
                        income.Add(unclearincome);
                        clearincome.Add(subres);
                        n++;
                        if (daily)
                        {
                            for (int k = 0; k < DateTime.DaysInMonth(i.Year, i.Month); k++)
                            {
                                subres = 0m;
                                for (var j = 0; j < res.Rows.Count; j++)
                                {
                                    if (res.Rows[j][1].ToString() == "Доходы" || res.Rows[j][1].ToString() == "Доходы корпоративные")
                                    {
                                        unclearincome += (decimal)res.Rows[j][n];
                                        subres += (decimal)res.Rows[j][n];
                                    }
                                    else if (res.Rows[j][1].ToString() == "Расходы") subres -= (decimal)res.Rows[j][n];
                                }
                                cashleft.Add(null);
                                kkmleft.Add(null);
                                unsent.Add(null);
                                advances.Add(null);
                                terminal.Add(null);
                                realaccumulated.Add(null);

                                delta.Add(null);

                                accumulated.Add(null);
                                income.Add(unclearincome);
                                clearincome.Add(subres);

                                n++;
                            }
                        }
                        if (weekly)
                        {
                            foreach (var k in GetWeeksInMonth(i))
                            {
                                subres = 0m;
                                for (var j = 0; j < res.Rows.Count; j++)
                                {
                                    if (res.Rows[j][1].ToString() == "Доходы" || res.Rows[j][1].ToString() == "Доходы корпоративные")
                                    {
                                        unclearincome += (decimal)res.Rows[j][n];
                                        subres += (decimal)res.Rows[j][n];
                                    }
                                    else if (res.Rows[j][1].ToString() == "Расходы") subres -= (decimal)res.Rows[j][n];
                                }
                                cashleft.Add(null);
                                kkmleft.Add(null);
                                unsent.Add(null);
                                advances.Add(null);
                                terminal.Add(null);
                                realaccumulated.Add(null);

                                delta.Add(null);

                                accumulated.Add(null);
                                income.Add(unclearincome);
                                clearincome.Add(subres);

                                n++;
                            }
                        }
                    }
                    res.Rows.Add(income.ToArray());
                    res.Rows.Add(clearincome.ToArray());
                    res.Rows.Add(accumulated.ToArray());
                    res.Rows.Add(cashleft.ToArray());
                    res.Rows.Add(kkmleft.ToArray());
                    res.Rows.Add(unsent.ToArray());
                    res.Rows.Add(advances.ToArray());
                    res.Rows.Add(terminal.ToArray());
                    res.Rows.Add(realaccumulated.ToArray());
                    res.Rows.Add(delta.ToArray());
                }
                #endregion
                var res1 = res.Clone();
                res1.Rows.Clear();

                foreach (var r in res.Select("Категория='План'"))
                {
                    AddRow(res1, r);
                }
                foreach (var r in res.Select("Категория='Итоги'"))
                {
                    AddRow(res1, r);
                }
                foreach (var r in res.Select("Категория='Доходы'"))
                {
                    AddRow(res1, r);
                }
                foreach (var r in res.Select("Категория='Расходы'"))
                {
                    AddRow(res1, r);
                }
                return res1;
            }
        }
    }
}
