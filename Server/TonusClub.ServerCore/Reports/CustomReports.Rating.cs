using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using TonusClub.Entities;

using System.Data.Objects;
using System.Data.Objects.SqlClient;

namespace TonusClub.ServerCore
{
    partial class CustomReports
    {
        public DataTable ClubRatingReport(DateTime start, DateTime end, Guid? divisionId)
        {
            end = end.AddMonths(1).AddMilliseconds(-1);
            var res = new DataTable();
            using (var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                res.ExtendedProperties.Add("Detailed", true);
                res.ExtendedProperties.Add("DefaultCellFormat", 1);
                res.ExtendedProperties.Add("DotsFormat", 2);

                res.Columns.Add("Номер", typeof(string));
                res.Columns.Add("Категория", typeof(string));

                AddDatesColumns(res, start, end, false, false, typeof(decimal));
                int baseNumber = 1;
                foreach (var div in context.Divisions.Where(i => i.Id == divisionId || (divisionId == null && i.CompanyId == user.CompanyId)))
                {
                    res.Rows.Add(baseNumber + ".", div.Name);

                    res.Rows.Add(baseNumber + ".1.", "Основные показатели");
                    #region Количество проданных повторно абонементов
                    bool flag;
                    var src1 = context.Tickets
                                    .Where(i => i.DivisionId == div.Id
                                        && i.CreatedOn >= start
                                        && i.CreatedOn < end
                                        && i.Customer.Tickets.OrderBy(j => j.CreatedOn).FirstOrDefault().Id != i.Id)
                                    .Select(i => i.CreatedOn).ToList();
                    var tinc = CalcValue(new List<object> { baseNumber + ".1.1." },
                                    "Количество проданных повторно абонементов",
                                    src1,
                                    start, end,
                                    j => j,
                                    false,
                                    false,
                                    out flag,
                                    false,
                                    (i, _, __) => i.Count());
                    if (flag)
                    {
                        res.Rows.Add(tinc.ToArray());
                    }
                    #endregion

                    #region Количество проданных единиц
                    var src2 = context.Tickets.Where(i => i.DivisionId == div.Id)
                        .Where(i => i.CreatedOn >= start && i.CreatedOn < end)
                        .Select(i => new { date = i.CreatedOn, units = i.UnitsAmount }).ToList();
                    tinc = CalcValue(new List<object> { baseNumber + ".1.2." },
                                    "Количество проданных единиц",
                                    src2,
                                    start, end,
                                    j => j.date,
                                    false,
                                    false,
                                    out flag,
                                    false,
                                    (i, _, __) => i.Sum(j => j.units));
                    if (flag)
                    {
                        res.Rows.Add(tinc.ToArray());
                    }
                    #endregion

                    #region Общее количество израсходованных единиц
                    var src3 = context.TreatmentEvents.Where(i => i.Ticket.DivisionId == div.Id
                        && (i.VisitStatus == 2 || i.VisitStatus == 3)
                        && i.VisitDate >= start && i.VisitDate < end)
                        .Select(i => new { date = i.VisitDate, charge = i.TreatmentConfig.Price }).ToList();
                    tinc = CalcValue(new List<object> { baseNumber + ".1.3." },
                                    "Общее количество израсходованных единиц",
                                    src3,
                                    start, end,
                                    j => j.date,
                                    false,
                                    false,
                                    out flag,
                                    false,
                                    (i, _, __) => i.Sum(j => j.charge));
                    if (flag)
                    {
                        res.Rows.Add(tinc.ToArray());
                    }
                    #endregion

                    #region Среднее количество израсходованных единиц на абонемент
                    var src4 = context.UnitCharges.Where(i => i.Ticket.DivisionId == div.Id && i.Date >= start && i.Date < end)
                        .Select(i => new { date = i.Date, charge = i.Charge, ticketid = i.TicketId }).ToList();
                    tinc = CalcValue(new List<object> { baseNumber + ".1.4." },
                    "Среднее количество израсходованных единиц на абонемент",
                    src4,
                    start, end,
                    j => j.date,
                    false,
                    false,
                    out flag,
                    false,
                    (i, _, __) => i.Count() > 0 ? i.Sum(j => (decimal)j.charge) / i.Select(j => j.ticketid).Distinct().Count() : (decimal?)null);
                    if (flag)
                    {
                        res.Rows.Add(tinc.ToArray());
                    }
                    #endregion

                    res.Rows.Add(baseNumber + ".2.", "Количество единиц, израсходованных по каждому виду оборудования");

                    #region Количество единиц, израсходованных по каждому виду оборудования
                    int curr = 1;
                    foreach (var tr in context.Treatments.Select(i => i.TreatmentType).Distinct().OrderBy(i => i.Name))
                    {
                        var src = context.TreatmentEvents
                            .Where(i => i.Ticket.DivisionId == div.Id
                                && (i.VisitStatus == 2 || i.VisitStatus == 3)
                                && i.TreatmentConfig.TreatmentTypeId == tr.Id
                                && i.VisitDate >= start
                                && i.VisitDate < end)
                            .Select(i => new { date = i.VisitDate, price = i.TreatmentConfig.Price })
                            .ToList();
                        tinc = CalcValue(new List<object> { baseNumber + ".2." + curr + "." },
                        tr.Name,//TODO:Localize
                        src,
                        start, end,
                        j => j.date,
                        false,
                        false,
                        out flag,
                        false,
                        (i, _, __) => i.Sum(j => j.price));
                        if (flag)
                        {
                            res.Rows.Add(tinc.ToArray());
                        }
                    }

                    #endregion

                    #region Среднее количество единиц, потраченное при первом посещении
                    var src5 = context.TreatmentEvents
                        .Where(i => i.Ticket.DivisionId == div.Id
                            && i.VisitDate > start && i.VisitDate < end
                            && (i.VisitStatus == 2 || i.VisitStatus == 3)
                            && i.Customer.CustomerVisits.Any()
                            && i.VisitDate < EntityFunctions.AddDays(EntityFunctions.TruncateTime(i.Customer.CustomerVisits.OrderBy(j => j.InTime).FirstOrDefault().OutTime), 1)
                            && i.VisitDate > EntityFunctions.TruncateTime(i.Customer.CustomerVisits.OrderBy(j => j.InTime).FirstOrDefault().InTime))
                        .Select(i => new { customer = i.CustomerId, date = i.VisitDate, cost = i.TreatmentConfig.Price })
                        .ToList();

                    tinc = CalcValue(new List<object> { baseNumber + ".3." },
                                    "Среднее количество единиц, потраченное при первом посещении",
                                    src5,
                                    start, end,
                                    j => j.date,
                                    false,
                                    false,
                                    out flag,
                                    false,
                                    (i, _, __) => i.Select(j => new { customer = j.customer, cost = j.cost })
                                        .GroupBy(j => j.customer).Average(x => x.Sum(y => y.cost)));
                    if (flag)
                    {
                        res.Rows.Add(tinc.ToArray());
                    }
                    #endregion

                    var incRow = res.Rows.Add(baseNumber + ".4.", "Выручка по " + ClubTextU);

                    #region Выручка от продажи абонементов
                    var src6 = context.TicketPayments.Where(i => i.Ticket.DivisionId == div.Id
                        && i.PaymentDate >= start
                        && i.PaymentDate < end)
                        .Select(i => new { date = i.PaymentDate, cost = i.Amount })
                        .ToList();
                    tinc = CalcValue(new List<object> { baseNumber + ".4.1." },
                                    "Выручка от продажи абонементов",
                                    src6,
                                    start, end,
                                    j => j.date,
                                    false,
                                    false,
                                    out flag,
                                    false,
                                    (i, _, __) => i.Sum(j => j.cost));
                    if (flag)
                    {
                        ApplyRowValues(tinc, incRow);
                        res.Rows.Add(tinc.ToArray());
                    }
                    #endregion

                    #region Выручка от продажи товаров бара
                    var src7 = context.GoodSales.Where(i => i.BarOrder.DivisionId == div.Id
                        && i.BarOrder.PurchaseDate >= start
                        && i.BarOrder.PurchaseDate < end)
                        .Select(i => new { date = i.BarOrder.PurchaseDate, cost = ((int)i.Amount) * (i.PriceMoney ?? 0) })
                        .ToList();
                    tinc = CalcValue(new List<object> { baseNumber + ".4.2." },
                                    "Выручка от продажи товаров бара",
                                    src7,
                                    start, end,
                                    j => j.date,
                                    false,
                                    false,
                                    out flag,
                                    false,
                                    (i, _, __) => i.Sum(j => j.cost));
                    if (flag)
                    {
                        ApplyRowValues(tinc, incRow);
                        res.Rows.Add(tinc.ToArray());
                    }
                    #endregion

                    #region Количество новых членов клуба
                    var src8 = context
                        .Customers
                        .Where(i => i.Tickets.Any(j => j.DivisionId == div.Id))
                        .Select(i => new { date = i.Tickets.Where(j => j.DivisionId == div.Id).OrderBy(j => j.CreatedOn).FirstOrDefault().CreatedOn })
                        .ToList();
                    tinc = CalcValue(new List<object> { baseNumber + ".5." },
                                    "Количество новых членов "+ClubTextV,
                                    src8,
                                    start, end,
                                    j => j.date,
                                    false,
                                    false,
                                    out flag,
                                    false,
                                    (i, _, __) => i.Count());
                    var newMembers = res.Rows.Add(tinc.ToArray());
                    #endregion

                    #region Количество добавленных в данном клубе потенциальных клиентов
                    var src9 = context
                        .Customers
                        .Where(i => i.ClubId == div.Id)
                        .Select(i => i.CreatedOn)
                        .ToList();
                    tinc = CalcValue(new List<object> { baseNumber + ".6." },
#if BEAUTINIKA
                                    "Количество добавленных в данной студии потенциальных клиентов",
#else
                                    "Количество добавленных в данном клубе потенциальных клиентов",
#endif
                                    src9,
                                    start, end,
                                    j => j,
                                    false,
                                    false,
                                    out flag,
                                    false,
                                    (i, _, __) => i.Count());
                    var newPotential = res.Rows.Add(tinc.ToArray());
                    #endregion

                    var total = res.Rows.Add(baseNumber + ".7.", "Конвертация, %");
                    ApplyRowValues(total, newPotential, (i, j) => j);
                    ApplyRowValues(total, newMembers, (i, j) => (i != 0) ? (j / i * 100) : 0);


                    baseNumber++;
                }

            }
            return res;
        }

        public DataTable ClubRatingReportTotal(DateTime start, DateTime end)
        {
            end = end.AddDays(1).AddMilliseconds(-1);
            var res = new DataTable();
            using (var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                res.ExtendedProperties.Add("DefaultCellFormat", 1);
                res.ExtendedProperties.Add("VerticalHeadersStart", 2);
                res.ExtendedProperties.Add("DotsFormat", 3);

                res.Columns.Add("Номер", typeof(string));
                res.Columns.Add("Категория", typeof(string));
                var cols = new Dictionary<Guid, DataColumn>();
                foreach (var div in context.Divisions.OrderBy(i => i.Name).Select(i => new { Id = i.Id, Name = i.Name }))
                {
                    cols.Add(div.Id, res.Columns.Add(div.Name, typeof(decimal)));
                }

                var incRow = res.Rows.Add("1.", "Выручка по "+ClubTextU);

                #region Выручка от продажи абонементов
                var inc = context.TicketPayments.Where(i =>
                    i.PaymentDate >= start && i.PaymentDate < end)
                    .GroupBy(i => i.Ticket.DivisionId)
                    .Select(i => new { div = i.Key, sum = i.Sum(j => j.Amount) })
                    .ToDictionary(i => i.div, i => i.sum);

                var row = res.Rows.Add("1.1.", "Выручка от продажи абонементов");
                UpdateRowValues(row, cols, inc);
                ApplyRowValues(incRow, row, (a, b) => b);

                #endregion

                #region Выручка от продажи товаров бара
                inc = context.GoodSales.Where(i =>
                    i.BarOrder.PurchaseDate >= start
                    && i.BarOrder.PurchaseDate < end)
                    .GroupBy(i => i.BarOrder.DivisionId)
                    .Select(i => new { div = i.Key, cost = i.Sum(j => ((int)j.Amount) * (j.PriceMoney ?? 0)) })
                    .ToDictionary(i => i.div, i => i.cost);

                row = res.Rows.Add("1.2.", "Выручка от продажи товаров бара");
                UpdateRowValues(row, cols, inc);
                ApplyRowValues(incRow, row, (a, b) => a + b);

                #endregion

                #region Количество проданных абонементов
                inc = context.Tickets
                        .Where(i =>
                            i.CreatedOn >= start
                            && i.CreatedOn < end)
                        .GroupBy(i => i.DivisionId)
                        .Select(i => new { div = i.Key, count = i.Count() })
                        .ToDictionary(i => i.div, i => (decimal)i.count);
                row = res.Rows.Add("2.", "Количество проданных абонементов");
                UpdateRowValues(row, cols, inc);

                #endregion

                #region Количество проданных повторно абонементов
                inc = context.Tickets
                        .Where(i =>
                            i.CreatedOn >= start
                            && i.CreatedOn < end
                            && i.Customer.Tickets.OrderBy(j => j.CreatedOn).FirstOrDefault().Id != i.Id)
                        .GroupBy(i => i.DivisionId)
                        .Select(i => new { div = i.Key, count = i.Count() })
                        .ToDictionary(i => i.div, i => (decimal)i.count);
                row = res.Rows.Add("3.", "Количество проданных повторно абонементов");
                UpdateRowValues(row, cols, inc);

                #endregion

                #region Количество проданных единиц
                inc = context.Tickets
                        .Where(i =>
                            i.CreatedOn >= start
                            && i.CreatedOn < end)
                        .GroupBy(i => i.DivisionId)
                        .Select(i => new { div = i.Key, units = i.Sum(j => j.UnitsAmount) })
                        .ToDictionary(i => i.div, i => (decimal)i.units);
                row = res.Rows.Add("4.", "Количество проданных единиц");
                UpdateRowValues(row, cols, inc);
                #endregion

                #region Общее количество израсходованных единиц
                inc = context.TreatmentEvents
                        .Where(i =>
                            (i.VisitStatus == 2 || i.VisitStatus == 3)
                            && i.VisitDate >= start
                            && i.VisitDate < end
                            && i.TicketId.HasValue
                            && i.Customer.Tickets.OrderBy(j => j.CreatedOn).FirstOrDefault().Id != i.Id)
                        .GroupBy(i => i.Ticket.DivisionId)
                        .Select(i => new { div = i.Key, units = i.Sum(j => j.TreatmentConfig.Price) })
                        .ToDictionary(i => i.div, i => (decimal)i.units);
                row = res.Rows.Add("5.", "Общее количество израсходованных единиц");
                UpdateRowValues(row, cols, inc);

                #endregion

                #region Количество единиц, израсходованных по каждому виду оборудования
                int curr = 1;
                foreach (var tr in context.TreatmentTypes.OrderBy(i => i.Name))
                {
                    inc = context.TreatmentEvents
                        .Where(i => (i.VisitStatus == 2 || i.VisitStatus == 3)
                            && i.TicketId != null
                            && i.TreatmentConfig.TreatmentTypeId == tr.Id
                            && i.VisitDate >= start
                            && i.VisitDate < end)
                        .GroupBy(i => i.Ticket.DivisionId)
                        .Select(i => new { div = i.Key, units = i.Sum(j => j.TreatmentConfig.Price) })
                        .ToDictionary(i => i.div, i => (decimal)i.units);
                    row = res.Rows.Add("5." + curr++ + ".", tr.Name);
                    UpdateRowValues(row, cols, inc);
                }

                #endregion


                #region Среднее количество израсходованных единиц на абонемент

                inc = context.UnitCharges
                        .Where(i =>
                            i.Date >= start
                            && i.Date < end)
                        .GroupBy(i => i.Ticket.DivisionId)
                        .Select(i => new { div = i.Key, avg = i.Count() > 0 ? (((decimal)i.Sum(j => j.Charge)) / i.Select(j => j.TicketId).Distinct().Count()) : 0 })
                        .ToDictionary(i => i.div, i => (decimal)i.avg);
                row = res.Rows.Add("6.", "Среднее количество израсходованных единиц на абонемент");
                UpdateRowValues(row, cols, inc);
                #endregion

                #region Среднее количество единиц, потраченное при первом посещении

                inc = context.TreatmentEvents
                    .Where(i =>
                        i.VisitDate >= start
                        && i.VisitDate < end
                        && i.TicketId.HasValue
                        && (i.VisitStatus == 2 || i.VisitStatus == 3)
                        && i.VisitDate < EntityFunctions.AddDays(EntityFunctions.TruncateTime(i.Customer.CustomerVisits.OrderBy(j => j.InTime).FirstOrDefault().OutTime), 1)
                        && i.VisitDate > EntityFunctions.TruncateTime(i.Customer.CustomerVisits.OrderBy(j => j.InTime).FirstOrDefault().InTime))
                    .GroupBy(i => i.Ticket.DivisionId)
                    .Select(i => new
                    {
                        div = i.Key,
                        avg = i.Sum(j => j.TreatmentConfig.Price) /
                                            i.Select(j => new
                                            {
                                                date = EntityFunctions.TruncateTime(j.VisitDate).Value,
                                                cid = j.CustomerId
                                            })
                                                .Distinct().Count()
                    })
                    .ToDictionary(i => i.div, i => (decimal)i.avg);
                row = res.Rows.Add("7.", "Среднее количество единиц, потраченное при первом посещении");
                UpdateRowValues(row, cols, inc);
                #endregion

                #region Количество новых членов клуба
                inc = context.Customers
                    .Where(i =>
                        i.Tickets.Any()
                        && i.Tickets.OrderBy(j => j.CreatedOn).FirstOrDefault().CreatedOn >= start
                        && i.Tickets.OrderBy(j => j.CreatedOn).FirstOrDefault().CreatedOn < end)
                    .GroupBy(i => i.Tickets.OrderBy(j => j.CreatedOn).FirstOrDefault().DivisionId)
                    .Select(i => new
                    {
                        div = i.Key,
                        count = i.Count()
                    })
                    .ToDictionary(i => i.div, i => (decimal)i.count);
                var newMembers = res.Rows.Add("8.", "Количество новых членов "+ClubTextV);
                UpdateRowValues(newMembers, cols, inc);

                #endregion

                #region Количество добавленных в данном клубе потенциальных клиентов

                inc = context.Customers
                        .Where(i =>
                            i.ClubId.HasValue
                            && i.CreatedOn >= start
                            && i.CreatedOn < end)
                        .GroupBy(i => i.ClubId)
                        .Select(i => new
                        {
                            div = i.Key.Value,
                            count = i.Count()
                        })
                        .ToDictionary(i => i.div, i => (decimal)i.count);
                var newPotential = res.Rows.Add("9.", "Количество добавленных в данном клубе потенциальных клиентов");
                UpdateRowValues(newPotential, cols, inc);

                #endregion

                var total = res.Rows.Add("10.", "Конвертация, %");
                ApplyRowValues(total, newPotential, (i, j) => j);
                ApplyRowValues(total, newMembers, (i, j) => (i != 0) ? (j / i * 100) : 0);

            }
            return res;
        }

        private void UpdateRowValues(DataRow row, Dictionary<Guid, DataColumn> cols, Dictionary<Guid, decimal> inc)
        {
            foreach (var i in inc)
            {
                row[cols[i.Key]] = i.Value;
            }
        }
    }
}
