using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Data.Objects.SqlClient;
using System.Linq;
using ExtraClub.Entities;
using ExtraClub.ServiceModel;
using ExtraClub.ServiceModel.Reports;

namespace ExtraClub.ServerCore
{
    partial class CustomReports
    {
        public DataTable EmployeeContactActivity(DateTime start, DateTime end, Guid? employeeId)
        {
            using (var context = new ExtraEntities())
            {
                var res = new DataTable();
                end = end.Date.AddDays(1).AddMilliseconds(-1);

                var user = UserManagement.GetUser(context);

                res.Columns.Add("_customerId", typeof(Guid));
                res.Columns.Add("ФИО сотрудника", typeof(string));
                res.Columns.Add("Дата контакта", typeof(DateTime));
                res.Columns.Add("ФИО клиента", typeof(string));
                res.Columns.Add("Номер карты", typeof(string));
                res.Columns.Add("Телефон", typeof(string));
                res.Columns.Add("Рекламная группа", typeof(string));
                res.Columns.Add("Реламный канал", typeof(string));
                res.Columns.Add("Тип контакта", typeof(string));
                res.Columns.Add("Цель", typeof(string));
                res.Columns.Add("Результат", typeof(string));
                res.Columns.Add("Комментарии", typeof(string));

                context.Calls.Where(i => i.CompanyId == user.CompanyId && i.StartAt >= start && i.StartAt < end && (i.AuthorId == employeeId || employeeId == null))
                    .Select(i => new
                    {
                        Id = i.CustomerId,
                        EmployeeName = i.CreatedBy.FullName,
                        i.StartAt,
                        FullName = (i.Customer.LastName + " " ?? "") + (i.Customer.FirstName + " " ?? "") + (i.Customer.MiddleName ?? ""),
                        CardNumber = i.Customer.CustomerCards.Where(j => j.IsActive).FirstOrDefault().CardBarcode,
                        i.Customer.Phone2,
                        AGName = i.Customer.AdvertType.AdvertGroup.Name,
                        ATName = i.Customer.AdvertType.Name,
                        ContType = i.IsIncoming ? "Входящий звонок" : "Исходящий звонок",
                        i.Goal,
                        i.Result,
                        i.ResultType,
                        i.IsIncoming,
                        i.Log
                    }).ToList().ForEach(i =>
                    {
                        res.Rows.Add(i.Id, i.EmployeeName, i.StartAt, i.FullName, i.CardNumber, i.Phone2, i.AGName, i.ATName, i.ContType, i.Goal, Call.GetKindText(i.Result, i.ResultType, i.Phone2), Call.GetComments(i.IsIncoming, i.Log));
                    });

                return res;
            }
        }

        public DataTable EmployeeCallActivity(DateTime start, DateTime end, string employees, Guid? divisionId)
        {
            using (var context = new ExtraEntities())
            {
                var res = new DataTable();
                if (divisionId == null) return res;

                var cid = UserManagement.GetCompanyIdOrDefaultId(context);
                end = end.Date.AddDays(1).AddMilliseconds(-1);

                res.Columns.Add("_employeeId", typeof(Guid));
                res.Columns.Add("ФИО", typeof(string));
                res.Columns.Add("Дата", typeof(Date));

                for (int i = 7; i < 24; i++)
                {
                    res.Columns.Add(i.ToString(), typeof(int));
                }
                res.Columns.Add("Итого за день", typeof(int));

                var empsFilter = new List<Guid>();
                if (!String.IsNullOrWhiteSpace(employees))
                {
                    empsFilter = employees.Split(';').Select(i => Guid.Parse(i)).ToList();
                }

                var src = context.Calls.Where(i => i.StartAt >= start && i.StartAt < end)
                    .Where(i => i.CreatedBy.EmployeeId.HasValue)
                    .Where(i => i.DivisionId == divisionId)
                    .Where(i => empsFilter.Contains(i.CreatedBy.EmployeeId.Value) || empsFilter.Count == 0)
                    .GroupBy(i => new { Date = EntityFunctions.TruncateTime(i.StartAt), Hour = SqlFunctions.DatePart("hour", i.StartAt), EmployeeId = i.CreatedBy.EmployeeId.Value })
                    .Select(i => new { EmployeeId = i.Key.EmployeeId, i.Key.Date, i.Key.Hour, Count = i.Count() })
                    .ToArray()
                    .GroupBy(i => i.Date)
                    .ToDictionary(i => i.Key, i => i.ToArray());
                var allemps = context.Employees.Where(i => i.CompanyId == cid && (empsFilter.Count == 0 || empsFilter.Contains(i.Id))).ToDictionary(i => i.Id, i => i);
                var date = start;
                while (date <= end)
                {
                    var emps = EmployeeCore.GetEmployeesWorkingAt(divisionId.Value, date).Select(i => i.Id);
                    var empssrc = emps.Union(src.ContainsKey(date) ? src[date].Select(i => i.EmployeeId).Distinct() : new Guid[0]).Distinct().Where(i => allemps.ContainsKey(i)).Select(i => allemps[i]);
                    foreach (var emp in empssrc)
                    {
                        var arr = new List<object> { emp.Id, emp.BoundCustomer.FullName, (Date)date };
                        var s = 0;
                        for (int j = 7; j <= 23; j++)
                        {
                            var r = src.ContainsKey(date) ? src[date].Where(i => i.EmployeeId == emp.Id && i.Hour == j).Select(i => i.Count).FirstOrDefault() : 0;
                            s += r;
                            arr.Add(r == 0 ? (int?)null : r);
                        }
                        arr.Add(s);
                        res.Rows.Add(arr.ToArray());
                    }
                    date = date.AddDays(1);
                }

                return res;
            }
        }
    }
}
