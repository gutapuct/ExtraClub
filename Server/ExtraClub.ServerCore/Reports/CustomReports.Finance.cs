using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ExtraClub.Entities;

using ExtraClub.ServiceModel;
using System.Data.Objects;

namespace ExtraClub.ServerCore
{
    partial class CustomReports
    {
        public DataTable GetFinanceReportCat(DateTime start, DateTime end, Guid? divisionId)
        {
            end = end.AddMonths(1).AddMilliseconds(-1);

            using(var context = new ExtraEntities())
            {
                var user = UserManagement.GetUser(context);
                var res = new DataTable();

                var incomesUsl = new List<DataRow>();
                var incomesTov = new List<DataRow>();
                var incomesOth = new List<DataRow>();

                var spends = new List<DataRow>();

                var invs = new List<DataRow>();

                var commonSpesList = new List<DataRow>();

                res.ExtendedProperties.Add("Detailed", true);
                res.ExtendedProperties.Add("DotsFormat", 1);
                res.ExtendedProperties.Add("Fin", true);

                res.Columns.Add("_style", typeof(string));
                res.Columns.Add("Номер", typeof(string));
                res.Columns.Add("Категория", typeof(string));

                AddDatesColumnsFin(res, start, end, typeof(decimal));

                int baseNumber = 1;
                var divs = context.Divisions.Where(i => i.Id == divisionId || (divisionId == null && i.CompanyId == user.CompanyId)).ToList();
                var divsCount = divs.Count;

                bool flag;
                var src99 = context.Incomes.Where(i => !i.DivisionId.HasValue
                    && i.CompanyId == user.CompanyId
                    && i.CreatedOn >= start && i.CreatedOn < end).Where(i => !i.IsFinAction).ToList();
                var commonIncomes = CalcValueFin(new List<object> { "", "", "доходы Общепроектные" },
                    src99, start, end,
                    j => j.CreatedOn,
                    out flag,
                    (j, _, __) => j.Sum(i => i.Amount),
                    i => false,
                    i => i.PaymentType.ToLower() == "нал",
                    i => i.PaymentType.ToLower().Contains("безнал"),
                    i => i.PaymentType.ToLower().Contains("прочее"));

                var src0 = context.Spendings.Where(i => !i.DivisionId.HasValue && i.CompanyId == user.CompanyId
                    && !i.IsInvestment && i.CreatedOn >= start && i.CreatedOn < end)
                    .Where(i => !i.IsFinAction)
                    .Select(i => new { TypeId = i.SpendingTypeId, PaymentType = i.PaymentType, Amount = i.Amount, CreatedOn = i.CreatedOn })
                    .ToList();
                var commonSpendings = CalcValueFin(new List<object> { "", "", "расходы Общепроектные" },
                    src0, start, end,
                    j => j.CreatedOn,
                    out flag,
                    (i, s, e) => i.Sum(j => j.Amount),
                                j => j.PaymentType.ToLower().Contains("касса"),
                                j => j.PaymentType.ToLower() == "нал",
                                j => j.PaymentType.ToLower().Contains("безнал"),
                                i => i.PaymentType.ToLower().Contains("прочее"));

                //var src1 = context.Spendings.Where(i => !i.DivisionId.HasValue && i.IsInvestment && i.CreatedOn >= start && i.CreatedOn < end)
                //    .Select(i => new { TypeId = i.SpendingTypeId, PaymentType = i.PaymentType, Amount = i.Amount, CreatedOn = i.CreatedOn })
                //    .ToList();

                //var commonInvestitions = CalcValue(new List<object> { "", "" },
                //    "инвестиции Общепроектные",
                //    src1,
                //    start, end,
                //    j => j.CreatedOn,
                //    days, weeks,
                //    out flag, false,
                //    (i, s, e) => i.Sum(j => j.Amount));

                foreach(var div in divs)
                {
                    res.Rows.Add("cs1", baseNumber + ".", div.Name);
                    res.Rows.Add("cs1", baseNumber + ".1.", "Плановые показатели");
                    //План
                    #region План
                    var plancells = new List<object>();
                    var planperc = new List<object>();

                    plancells.AddRange(new object[] { "cs1", baseNumber + ".1.1.", "План " + ClubTextV + ", выручка" });
                    planperc.AddRange(new object[] { "cs1", baseNumber + ".1.2.", "План выполнен, %" });
                    for(var i = start; i <= end; i = i.AddMonths(1))
                    {
                        var plan = context.SalesPlans.FirstOrDefault(j => (j.DivisionId == div.Id) && (j.Month == i));
                        if(plan != null)
                        {
                            plancells.Add(plan.Value + plan.CorpValue);
                            planperc.Add((SalaryCalculation.Get01TotalSalesPercent(context, div.Id, i, DateTime.MinValue) / 100 * plan.Value
                                + SalaryCalculation.Get01aTotalCorporateSalesPercent(context, div.Id, i, DateTime.MinValue) / 100 * plan.CorpValue)
                                / (plan.CorpValue + plan.Value) * 100);
                        }
                        else
                        {
                            plancells.Add(0);
                            planperc.Add(0);
                        }

                        plancells.Add(0);
                        planperc.Add(0);
                        plancells.Add(0);
                        planperc.Add(0);
                        plancells.Add(0);
                        planperc.Add(0);
                        plancells.Add(0);
                        planperc.Add(0);

                    }
                    res.Rows.Add(plancells.ToArray());
                    res.Rows.Add(planperc.ToArray());
                    #endregion

                    //Доходы
                    var incomesRow = res.Rows.Add("cs2", baseNumber + ".2.", "Доходы");
                    var uslugiRow = res.Rows.Add("cs8-", baseNumber + ".2.1.", "Услуги");

                    var cardsRow = res.Rows.Add("cs3", baseNumber + ".2.1.1.", "Клиентские карты");
                    #region карты обычные
                    var currNumber = 1;
                    foreach(var ct in context.CustomerCardTypes)
                    {
                        var tinc = CalcValueFin(new List<object> { "cs4", baseNumber + ".2.1.1." + currNumber + ".", ct.Name },
                                        context.CustomerCards
                                            .Where(j => j.Customer.Tickets.FirstOrDefault().DivisionId == div.Id
                                                && j.CustomerCardTypeId == ct.Id),
                                        start, end,
                                        j => j.EmitDate,
                                        out flag,
                                        (i, _, __) => i.Sum(j => j.Price),
                                        i => true,
                                        i => false,
                                        i => false,
                                        i => false);
                        if(flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                            currNumber++;
                            ApplyRowValues(tinc, cardsRow, uslugiRow, incomesRow);
                        }
                    }
                    #endregion

                    var usualTicketsRow = res.Rows.Add("cs3", baseNumber + ".2.1.2.", "Абонементы (не солярий, не акционные)");
                    #region абонементы
                    currNumber = 1;

                    var src2 = context.TicketPayments
                        .Where(j => j.Ticket.DivisionId == div.Id && !j.Ticket.InheritedTicketId.HasValue)
                        .Where(j => !j.BarOrderId.HasValue || !context.BarOrders.Any(k => k.Id == j.BarOrderId && k.CertificateId.HasValue))
                        .Select(i => new
                        {
                            TicketTypeId = i.Ticket.TicketTypeId,
                            PaymentDate = i.PaymentDate,
                            Amount = i.Amount
                        })
                        .ToList();

                    foreach(var tt in context.TicketTypes.Where(i => !i.IsAction && (i.Units > 0 || i.GuestUnits > 0)))
                    {
                        var tinc = CalcValueFin(new List<object> { "cs4", baseNumber + ".2.1.2." + currNumber + ".", tt.Name },
                            src2.Where(j => j.TicketTypeId == tt.Id), start, end,
                                        j => j.PaymentDate,
                                        out flag,
                                        (i, _, __) => i.Sum(j => j.Amount),
                                        i => true,
                                        i => false,
                                        i => false,
                                        i => false);
                        if(flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                            currNumber++;
                            ApplyRowValues(tinc, usualTicketsRow, uslugiRow, incomesRow);
                        }
                    }
                    #endregion

                    var actTicketsRow = res.Rows.Add("cs3", baseNumber + ".2.1.3.", "Абонементы акционные");
                    #region абонементы акционные
                    currNumber = 1;

                    var src2a = context.TicketPayments.Where(i => i.Ticket.DivisionId == div.Id
                                                                && i.PaymentDate >= start && i.PaymentDate < end
                                                                && !i.Ticket.InheritedTicketId.HasValue)
                                                      .Where(j => !j.BarOrderId.HasValue || !context.BarOrders.Any(k => k.Id == j.BarOrderId && k.CertificateId.HasValue))
                                                      .Select(i => new
                                                                {
                                                                    TicketTypeId = i.Ticket.TicketTypeId,
                                                                    PaymentDate = i.PaymentDate,
                                                                    Amount = i.Amount
                                                                }).ToArray();
                    foreach(var tt in context.TicketTypes.Where(i => i.IsAction && (i.SolariumMinutes == 0)))
                    {
                        var tinc = CalcValueFin(new List<object> { "cs4", baseNumber + ".2.1.3." + currNumber + ".",
                            tt.Name }, src2a.Where(j => j.TicketTypeId == tt.Id), start, end,
                                        j => j.PaymentDate,
                                        out flag,
                                        (j, _, __) => j.Sum(i => i.Amount),
                                        i => true,
                                        i => false,
                                        i => false,
                                        i => false);
                        if(flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                            currNumber++;
                            ApplyRowValues(tinc, actTicketsRow, uslugiRow, incomesRow);
                        }
                    }
                    #endregion

                    var chTicketsRow = res.Rows.Add("cs3", baseNumber + ".2.1.4.", "Доплата за абонемент при обмене");
                    #region доплата за обмен абонемента
                    currNumber = 1;
                    var src2b = context.TicketPayments.Where(i => i.Ticket.DivisionId == div.Id
                                                                && i.PaymentDate >= start && i.PaymentDate < end
                                                                && i.Ticket.InheritedTicketId.HasValue
                                                                && i.Ticket.InheritedFrom.CustomerId == i.Ticket.CustomerId)
                                                        .Where(j => !j.BarOrderId.HasValue || !context.BarOrders.Any(k => k.Id == j.BarOrderId && k.CertificateId.HasValue))
                                                        .Select(i => new
                                                                {
                                                                    TicketTypeId = i.Ticket.TicketTypeId,
                                                                    PaymentDate = i.PaymentDate,
                                                                    Amount = i.Amount
                                                                }).ToArray();
                    foreach(var tt in context.TicketTypes)
                    {
                        var tinc = CalcValueFin(new List<object> { "cs4", baseNumber + ".2.1.4." + currNumber + ".",
                            tt.Name }, src2b.Where(j => j.TicketTypeId == tt.Id),
                            start, end,
                                        j => j.PaymentDate,
                                        out flag,
                                        (j, _, __) => j.Sum(i => i.Amount),
                                        i => true,
                                        i => false,
                                        i => false,
                                        i => false);
                        if(flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                            currNumber++;
                            ApplyRowValues(tinc, chTicketsRow, uslugiRow, incomesRow);
                        }
                    }

                    #endregion

                    var solTicketsRow = res.Rows.Add("cs3", baseNumber + ".2.1.5.", "Абонементы в солярий");
                    #region абонементы солярий
                    currNumber = 1;

                    var src2c = context.TicketPayments.Where(i => i.Ticket.DivisionId == div.Id
                                            && i.PaymentDate >= start && i.PaymentDate < end
                                            && !i.Ticket.InheritedTicketId.HasValue)
                                    .Where(j => !j.BarOrderId.HasValue || !context.BarOrders.Any(k => k.Id == j.BarOrderId && k.CertificateId.HasValue))
                                    .Select(i => new
                                            {
                                                TicketTypeId = i.Ticket.TicketTypeId,
                                                PaymentDate = i.PaymentDate,
                                                Amount = i.Amount
                                            }).ToArray();

                    foreach(var tt in context.TicketTypes.Where(i => i.SolariumMinutes > 0))
                    {
                        var tinc = CalcValueFin(new List<object> { "cs4", baseNumber + ".2.1.5." + currNumber + ".",
                            tt.Name }, src2c.Where(j => j.TicketTypeId == tt.Id), start, end,
                                        j => j.PaymentDate,
                                        out flag,
                                        (j, _, __) => j.Sum(i => i.Amount),
                                        i => true,
                                        i => false,
                                        i => false,
                                        i => false);
                        if(flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                            currNumber++;
                            ApplyRowValues(tinc, solTicketsRow, uslugiRow, incomesRow);
                        }
                    }
                    #endregion

                    var dopUslRow = res.Rows.Add("cs3", baseNumber + ".2.1.6.", "Дополнительные услуги");
                    #region Доп.услуги
                    currNumber = 1;
                    {
                        var tinc = CalcValue(new List<object> { "cs4", baseNumber + ".2.1.6." + currNumber + "." },
                            "Детская комната", context.ChildrenRooms.Where(j => j.DivisionId == div.Id).ToList(), start, end,
                                        j => j.CreatedOn,
                                        false, false,
                                        out flag, false,
                                        (i, _, __) => i.Sum(j => j.Cost));
                        if(flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                            currNumber++;
                            ApplyRowValues(tinc, dopUslRow, uslugiRow, incomesRow);
                        }
                    }
                    {
                        var src2d = context.SolariumVisits.Where(j => j.Cost.HasValue && j.DivisionId == div.Id
                            && j.VisitDate >= start && j.VisitDate < end)
                            .Select(i => new { VisitDate = i.VisitDate, Cost = i.Cost }).ToArray();
                        var tinc = CalcValueFin(new List<object> { "cs3", baseNumber + ".2.1.6." + currNumber + ".",
                            "Выручка от продажи минут солярия" }, src2d, start, end,
                                        j => j.VisitDate,
                                        out flag,
                                        (j, _, __) => j.Sum(i => i.Cost),
                                        i => true,
                                        i => false,
                                        i => false,
                                        i => false);
                        if(flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                            currNumber++;
                            ApplyRowValues(tinc, dopUslRow, uslugiRow, incomesRow);
                        }
                    }
                    var src2e = context.Rents
                        .Where(j => j.Storehouse.DivisionId == div.Id && j.CreatedOn >= start && j.CreatedOn < end)
                        .ToList();
                    foreach(var g in context.Goods)
                    {
                        var tinc = CalcValueFin(new List<object> { "cs4", baseNumber + ".2.1.6." + currNumber + ".",
                            "Прокат - " + g.Name }, src2e.Where(j => j.GoodId == g.Id), start, end,
                                    j => j.CreatedOn,
                                    out flag,
                                    (i, _, __) => i.Sum(j => j.IsPayed ? j.Cost + (j.LostFine ?? 0m) + (j.OverdueFine ?? 0m) : 0),
                                        i => true,
                                        i => false,
                                        i => false,
                                        i => false);
                        if(flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                            currNumber++;
                            ApplyRowValues(tinc, dopUslRow, uslugiRow, incomesRow);
                        }
                    }
                    #endregion
                    incomesUsl.Add(uslugiRow);

                    var goodsRow = res.Rows.Add("cs8", baseNumber + ".2.2.", "Товары");
                    var ourGoodsRow = res.Rows.Add("cs3-", baseNumber + ".2.2.1.", "Наши товары");
                    #region Наши товары

                    var src3 = context.GoodSales.Where(x => x.BarOrder.DivisionId == div.Id)
                        .Where(i => !i.ReturnDate.HasValue)
                        //.Where(i => !i.ReturnDate.HasValue || EntityFunctions.TruncateTime(i.ReturnDate) != EntityFunctions.TruncateTime(i.BarOrder.PurchaseDate))
                        .Where(i => (i.BarOrder.CashPayment > 0 || i.BarOrder.CardPayment > 0) && i.BarOrder.DepositPayment == 0)
                        .Select(i => new { GoodId = i.GoodId, Date = i.BarOrder.PaymentDate ?? i.BarOrder.PurchaseDate, Amount = i.Amount, Price = i.PriceMoney ?? 0 })
                        .ToList();

                    currNumber = 1;
                    foreach(var g in context.Goods.Where(i => i.IsOurs))
                    {
                        var tinc = CalcValueFin(new List<object> { "cs4", baseNumber + ".2.2.1." + currNumber + ".",
                                    g.Name }, src3.Where(i => i.GoodId == g.Id),
                                    start, end,
                                    j => j.Date,
                                    out flag,
                                    (i, _, __) => i.Sum(j => (decimal)j.Amount * j.Price),
                                    i => true,
                                    i => false,
                                    i => false,
                                        i => false);
                        if(flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                            currNumber++;
                            ApplyRowValues(tinc, ourGoodsRow, goodsRow, incomesRow);
                        }
                    }
                    #endregion


                    var notOurGoodsRow = res.Rows.Add("cs3-", baseNumber + ".2.2.2.", "Не наши товары");
                    #region Не наши товары

                    currNumber = 1;
                    foreach(var g in context.Goods.Where(i => !i.IsOurs))
                    {
                        var tinc = CalcValueFin(new List<object> { "cs4", baseNumber + ".2.2.2." + currNumber + ".",
                                    g.Name }, src3.Where(x => x.GoodId == g.Id),
                                    start, end,
                                    j => j.Date,
                                    out flag,
                                    (i, _, __) => i.Sum(j => (decimal)j.Amount * j.Price),
                                    i => true,
                                    i => false,
                                    i => false,
                                    i => false);
                        if(flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                            currNumber++;
                            ApplyRowValues(tinc, notOurGoodsRow, goodsRow, incomesRow);
                        }
                    }
                    #endregion

                    var certsRow = res.Rows.Add("cs3-", baseNumber + ".2.2.3.", "Сертификаты");
                    #region Сертификаты

                    currNumber = 1;
                    var certificates = context.Certificates.Where(i => i.SellDate.HasValue && i.DivisionId == div.Id).ToList();
                    foreach(var cert in certificates)
                    {
                        var tinc = CalcValueFin(new List<object> { "cs4", baseNumber + ".2.2.3." + currNumber + ".",
                                    cert.Name }, Enumerable.Repeat(cert, 1),
                                    start, end,
                                    j => j.SellDate,
                                    out flag,
                                    (i, _, __) => i.Sum(j => j.SellBarOrder.CashPayment + j.SellBarOrder.CardPayment),
                                    i => true,
                                    i => false,
                                    i => false,
                                    i => false);
                        if(flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                            currNumber++;
                            ApplyRowValues(tinc, certsRow, goodsRow, incomesRow);
                        }
                    }
                    #endregion


                    var packetsRow = res.Rows.Add("cs3-", baseNumber + ".2.2.4.", "Пакеты товаров");
                    #region Пакеты товаров

                    currNumber = 1;
                    var goodPackages = context.BarOrders.Where(i => i.DivisionId == div.Id && i.Kind1C == 10 && i.PurchaseDate >= start && i.PurchaseDate <= end).ToList()
                        .Select(i => new { Date = i.PurchaseDate, Lines = i.GetContent() }).ToList()
                        .SelectMany(i => i.Lines.Select(j => new { Date = i.Date.Date, Line = j }))
                        .GroupBy(i => i.Line.Name)
                        .ToArray();
                    foreach(var gp in goodPackages)
                    {
                        var tinc = CalcValueFin(new List<object> { "cs4", baseNumber + ".2.2.4." + currNumber + ".",
                                    gp.Key }, gp,
                                    start, end,
                                    j => j.Date,
                                    out flag,
                                    (i, _, __) => i.Sum(j => (decimal)j.Line.Cost),
                                    i => true,
                                    i => false,
                                    i => false,
                                    i => false);
                        if(flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                            currNumber++;
                            ApplyRowValues(tinc, packetsRow, goodsRow, incomesRow);
                        }
                    }
                    #endregion

                    incomesTov.Add(goodsRow);

                    var depsRow = res.Rows.Add("cs8", baseNumber + ".2.3.", "Пополнения депозита");
                    var srcD = context.BarOrders.Where(i => i.Kind1C == 9 && i.DivisionId == div.Id && i.PurchaseDate >= start && i.PurchaseDate <= end)
                        .Select(i => new { Customer = i.Customer.LastName + " " + i.Customer.FirstName + " " + i.Customer.MiddleName, i.PurchaseDate, i.CashPayment }).ToArray();

                    currNumber = 1;
                    foreach(var g in srcD.GroupBy(i => i.Customer))
                    {
                        var tinc = CalcValueFin(new List<object> { "cs4", baseNumber + ".2.3." + currNumber + ".",
                                    g.Key }, srcD.Where(j => j.Customer == g.Key),
                                    start, end,
                                    j => j.PurchaseDate,
                                    out flag,
                                    (i, _, __) => i.Sum(j => j.CashPayment),
                                    i => true,
                                    i => false,
                                    i => false,
                                        i => false);
                        if(flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                            currNumber++;
                            ApplyRowValues(tinc, depsRow, incomesRow);
                        }
                    }

                    incomesTov.Add(depsRow);

                    var commonInc = res.Rows.Add("cs8", baseNumber + ".2.4.", "Общепроектные доходы");
                    ApplyRowValues(commonIncomes, commonInc);
                    ApplyRowValues(commonInc, commonInc, (i, j) => i / divsCount);
                    ApplyRowValues(incomesRow, commonInc, (i, j) => i + j);

                    incomesOth.Add(commonInc);


                    currNumber = 1;
                    #region доходы прочие
                    var src3a = context.Incomes.Where(i => i.DivisionId == div.Id && i.CreatedOn >= start && i.CreatedOn < end)
                        .Where(i => !i.IsFinAction)
                        .Select(i => new { IncomeTypeId = i.IncomeTypeId, CreatedOn = i.CreatedOn, Amount = i.Amount, PmtType = i.PaymentType })
                        .ToList();
                    if(src3a.Any())
                    {
                        var othersRow = res.Rows.Add("cs8", baseNumber + ".2.3.", "Прочее");
                        foreach(var it in context.IncomeTypes)
                        {
                            var tinc = CalcValueFin(new List<object> { "cs3", baseNumber + ".2.1.4." + currNumber + ".",
                            it.Name }, src3a.Where(j => j.IncomeTypeId == it.Id), start, end,
                                            j => j.CreatedOn,
                                            out flag,
                                            (i, _, __) => i.Sum(j => j.Amount),
                                            i => i.PmtType.ToLower().Contains("касса"),
                                            i => i.PmtType.ToLower() == "нал",
                                            i => i.PmtType.ToLower().Contains("безнал"),
                                            i => i.PmtType.ToLower().Contains("прочее"));
                            if(flag)
                            {
                                res.Rows.Add(tinc.ToArray());
                                currNumber++;
                                ApplyRowValues(tinc, othersRow, incomesRow);
                            }
                        }
                    #endregion
                        incomesOth.Add(othersRow);
                    }

                    #region возвраты
                    //var goodsRet = context.GoodSales.Where(i => i.BarOrder.DivisionId == div.Id)
                    //    .Where(i => i.ReturnDate.HasValue && EntityFunctions.TruncateTime(i.ReturnDate) == EntityFunctions.TruncateTime(i.BarOrder.PurchaseDate))
                    //    .Select(i => new { Date = i.BarOrder.PurchaseDate, Price = (i.PriceMoney ?? 0), Amount = i.Amount })
                    //    .ToList();
                    {
                        var srcX = context.Spendings.Where(i => i.DivisionId == div.Id
                                && !i.IsInvestment
                                && i.CreatedOn >= start && i.CreatedOn < end
                                && (i.SpendingType.Name == Localization.Resources.GoodRefund || i.SpendingType.Name == "Возврат абонемента"))
                                .Where(i => !i.IsFinAction)
                            .Select(i => new { TypeId = i.SpendingTypeId, PaymentType = i.PaymentType, Amount = i.Amount, CreatedOn = i.CreatedOn })
                            .ToList();
                        var tinc = CalcValueFin(new List<object> { "cs8", baseNumber + ".2.5.",
                                "Возвраты" },
                                srcX,
                                start, end,
                                j => j.CreatedOn,
                                out flag,
                                (i, s, e) => -(i.Sum(j => j.Amount)/* - goodsRet.Where(j => j.Date >= s && j.Date < e).Sum(j => (decimal)j.Amount * j.Price)*/),
                                j => j.PaymentType.ToLower().Contains("касса") || j.PaymentType == "Возврат абонемента" || j.PaymentType == "Возврат товара",
                                j => j.PaymentType.ToLower() == "нал",
                                j => j.PaymentType.ToLower().Contains("безнал"),
                                j => j.PaymentType.ToLower().Contains("прочее"));
                        if(flag)
                        {
                            var retsRow = res.Rows.Add(tinc.ToArray());
                            ApplyRowValues(tinc, incomesRow);
                            incomesTov.Add(retsRow);
                        }

                    }
                    #endregion

                    //Расходы
                    var outcomesRow = res.Rows.Add("cs2", baseNumber + ".3.", "Расходы");
                    #region расходы
                    currNumber = 1;

                    var src = context.Spendings
                        .Where(i => i.DivisionId == div.Id && !i.IsInvestment && i.CreatedOn >= start && i.CreatedOn < end)
                        .Where(i => !i.IsFinAction)
                        .Select(i => new { TypeId = i.SpendingTypeId, PaymentType = i.PaymentType, Amount = i.Amount, CreatedOn = i.CreatedOn })
                        .ToList();

                    foreach(var st in context.SpendingTypes)
                    {
                        List<object> tinc;
                        if(st.Name != Localization.Resources.GoodRefund && st.Name != "Возврат абонемента")
                        {
                            tinc = CalcValueFin(new List<object> { "cs8", baseNumber + ".3." + currNumber + ".",
                                st.Name},
                                src.Where(j => j.TypeId == st.Id),
                                start, end,
                                j => j.CreatedOn,
                                out flag,
                                (i, s, e) => i.Sum(j => j.Amount),
                                j => j.PaymentType.ToLower().Contains("касса"),
                                j => j.PaymentType.ToLower() == "нал",
                                j => j.PaymentType.ToLower().Contains("безнал"),
                                j => j.PaymentType.ToLower().Contains("прочее"));
                            if(flag)
                            {
                                var outcomeRow = res.Rows.Add(tinc.ToArray());

                                spends.Add(outcomeRow);
                                currNumber++;
                                ApplyRowValues(tinc, outcomesRow);
                            }
                        }
                    }

                    var commonSpes = res.Rows.Add("cs8", baseNumber + ".3." + currNumber + ".", "Общепроектные расходы");

                    ApplyRowValues(commonSpendings, commonSpes);
                    ApplyRowValues(commonSpes, commonSpes, (i, j) => i / divsCount);
                    ApplyRowValues(outcomesRow, commonSpes, (i, j) => i + j);

                    commonSpesList.Add(commonSpes);

                    #endregion
                    currNumber++;

                    var depsOutRow = res.Rows.Add("cs8", baseNumber + ".3." + currNumber + ".", "Вывод с депозита");
                    var srcDO = context.DepositAccounts.Where(i => (i.Customer.ClubId ?? i.Customer.CustomerCards.FirstOrDefault().DivisionId) == div.Id && i.CreatedOn >= start && i.CreatedOn <= end && i.Description.StartsWith("Вывод средств по заявлению"))
                        .Select(i => new { Customer = i.Customer.LastName + " " + i.Customer.FirstName + " " + i.Customer.MiddleName, i.CreatedOn, Amount = -i.Amount }).ToArray();

                    var subCurrNumber = 1;
                    foreach(var g in srcDO.GroupBy(i => i.Customer))
                    {
                        var tinc = CalcValueFin(new List<object> { "cs4", baseNumber + ".3." + currNumber + "." + subCurrNumber,
                                    g.Key }, srcDO.Where(j => j.Customer == g.Key),
                                    start, end,
                                    j => j.CreatedOn,
                                    out flag,
                                    (i, _, __) => i.Sum(j => j.Amount),
                                    i => true,
                                    i => false,
                                    i => false,
                                        i => false);
                        if(flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                            subCurrNumber++;
                            ApplyRowValues(tinc, depsOutRow, outcomesRow);
                        }
                    }


                    //Итоги
                    var totals = res.Rows.Add("cs2", baseNumber + ".4.", "Итого по " + ClubTextU);
                    var totRevenues = res.Rows.Add("cs2", baseNumber + ".4.1.", "Прибыль");
                    ApplyRowValues(totRevenues, incomesRow, (a, b) => a + b);
                    ApplyRowValues(totRevenues, outcomesRow, (a, b) => a - b);

                    var rent = res.Rows.Add("cs5", baseNumber + ".4.2.", "Рентабельность, %");
                    ApplyRowValues(rent, totRevenues, (a, b) => a + b);
                    ApplyRowValues(rent, incomesRow, (a, b) => b == 0 ? 0 : (a / b * 100));


                    //Инвестиции
                    #region Инвестиции
                    currNumber = 1;

                    var src1 = context.Spendings.Where(i => i.DivisionId == div.Id && i.IsInvestment && i.CreatedOn >= start && i.CreatedOn < end)
                        .Select(i => new { TypeId = i.SpendingTypeId, PaymentType = i.PaymentType, Amount = i.Amount, CreatedOn = i.CreatedOn }).ToList();

                    var invsRow = res.Rows.Add("cs2", baseNumber + ".4.3.", "Инвестиции");
                    var hasInvs = false;
                    foreach(var st in context.SpendingTypes.Where(i => i.DivisionId == div.Id || !i.DivisionId.HasValue))
                    {
                        var tinc = CalcValueFin(new List<object> { "cs4", baseNumber + ".4.3." + currNumber + ".",
                            st.Name }, src1.Where(j => j.TypeId == st.Id), start, end,
                                        j => j.CreatedOn,
                                        out flag,
                                        (j, _, __) => j.Sum(i => i.Amount),
                                        j => j.PaymentType.ToLower() == "касса",
                                        j => j.PaymentType.ToLower() != "нал",
                                        j => j.PaymentType.ToLower() == "безнал",
                                        j => j.PaymentType.ToLower() == "прочее");
                        if(flag)
                        {
                            invs.Add(res.Rows.Add(tinc.ToArray()));
                            currNumber++;
                            ApplyRowValues(tinc, invsRow);
                            hasInvs = true;
                        }
                    }
                    if(!hasInvs)
                    {
                        res.Rows.Remove(invsRow);
                    }
                    //else
                    {
                        var incMinInv = res.Rows.Add("cs2", baseNumber + ".4.4.", "Прибыль-инвестиции");
                        if(hasInvs)
                        {
                            ApplyRowValues(incMinInv, invsRow, (i, j) => -j);
                        }
                        ApplyRowValues(incMinInv, totRevenues, (i, j) => i + j);
                    }
                    #endregion

                    baseNumber++;
                }

                if(!divisionId.HasValue)
                {
                    res.Rows.Add("cs1", baseNumber + ".", "Общепроектные");

                    var incsRow = res.Rows.Add("cs2", baseNumber + ".1.", "Доходы");
                    #region доходы Общепроектные
                    int currNumber = 1;
                    var src4a = context.Incomes
                        .Where(i => !i.DivisionId.HasValue && i.CreatedOn >= start && i.CreatedOn < end && i.CompanyId == user.CompanyId)
                        .Where(i => !i.IsFinAction)
                        .ToList();
                    foreach(var it in context.IncomeTypes)
                    {
                        var tinc = CalcValueFin(new List<object> { "cs3", baseNumber + ".1." + currNumber + ".",
                            it.Name }, src4a.Where(j => j.IncomeTypeId == it.Id), start, end,
                            j => j.CreatedOn,
                            out flag,
                            (i, _, __) => i.Sum(j => j.Amount),
                            j => j.PaymentType.ToLower() == "касса",
                            j => j.PaymentType.ToLower() == "нал",
                            j => j.PaymentType.ToLower() == "безнал",
                            j => j.PaymentType.ToLower() == "прочее");
                        if(flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                            currNumber++;
                            ApplyRowValues(tinc, incsRow);
                        }
                    }
                    #endregion
                    //incomesOth.Add(incsRow);

                    #region расходы Общепроектные


                    var outcomesRow = res.Rows.Add("cs2", baseNumber + ".2.", "Расходы");
                    currNumber = 1;

                    var src = context.Spendings.Where(i => !i.DivisionId.HasValue && i.CompanyId == user.CompanyId
                        && !i.IsInvestment && i.CreatedOn >= start && i.CreatedOn < end)
                        .Where(i => !i.IsFinAction)
                        .Select(i => new { TypeId = i.SpendingTypeId, PaymentType = i.PaymentType, Amount = i.Amount, CreatedOn = i.CreatedOn })
                        .ToList();

                    foreach(var st in context.SpendingTypes)
                    {
                        var tinc = CalcValueFin(new List<object> { "cs4", baseNumber + ".2." + currNumber + ".",
                                st.Name },
                                src.Where(j => j.TypeId == st.Id),
                                start, end,
                                j => j.CreatedOn,
                                out flag,
                                (i, s, e) => i.Sum(j => j.Amount),
                                j => j.PaymentType.ToLower().Contains("касса"),
                                j => j.PaymentType.ToLower() == "нал",
                                j => j.PaymentType.ToLower().Contains("безнал"),
                                j => j.PaymentType.ToLower() == "прочее");
                        if(flag)
                        {
                            var outcomeRow = res.Rows.Add(tinc.ToArray());

                            spends.Add(outcomeRow);
                            currNumber++;
                            ApplyRowValues(tinc, outcomesRow);
                        }
                    }
                    #endregion

                    #region инвестиции Общепроектные


                    var invsRow = res.Rows.Add("cs2", baseNumber + ".3.", "Инвестиции");
                    currNumber = 1;

                    var src1 = context.Spendings.Where(i => !i.DivisionId.HasValue && i.IsInvestment
                        && i.CompanyId == user.CompanyId
                        && i.CreatedOn >= start && i.CreatedOn < end)
                        .Where(i => !i.IsFinAction)
                        .Select(i => new { TypeId = i.SpendingTypeId, PaymentType = i.PaymentType, Amount = i.Amount, CreatedOn = i.CreatedOn })
                        .ToList();

                    foreach(var st in context.SpendingTypes)
                    {
                        var tinc = CalcValueFin(new List<object> { "cs4", baseNumber + ".3." + currNumber + ".",
                                st.Name },
                                src1.Where(j => j.TypeId == st.Id),
                                start, end,
                                j => j.CreatedOn,
                                out flag,
                                (i, s, e) => i.Sum(j => j.Amount),
                                i => i.PaymentType.ToLower() == "касса",
                                i => i.PaymentType.ToLower() == "нал",
                                i => i.PaymentType.ToLower().Contains("безнал"),
                                i => i.PaymentType.ToLower() == "прочее");
                        if(flag)
                        {
                            var outcomeRow = res.Rows.Add(tinc.ToArray());

                            invs.Add(outcomeRow);
                            currNumber++;
                            ApplyRowValues(tinc, invsRow);
                        }
                    }
                    #endregion


                    baseNumber++;

                    res.Rows.Add("cs1", baseNumber + ".", "Итоги по " + ClubTextUs);
                    var totInc = res.Rows.Add("cs7-", baseNumber + ".1.", "Доходы");

                    var totUslRow = res.Rows.Add("cs3", baseNumber + ".1.1.", "Услуги");
                    var totGoodsRow = res.Rows.Add("cs3", baseNumber + ".1.2.", "Товары");
                    var totOthersRow = res.Rows.Add("cs3", baseNumber + ".1.3.", "Прочее");

                    foreach(var r in incomesUsl)
                    {
                        ApplyRowValues(totUslRow, r, (a, b) => a + b);
                    }
                    foreach(var r in incomesTov)
                    {
                        ApplyRowValues(totGoodsRow, r, (a, b) => a + b);
                    }
                    foreach(var r in incomesOth)
                    {
                        ApplyRowValues(totOthersRow, r, (a, b) => a + b);
                    }
                    ApplyRowValues(totInc, totUslRow, (a, b) => a + b);
                    ApplyRowValues(totInc, totGoodsRow, (a, b) => a + b);
                    ApplyRowValues(totInc, totOthersRow, (a, b) => a + b);

                    var totSpe = res.Rows.Add("cs7", baseNumber + ".2.", "Расходы");

                    foreach(var r in spends)
                    {
                        ApplyRowValues(totSpe, r, (a, b) => a + b);
                    }

                    //foreach (var r in commonSpesList)
                    //{
                    //    ApplyRowValues(totSpe, r, (a, b) => a + b);
                    //}


                    var totPrib = res.Rows.Add("cs7", baseNumber + ".3.", "Чистая прибыль");
                    ApplyRowValues(totPrib, totInc, (a, b) => a + b);
                    ApplyRowValues(totPrib, totSpe, (a, b) => a - b);
                    //var totRent = res.Rows.Add("cs7", baseNumber + ".4.", "Рентабельность, %");
                    //ApplyRowValues(totRent, totPrib, (a, b) => a + b);
                    //ApplyRowValues(totRent, totInc, (a, b) => b == 0 ? 0 : (a / b * 100));

                    var totInv = res.Rows.Add("cs7", baseNumber + ".4.", "Инвестиции");

                    foreach(var r in invs)
                    {
                        ApplyRowValues(totInv, r, (a, b) => a + b);
                    }

                    var totItog = res.Rows.Add("cs7", baseNumber + ".5.", "Прибыль-инвестиции");
                    ApplyRowValues(totItog, totPrib, (a, b) => a + b);
                    ApplyRowValues(totItog, totInv, (a, b) => a - b);

                    #region Фин деятельность
                    baseNumber++;

                    var fsrcinc = context.Incomes.Where(i => i.IsFinAction && i.CompanyId == user.CompanyId
                            && i.CreatedOn >= start && i.CreatedOn < end).GroupBy(i => i.IncomeType.Name)
                            .OrderBy(i => i.Key).ToArray();

                    res.Rows.Add("cs1", baseNumber + ".", "Финансовая деятельность");
                    var finc = res.Rows.Add("cs2", baseNumber + ".1.", "Доходы");
                    var curr = 1;
                    foreach(var pair in fsrcinc)
                    {
                        var inc = res.Rows.Add("cs8", baseNumber + ".1." + curr + ".", pair.Key);
                        var curr1 = 1;
                        foreach(var i in pair)
                        {
                            var t = CalcValueFin(new List<object> { "cs3", baseNumber + ".1." + curr + "." + curr1 + ".", i.Name },
                                new List<Income> { i }, start, end,
                                j => j.CreatedOn,
                                out flag,
                                (j, _, __) => j.Sum(k => k.Amount),
                                j => j.PaymentType.ToLower() == "касса",
                                j => j.PaymentType.ToLower() == "нал",
                                j => j.PaymentType.ToLower().Contains("безнал"),
                                j => j.PaymentType.ToLower() == "прочее");
                            if(flag)
                            {
                                res.Rows.Add(t.ToArray());
                                ApplyRowValues(t, inc, finc);
                                curr1++;
                            }
                        }

                        curr++;
                    }

                    var fsrcspe = context.Spendings.Where(i => i.IsFinAction && i.CompanyId == user.CompanyId
                        && i.CreatedOn >= start && i.CreatedOn < end).GroupBy(i => i.SpendingType.Name)
                        .OrderBy(i => i.Key).ToArray();
                    var fspe = res.Rows.Add("cs2", baseNumber + ".2.", "Расходы");
                    curr = 1;

                    foreach(var pair in fsrcspe)
                    {
                        var spe = res.Rows.Add("cs8", baseNumber + ".2." + curr + ".", pair.Key);
                        var curr1 = 1;
                        foreach(var i in pair)
                        {
                            var t = CalcValueFin(new List<object> { "cs3", baseNumber + ".2." + curr + "." + curr1 + ".", i.Name },
                                new List<Spending> { i }, start, end,
                                j => j.CreatedOn,
                                out flag,
                                (j, _, __) => j.Sum(k => k.Amount),
                                j => j.PaymentType.ToLower() == "касса",
                                j => j.PaymentType.ToLower() == "нал",
                                j => j.PaymentType.ToLower().Contains("безнал"),
                                j => j.PaymentType.ToLower() == "прочее");
                            if(flag)
                            {
                                res.Rows.Add(t.ToArray());
                                ApplyRowValues(t, spe, fspe);
                                curr1++;
                            }

                        }
                        curr++;
                    }
                    #endregion

                }
                return res;
            }
        }

        private void AddDatesColumnsFin(DataTable res, DateTime start, DateTime end, Type type)
        {
            var startM = start.AddDays(-start.Day + 1);
            var i = startM;
            for(i = startM; i <= end; i = i.AddMonths(1))
            {
                var dc = res.Columns.Add(i.ToString("MMMM yyyy"), type);
                dc.ExtendedProperties.Add("MonthColumn", true);
                res.Columns.Add("Касса " + i.ToString("MMMM yyyy"), type);
                res.Columns.Add("Нал " + i.ToString("MMMM yyyy"), type);
                res.Columns.Add("Безнал " + i.ToString("MMMM yyyy"), type);
                res.Columns.Add("Прочее " + i.ToString("MMMM yyyy"), type);
            }
        }

        private void ApplyRowValues(List<object> source, params DataRow[] destination)
        {
            foreach(var dest in destination)
            {
                for(int i = 0; i < source.Count; i++)
                {
                    if(source[i] is decimal)
                    {
                        if(dest[i] is DBNull) dest[i] = 0m;
                        dest[i] = (decimal)dest[i] + (decimal)source[i];
                    }
                }
            }
        }

        private void ApplyRowValues(DataRow destination, DataRow source, Func<decimal, decimal, decimal> func)
        {
            for(int i = 0; i < destination.ItemArray.Length; i++)
            {
                if(source[i] is decimal)
                {
                    if(destination[i] is DBNull) destination[i] = 0m;
                    destination[i] = func((decimal)destination[i], (decimal)source[i]);
                }
            }
        }

        private List<object> CalcValueFin<TSource>(IEnumerable<object> title, IEnumerable<TSource> source, DateTime start, DateTime end, Func<TSource, DateTime?> dateFunc, out bool addFlag,
            Func<IEnumerable<TSource>, DateTime, DateTime, decimal?> statFunc, Func<TSource, bool> fr, Func<TSource, bool> nal, Func<TSource, bool> beznal, Func<TSource, bool> prochee)
        {
            var tinc = new List<object>();
            tinc.AddRange(title.ToArray());

            addFlag = false;
            var startM = start.AddDays(-start.Day + 1);
            var i = startM;
            for(i = startM; i <= end; i = i.AddMonths(1))
            {

                var i1 = i.AddMonths(1);

                var q = System.Linq.Enumerable
                    .Where(source, j => (dateFunc(j) >= i && dateFunc(j) < i1) || dateFunc(j) == null);

                //total
                if(q.Count() > 0)
                {
                    var sum = statFunc(q, i, i1);
                    tinc.Add(sum);
                    if(sum != 0)
                        addFlag = true;
                }
                else tinc.Add(0m);
                //fr
                var q1 = q.Where(fr);
                if(q1.Count() > 0)
                {
                    var sum = statFunc(q1, i, i1);
                    tinc.Add(sum);
                    if(sum != 0)
                        addFlag = true;
                }
                else tinc.Add(0m);
                //nal
                q1 = q.Where(nal);
                if(q1.Count() > 0)
                {
                    var sum = statFunc(q1, i, i1);
                    tinc.Add(sum);
                    if(sum != 0)
                        addFlag = true;
                }
                else tinc.Add(0m);
                //beznal
                q1 = q.Where(beznal);
                if(q1.Count() > 0)
                {
                    var sum = statFunc(q1, i, i1);
                    tinc.Add(sum);
                    if(sum != 0)
                        addFlag = true;
                }
                else tinc.Add(0m);
                //prochee
                q1 = q.Where(prochee);
                if(q1.Count() > 0)
                {
                    var sum = statFunc(q1, i, i1);
                    tinc.Add(sum);
                    if(sum != 0)
                        addFlag = true;
                }
                else tinc.Add(0m);

            }
            return tinc;
        }


    }
}
