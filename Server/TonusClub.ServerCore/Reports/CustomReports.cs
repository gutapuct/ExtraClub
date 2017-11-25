using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using TonusClub.Entities;
using System.Collections.Concurrent;
using TonusClub.ServiceModel;
using System.Threading.Tasks;

namespace TonusClub.ServerCore
{
    public partial class CustomReports
    {
        public DataTable SalesFunnel(DateTime start, DateTime end, Guid divisionId)
        {
            return new SalesFunnelReport(start, end, divisionId).Execute();
        }

        public DataTable GetEmployeesVisitReport(DateTime start, DateTime end, Guid divisionId)
        {
            using(var context = new TonusEntities())
            {
                var res = new DataTable();
                res.Columns.Add("_Id", typeof(Guid));
                res.Columns.Add("Код сотрудника", typeof(int));
                res.Columns.Add("Дата", typeof(DateTime));
                res.Columns.Add("ФИО", typeof(string));
                res.Columns.Add("Подразделение", typeof(string));
                res.Columns.Add("Должность", typeof(string));
                res.Columns.Add("Время прихода", typeof(TimeSpan));
                res.Columns.Add("Отклонение прихода, мин.", typeof(int));
                res.Columns.Add("Время ухода", typeof(TimeSpan));
                res.Columns.Add("Отклонение ухода, мин.", typeof(int));
                res.Columns.Add("Длительность отлучек, мин.", typeof(int));
                res.Columns.Add("Всего отработано часов", typeof(double));
                res.Columns.Add("Отклонение, мин.", typeof(int));//Отклонение прихода-Отклонение ухода+Длительность отлучек

                res.ExtendedProperties.Add("EntityType", typeof(Employee));

                var tmp = new BlockingCollection<object[]>();

                var emps = context.JobPlacements.Where(i => i.ApplyDate <= end && (i.FireDate >= start || !i.FireDate.HasValue) && i.Job.DivisionId == divisionId).Select(i => i.Employee).Distinct().ToList();
                emps.ForEach(emp => emp.BoundCustomer.FullName.GetHashCode());
                emps.AsParallel().ForAll(emp =>
                {
                    Parallel.For(0, (int)((end.Date - start.Date).TotalDays), n =>
                    {
                        using(var innerContext = new TonusEntities())
                        {
                            var curr = start.Date.AddDays(n);
                            JobPlacement jp;
                            jp = innerContext.JobPlacements.Where(i => i.EmployeeId == emp.Id && i.ApplyDate <= curr && (i.FireDate >= curr || !i.FireDate.HasValue)).FirstOrDefault();
                            if(jp != null)
                            {
                                var curr1 = curr.AddDays(1);
                                List<EmployeeVisit> vis;
                                vis = innerContext.EmployeeVisits.Where(i => i.EmployeeId == emp.Id && i.CreatedOn >= curr && i.CreatedOn < curr1).OrderBy(i => i.CreatedOn).ToList();
                                var first = vis.FirstOrDefault(i => i.IsIncome);
                                if(first != null)
                                {
                                    /**/
                                    var beginDelta = (int)Math.Round(((first.CreatedOn - first.CreatedOn.Date) - jp.Job.WorkStart).TotalMinutes);
                                    /**/
                                    var totalOut = 0.0;
                                    for(int i = vis.IndexOf(first) + 2; i < vis.Count; i++)
                                    {
                                        if(!vis[i].IsIncome) continue;
                                        totalOut += (vis[i].CreatedOn - vis[i - 1].CreatedOn).TotalMinutes;
                                    }
                                    vis.Reverse();
                                    var last = vis.FirstOrDefault(i => !i.IsIncome);
                                    if(last != null)
                                    {
                                        /**/
                                        var endDelta = (int)Math.Round(((last.CreatedOn - last.CreatedOn.Date) - jp.Job.WorkEnd).TotalMinutes);
                                        tmp.Add(new object[]{emp.Id
                                        , emp.Number
                                        , curr
                                        , emp.BoundCustomer.FullName
                                        , jp.Job.Unit
                                        , jp.Job.Name
                                        , first.CreatedOn - first.CreatedOn.Date
                                        , beginDelta
                                        , last.CreatedOn - last.CreatedOn.Date
                                        , endDelta
                                        , totalOut
                                        , ((jp.Job.WorkEnd - jp.Job.WorkStart).TotalMinutes - (beginDelta - endDelta + totalOut)) / 60
                                        , -(beginDelta - endDelta + totalOut)/60});
                                    }
                                }

                            }
                        }
                    });
                });
                tmp.ToList().ForEach(i => res.Rows.Add(i));
                return res;
            }
        }

        public DataTable GetEmployeesAggregateVisitReport(DateTime start, DateTime end, Guid divisionId)
        {
            using(var context = new TonusEntities())
            {
                var r1 = new Dictionary<Guid, EmployeesAggregateVisitReportHelper>();

                var emps = context.JobPlacements.Where(i => i.ApplyDate <= end && (i.FireDate >= start || !i.FireDate.HasValue) && i.Job.DivisionId == divisionId).Select(i => i.Employee).Distinct().ToList();

                foreach(var emp in emps)
                {
                    var curr = start.Date;
                    while(curr <= end)
                    {
                        var jp = context.JobPlacements.Where(i => i.EmployeeId == emp.Id && i.ApplyDate <= curr && (i.FireDate >= curr || !i.FireDate.HasValue)).FirstOrDefault();
                        if(jp != null)
                        {
                            var curr1 = curr.AddDays(1);
                            var vis = context.EmployeeVisits.Where(i => i.EmployeeId == emp.Id && i.CreatedOn >= curr && i.CreatedOn < curr1).OrderBy(i => i.CreatedOn).ToList();
                            var first = vis.FirstOrDefault(i => i.IsIncome);
                            if(first != null)
                            {
                                /**/
                                var beginDelta = (int)Math.Round(((first.CreatedOn - first.CreatedOn.Date) - jp.Job.WorkStart).TotalMinutes);
                                /**/
                                var totalOut = 0.0;
                                for(int i = vis.IndexOf(first) + 2; i < vis.Count; i++)
                                {
                                    if(!vis[i].IsIncome) continue;
                                    totalOut += (vis[i].CreatedOn - vis[i - 1].CreatedOn).TotalMinutes;
                                }
                                vis.Reverse();
                                var last = vis.FirstOrDefault(i => !i.IsIncome);
                                /**/
                                if(last != null)
                                {
                                    var endDelta = (int)Math.Round(((last.CreatedOn - last.CreatedOn.Date) - jp.Job.WorkEnd).TotalMinutes);

                                    if(!r1.ContainsKey(emp.Id))
                                    {
                                        r1.Add(emp.Id, new EmployeesAggregateVisitReportHelper
                                        {
                                            Days = 0,
                                            Delta = 0,
                                            EmployeeId = emp.Id,
                                            Hours = 0,
                                            Job = jp.Job.Name,
                                            Unit = jp.Job.Unit,
                                            Name = emp.BoundCustomer.FullName
                                        });
                                    }

                                    r1[emp.Id].Days++;
                                    r1[emp.Id].Delta += beginDelta - endDelta + totalOut;
                                    r1[emp.Id].Hours += ((jp.Job.WorkEnd - jp.Job.WorkStart).TotalMinutes - (beginDelta - endDelta + totalOut)) / 60;
                                }
                            }
                        }
                        curr = curr.AddDays(1);
                    }
                }

                var res = new DataTable();
                res.Columns.Add("_Id", typeof(Guid));
                res.Columns.Add("ФИО", typeof(string));
                res.Columns.Add("Подразделение", typeof(string));
                res.Columns.Add("Должность", typeof(string));
                res.Columns.Add("Всего отработано дней", typeof(int));
                res.Columns.Add("Всего отработано часов", typeof(double));
                res.Columns.Add("Отклонение, мин.", typeof(int));
                res.ExtendedProperties.Add("EntityType", typeof(Employee));

                foreach(var v in r1.Values)
                {
                    res.Rows.Add(v.EmployeeId, v.Name, v.Unit, v.Job, v.Days, v.Hours, -(v.Delta) / 60);
                }

                return res;
            }
        }

        private void AddRow(DataTable res, DataRow dataRow)
        {
            var newRow = res.NewRow();
            for(int i = 0; i < dataRow.ItemArray.Length; i++)
            {
                newRow[i] = dataRow[i];
            }
            res.Rows.Add(newRow);
        }

        private IEnumerable<DateTime[]> GetWeeksInMonth(DateTime month)
        {
            var res = new List<DateTime[]>();
            for(int j = 0; j < DateTime.DaysInMonth(month.Year, month.Month); j += 1)
            {
                var week = GetWeek(month, month.AddMonths(1).AddDays(-1), month.AddDays(j));
                if(!res.Any(i => i[0] == week[0] && i[1] == week[1]))
                {
                    res.Add(week);
                }
            }
            return res;
        }

        private DateTime[] GetWeek(DateTime start, DateTime end, DateTime dateTime)
        {
            int weekDay = (int)dateTime.DayOfWeek - 1;
            if(weekDay < 0) weekDay += 7;
            return new DateTime[] { Extensions.Max(start, dateTime.AddDays(-weekDay)), Extensions.Min(end, dateTime.AddDays(-weekDay + 6)) };
        }

        private List<object> CalcValue<TSource>(IEnumerable<object> title,
            string subdetailsName,
            IEnumerable<TSource> source,
            DateTime start, DateTime end,
            Func<TSource, bool> selector,
            Func<TSource, DateTime?> dateFunc,
            Func<TSource, decimal?> sumFunc,
            bool daily, bool weekly, out bool addFlag,
            bool years, bool addMonths = true)
        {
            return CalcValue<TSource>(title, subdetailsName, source.Where(selector), start, end, dateFunc, daily, weekly, out addFlag, years, (i, _, __) => i.Sum(sumFunc), addMonths);
        }

        private List<object> CalcValue<TSource>(IEnumerable<object> title, string subdetailsName, IEnumerable<TSource> source, DateTime start, DateTime end, Func<TSource, DateTime?> dateFunc, bool daily, bool weekly, out bool addFlag, bool years, Func<IEnumerable<TSource>, DateTime, DateTime, decimal?> statFunc, bool addMonths = true)
        {
            var tinc = new List<object>();
            tinc.AddRange(title.ToArray());
            if(subdetailsName != null)
            {
                tinc.Add(subdetailsName);
            }
            addFlag = false;
            var startM = start.AddDays(-start.Day + 1);
            var i = startM;
            for(i = startM; i <= end; i = i.AddMonths(1))
            {
                if(addMonths)
                {
                    var i1 = i.AddMonths(1);
                    var q = System.Linq.Enumerable.Where(source, j => (dateFunc(j) >= i && dateFunc(j) < i1) || dateFunc(j) == null);
                    if(q.Count() > 0)
                    {
                        var sum = statFunc(q, i, i1);
                        tinc.Add(sum);
                        if(sum != 0)
                            addFlag = true;
                    }
                    else tinc.Add(0m);
                }
                if(daily)
                {
                    var ed = (end.Month == i.Month) ? end : i.AddMonths(1).AddDays(-1);

                    for(var i0 = (start.Month == i.Month) ? start : i; i0 <= ed; i0 = i0.AddDays(1))
                    {
                        var i1 = i0.AddDays(1);
                        var q = System.Linq.Enumerable.Where(source, j => (dateFunc(j) >= i0 && dateFunc(j) < i1) || dateFunc(j) == null);
                        if(q.Count() > 0)
                        {
                            var sum = statFunc(q, i0, i1);
                            tinc.Add(sum);
                            if(sum != 0)
                                addFlag = true;
                        }
                        else tinc.Add(0m);
                    }
                }
                if(weekly)
                {
                    foreach(var k in GetWeeksInMonth(i))
                    {
                        if(Core.DatesIntersectsEx(start, end, k[0], k[1]))
                        {
                            var q = System.Linq.Enumerable.Where(source, j => (dateFunc(j) >= k[0] && dateFunc(j) < k[1].AddDays(1)) || dateFunc(j) == null);
                            if(q.Count() > 0)
                            {
                                var sum = statFunc(q, k[0], k[1].AddDays(1));
                                tinc.Add(sum);
                                if(sum != 0)
                                    addFlag = true;
                            }
                            else tinc.Add(0m);
                        }
                    }
                }
                if(years && i.Month == 12 && addMonths)
                {
                    var i0 = new DateTime(i.Year, 1, 1);
                    var i1 = new DateTime(i.Year + 1, 1, 1);
                    var q = System.Linq.Enumerable.Where(source, j => (dateFunc(j) >= i0 && dateFunc(j) < i1) || dateFunc(j) == null);
                    if(q.Count() > 0)
                    {
                        var sum = statFunc(q, i0, i1);
                        tinc.Add(sum);
                        if(sum != 0)
                            addFlag = true;
                    }
                    else tinc.Add(0m);
                }

            }
            if(years && i.Month != 12 && addMonths)
            {
                var i0 = new DateTime(i.Year, 1, 1);
                var i1 = new DateTime(i.Year + 1, 1, 1);
                var q = System.Linq.Enumerable.Where(source, j => (dateFunc(j) >= i0 && dateFunc(j) < i1) || dateFunc(j) == null);
                if(q.Count() > 0)
                {
                    var sum = statFunc(q, i0, i1);
                    tinc.Add(sum);
                    if(sum != 0)
                        addFlag = true;
                }
                else tinc.Add(0m);
            }

            {
                var q = System.Linq.Enumerable.Where(source, j => (dateFunc(j) >= start && dateFunc(j) < end) || dateFunc(j) == null);
                if(q.Count() > 0)
                {
                    var sum = statFunc(q, start, end.AddMilliseconds(1));
                    tinc.Add(sum);
                    if(sum != 0)
                        addFlag = true;
                }
                else tinc.Add(0m);
            }
            return tinc;
        }

        public DataTable GetVisitsByTreatmentConfig(DateTime start, DateTime end, Guid? divisionId, bool daily, bool weekly, bool years)
        {
            end = end.AddDays(1).AddSeconds(-1);
            if(weekly && daily)
            {
                throw new Exception("Детализация одновременно по дням и по неделям невозможна!");
            }
            if(!divisionId.HasValue)
            {
                throw new Exception("Необходимо указать " + ClubTextR + "!");
            }
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);

                var res = new DataTable();
                res.ExtendedProperties.Add("Detailed", true);

                res.Columns.Add("Услуга", typeof(string));
                AddDatesColumns(res, start, end, daily, weekly, typeof(int), years);

                var src = context.TreatmentEvents.Where(i => i.DivisionId == divisionId.Value && (i.VisitStatus == 2 || i.VisitStatus == 3))
                    .Select(i => new { i.TreatmentConfigId, i.VisitDate }).ToArray();

                foreach(var tc in context.TreatmentConfigs.OrderBy(i => i.Name))
                {
                    bool add;
                    var row = CalcValue(new object[0], tc.Name,
                        src.Where(i => i.TreatmentConfigId == tc.Id),
                        start, end, i => i.VisitDate, daily, weekly, out add, years, (i, _, __) => i.Count()).ToArray();
                    if(add)
                    {
                        res.Rows.Add(row);
                    }
                }
                return res;
            }
        }

        public DataTable GetAvgAge(DateTime start, DateTime end, Guid? divisionId, bool daily, bool weekly, bool years)
        {
            end = end.AddDays(1).AddSeconds(-1);
            if(weekly && daily)
            {
                throw new Exception("Детализация одновременно по дням и по неделям невозможна!");
            }
            if(!divisionId.HasValue)
            {
                throw new Exception("Необходимо указать " + ClubTextR + "!");
            }
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);

                var res = new DataTable();
                //res.ExtendedProperties.Add("Detailed", true);

                res.Columns.Add("Возраст", typeof(string));
                AddDatesColumns(res, start, end, daily, weekly, typeof(decimal), years);

                for(int a = 10; a < 90; a++)
                {
                    bool add;
                    var row = CalcValue(new object[0], a.ToString(),
                        context.CustomerVisits.Where(i => i.DivisionId == divisionId.Value && i.Customer.Birthday.HasValue).ToList().Where(i => (int)((i.InTime - i.Customer.Birthday.Value).TotalDays / 365) == a),
                        start, end, i => i.InTime, daily, weekly, out add, years, (x, _, __) => x.Select(y => y.CustomerId).Distinct().Count()).ToArray();
                    if(add)
                    {
                        res.Rows.Add(row);
                    }
                }

                var r = new List<object>();
                r.Add("Средний возраст");
                for(int i = 1; i < res.Columns.Count; i++)
                {
                    var sum = 0;
                    var age = 0;
                    for(int j = 0; j < res.Rows.Count; j++)
                    {
                        age += Int32.Parse((string)res.Rows[j][0]) * (int)(decimal)res.Rows[j][i];
                        sum += (int)(decimal)res.Rows[j][i];
                    }
                    if(sum > 0)
                    {
                        r.Add((decimal)age / (decimal)sum);
                    }
                    else
                    {
                        r.Add(DBNull.Value);
                    }
                }

                res.Rows.Add(r.ToArray());
                return res;
            }
        }

        public DataTable GetReklama(DateTime start, DateTime end, Guid? divisionId, bool daily, bool weekly, bool years, bool groups)
        {
            end = end.AddDays(1).AddSeconds(-1);
            if(weekly && daily)
            {
                throw new Exception("Детализация одновременно по дням и по неделям невозможна!");
            }
            if(!divisionId.HasValue)
            {
                throw new Exception("Необходимо указать " + ClubTextR + "!");
            }
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);

                var res = new DataTable();
                res.ExtendedProperties.Add("VerticalColor2", true);
                if(!groups)
                {
                    res.Columns.Add("Рекламная группа", typeof(string));
                }
                res.Columns.Add("Источник рекламы", typeof(string));

                DataTable t1 = new DataTable(), t2 = new DataTable();
                AddDatesColumns(t1, start, end, daily, weekly, typeof(int), years);
                AddDatesColumns(t2, start, end, daily, weekly, typeof(int), years);
                for(int i = 0; i < t1.Columns.Count; i++)
                {
                    res.Columns.Add("Звонили " + t1.Columns[i].ColumnName, typeof(int));
                    res.Columns.Add("Пришли " + t2.Columns[i].ColumnName, typeof(int));
                }
                if(!groups)
                {
                    var src = context.Calls.Where(i =>
                            i.DivisionId == divisionId.Value
                            && i.CustomerId.HasValue
                            && i.Customer.AdvertTypeId.HasValue
                            && i.Customer.Calls.OrderBy(j => j.StartAt).FirstOrDefault() == i)
                            .Where(i => i.StartAt >= start && i.StartAt < end)
                            .Select(i => new { i.StartAt, i.Customer.AdvertTypeId, i.CustomerId })
                            .ToArray();
                    var src2 = context.CustomerCards.Where(i =>
                            i.DivisionId == divisionId.Value
                            && i.Customer.AdvertTypeId.HasValue
                            && i.Customer.CustomerCards.OrderBy(j => j.EmitDate).FirstOrDefault() == i)
                            .Where(i => i.EmitDate >= start && i.EmitDate < end)
                            .Select(i => new { i.EmitDate, i.Customer.AdvertTypeId, i.CustomerId })
                            .ToArray();
                    foreach(var adv in context.AdvertTypes.OrderBy(i => i.Name).ToArray())
                    {
                        bool add1, add2;
                        var row1 = CalcValue(new object[0], null, src.Where(i => i.AdvertTypeId == adv.Id).ToArray(),
                            start, end, i => i.StartAt, daily, weekly, out add1, years, (x, _, __) => x.Select(y => y.CustomerId).Distinct().Count()).ToArray();
                        var row2 = CalcValue(new object[0], null, src2.Where(i => i.AdvertTypeId == adv.Id).ToArray(),
                            start, end, i => i.EmitDate, daily, weekly, out add2, years, (x, _, __) => x.Select(y => y.CustomerId).Distinct().Count()).ToArray();
                        if(add1 || add2)
                        {
                            var row = res.Rows.Add(adv.AdvertGroup.Name, adv.Name);
                            for(int i = 0; i < row1.Length; i++)
                            {
                                row[i * 2 + 2] = row1[i];
                                row[i * 2 + 3] = row2[i];
                            }
                        }
                    }
                }
                else
                {
                    var src = context.Calls.Where(i =>
                            i.DivisionId == divisionId.Value
                            && i.CustomerId.HasValue
                            && i.Customer.AdvertTypeId.HasValue
                            && i.Customer.Calls.OrderBy(j => j.StartAt).FirstOrDefault() == i)
                            .Where(i => i.StartAt >= start && i.StartAt < end)
                            .Select(i => new { i.StartAt, i.Customer.AdvertType.AdvertGroupId, i.CustomerId })
                            .ToArray();
                    var src2 = context.CustomerCards.Where(i =>
                            i.DivisionId == divisionId.Value
                            && i.Customer.AdvertTypeId.HasValue
                            && i.Customer.CustomerCards.OrderBy(j => j.EmitDate).FirstOrDefault() == i)
                            .Where(i => i.EmitDate >= start && i.EmitDate < end)
                            .Select(i => new { i.EmitDate, i.Customer.AdvertType.AdvertGroupId, i.CustomerId })
                            .ToArray();
                    foreach(var ag in context.AdvertGroups.OrderBy(i => i.Name))
                    {
                        bool add1, add2;
                        var row1 = CalcValue(new object[0], null, src.Where(i => i.AdvertGroupId == ag.Id).ToArray(),
                            start, end, i => i.StartAt, daily, weekly, out add1, years, (x, _, __) => x.Select(y => y.CustomerId).Distinct().Count()).ToArray();
                        var row2 = CalcValue(new object[0], null, src2.Where(i => i.AdvertGroupId == ag.Id).ToArray(),
                            start, end, i => i.EmitDate, daily, weekly, out add2, years, (x, _, __) => x.Select(y => y.CustomerId).Distinct().Count()).ToArray();
                        if(add1 || add2)
                        {
                            var row = res.Rows.Add(ag.Name);
                            for(int i = 0; i < row1.Length; i++)
                            {
                                row[i * 2 + 1] = row1[i];
                                row[i * 2 + 2] = row2[i];
                            }
                        }
                    }
                }
                return res;
            }
        }

        #region clubload

        public DataTable GetClubLoad(DateTime start, DateTime end, Guid? divisionId, bool daily, bool weekly, bool years)
        {
            end = end.AddDays(1).AddSeconds(-1);

            if(weekly && daily)
            {
                throw new Exception("Детализация одновременно по дням и по неделям невозможна!");
            }
            if(!divisionId.HasValue)
            {
                throw new Exception("Необходимо указать " + ClubTextR + "!");
            }
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);

                var res = new DataTable();
                res.ExtendedProperties.Add("Detailed", true);

                res.Columns.Add("Параметр", typeof(string));
                AddDatesColumns(res, start, end, daily, weekly, typeof(decimal), years);

                var src = context.Treatments.Where(i => i.DivisionId == divisionId && i.IsActive).ToArray();

                bool add;
                res.Rows.Add(CalcValue(new object[0],
                    "100% загрузка единицы",
                    src,
                    start, end, i => null, daily, weekly, out add, years,
                    (x, st, fin) => GetMaxLoadEd(x, st, fin)).ToArray());
                res.Rows.Add(CalcValue(new object[0],
                    "100% загрузка посещения",
                    src,
                    start, end, i => null, daily, weekly, out add, years,
                    (x, st, fin) => GetMaxLoadVis(x, st, fin)).ToArray());

                var src2 = context.UnitCharges
                    .Where(i => (i.Date >= start && i.Date < end))
                    .Join(context.TreatmentEvents, i => i.EventId, i => i.Id, (i, j) => new { i.Date, i.Charge, j.DivisionId })
                    .Where(i => i.DivisionId == divisionId)
                    .Select(i => new { i.Date, i.Charge })
                    .ToArray();

                res.Rows.Add(CalcValue(new object[0],
                    "Единиц по факту",
                    src2,
                    start, end, i => i.Date, daily, weekly, out add, years,
                    (x, st, fin) => x.Sum(i => i.Charge)).ToArray());
                var row = new List<object>();
                row.Add("% от полной загрузки, ед.");
                for(int i = 1; i < res.Columns.Count; i++)
                {
                    if((decimal)res.Rows[0][i] == 0)
                    {
                        row.Add(0);
                    }
                    else
                    {
                        row.Add((decimal)(res.Rows[2][i]) / (decimal)(res.Rows[0][i]) * 100);
                    }
                }
                res.Rows.Add(row.ToArray());
                var src3 = context.CustomerVisits.Where(i => i.DivisionId == divisionId && (i.InTime >= start && i.InTime < end)).ToArray();

                res.Rows.Add(CalcValue(new object[0],
                    "Число приходов клиентов по факту",
                    src3,
                    start, end, i => i.InTime, daily, weekly, out add, years,
                    (x, st, fin) => x.Count()).ToArray());

                res.Rows.Add(CalcValue(new object[0],
                    "Уникальных посетителей",
                    src3,
                    start, end, i => i.InTime, daily, weekly, out add, years,
                    (x, st, fin) => x.Select(i => i.CustomerId).Distinct().Count()).ToArray());

                row.Clear();
                row.Add("% от полной загрузки, посещений");
                for(int i = 1; i < res.Columns.Count; i++)
                {
                    if((decimal)res.Rows[1][i] == 0)
                    {
                        row.Add(0);
                    }
                    else
                    {
                        row.Add((decimal)(res.Rows[4][i]) / (decimal)(res.Rows[1][i]) * 100);
                    }
                }
                res.Rows.Add(row.ToArray());

                var visitsSrc = src3.Select(i => i.InTime).ToArray();

                res.Rows.Add(CalcValue(new object[0],
                    "Средний чек по единицам",
                    visitsSrc,
                    start, end, i => i, daily, weekly, out add, years,
                    (_, st, fin) => GetAvgBill(visitsSrc, src2, st, fin, context, divisionId.Value)).ToArray());

                res.Rows.Add(CalcValue(new object[0],
                    "В среднем единиц в день",
                    src2,
                    start, end, i => i.Date, daily, weekly, out add, years,
                    (x, st, fin) => x.Sum(i => (int?)i.Charge) / (decimal)Math.Ceiling((fin - st).TotalDays)).ToArray());

                return res;
            }
        }

        private decimal GetAvgBill(IEnumerable<DateTime> visitsSrc, IEnumerable<object> totalUnitsSrc, DateTime st, DateTime fin, TonusEntities context, Guid divisionId)
        {
            var visits = visitsSrc.Count(i => i >= st && i < fin);
            var totalUnits = totalUnitsSrc.Cast<dynamic>().Where(i => i.Date >= st && i.Date < fin).Sum(x => (int?)x.Charge) ?? 0;
            if(visits == 0) return 0;
            return (decimal)totalUnits / (decimal)visits;
        }

        private decimal? GetMaxLoadVis(IEnumerable<Treatment> x, DateTime st, DateTime fin)
        {
            var res = 0;

            foreach(var tr in x)
            {
                if(!tr.Division.OpenTime.HasValue || !tr.Division.CloseTime.HasValue)
                {
                    throw new Exception("У " + ClubTextV + " не указано время открытия/закрытия!");
                }
                var tc = tr.TreatmentType.TreatmentConfigs.OrderBy(i => i.FullDuration).FirstOrDefault();
                if(tc == null) continue;
                res += (int)((tr.Division.CloseTime.Value - tr.Division.OpenTime.Value).TotalMinutes / tc.FullDuration) * (int)Math.Ceiling((fin - st).TotalDays) * tr.MaxCustomers;
            }
            return res;
            ;
        }

        private decimal? GetMaxLoadEd(IEnumerable<Treatment> x, DateTime st, DateTime fin)
        {
            var res = 0;
            foreach(var tr in x)
            {
                if(!tr.Division.OpenTime.HasValue || !tr.Division.CloseTime.HasValue)
                {
                    throw new Exception("У " + ClubTextV + " не указано время открытия/закрытия!");
                }
                var tc = tr.TreatmentType.TreatmentConfigs.OrderByDescending(i => i.Price).FirstOrDefault();
                if(tc == null) continue;
                res += (int)((tr.Division.CloseTime.Value - tr.Division.OpenTime.Value).TotalMinutes / tc.FullDuration) * (int)Math.Ceiling((fin - st).TotalDays) * (int)tc.Price * tr.MaxCustomers;
            }
            return res;
        }

        #endregion

        #region inventary
        public DataTable Inventary(DateTime start, DateTime end, Guid? divisionId, bool daily, bool weekly, bool years)
        {
            end = end.AddDays(1).AddSeconds(-1);

            if(!divisionId.HasValue)
            {
                throw new Exception("Необходимо указать " + ClubTextR + "!");
            }
            using(var context = new TonusEntities())
            {
                var div = context.Divisions.Single(i => i.Id == divisionId.Value);
                var res = new DataTable();
                res.Columns.Add("_id", typeof(Guid));
                res.Columns.Add("Товар", typeof(string));
                res.Columns.Add("Склад", typeof(string));
                res.ExtendedProperties.Add("Inventary", true);
                res.ExtendedProperties.Add("EntityType", typeof(Good));
                var stores = context.Storehouses.Where(i => i.DivisionId == divisionId.Value && i.IsActive).OrderBy(i => i.Name).ToList();
                var rs = new DataTable();
                AddDatesColumns(rs, start, end, daily, weekly, typeof(int), years);

                for(int i = 0; i < rs.Columns.Count; i++)
                {
                    res.Columns.Add("Остаток на начало " + rs.Columns[i].ColumnName);
                    res.Columns.Add("Приход " + rs.Columns[i].ColumnName);
                    res.Columns.Add("Расход " + rs.Columns[i].ColumnName);
                    res.Columns.Add("Списание " + rs.Columns[i].ColumnName);
#if BEAUTINIKA
                    res.Columns.Add("Из них в расход " + rs.Columns[i].ColumnName);
#endif
                    res.Columns.Add("Остаток на конец " + rs.Columns[i].ColumnName);
                }

                foreach(var store in stores)
                {
                    foreach(var good in context.Goods
                        .Where(i => i.CompanyId == div.CompanyId && i.IsVisible))
                    {
                        var row = new List<object> { good.Id, good.Name, store.Name };
                        bool add;
                        var r0 = CalcValue<object>(new object[0], null, new object[1],
                            start, end, i => null, daily, weekly, out add, years, (_, st, __) => GetGoodLeftForDate(context, st, div, good, store)).ToArray();
                        var r1 = CalcValue<object>(new object[0], null, new object[1],
                            start, end, i => null, daily, weekly, out add, years, (_, st, fin) => GetGoodIncome(context, st, fin, div, good, store)).ToArray();
                        var r2 = CalcValue<object>(new object[0], null, new object[1],
                            start, end, i => null, daily, weekly, out add, years, (_, st, fin) => GetGoodOutcome(context, st, fin, div, good, store)).ToArray();
                        var r3 = CalcValue<object>(new object[0], null, new object[1],
                            start, end, i => null, daily, weekly, out add, years, (_, st, fin) => GetGoodSpis(context, st, fin, div, good, store)).ToArray();
#if BEAUTINIKA
                        var r4 = CalcValue<object>(new object[0], null, new object[1],
                            start, end, i => null, daily, weekly, out add, years, (_, st, fin) => GetGoodSpisWriteoff(context, st, fin, div, good, store)).ToArray();
#endif
                        var r5 = CalcValue<object>(new object[0], null, new object[1],
                            start, end, i => null, daily, weekly, out add, years, (_, __, fin) => GetGoodLeftForDate(context, fin, div, good, store)).ToArray();

                        for(int i = 0; i < r0.Length; i++)
                        {
                            row.AddRange(new object[] { r0[i], r1[i], r2[i], r3[i],
#if BEAUTINIKA
                                r4[i], 
#endif
                                r5[i] });
                        }

                        res.Rows.Add(row.ToArray());
                    }
                }

                return res;
            }
        }

#if BEAUTINIKA
        private decimal? GetGoodSpisWriteoff(TonusEntities context, DateTime st, DateTime fin, Division div, Good good, Storehouse store)
        {
            var outcome = 0;
            var outcomeList1 = good.ConsignmentLines.Where(l => l.Consignment.Date < fin && l.Consignment.Date >= st && l.Consignment.SourceStorehouseId == store.Id && l.Consignment.IsAsset && l.Consignment.DocType == 2 && l.Consignment.IsMaterialWriteoff);
            outcome = (int)(outcomeList1.Any() ? outcomeList1.Sum(i => i.Quantity ?? 0) : 0);
            return outcome;
        }
#endif

        private decimal? GetGoodSpis(TonusEntities context, DateTime st, DateTime fin, Division div, Good good, Storehouse store)
        {
            var outcome = 0;
            var outcomeList1 = good.ConsignmentLines.Where(l => l.Consignment.Date < fin && l.Consignment.Date >= st && l.Consignment.SourceStorehouseId == store.Id && l.Consignment.IsAsset && l.Consignment.DocType == 2);
            outcome = (int)(outcomeList1.Any() ? outcomeList1.Sum(i => i.Quantity ?? 0) : 0);
            return outcome;
        }

        private decimal? GetGoodOutcome(TonusEntities context, DateTime st, DateTime fin, Division div, Good good, Storehouse store)
        {
            var outcome = 0;
            var outcomeList1 = good.ConsignmentLines.Where(l => l.Consignment.Date >= st && l.Consignment.Date < fin && l.Consignment.SourceStorehouseId == store.Id && l.Consignment.IsAsset && l.Consignment.DocType == 1);
            var outcomeList2 = good.GoodSales.Where(s => s.BarOrder.PurchaseDate >= st && s.BarOrder.PurchaseDate < fin && s.StorehouseId == store.Id && !s.ReturnById.HasValue);
            var outcomeList3 = good.Rents.Where(r => (!r.FactReturnDate.HasValue || r.LostFine.HasValue) && r.StorehouseId == store.Id && r.CreatedOn >= st && r.CreatedOn < fin);
            var outcomeList4 = good.CustomerGoodsFlows.Where(i => i.Amount > 0 && i.CreatedOn >= st && i.CreatedOn < fin && i.StorehouseId == store.Id);
            var am2 = outcomeList2.Count() > 0 ? outcomeList2.Sum(i => i.Amount) : 0;
            outcome = (int)((outcomeList1.Count() > 0 ? outcomeList1.Sum(i => i.Quantity ?? 0) : 0) +
                am2 +
                outcomeList3.Count() +
                (outcomeList4.Sum(i => (int?)i.Amount) ?? 0));
            return outcome;
        }

        private decimal? GetGoodIncome(TonusEntities context, DateTime st, DateTime fin, Division div, Good good, Storehouse store)
        {
            var incomeList = good.ConsignmentLines.Where(l => l.Consignment.Date >= st && l.Consignment.Date < fin && l.Consignment.DestinationStorehouseId == store.Id && l.Consignment.IsAsset && (l.Consignment.DocType == 0 || l.Consignment.DocType == 1));
            if(incomeList.Any())
            {
                return (int)incomeList.Sum(i => i.Quantity ?? 0);
            }
            return 0;
        }

        private decimal GetGoodLeftForDate(TonusEntities context, DateTime date, Division div, Good good, Storehouse store)
        {
            var income = 0;
            var incomeList = good.ConsignmentLines.Where(l => l.Consignment.Date < date && l.Consignment.DestinationStorehouseId == store.Id && l.Consignment.IsAsset && (l.Consignment.DocType == 0 || l.Consignment.DocType == 1));
            if(incomeList.Any())
            {
                income = (int)incomeList.Sum(i => i.Quantity ?? 0);
            }
            var outcome = 0;
            var outcomeList1 = good.ConsignmentLines.Where(l => l.Consignment.Date < date && l.Consignment.SourceStorehouseId == store.Id && l.Consignment.IsAsset && (l.Consignment.DocType == 1 || l.Consignment.DocType == 2));
            var outcomeList2 = good.GoodSales.Where(s => s.BarOrder.PurchaseDate < date && s.StorehouseId == store.Id && !s.ReturnById.HasValue);
            var outcomeList3 = good.Rents.Where(r => (!r.FactReturnDate.HasValue || r.LostFine.HasValue) && r.StorehouseId == store.Id && r.CreatedOn < date);
            var outcomeList4 = good.CustomerGoodsFlows.Where(i => i.Amount > 0 && i.CreatedOn < date && i.StorehouseId == store.Id);
            var am2 = outcomeList2.Count() > 0 ? outcomeList2.Sum(i => i.Amount) : 0;
            outcome = (int)((outcomeList1.Count() > 0 ? outcomeList1.Sum(i => i.Quantity ?? 0) : 0) +
                am2 + outcomeList3.Count()) +
                (outcomeList4.Sum(i => (int?)i.Amount) ?? 0);
            return income - outcome;
        }
        #endregion

        public DataTable GetGoodDetails(Guid? divisionId, Guid? goodId)
        {
            if(!divisionId.HasValue)
            {
                throw new Exception("Необходимо указать " + ClubTextR + "!");
            }
            if(!goodId.HasValue)
            {
                throw new Exception("Необходимо указать товар!");
            }

            using(var context = new TonusEntities())
            {
                var res = new DataTable();
                res.Columns.Add("Склад", typeof(string));
                res.Columns.Add("Дата", typeof(DateTime));
                res.Columns.Add("Операция", typeof(string));
                res.Columns.Add("Приход", typeof(int));
                res.Columns.Add("Расход", typeof(int));
                res.Columns.Add("Остаток по клубу", typeof(int));
                res.Columns.Add("Документ", typeof(string));
                res.Columns.Add("Автор", typeof(string));

                var div = context.Divisions.Single(i => i.Id == divisionId.Value);
                var good = context.Goods.Where(i => i.Id == goodId);
                foreach(var store in div.Storehouses)
                {
                    foreach(var line in context.ConsignmentLines.Where(i => i.GoodId == goodId && i.Consignment.IsAsset && i.Consignment.DestinationStorehouseId == store.Id && i.Consignment.DocType == 0))
                    {
                        //Приход
                        res.Rows.Add(store.Name, line.Consignment.Date, "Приход", line.Quantity, 0, 0, line.Consignment.Number, line.Consignment.CreatedBy.FullName);
                    }
                    foreach(var line in context.ConsignmentLines.Where(i => i.GoodId == goodId && i.Consignment.IsAsset && i.Consignment.DestinationStorehouseId == store.Id && i.Consignment.DocType == 1))
                    {
                        //Перемещение на склад
                        res.Rows.Add(store.Name, line.Consignment.Date, "Перемещение на склад", line.Quantity, 0, 0, line.Consignment.Number, line.Consignment.CreatedBy.FullName);
                    }
                    foreach(var line in context.ConsignmentLines.Where(i => i.GoodId == goodId && i.Consignment.IsAsset && i.Consignment.SourceStorehouseId == store.Id && i.Consignment.DocType == 1))
                    {
                        //Перемещение со склада
                        res.Rows.Add(store.Name, line.Consignment.Date, "Перемещение со склада", 0, line.Quantity, 0, line.Consignment.Number, line.Consignment.CreatedBy.FullName);
                    }
                    foreach(var line in context.ConsignmentLines.Where(i => i.GoodId == goodId && i.Consignment.IsAsset && i.Consignment.SourceStorehouseId == store.Id && i.Consignment.DocType == 2))
                    {
                        //Списание
                        res.Rows.Add(store.Name, line.Consignment.Date, "Списание", 0, line.Quantity, 0, line.Consignment.Number, line.Consignment.CreatedBy.FullName);
                    }
                    foreach(var gs in context.GoodSales.Where(i => i.BarOrder.DivisionId == divisionId && i.StorehouseId == store.Id && i.GoodId == goodId))
                    {
                        //Продажа
                        res.Rows.Add(store.Name, gs.BarOrder.PurchaseDate, "Продажа", 0, gs.Amount, 0, gs.BarOrder.OrderNumber, gs.BarOrder.CreatedBy.FullName);
                    }
                    foreach(var re in context.Rents.Where(i => i.StorehouseId == store.Id && i.GoodId == goodId))
                    {
                        //Выдача в аренду
                        res.Rows.Add(store.Name, re.CreatedOn, "Выдача в аренду", 0, 1, 0, null, re.CreatedBy.FullName);
                        if(re.FactReturnDate.HasValue)
                        {
                            //Возврат из аренды
                            res.Rows.Add(store.Name, re.FactReturnDate.Value, "Возврат из аренды", 1, 0, 0, null, re.ReturnBy.FullName);
                        }
                    }
                }
                res.DefaultView.Sort = "Дата asc";
                res = res.DefaultView.ToTable();
                if(res.Rows.Count > 0)
                {
                    res.Rows[0][5] = (int)res.Rows[0][3] - (int)res.Rows[0][4];
                    for(int i = 1; i < res.Rows.Count; i++)
                    {
                        res.Rows[i][5] = (int)res.Rows[i - 1][5] + (int)res.Rows[i][3] - (int)res.Rows[i][4];
                    }
                }
                return res;
            }
        }

        public DataTable WeeklyReport(DateTime date, Guid? divisionId, bool days)
        {
            if(!divisionId.HasValue)
            {
                throw new Exception("Необходимо указать " + ClubTextR + "!");
            }

            var dates = GetWeek(DateTime.MinValue, DateTime.MaxValue, date);
            var d0 = dates[0];
            var d1 = dates[1].AddDays(1);

            using(var context = new TonusEntities())
            {

                var res = new DataTable();
                res.Columns.Add("Дата", typeof(DateTime));
                res.Columns.Add("Событие", typeof(string));
                res.Columns.Add("Сумма/количество", typeof(int));
                res.Columns.Add("Комментарий", typeof(string));
                if(days)
                {
                    for(int i = 0; i < 7; i++)
                    {
                        res.Columns.Add(d0.AddDays(i).ToString("d.MM"), typeof(int));
                    }
                }

                foreach(var t in context.Tickets.Where(i => i.CreatedOn >= d0 && i.CreatedOn < d1 && i.DivisionId == divisionId).OrderBy(i => i.CreatedOn))
                {
                    var pmts = 0m;
                    var ps = t.TicketPayments.Where(p => p.PaymentDate >= d0 && p.PaymentDate < d1);
                    if(ps.Any())
                    {
                        pmts = ps.Sum(i => i.Amount);
                    }
                    res.Rows.Add(
                        t.CreatedOn,
                        String.Format("Продажа {0}, скидка {1:p}, номер {2}", t.TicketType.Name, t.DiscountPercent, t.Number),
                        pmts,
                        t.Customer.Tickets.Count > 1 ? "Старый клиент" : (t.Customer.AdvertTypeId.HasValue ? t.Customer.AdvertType.Name : "Канал рекламы не указан")
                        + (t.InstalmentId.HasValue ? "; рассрочка" : "")
                        );
                }


                var tpmts = 0m;
                var tps = context.TicketPayments.Where(p => p.PaymentDate >= d0 && p.PaymentDate < d1 && p.Ticket.DivisionId == divisionId);
                if(tps.Any()) tpmts = tps.Sum(i => i.Amount);

                bool addFlag;

                res.Rows.Add(new object[] { DBNull.Value, "Итого по абонементам, включая старые рассрочки", tpmts, "" }.Concat(CalcValue(new object[0], null,
                    context.TicketPayments.Where(p => p.Ticket.DivisionId == divisionId),
                    d0,
                    d1,
                    i => i.PaymentDate,
                    true,
                    false, out addFlag, false, (_t, _, __) => _t.Sum(i => i.Amount), false).Skip(1).Take(days ? 7 : 0).ToArray()).ToArray());

                //var spmts = 0m;
                var sps = context.SolariumVisits.Where(p => p.VisitDate >= d0 && p.VisitDate < d1 && p.DivisionId == divisionId && p.Cost.HasValue);
                //if (sps.Any()) spmts = sps.Sum(i => i.Amount);

                //res.Rows.Add(new object[] { DBNull.Value, "Итого по солярию минут (без абонементов)", spmts, "" }.Concat(CalcValue(new object[0], null,
                //    context.SolariumVisits.Where(p => p.DivisionId == divisionId && p.Cost.HasValue),
                //    d0,
                //    d1,
                //    i => i.VisitDate,
                //    true,
                //    false, out addFlag, false, (_t, _, __) => _t.Sum(i => i.Amount)).Skip(1).Take(7).ToArray()).ToArray());


                var spmtsm = 0m;
                if(sps.Any()) spmtsm = sps.Sum(i => i.Cost ?? 0);
                res.Rows.Add(new object[] { DBNull.Value, "Итого по солярию выручка (без абонементов)", spmtsm, "" }.Concat(CalcValue(new object[0], null,
                    context.SolariumVisits.Where(p => p.DivisionId == divisionId && p.Cost.HasValue),
                    d0,
                    d1,
                    i => i.VisitDate,
                    true,
                    false, out addFlag, false, (_t, _, __) => _t.Sum(i => i.Cost), false).Skip(1).Take(days ? 7 : 0).ToArray()).ToArray());



                var bpmts = 0m;
                var bps = context.GoodSales.Where(p => p.BarOrder.PurchaseDate >= d0 && p.BarOrder.PurchaseDate < d1 && p.BarOrder.DivisionId == divisionId);
                if(bps.Any()) bpmts = bps.ToList().Sum(i => i.Cost);
                res.Rows.Add(new object[] { DBNull.Value, "Итого по бару", bpmts, "" }.Concat(CalcValue(new object[0], null,
                    context.GoodSales.Where(p => p.BarOrder.DivisionId == divisionId),
                    d0,
                    d1,
                    i => i.BarOrder.PurchaseDate,
                    true,
                    false, out addFlag, false, (_t, _, __) => _t.Sum(i => i.Cost), false).Skip(1).Take(days ? 7 : 0).ToArray()).ToArray());


                var row = new object[] { DBNull.Value, "Итого выручка за неделю", tpmts + spmtsm + bpmts, "" }.ToList();
                for(int i = 0; i < (days ? 7 : 0); i++)
                {
                    var n = 0m;
                    for(int j = 0; j < res.Rows.Count; j++)
                    {
                        if(res.Rows[j][i + 4] != DBNull.Value && res.Rows[j][i + 4] != null)
                        {
                            n += (int)res.Rows[j][i + 4];
                        }
                    }
                    row.Add(n);
                }

                res.Rows.Add(row.ToArray());

                foreach(var adv in context.AdvertTypes.Where(i => i.IsAvail))
                {
                    var calls = context.Calls.Where(i =>
                        i.DivisionId == divisionId.Value
                        && i.CustomerId.HasValue
                        && i.Customer.AdvertTypeId.HasValue
                        && i.Customer.AdvertTypeId == adv.Id
                        && i.Customer.Calls.OrderBy(j => j.StartAt).FirstOrDefault() == i
                        && i.StartAt >= d0 && i.StartAt < d1
                        );
                    if(calls.Any())
                    {
                        res.Rows.Add(new object[] { DBNull.Value, "Звонки - " + adv.Name, calls.Count(), "Рекламный канал: " + adv.AdvertGroup.Name }.Concat(CalcValue(new object[0], null,
                            context.Calls.Where(i =>
                                i.DivisionId == divisionId.Value
                                && i.CustomerId.HasValue
                                && i.Customer.AdvertTypeId.HasValue
                                && i.Customer.AdvertTypeId == adv.Id
                                && i.Customer.Calls.OrderBy(j => j.StartAt).FirstOrDefault() == i),
                            d0,
                            d1,
                            i => i.StartAt,
                            true,
                            false, out addFlag, false, (_t, _, __) => _t.Count()).Skip(1).Take(days ? 7 : 0).ToArray()).ToArray());
                    }
                }
                foreach(var adv in context.AdvertTypes.Where(i => i.IsAvail))
                {
                    var cards = context.CustomerCards.Where(i =>
                        i.DivisionId == divisionId.Value
                        && i.Customer.AdvertTypeId.HasValue
                        && i.Customer.AdvertTypeId == adv.Id
                        && i.Customer.CustomerCards.OrderBy(j => j.EmitDate).FirstOrDefault() == i
                        && i.EmitDate >= d0 && i.EmitDate < d1
                        );
                    if(cards.Any())
                    {
                        res.Rows.Add(new object[] { DBNull.Value, "Посещения - " + adv.Name, cards.Count(), "Рекламный канал: " + adv.AdvertGroup.Name }.Concat(CalcValue(new object[0], null,
                            context.CustomerCards.Where(i =>
                                i.DivisionId == divisionId.Value
                                && i.Customer.AdvertTypeId.HasValue
                                && i.Customer.AdvertTypeId == adv.Id
                                && i.Customer.CustomerCards.OrderBy(j => j.EmitDate).FirstOrDefault() == i),
                            d0,
                            d1,
                            i => i.EmitDate,
                            true,
                            false, out addFlag, false, (_t, _, __) => _t.Count()).Skip(1).Take(days ? 7 : 0).ToArray()).ToArray());
                    }

                }

                return res;
            }
        }

        public DataTable TicketSales(DateTime start, DateTime end, Guid? divisionId, bool daily, bool weekly, bool years, bool isEd)
        {
            end = end.AddDays(1).AddSeconds(-1);
            if(weekly && daily)
            {
                throw new Exception("Детализация одновременно по дням и по неделям невозможна!");
            }
            if(!divisionId.HasValue)
            {
                throw new Exception("Необходимо указать " + ClubTextR + "!");
            }
            using(var context = new TonusEntities())
            {
                var res = new DataTable();
                res.ExtendedProperties.Add("Detailed", true);
                res.Columns.Add("Тип абонемента", typeof(string));
                AddDatesColumns(res, start, end, daily, weekly, typeof(int), years);
                bool add;

                var src = context.Tickets.Where(i => i.DivisionId == divisionId.Value)
                    .Where(i => i.TicketPayments.Any() && i.TicketPayments.Min(j => j.PaymentDate) >= start && i.TicketPayments.Min(j => j.PaymentDate) < end)
                    .Where(i => !i.ReturnDate.HasValue)
                    .Select(i => new { i.TicketTypeId, i.CreatedOn, i.TicketType.Units }).ToArray();

                if(!isEd)
                {
                    foreach(var t in context.TicketTypes.OrderBy(i => i.Name).ToArray())
                    {
                        var row = CalcValue(new object[0], t.Name, src.Where(i => i.TicketTypeId == t.Id).ToArray(), start, end, i => i.CreatedOn, daily, weekly, out add, years, (i, _, __) => i.Count());
                        if(add) res.Rows.Add(row.ToArray());
                    }
                }
                else
                {
                    foreach(var t in context.TicketTypes.Select(i => i.Units).OrderBy(i => i).Distinct().ToArray())
                    {
                        var row = CalcValue(new object[0], String.Format("{0:n0} ед.", t), src.Where(i => i.Units == t).ToArray(), start, end, i => i.CreatedOn, daily, weekly, out add, years, (i, _, __) => i.Count());
                        if(add) res.Rows.Add(row.ToArray());
                    }
                }


                res.Rows.Add(CalcValue(new object[0], "Итого", src, start, end, i => i.CreatedOn, daily, weekly, out add, years, (i, _, __) => i.Count()).ToArray());

                var src2 = context.Tickets.Where(i => i.DivisionId == divisionId.Value && !i.ReturnDate.HasValue)
                    .Where(i => i.Customer.Tickets
                        .Where(j => !j.ReturnDate.HasValue)
                        .OrderBy(j => j.CreatedOn).FirstOrDefault() == i)
                    .Where(i => i.TicketPayments.Any() && i.TicketPayments.Min(j => j.PaymentDate) >= start && i.TicketPayments.Min(j => j.PaymentDate) < end)
                    .Select(i => i.CreatedOn).ToArray();

                res.Rows.Add(CalcValue(new object[0], "Количество новых клиентов", src2, start, end, i => i, daily, weekly, out add, years, (i, _, __) => i.Count()).ToArray());

                var src3 = context.Tickets.Where(i => i.DivisionId == divisionId.Value && !i.ReturnDate.HasValue)
                        .Where(i => i.Customer.Tickets
                            .Where(j => !j.ReturnDate.HasValue)
                            .OrderBy(j => j.CreatedOn).FirstOrDefault() != i)
                    .Where(i => i.TicketPayments.Any() && i.TicketPayments.Min(j => j.PaymentDate) >= start && i.TicketPayments.Min(j => j.PaymentDate) < end)
                        .Select(i => i.CreatedOn).ToArray();

                res.Rows.Add(CalcValue(new object[0], "Количество повторов", src3, start, end, i => i, daily, weekly, out add, years, (i, _, __) => i.Count()).ToArray());

                var reps = res.Rows.Count - 1;
                var tot = res.Rows.Count - 3;

                var row1 = new List<object>() { "Процент повторов" };
                var row2 = new List<object>() { "Процент новых" };
                for(int i = 1; i < res.Columns.Count; i++)
                {
                    if((int)res.Rows[tot][i] != 0)
                    {
                        var pn = (decimal)(int)res.Rows[reps][i] / (decimal)(int)res.Rows[tot][i] * 100;
                        row1.Add(pn);
                        row2.Add(100 - pn);
                    }
                    else
                    {
                        row1.Add(0);
                        row2.Add(0);
                    }
                }
                res.Rows.Add(row1.ToArray());
                res.Rows.Add(row2.ToArray());

                return res;
            }
        }

        public DataTable GoodSales(DateTime start, DateTime end, Guid? divisionId, bool daily, bool weekly, bool years)
        {
            end = end.AddDays(1).AddSeconds(-1);
            if(weekly && daily)
            {
                throw new Exception("Детализация одновременно по дням и по неделям невозможна!");
            }
            using(var context = new TonusEntities())
            {
                var user = UserManagement.GetUser(context);
                var res = new DataTable();
                res.ExtendedProperties.Add("Detailed", true);
                res.Columns.Add("Параметр", typeof(string));
                res.Columns.Add("Товар", typeof(string));
                AddDatesColumns(res, start, end, daily, weekly, typeof(decimal), years);
                bool add, add1, add2;
                IQueryable<GoodSale> q1 = context.GoodSales.Where(i => i.BarOrder.PurchaseDate >= start && i.BarOrder.PurchaseDate <= end);
                if(divisionId.HasValue)
                {
                    q1 = q1.Where(i => i.BarOrder.DivisionId == divisionId.Value);
                }
                var q = q1.Select(i => new { i.GoodId, i.BarOrder.PurchaseDate, i.Amount, PriceMoney = i.PriceMoney ?? 0 }).ToArray();
                foreach(var g in context.Goods.Where(i => i.CompanyId == user.CompanyId).OrderBy(i => i.Name))
                {
                    var src = q.Where(i => i.GoodId == g.Id).ToList();

                    var row = CalcValue(new object[] { "Количество" }, g.Name,
                        src,
                        start, end, i => i.PurchaseDate, daily, weekly, out add, years,
                        (i, _, __) => i.Sum(j => (decimal)j.Amount));

                    var row1 = CalcValue(new object[] { "Стоимость" }, g.Name,
                        src,
                        start, end, i => i.PurchaseDate, daily, weekly, out add1, years,
                        (i, _, __) => i.Sum(j => Decimal.Round((decimal)j.Amount * j.PriceMoney, 2)));

                    var row2 = CalcValue(new object[] { "Средняя цена" }, g.Name,
                        src,
                        start, end, i => i.PurchaseDate, daily, weekly, out add2, years,
                        (i, _, __) => Decimal.Round(i.Average(j => j.PriceMoney), 2));
                    if(add || add1 || add2)
                    {
                        res.Rows.Add(row.ToArray());
                        res.Rows.Add(row1.ToArray());
                        res.Rows.Add(row2.ToArray());
                    }
                }
                var goodPackages = context.BarOrders
                    .Where(i => (divisionId == null || i.DivisionId == divisionId) && i.Kind1C == 10 && i.PurchaseDate >= start && i.PurchaseDate <= end).ToList()
                    .Select(i => new { Date = i.PurchaseDate, Lines = i.GetContent() }).ToList()
                    .SelectMany(i => i.Lines.Select(j => new { Date = i.Date.Date, Line = j }))
                    .GroupBy(i => i.Line.Name)
                    .ToArray();
                foreach(var src in goodPackages)
                {
                    var row = CalcValue(new object[] { "Количество" }, src.Key,
                        src,
                        start, end, i => i.Date, daily, weekly, out add, years,
                        (i, _, __) => i.Sum(j => (decimal)j.Line.InBasket));

                    var row1 = CalcValue(new object[] { "Стоимость" }, src.Key,
                        src,
                        start, end, i => i.Date, daily, weekly, out add1, years,
                        (i, _, __) => i.Sum(j => (decimal)j.Line.InBasket * j.Line.Price));

                    var row2 = CalcValue(new object[] { "Средняя цена" }, src.Key,
                        src,
                        start, end, i => i.Date, daily, weekly, out add2, years,
                        (i, _, __) => i.Average(j => j.Line.Cost));
                    if(add || add1 || add2)
                    {
                        res.Rows.Add(row.ToArray());
                        res.Rows.Add(row1.ToArray());
                        res.Rows.Add(row2.ToArray());
                    }
                }

                return res;
            }
        }

        public DataTable RecurringCustomers(DateTime start, DateTime end, Guid? divisionId, bool daily, bool weekly, bool years)
        {
            end = end.AddDays(1).AddSeconds(-1);

            if(weekly && daily)
            {
                throw new Exception("Детализация одновременно по дням и по неделям невозможна!");
            }
            if(!divisionId.HasValue)
            {
                throw new Exception("Необходимо указать " + ClubTextR + "!");
            }
            using(var context = new TonusEntities())
            {
                var div = context.Divisions.Single(i => i.Id == divisionId.Value);

                var res = new DataTable();
                res.ExtendedProperties.Add("Detailed", true);

                res.Columns.Add("Критерий", typeof(string));
                AddDatesColumns(res, start, end, daily, weekly, typeof(decimal), years);
                bool add, add1, add2;

                for(int n = 1; n <= 20; n++)
                {
                    var row = CalcValue(new object[0], "Количество купивших " + n + "й абонемент",
                        context.Tickets.Where(i => (!i.TicketType.IsGuest && !i.TicketType.IsVisit) && i.DivisionId == divisionId && i.Customer.Tickets.Where(j => (!j.TicketType.IsGuest && !j.TicketType.IsVisit)).Count() >= n).ToArray()
                            .Where(i => i.Customer.Tickets.Where(j => (!j.TicketType.IsGuest && !j.TicketType.IsVisit)).OrderBy(j => j.CreatedOn).ToArray()[n - 1] == i),
                        start, end, i => i.CreatedOn, daily, weekly, out add, years, (i, _, __) => i.Select(j => j.CustomerId).Distinct().Count());

                    var row1 = CalcValue(new object[0], "Процент купивших " + n + "й абонемент",
                        context.Tickets.Where(i => (!i.TicketType.IsGuest && !i.TicketType.IsVisit) && i.DivisionId == divisionId && i.Customer.Tickets.Where(j => (!j.TicketType.IsGuest && !j.TicketType.IsVisit)).Count() >= n).ToArray()
                            .Where(i => i.Customer.Tickets.Where(j => (!j.TicketType.IsGuest && !j.TicketType.IsVisit)).OrderBy(j => j.CreatedOn).ToArray()[n - 1] == i),
                        start, end, i => i.CreatedOn, daily, weekly, out add1, years,
                        (i, st, fin) => ProcessBuyProc(divisionId, context, i, st, fin));

                    var row2 = CalcValue(new object[0], "Потрачено на " + n + "й абонемент",
                        context.Tickets.Where(i => (!i.TicketType.IsGuest && !i.TicketType.IsVisit) && i.DivisionId == divisionId && i.Customer.Tickets.Where(j => (!j.TicketType.IsGuest && !j.TicketType.IsVisit)).Count() >= n).ToArray()
                            .Where(i => i.Customer.Tickets.Where(j => (!j.TicketType.IsGuest && !j.TicketType.IsVisit)).OrderBy(j => j.CreatedOn).ToArray()[n - 1] == i).SelectMany(i => i.TicketPayments),
                        start, end, i => i.PaymentDate, daily, weekly, out add2, years, (i, _, __) => i.Sum(j => j.Amount));

                    if(add || add1 || add2)
                    {
                        res.Rows.Add(row.ToArray());
                        res.Rows.Add(row1.ToArray());
                        res.Rows.Add(row2.ToArray());
                    }
                }

                return res;
            }
        }

        private decimal? ProcessBuyProc(Guid? divisionId, TonusEntities context, IEnumerable<Ticket> i, DateTime st, DateTime fin)
        {
            var n = (decimal)context.Tickets.Where(j => (!j.TicketType.IsGuest && !j.TicketType.IsVisit)).Where(j => j.DivisionId == divisionId && j.CreatedOn >= st && j.CreatedOn < fin).Select(j => j.CustomerId).Distinct().Count();
            if(n == 0) return null;
            return i.Select(j => j.CustomerId).Distinct().Count() / n * 100;
        }

        private void AddDatesColumns(DataTable res, DateTime start, DateTime end, bool daily, bool weekly, Type type, bool year = false)
        {
            var startM = start.AddDays(-start.Day + 1);
            var i = startM;
            for(i = startM; i <= end; i = i.AddMonths(1))
            {
                var dc = res.Columns.Add(i.ToString("MMMM yyyy"), type);
                dc.ExtendedProperties.Add("MonthColumn", true);
                if(daily)
                {
                    var ed = (end.Month == i.Month) ? end : i.AddMonths(1).AddDays(-1);

                    for(var td = (start.Month == i.Month) ? start : i; td <= ed; td = td.AddDays(1))
                    {
                        res.Columns.Add(td.ToString("dd.MM.yyyy"), type);
                    }
                }
                if(weekly)
                {
                    foreach(var j in GetWeeksInMonth(i))
                    {
                        if(Core.DatesIntersectsEx(start, end, j[0], j[1]))
                        {
                            res.Columns.Add(j.ToArrayString("dd.MM.yyyy"), type);
                        }
                    }
                }
                if(year && i.Month == 12)
                {
                    res.Columns.Add(i.Year.ToString(), type);
                }
            }
            if(year && i.Month != 12)
            {
                res.Columns.Add(i.Year.ToString(), type);
            }
            res.Columns.Add("Итого за период", type);
        }

        public DataTable AdvertBarCustomers(DateTime start, DateTime end, Guid? divisionId, bool daily, bool weekly, bool adverts, bool years, bool tdet)
        {
            end = end.AddDays(1).AddSeconds(-1);

            if(weekly && daily)
            {
                throw new Exception("Детализация одновременно по дням и по неделям невозможна!");
            }
            if(!divisionId.HasValue)
            {
                throw new Exception("Необходимо указать " + ClubTextR + "!");
            }
            using(var context = new TonusEntities())
            {
                var div = context.Divisions.Single(i => i.Id == divisionId.Value);

                var res = new DataTable();
                res.ExtendedProperties.Add("Detailed", true);

                res.Columns.Add("Рекламная группа", typeof(string));
                res.Columns.Add("Рекламный канал", typeof(string));
                res.Columns.Add("Критерий", typeof(string));
                AddDatesColumns(res, start, end, daily, weekly, typeof(decimal), years);
                bool add, add1;
                foreach(var adv in context.AdvertTypes.Where(i => i.IsAvail).OrderBy(i => i.AdvertGroup.Name).ThenBy(i => i.Name))
                {
                    for(int n = 1; n <= (tdet ? 20 : 1); n++)
                    {
                        var row = CalcValue(new object[] { adverts ? adv.AdvertGroup.Name : "Все", adverts ? adv.Name : "Все" }, "Количество клиентов" + (tdet ? (", имеющих " + n + " абонемент(ов)") : ""),
                            context.Customers.Where(i => i.AdvertTypeId == adv.Id || !adverts),
                            start, end, i => null, daily, weekly, out add, years, (i, st, fin) => FilterCustomersByTicketCount(i, st, fin, n, div).Count());

                        var row1 = CalcValue(new object[] { adverts ? adv.AdvertGroup.Name : "Все", adverts ? adv.Name : "Все" }, "Потрачено в баре клиентами" + (tdet ? (", имеющими " + n + " абонемент(ов)") : ""),
                             context.Customers.Where(i => i.AdvertTypeId == adv.Id || !adverts),
                            start, end, i => null, daily, weekly, out add1, years, (j, st, fin) =>
                                FilterCustomersByTicketCount(j, null, fin, tdet ? n : (int?)null, div)
                                .SelectMany(i => i.BarOrders)
                                .Where(i => i.PurchaseDate >= st && i.PurchaseDate < fin && i.DivisionId == div.Id)
                                .SelectMany(i => i.GoodSales)
                                .Sum(i => (decimal)i.Amount * (i.PriceMoney ?? 0)));

                        if(add || add1 || !adverts)
                        {
                            res.Rows.Add(row.ToArray());
                            res.Rows.Add(row1.ToArray());
                        }
                    }
                    if(!adverts) break;
                }

                return res;
            }
        }

        public DataTable GetTicketRemainReport(Guid? divisionId, string ticketNumber)
        {
            using(var context = new TonusEntities())
            {
                ticketNumber = (ticketNumber ?? "").ToLower();
                var tickets = context.Tickets.Where(i => i.Number.ToLower() == ticketNumber);
                if(tickets.Count() == 0) throw new Exception("Абонемент с таким номером не существует!");
                if(tickets.Count() == 2 && !divisionId.HasValue) throw new Exception("Необходимо указать " + ClubTextR + "!");

                Ticket ticket;
                if(tickets.Count() == 1) ticket = tickets.First();
                else
                {
                    ticket = tickets.FirstOrDefault(i => i.DivisionId == divisionId);
                }
                if(ticket == null) throw new Exception("Абонемент с таким номером не существует!");

                if(!ticket.TicketType.TicketTypeLimits.Any())
                {
                    throw new Exception("Абонемент не имеет ограничений по типам услуг!");
                }

                var res = new DataTable();
                res.Columns.Add("Услуга", typeof(string));
                res.Columns.Add("Начальное количество", typeof(int));
                res.Columns.Add("Остаток", typeof(string));

                foreach(var lim in ticket.TicketType.TicketTypeLimits)
                {
                    var vis = ticket.TreatmentEvents.Count(i => i.TreatmentConfigId == lim.TreatmentConfigId && i.VisitStatus != 1);

                    res.Rows.Add(lim.TreatmentConfig.Name, lim.Amount, lim.Amount - vis);
                }

                return res;
            }
        }

        private IEnumerable<Customer> FilterCustomersByTicketCount(IEnumerable<Customer> src, DateTime? st, DateTime fin, int? n, Division div)
        {
            foreach(var cust in src)
            {
                if(n.HasValue)
                {
                    if(cust.Tickets.Count(t => !t.TicketType.IsVisit && !t.TicketType.IsGuest && t.CreatedOn < fin && t.DivisionId == div.Id) == n)
                    {
                        if(!st.HasValue || cust.CustomerVisits.Any(i => i.InTime <= fin && i.InTime >= st && i.DivisionId == div.Id))
                            yield return cust;
                    }
                }
                else
                {
                    if(!st.HasValue || cust.CustomerVisits.Any(i => i.InTime <= fin && i.InTime >= st && i.DivisionId == div.Id))
                        yield return cust;
                }
            }
        }

        public DataTable GetTestdriveReport(DateTime start, DateTime end, Guid? divisionId)
        {
            using(var context = new TonusEntities())
            {
                if(!divisionId.HasValue) throw new Exception("Необходимо указать " + ClubTextR + "!");

                var res = new DataTable();
                res.Columns.Add("Клиент", typeof(string));
                res.Columns.Add("Карта", typeof(string));
                res.Columns.Add("Дата", typeof(DateTime));
                res.Columns.Add("Услуги", typeof(string));
                res.Columns.Add("Количество услуг", typeof(int));
                res.Columns.Add("Цена", typeof(int));

                foreach(var uc in context.UnitCharges.Where(i => i.Charge == 1 && i.Reason.StartsWith("Пробное занятие") && i.Ticket.DivisionId == divisionId && i.Date >= start && i.Date <= end))
                {
                    var cust = uc.Ticket.Customer;
                    cust.InitActiveCard();
                    var date = uc.Date.Date;
                    var date1 = date.AddDays(1);
                    var tevs = cust.TreatmentEvents.Where(i => (i.VisitStatus == 2 || i.VisitStatus == 3) && i.VisitDate >= date && i.VisitDate < date1).ToList().Where(i => !context.UnitCharges.Any(j => j.EventId == i.Id && i.Id != uc.Id)).ToList().Init();
                    if(tevs.Count() == 0) continue;
                    res.Rows.Add(cust.FullName, cust.ActiveCard != null ? cust.ActiveCard.CardBarcode : "", date,
                    String.Join("; ", tevs.Select(i => i.TreatmentConfig.Name).ToArray()), tevs.Count(), tevs.Sum(i => i.Price));
                }


                return res;
            }
        }

        public DataTable GetCurrentSalesProposal(Guid? divisionId)
        {
            using(var context = new TonusEntities())
            {
                if(!divisionId.HasValue) throw new Exception("Необходимо указать " + ClubTextR + "!");
                var res = new DataTable();
                res.Columns.Add("Рекламная группа", typeof(string));
                res.Columns.Add("Рекламный канал", typeof(string));
                res.Columns.Add("Всего звонков", typeof(int));
                res.Columns.Add("Всего приходов", typeof(int));
                res.Columns.Add("Всего посещений и приходов", typeof(int));
                res.Columns.Add("Всего продаж", typeof(int));
                res.Columns.Add("Стоимость продаж", typeof(string));
                res.Columns.Add("Коэффициент по количеству", typeof(decimal));
                res.Columns.Add("Коэффициент по стоимости", typeof(decimal));
                res.Columns.Add("Звонков с начала месяца", typeof(int));
                res.Columns.Add("Приходов с начала месяца", typeof(int));
                res.Columns.Add("Звонков и приходов с начала месяца", typeof(int));

                res.Columns.Add("Прогноз суммы звонков и приходов", typeof(decimal));
                res.Columns.Add("Прогноз количества продаж", typeof(decimal));
                res.Columns.Add("Прогноз стоимости продаж", typeof(decimal));
                res.Columns.Add("Прогноз выполнения плана", typeof(decimal));

                var monthStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                var monthMulti = ((decimal)DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month)) / ((decimal)DateTime.Today.Day);
                var salesPlan = 0m;
                var plan = context.SalesPlans.FirstOrDefault(i => i.DivisionId == divisionId && i.Month == monthStart);
                if(plan != null) salesPlan = plan.Value + plan.CorpValue;
                var cId = UserManagement.GetCompanyIdOrDefaultId(context);


                var callsSrc = context.Customers.GroupBy(i => i.AdvertTypeId).Select(j => new
                {
                    j.Key,
                    Count = j.Count(c => c.ClubId == divisionId
                        && c.Calls.Any()
                        && (!c.CustomerVisits.Any()
                        || c.Calls.Min(i => i.StartAt) > c.CustomerVisits.Min(i => i.InTime)))
                }).ToArray();

                var visitsSrc = context.Customers.GroupBy(i => i.AdvertTypeId).Select(j => new
                {
                    j.Key,
                    Count = j.Count(c => c.ClubId == divisionId
                    && (!c.Calls.Any() || (c.CustomerVisits.Any() && c.CustomerVisits.Min(i => i.InTime) < c.Calls.Min(i => i.StartAt))))
                }).ToArray();

                var salesSrc = context.Customers.GroupBy(i => i.AdvertTypeId).Select(j => new
                {
                    j.Key,
                    Count = j.Count(c => c.ClubId == divisionId
                    && c.Tickets.Any(i => i.DivisionId == divisionId && !i.TicketType.IsGuest && !i.TicketType.IsVisit))
                }).ToArray();

                var salesSumSrc = context.Customers.Where(c => c.ClubId == divisionId)
                    .SelectMany(c => c.Tickets.Where(i => i.DivisionId == divisionId && !i.TicketType.IsGuest && !i.TicketType.IsVisit))
                    .GroupBy(i => i.Customer.AdvertTypeId).Select(j => new { j.Key, Sum = j.Sum(i => (decimal?)i.Price * (1 - i.DiscountPercent)) ?? 0 }).ToArray();

                var callsMSrc = context.Customers.GroupBy(i => i.AdvertTypeId).Select(j => new
                {
                    j.Key,
                    Count = j.Count(c => c.CreatedOn >= monthStart
                        && c.ClubId == divisionId
                        && c.Calls.Any()
                        && (!c.CustomerVisits.Any()
                        || c.Calls.Min(i => i.StartAt) > c.CustomerVisits.Min(i => i.InTime)))
                }).ToArray();

                var visitsMSrc = context.Customers.GroupBy(i => i.AdvertTypeId).Select(j => new
                {
                    j.Key,
                    Count = j.Count(c => c.CreatedOn >= monthStart && c.ClubId == divisionId && (!c.Calls.Any()
                        || (c.CustomerVisits.Any() && c.CustomerVisits.Min(i => i.InTime) < c.Calls.Min(i => i.StartAt))))
                }).ToArray();

                foreach(var at in context.AdvertTypes.Where(i => i.IsAvail && (i.CompanyId == cId || !i.CompanyId.HasValue)).OrderBy(i => i.AdvertGroup.Name).ThenBy(i => i.Name).ToArray())
                {
                    var row = new object[] { at.AdvertGroup.Name, at.Name }.ToList();

                    var calls = callsSrc.Where(i => i.Key == at.Id).Select(i => i.Count).FirstOrDefault();
                    row.Add(calls);
                    var visits = visitsSrc.Where(i => i.Key == at.Id).Select(i => i.Count).FirstOrDefault();
                    row.Add(visits);
                    row.Add(calls + visits);

                    var sales = salesSrc.Where(i => i.Key == at.Id).Select(i => i.Count).FirstOrDefault();
                    row.Add(sales);

                    var salesSum = salesSumSrc.Where(i => i.Key == at.Id).Select(i => i.Sum).FirstOrDefault();
                    row.Add(salesSum);

                    var amountMulty = (calls + visits) > 0 ? ((decimal)sales / ((decimal)calls + (decimal)visits)) : 0;
                    row.Add(amountMulty);

                    var salesMulty = (calls + visits) > 0 ? ((decimal)salesSum / ((decimal)calls + (decimal)visits)) : 0;
                    row.Add(salesMulty);

                    var callsM = callsMSrc.Where(i => i.Key == at.Id).Select(i => i.Count).FirstOrDefault();
                    row.Add(callsM);

                    var visitsM = visitsMSrc.Where(i => i.Key == at.Id).Select(i => i.Count).FirstOrDefault();
                    row.Add(visitsM);
                    row.Add(callsM + visitsM);

                    var propCallsAndVisits = (callsM + visitsM) * monthMulti;
                    row.Add(propCallsAndVisits);

                    row.Add(propCallsAndVisits * amountMulty);

                    var propSalesSum = propCallsAndVisits * salesMulty;
                    row.Add(propSalesSum);

                    if(salesPlan > 0)
                    {
                        row.Add(propSalesSum / salesPlan * 100);
                    }
                    else
                    {
                        row.Add(DBNull.Value);
                    }

                    res.Rows.Add(row.ToArray());
                }

                return res;
            }
        }

        class EmployeesAggregateVisitReportHelper
        {
            public Guid EmployeeId { get; set; }
            public string Name { get; set; }
            public string Job { get; set; }
            public string Unit { get; set; }
            public int Days { get; set; }
            public double Hours { get; set; }
            public double Delta { get; set; }
        }
    }
}

