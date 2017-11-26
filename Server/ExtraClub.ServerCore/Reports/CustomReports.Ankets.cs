using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ExtraClub.Entities;
using ExtraClub.ServiceModel.Reports;

namespace ExtraClub.ServerCore
{
    partial class CustomReports
    {

        public DataTable UniedAnketsReport(DateTime start, DateTime end)
        {
            using (var context = new ExtraEntities())
            {
                var res = new DataTable();

                var ankets = context.Ankets
                    .Include("AnketAdverts").Include("AnketTickets").Include("AnketTreatments")
                    .Where(i => i.Period >= start && i.Period <= end && i.SentDate.HasValue)
                    .OrderBy(i => i.Period)
                    .ToList();

                var clubs = context.Divisions.Select(i => new { i.Id, i.Name }).ToDictionary(i => i.Id, i => i.Name);
                var ttypes = context.TicketTypes.Select(i => new { i.Id, i.Name }).ToDictionary(i => i.Id, i => i.Name);
                var treatmenttypes = context.TreatmentTypes.Select(i => new { i.Id, i.Name }).ToDictionary(i => i.Id, i => i.Name);

                res.Columns.Add("Клуб", typeof(string));
                res.Columns.Add("Месяц", typeof(Date));
                res.Columns.Add("Заполнена", typeof(DateTime));
                res.Columns.Add("Прайс менялся", typeof(string));
                res.Columns.Add("Абонементов всего", typeof(int));

                var prTts = ankets.SelectMany(i => i.AnketTickets).Select(i => i.TicketTypeId).Distinct().OrderBy(i => ttypes[i]).ToArray();

                foreach (var tt in prTts)
                {
                    var cName = ttypes[tt];
                    var resName = cName;
                    int i = 0;
                    while (res.Columns.Contains(resName))
                    {
                        resName = cName + " (" + ++i + ")";
                    }
                    res.Columns.Add(resName, typeof(int));
                }

                res.Columns.Add("Количество рабочих дней клуба", typeof(int));
                res.Columns.Add("Среднее количество человек в день", typeof(int));
                res.Columns.Add("Среднее количество услуг в день", typeof(int));
                res.Columns.Add("Количество человек, пришедших на пробное занятие", typeof(int));
                res.Columns.Add("Количество человек, купивших абонемент после пробного занятия", typeof(int));
                res.Columns.Add("Количество повторных абонементов", typeof(int));

                var prTs = ankets.SelectMany(i => i.AnketTreatments).Select(i => i.TreatmentTypeId).Distinct().OrderBy(i => treatmenttypes[i]).ToArray();

                res.Columns.Add("Посещено услуг всего", typeof(int));

                foreach (var tt in prTs)
                {
                    var cName = treatmenttypes[tt];
                    var resName = cName;
                    int i = 0;
                    while (res.Columns.Contains(resName))
                    {
                        resName = cName + " (" + ++i + ")";
                    }
                    res.Columns.Add(resName, typeof(int));
                }

                res.Columns.Add("Затраты на рекламу", typeof(decimal));

                var pfGroups = ankets.SelectMany(i => i.AnketAdverts).Select(i => i.AdvertGroupName).Distinct().OrderBy(i => i).ToArray();

                foreach (var tt in pfGroups)
                {
                    res.Columns.Add(tt + " звонки", typeof(int));
                    res.Columns.Add(tt + " пришли", typeof(int));
                    res.Columns.Add(tt + " купили", typeof(int));
                }

                res.Columns.Add("Сетевые акции", typeof(string));
                res.Columns.Add("Свои акции", typeof(string));
                res.Columns.Add("План на след. месяц акции", typeof(string));

                res.Columns.Add("План по выручке выполнен, %", typeof(decimal));
                res.Columns.Add("Выручка", typeof(decimal));
                res.Columns.Add("Оценка работы персонала", typeof(decimal));
                res.Columns.Add("Обоснование работы персонала", typeof(string));
                res.Columns.Add("Смена кадров", typeof(string));
                res.Columns.Add("Собрание персонала", typeof(string));
                res.Columns.Add("Тестирование персонала", typeof(string));

                res.Columns.Add("Новое оборудование", typeof(string));
                res.Columns.Add("Проблемы с оборудованием", typeof(string));

                res.Columns.Add("Оценка развития клуба", typeof(decimal));
                res.Columns.Add("Обоснование развития клуба", typeof(string));

                res.Columns.Add("Оценка своей работы", typeof(decimal));
                res.Columns.Add("Обоснование своей работы", typeof(string));

                res.Columns.Add("Оценка работы франчайзора", typeof(decimal));
                res.Columns.Add("Обоснование работы франчайзора", typeof(string));

                res.Columns.Add("Оценка СПФС", typeof(decimal));
                res.Columns.Add("Обоснование СПФС", typeof(string));

                res.Columns.Add("Оценка СП АСУ", typeof(decimal));
                res.Columns.Add("Обоснование СП АСУ", typeof(string));

                res.Columns.Add("Оценка дизайнера", typeof(decimal));
                res.Columns.Add("Обоснование дизайнера", typeof(string));

                res.Columns.Add("Оценка адм.сайтов", typeof(decimal));
                res.Columns.Add("Обоснование адм.сайтов", typeof(string));

                res.Columns.Add("Оценка бухгалтерии", typeof(decimal));
                res.Columns.Add("Обоснование бухгалтерии", typeof(string));

                res.Columns.Add("Оценка логиста", typeof(decimal));
                res.Columns.Add("Обоснование логиста", typeof(string));

                res.Columns.Add("Оценка мастера по ремонту", typeof(decimal));
                res.Columns.Add("Обоснование мастера по ремонту", typeof(string));

                res.Columns.Add("Факторы, повлиявшие на выручку", typeof(string));
                res.Columns.Add("Пожелания", typeof(string));

                foreach (var anket in ankets)
                {
                    var row = new List<object> { clubs[anket.DivisionId], (Date)new DateTime(anket.Period.Year, anket.Period.Month, 1), anket.CreatedOn, anket.PriceChanges ? "да" : "нет" };
                    row.Add(anket.AnketTickets.Sum(i => (int?)i.Amount));
                    foreach (var tt in prTts)
                    {
                        row.Add(anket.AnketTickets.Where(i => i.TicketTypeId == tt).Select(i => i.Amount).FirstOrDefault());
                    }
                    row.AddRange(new object[] { anket.TotalWorkdays, anket.AvgVisitors, anket.AvgTreatments, anket.TotalTestVisitors, anket.TotalBuyAfterTest, anket.RecurringTickets });

                    row.Add(anket.AnketTreatments.Sum(i => (int?)i.Amount));
                    foreach (var tt in prTs)
                    {
                        row.Add(anket.AnketTreatments.Where(i => i.TreatmentTypeId == tt).Select(i => i.Amount).FirstOrDefault());
                    }

                    row.Add(anket.AdvertSpendings);

                    foreach (var tt in pfGroups)
                    {
                        row.Add(anket.AnketAdverts.Where(i => i.AdvertGroupName == tt).Select(i => i.Calls).FirstOrDefault());
                        row.Add(anket.AnketAdverts.Where(i => i.AdvertGroupName == tt).Select(i => i.Visits).FirstOrDefault());
                        row.Add(anket.AnketAdverts.Where(i => i.AdvertGroupName == tt).Select(i => i.Purchases).FirstOrDefault());
                    }

                    row.AddRange(new object[] { anket.NetActions, anket.SelfActions, anket.NextNetActions });
                    row.AddRange(new object[] { anket.PlanComplete, anket.TotalCash, 5-(anket.EmployeesGrade ?? 0), anket.EmployeesGradeDesc, anket.EmployeesChange ? "да" : "нет", 
                        anket.MeetingDesc, anket.TestDesc });

                    row.AddRange(new object[] { anket.NewTreatments ? "да" : "нет", anket.TreatmentProblemsDesc });
                    row.AddRange(new object[] { 5 - (anket.ClubDevGrade ?? 0), anket.ClubDevDesc });
                    row.AddRange(new object[] { 5 - (anket.SelfGrade ?? 0), anket.SelfDesc });
                    row.AddRange(new object[] { 5 - (anket.FranchGrade ?? 0), anket.FranchDesc });
                    row.AddRange(new object[] { 5 - (anket.FranchSuppGrade ?? 0), anket.FranchSuppDesc });
                    row.AddRange(new object[] { 5 - (anket.AsuSuppGrade ?? 0), anket.AsuSuppDesc });
                    row.AddRange(new object[] { 5 - (anket.DesignerGrade ?? 0), anket.DesignerDesc });
                    row.AddRange(new object[] { 5 - (anket.SiteAdmGrade ?? 0), anket.SiteAdmDesc });
                    row.AddRange(new object[] { 5 - (anket.AccountantsGrade ?? 0), anket.AccountantsDesc });
                    row.AddRange(new object[] { 5 - (anket.LogistGrade ?? 0), anket.LogistDesc });
                    row.AddRange(new object[] { 5 - (anket.RepairGrade ?? 0), anket.RepairDesc });

                    row.Add(anket.IncomeFactors);
                    row.Add(anket.Wishes);
                    res.Rows.Add(row.ToArray());
                }

                return res;
            }
        }
    }
}
