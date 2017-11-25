using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Text;
using TonusClub.Entities;
using TonusClub.ServiceModel;
using TonusClub.ServiceModel.Reports;

namespace TonusClub.ServerCore
{
    partial class CustomReports
    {
        public DataTable CustomersByDateReport(DateTime date, Guid? divisionId)
        {
            using(var context = new TonusEntities())
            {
                var res = new DataTable();

                res.Columns.Add("_Id", typeof(Guid));
                res.Columns.Add("Номер карты", typeof(string));
                res.Columns.Add("ФИО", typeof(string));
                res.Columns.Add("Телефон", typeof(string));
                res.Columns.Add("Остаток единиц", typeof(int));
                res.Columns.Add("Задолженность", typeof(decimal));
                res.Columns.Add("Дата последнего замера", typeof(DateTime));
                res.Columns.Add("Наличие составленной программы", typeof(string));

                res.ExtendedProperties.Add("EntityType", typeof(Customer));

                var cId = UserManagement.GetCompanyIdOrDefaultId(context);

                date = date.Date;
                var date2 = date.Date.AddDays(1);

                var src = context.TreatmentEvents.Where(i => i.VisitStatus != 1
                        && i.CompanyId == cId
                        && (divisionId == null || i.DivisionId == divisionId)
                        && EntityFunctions.TruncateTime(i.VisitDate) == date).OrderBy(i => i.VisitDate)
                    .Select(i => i.Customer).Distinct().Select(i => new
                    {
                        i.Id,
                        Card = i.CustomerCards.OrderByDescending(j => j.EmitDate).Select(j => j.CardBarcode).FirstOrDefault(),
                        Name = (i.LastName ?? "") + ((" " + i.FirstName) ?? "") + ((" " + i.MiddleName) ?? ""),
                        Tickets = i.Tickets,//Where(j => j.IsActive),
                        LastMeasureDate = i.CustomerMeasures.Max(j => (DateTime?)j.CreatedOn),
                        HasProgram = i.TreatmentEvents.Any(j => j.VisitDate >= date2),
                        i.Phone2
                    });

                foreach(var i in src)
                {
                    i.Tickets.ToList().ForEach(x => x.InitDetails());
                    res.Rows.Add(i.Id, i.Card, i.Name, i.Phone2,
                        i.Tickets.Where(x => x.Status == TicketStatus.Active || x.Status == TicketStatus.Available).Sum(x => (decimal?)x.UnitsLeft),
#if BEAUTINIKA
                        i.Tickets.Where(x => x.Status == TicketStatus.Active || x.Status == TicketStatus.Available).Sum(x => (decimal?)x.ExtraUnitsLeft),
#endif
 i.Tickets.Sum(x => (decimal?)x.Loan),
                        i.LastMeasureDate, i.HasProgram ? "Есть" : "Нет");
                }

                return res;
            }
        }


        public DataTable GetAllCustomersEx()
        {
            using(var context = new TonusEntities())
            {
                context.CommandTimeout = 600;
                var cId = UserManagement.GetCompanyIdOrDefaultId(context);
                var today = DateTime.Today;
                var cs = context.Customers.Where(i => i.CompanyId == cId)
                    .OrderBy(i => i.LastName)
                    .ThenBy(i => i.FirstName)
                    .Select(i => new
                    {
                        i.Id,
                        i.LastName,
                        i.FirstName,
                        i.MiddleName,
                        CustomerCard = i.CustomerCards.Where(j => j.IsActive).OrderByDescending(j => j.EmitDate).Select(j => j.CardBarcode).FirstOrDefault(),
                        i.Birthday,
                        Division = i.ClubId.HasValue ? context.Divisions.Where(j => j.Id == i.ClubId).Select(j => j.Name).FirstOrDefault() : "",
                        Age = SqlFunctions.DateDiff("year", i.Birthday, today),
                        i.Phone2,
                        Active = i.Tickets.Where(j => j.IsActive && j.TicketType.SolariumMinutes == 0).Count(),
                        TotalUnits = i.Tickets.Where(j => j.IsActive).Sum(j => (int?)(j.UnitsAmount - (j.UnitCharges.Sum(k => (int?)k.Charge) ?? 0))) ?? 0,
                        Deposit = i.DepositAccounts.Sum(j => (decimal?)j.Amount) ?? 0,
                        Statuses = i.CustomerStatuses.Select(j => j.Name),
                        PresentAt = i.CustomerVisits.Where(j => !j.OutTime.HasValue).Select(j => j.Division.Name).FirstOrDefault(),
                        HasEvents = i.TreatmentEvents.Count(j => EntityFunctions.TruncateTime(j.VisitDate) == today && j.VisitStatus != 1),
                        LastVisitDate = i.CustomerVisits.OrderByDescending(j => j.InTime).Select(j => j.InTime).FirstOrDefault(),
                        AdType = i.AdvertType.Name,
                        AdGroup = i.AdvertType.AdvertGroup.Name,
                        i.AdvertComment,
                        i.CreatedOn,
                        InvitedBy = i.InvitedBy.LastName + (" " + i.InvitedBy.FirstName ?? "") + (" " + i.InvitedBy.MiddleName ?? ""),
                        i.Email,
                        i.FromSite,
                        Manager = i.ManagerId.HasValue ? i.Employee.BoundCustomer.LastName + " " + i.Employee.BoundCustomer.FirstName : "",
                        Bonuses = (i.BonusAccounts.Any()) ? i.BonusAccounts.Sum(j => j.Amount) : 0m
                    }).ToArray();
                var res = new DataTable();
                res.ExtendedProperties.Add("EntityType", typeof(Customer));
                res.Columns.Add("_id", typeof(Guid));
                res.Columns.Add("Фамилия", typeof(string));
                res.Columns.Add("Имя", typeof(string));
                res.Columns.Add("Отчество", typeof(string));
                res.Columns.Add("Номер карты", typeof(string));
                res.Columns.Add("Дата рождения", typeof(DateTime));
                res.Columns.Add("Возраст", typeof(int));
                res.Columns.Add("День рождения через", typeof(int));
                res.Columns.Add("Телефон", typeof(string));
                res.Columns.Add("Емейл", typeof(string));
                res.Columns.Add(ClubText, typeof(string));
                res.Columns.Add("Активные абонементы", typeof(int));
                res.Columns.Add("Единиц на активных", typeof(int));
                res.Columns.Add("Депозит", typeof(decimal));
                res.Columns.Add("Статусы", typeof(string));
                res.Columns.Add("В клубе", typeof(string));
                res.Columns.Add("Запись на сегодня", typeof(int));
                res.Columns.Add("Последнее посещение", typeof(DateTime));
                res.Columns.Add("С последнего посещения дней", typeof(int));
                res.Columns.Add("Рекламная группа", typeof(string));
                res.Columns.Add("Рекламный канал", typeof(string));
                res.Columns.Add("Комментарий рекламы", typeof(string));
                res.Columns.Add("Дата создания клиента", typeof(DateTime));
                res.Columns.Add("Пригласивший клиент", typeof(string));
                res.Columns.Add("Клиент с сайта", typeof(string));
                res.Columns.Add("Количество бонусов", typeof(string));
                res.Columns.Add("Менеджер", typeof(string));

                foreach (var c in cs)
                {
                    res.Rows.Add(
                        c.Id, c.LastName, c.FirstName, c.MiddleName, c.CustomerCard, c.Birthday, c.Age,
                        GetDaysToBirthday(c.Birthday),
                        c.Phone2, c.Email, c.Division, c.Active, c.TotalUnits,
 c.Deposit, String.Join("; ", c.Statuses), c.PresentAt,
                        c.HasEvents, c.LastVisitDate == DateTime.MinValue ? (DateTime?)null : c.LastVisitDate,
                        c.LastVisitDate == DateTime.MinValue ? (int?)null : (int)((today - c.LastVisitDate).TotalDays),
                        c.AdGroup, c.AdType, c.AdvertComment, c.CreatedOn, c.InvitedBy
, c.FromSite ? "Да" : ""
, Decimal.Round(c.Bonuses, 2)
, c.Manager
);
                }
                return res;
            }
        }

        public DataTable AllCustomersExtended(Guid? divisionId, DateTime start, DateTime end)
        {
            end = end.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            using (var context = new TonusEntities())
            {
                end = end.Date.AddDays(1).AddTicks(-1);
                context.CommandTimeout = 600;
                var cId = UserManagement.GetCompanyIdOrDefaultId(context);
                var today = DateTime.Today;
                var now = DateTime.Now;

                var consultationTreatmentTypeId = Guid.Parse("73EE31C6-379E-49C3-8ADE-EB1F46282523");
                var oneTimeTickets = context.Tickets.Where(i => i.TicketType.IsSmart && i.TicketType.IsActive && !i.TicketType.IsVisit && !i.TicketType.IsGuest && i.TicketType.Units == 8 && !i.ReturnDate.HasValue).Select(i => i.Id).ToList();

                var cs = context.Customers.Where(i => i.CompanyId == cId)
                    .OrderBy(i => i.LastName)
                    .ThenBy(i => i.FirstName)
                    .Select(i => new
                    {
                        i.Id,
                        i.LastName,
                        i.FirstName,
                        i.MiddleName,
                        CustomerCard = i.CustomerCards.Where(j => j.IsActive).OrderByDescending(j => j.EmitDate).Select(j => j.CardBarcode).FirstOrDefault(),
                        i.Birthday,
                        Division = i.ClubId.HasValue ? context.Divisions.Where(j => j.Id == i.ClubId).Select(j => j.Name).FirstOrDefault() : "",
                        Age = SqlFunctions.DateDiff("year", i.Birthday, today),
                        i.Phone2,
                        Active = i.Tickets.Where(j => j.IsActive && j.TicketType.SolariumMinutes == 0 && !j.ReturnDate.HasValue).Count(),
                        TotalUnits = i.Tickets.Where(j => j.IsActive).Sum(j => (int?)(j.UnitsAmount - (j.UnitCharges.Sum(k => (int?)k.Charge) ?? 0))) ?? 0,
                        Deposit = i.DepositAccounts.Sum(j => (decimal?)j.Amount) ?? 0,
                        Statuses = i.CustomerStatuses.Select(j => j.Name),
                        PresentAt = i.CustomerVisits.Where(j => !j.OutTime.HasValue).Select(j => j.Division.Name).FirstOrDefault(),
                        HasEvents = i.TreatmentEvents.Any(j => EntityFunctions.TruncateTime(j.VisitDate) >= today && j.VisitStatus == 0) ? "Да" : "Нет",
                        LastVisitDate = i.CustomerVisits.Max(j => (DateTime?)j.InTime),
                        AdType = i.AdvertType.Name,
                        AdGroup = i.AdvertType.AdvertGroup.Name,
                        i.AdvertComment,
                        i.CreatedOn,
                        TicketUnitsLeft = i.Tickets.OrderByDescending(j => j.CreatedOn).Take(1).Sum(j => (int?)(j.UnitsAmount - (j.UnitCharges.Sum(k => (int?)k.Charge) ?? 0))),
                        //PreTicketUnitsLeft = i.Tickets.OrderByDescending(j => j.CreatedOn).Skip(1).FirstOrDefault() != null ?
                        //    (int?)i.Tickets.OrderByDescending(j => j.CreatedOn).Skip(1).Take(1).Sum(j => (int?)(j.UnitsAmount - (j.UnitCharges.Sum(k => (int?)k.Charge) ?? 0))) : null,
                        UnitsOut = (int?)i.Tickets.SelectMany(j => j.UnitCharges).Where(j => j.Date >= start && j.Date <= end).Sum(j => j.Charge),
                        PreLastBuy = i.Tickets.OrderByDescending(j => j.CreatedOn).Skip(1).FirstOrDefault() != null ?
                            (DateTime?)i.Tickets.OrderByDescending(j => j.CreatedOn).Skip(1).FirstOrDefault().CreatedOn : null,
                        LastBuy = i.Tickets.OrderByDescending(j => j.CreatedOn).Select(j => (DateTime?)j.CreatedOn).FirstOrDefault(),
                        PreLastEnds = i.Tickets.OrderByDescending(j => j.CreatedOn).Skip(1).FirstOrDefault() != null ?
                            SqlFunctions.DateAdd("day", i.Tickets.OrderByDescending(j => j.CreatedOn).Skip(1).FirstOrDefault().TicketType.Length, i.Tickets.OrderByDescending(j => j.CreatedOn).Skip(1).FirstOrDefault().StartDate) : null,
                        LastEnds = i.Tickets.Where(j => j.IsActive).Max(j => SqlFunctions.DateAdd("day", j.TicketType.Length, j.StartDate)),
                        GuestUnits = i.Tickets.Where(j => j.IsActive).Sum(j => (int?)(j.GuestUnitsAmount - (j.UnitCharges.Sum(k => (int?)k.GuestCharge) ?? 0))) ?? 0,
                        VisCount = i.CustomerVisits.Count(j => j.InTime >= start && j.InTime <= end),
                        HasCall = i.CustomerNotifications.Any(j => !j.CompletedOn.HasValue) ? "Да" : "Нет",
                        i.Email,
                        FromSite = i.FromSite ? "Да" : "Нет",
                        Bonuses = (i.BonusAccounts.Any()) ? i.BonusAccounts.Sum(j => j.Amount) : 0m,
                        LeftAll = i.Tickets.Sum(j => (int?)(j.UnitsAmount - (j.UnitCharges.Sum(k => (int?)k.Charge) ?? 0))) ?? 0,
                        CountEvent = i.CustomerCrmEvents.Where(x => x.EventDate >= start && x.EventDate <= end).Count(),
                        Event = i.CustomerCrmEvents.OrderByDescending(j => j.EventDate).Select(j => new { j.EventDate, j.Subject, j.Comment, j.Result }).FirstOrDefault(),
                        Event1 = i.Calls.OrderByDescending(j => j.StartAt).Select(j => new
                        {
                            EventDate = j.StartAt,
                            Subject = j.IsIncoming ? "Входящий" : "Исходящий",
                            Comment = (j.Goal ?? ""),
                            Result = j.Result
                        }).FirstOrDefault(),
                        NextEvent = i.CustomerNotifications.Where(j => !j.CompletedOn.HasValue && j.ExpiryDate >= today).OrderBy(j => j.ExpiryDate).FirstOrDefault(),
                        Anthropometric = i.Anthropometrics.OrderByDescending(j => j.CreatedOn).Select(j => (DateTime?)j.CreatedOn).FirstOrDefault(),
                        DateConsultation = i.TreatmentEvents.Where(j => j.Treatment.TreatmentTypeId == consultationTreatmentTypeId && j.VisitDate < now && (j.VisitStatus == 2 || j.VisitStatus == 3)).OrderByDescending(j => j.VisitDate).Select(j => new { j.VisitDate, manager = j.CreatedBy.FullName }).FirstOrDefault(),
                        DateOneTimeTicket = i.Tickets.Where(j => oneTimeTickets.Contains(j.Id) && j.CreatedOn < now).OrderByDescending(j => j.CreatedOn).Select(j => new { j.CreatedOn, manager = j.CreatedBy.FullName }).FirstOrDefault(),
                        Manager = i.ManagerId.HasValue ? i.Employee.BoundCustomer.LastName + " " + i.Employee.BoundCustomer.FirstName : ""
                    }).ToArray();
                var res = new DataTable();
                res.ExtendedProperties.Add("EntityType", typeof(Customer));
                res.Columns.Add("_id", typeof(Guid));
                res.Columns.Add(" ", typeof(int));
                res.Columns.Add("Фамилия", typeof(string));
                res.Columns.Add("Имя", typeof(string));
                res.Columns.Add("Отчество", typeof(string));
                res.Columns.Add("Номер карты", typeof(string));
                res.Columns.Add("Дата рождения", typeof(DateTime));
                res.Columns.Add("Возраст", typeof(int));
                res.Columns.Add("День рождения через", typeof(int));
                res.Columns.Add("Телефон", typeof(string));
                res.Columns.Add("Емейл", typeof(string));
                res.Columns.Add(ClubText, typeof(string));
                res.Columns.Add("Активные абонементы", typeof(int));
                res.Columns.Add("Единиц на активных", typeof(int));
                res.Columns.Add("Гостевых на активных", typeof(int));
                res.Columns.Add("Единиц на всех", typeof(int));
                res.Columns.Add("Тренировок на всех", typeof(int));
                res.Columns.Add("Депозит", typeof(decimal));
                res.Columns.Add("Статусы", typeof(string));
                res.Columns.Add("В клубе", typeof(string));
                res.Columns.Add("Тренировка запланирована", typeof(string));
                res.Columns.Add("Последнее посещение", typeof(DateTime));
                res.Columns.Add("С последнего посещения дней", typeof(int));
                res.Columns.Add("Рекламная группа", typeof(string));
                res.Columns.Add("Рекламный канал", typeof(string));
                res.Columns.Add("Комментарий рекламы", typeof(string));
                res.Columns.Add("Дата создания клиента", typeof(DateTime));
                res.Columns.Add("Остаток единиц на последнем абонементе", typeof(int));
                res.Columns.Add("Единиц за период", typeof(int));
                res.Columns.Add("Дата покупки предыдущего аб-та", typeof(DateTime));
                res.Columns.Add("Дата покупки последнего аб-та", typeof(DateTime));
                res.Columns.Add("Окончание предыдущего аб-та", typeof(DateTime));
                res.Columns.Add("Окончание аб-та", typeof(DateTime));
                res.Columns.Add("До окончания аб-та", typeof(int));
                res.Columns.Add("Посещений за период", typeof(int));
                res.Columns.Add("Средний чек", typeof(int));
                res.Columns.Add("Есть задача на звонок", typeof(string));
                res.Columns.Add("Событий за период", typeof(string));
                res.Columns.Add("Дата последнего события", typeof(DateTime));
                res.Columns.Add("Тема последнего события", typeof(string));
                res.Columns.Add("Содержание последнего события", typeof(string));
                res.Columns.Add("Результат последнего события", typeof(string));
                res.Columns.Add("Дата след. запланированного звонка", typeof(string));
                res.Columns.Add("Цель след. запланированного звонка ", typeof(string));
                res.Columns.Add("Задача след. запланированного звонка ", typeof(string));
                res.Columns.Add("Дата последнего замера", typeof(DateTime));
                res.Columns.Add("Клиент с сайта", typeof(string));
                res.Columns.Add("Количество бонусов", typeof(string));
                res.Columns.Add("ФИО проводившего консультацию или разового посещения", typeof(string));
                res.Columns.Add("Дата пройденной консультации или разового платного занятия", typeof(DateTime));
                res.Columns.Add("Менеджер", typeof(string));

                foreach (var c in cs)
                {
                    if (c.Event != null || c.Event1 != null)
                    {
                        ;
                    }
                    dynamic ev = (c.Event == null) ? (object)c.Event1 : c.Event;
                    if (ev != null && c.Event1 != null && c.Event1.EventDate > ev.EventDate)
                    {
                        ev = c.Event1;
                    }
                    res.Rows.Add(
                        c.Id, 1, c.LastName, c.FirstName, c.MiddleName, c.CustomerCard, c.Birthday, c.Age,
                        GetDaysToBirthday(c.Birthday),
                        c.Phone2, c.Email, c.Division, c.Active, c.TotalUnits, c.GuestUnits,
                        c.LeftAll, Math.Ceiling(c.LeftAll / 8m),
                        c.Deposit, String.Join("; ", c.Statuses), c.PresentAt,
                        c.HasEvents, c.LastVisitDate == DateTime.MinValue ? (DateTime?)null : c.LastVisitDate,
                        (c.LastVisitDate == DateTime.MinValue || !c.LastVisitDate.HasValue) ? (int?)null : (int)((today - c.LastVisitDate.Value).TotalDays),
                        c.AdGroup, c.AdType, c.AdvertComment, c.CreatedOn, c.TicketUnitsLeft, c.UnitsOut, c.PreLastBuy,
                        c.LastBuy, c.PreLastEnds, c.LastEnds, c.LastEnds.HasValue ? (int?)(c.LastEnds.Value - DateTime.Today).TotalDays : (int?)null,
                        c.VisCount, c.VisCount == 0 ? 0 : c.UnitsOut / c.VisCount, c.HasCall,
                        c.CountEvent,
                        ev == null ? (DateTime?)null : ev.EventDate,
                        ev == null ? "" : ev.Subject,
                        ev == null ? "" : ev.Comment,
                        ev == null ? "" : ev.Result,
                        c.NextEvent == null || c.NextEvent?.ExpiryDate == DateTime.MinValue ? "" : c.NextEvent.ExpiryDate.ToString(),
                        c.NextEvent?.Subject,
                        c.NextEvent?.Message,
                        c.Anthropometric, c.FromSite, Decimal.Round(c.Bonuses, 2),
                        c.DateOneTimeTicket != null ? c.DateOneTimeTicket.manager : (c.DateConsultation != null ? c.DateConsultation.manager : ""),
                        c.DateOneTimeTicket != null ? c.DateOneTimeTicket.CreatedOn : c.DateConsultation?.VisitDate,
                        c.Manager
                        );
                }
                return res;
            }
        }

        private static int? GetDaysToBirthday(DateTime? birthday)
        {
            if(birthday == null) return null;
            if(birthday.Value.Month == 2 && birthday.Value.Day == 29) birthday = birthday.Value.AddDays(1);
            birthday = new DateTime(DateTime.Today.Year, birthday.Value.Month, birthday.Value.Day);
            if(birthday < DateTime.Today) birthday = birthday.Value.AddYears(1);
            return (int)((birthday.Value - DateTime.Today).TotalDays);
        }

        public DataTable GetAllTicketsEx()
        {
            using(var context = new TonusEntities())
            {
                var cId = UserManagement.GetCompanyIdOrDefaultId(context);
                var res = new DataTable();
                res.ExtendedProperties.Add("EntityType", typeof(Ticket));
                res.Columns.Add("_id", typeof(Guid));
                res.Columns.Add("Фамилия", typeof(string));
                res.Columns.Add("Имя", typeof(string));
                res.Columns.Add("Отчество", typeof(string));
                res.Columns.Add("Номер карты", typeof(string));
                res.Columns.Add("Телефон", typeof(string));
                res.Columns.Add(ClubText, typeof(string));
                res.Columns.Add("Название абонемента", typeof(string));
                res.Columns.Add("Номер абонемента", typeof(string));
                res.Columns.Add("Дата покупки", typeof(DateTime));
                res.Columns.Add("Цена", typeof(decimal));
                res.Columns.Add("Скидка", typeof(decimal));
                res.Columns.Add("Стоимость", typeof(decimal));
                res.Columns.Add("Количество единиц", typeof(int));
                res.Columns.Add("Потрачено единиц", typeof(int));
                res.Columns.Add("Остаток единиц", typeof(int));
                res.Columns.Add("Количество гостевых", typeof(int));
                res.Columns.Add("Остаток гостевых", typeof(int));
                res.Columns.Add("Минут солярия", typeof(decimal));
                res.Columns.Add("Минут осталось", typeof(decimal));
                res.Columns.Add("Длительность", typeof(int));
                res.Columns.Add("Дней до окончания", typeof(int));
                res.Columns.Add("Оплачено", typeof(decimal));
                res.Columns.Add("Дата последнего платежа", typeof(DateTime));
                res.Columns.Add("Планируемая дата платежа", typeof(DateTime));
                res.Columns.Add("Остаток оплаты", typeof(decimal));
                res.Columns.Add("Активен", typeof(string));
                res.Columns.Add("Статус клиента", typeof(string));
                res.Columns.Add("Комментарий", typeof(string));
                res.Columns.Add("Рекламная группа", typeof(string));
                res.Columns.Add("Рекламный канал", typeof(string));
                res.Columns.Add("Комментарий к рекламе", typeof(string));
                res.Columns.Add("Клиент с оф.сайта", typeof(string));
                res.Columns.Add("Количество бонусов", typeof(string));
                res.Columns.Add("Менеджер", typeof(string));

                var today = DateTime.Today;
                var cs = context.Tickets.Where(i => i.CompanyId == cId)
                    .OrderBy(i => i.Customer.LastName)
                    .ThenBy(i => i.Customer.FirstName)
                    .Select(i => new
                    {
                        i.Id,
                        i.Customer.LastName,
                        i.Customer.FirstName,
                        i.Customer.MiddleName,
                        CustomerCard = i.Customer.CustomerCards.Where(j => j.IsActive).FirstOrDefault().CardBarcode,
                        i.Customer.Phone2,
                        i.TicketType.Name,
                        i.CreatedOn,
                        i.Price,
                        Division = i.Division.Name,
                        i.Number,
                        Discount = i.DiscountPercent * 100,
                        Cost = i.Price * (1 - i.DiscountPercent),
                        i.UnitsAmount,
                        UnitsSpent = i.UnitCharges.Sum(j => (int?)j.Charge),
                        i.GuestUnitsAmount,
                        GuestLeft = i.GuestUnitsAmount - i.UnitCharges.Sum(j => (int?)j.GuestCharge),
                        i.SolariumMinutes,
                        MinutesLeft = i.SolariumMinutes - i.MinutesCharges.Sum(j => (decimal?)j.MinutesCharged),
                        i.Length,
                        FinishDate = SqlFunctions.DateAdd("day", i.Length, i.StartDate),
                        Paid = i.TicketPayments.Sum(j => (decimal?)j.Amount),
                        LastPmt = i.TicketPayments.Max(j => (DateTime?)j.PaymentDate),
                        IsActive = i.IsActive && !i.ReturnDate.HasValue,
                        Status = i.Customer.CustomerStatuses.Select(j => j.Name),
                        i.InheritedTicketId,
                        LoanPlanDate = i.PlanningInstalmentDay ?? i.LastInstalmentDay,
                        i.Comment,
                        AdvertGroup = (i.Customer.AdvertType != null && i.Customer.AdvertType.AdvertGroup != null) ? i.Customer.AdvertType.AdvertGroup.Name : String.Empty,
                        AdvertType = i.Customer.AdvertType != null ? i.Customer.AdvertType.Name : String.Empty,
                        AdvertComment = i.Customer.AdvertComment,
                        FromSite = i.Customer.FromSite ? "Да" : "Нет",
                        Bonuses =  (i.Customer.BonusAccounts.Any()) ? i.Customer.BonusAccounts.Sum(j => j.Amount) : 0m,
                        Manager = i.Customer.ManagerId.HasValue ? i.Customer.Employee.BoundCustomer.LastName + " " + i.Customer.Employee.BoundCustomer.FirstName : "",
                    }).ToArray();

                foreach(var c in cs)
                {
                    res.Rows.Add(
                        c.Id, c.LastName, c.FirstName, c.MiddleName, c.CustomerCard, c.Phone2, c.Division,
                        c.Name, c.Number, c.CreatedOn, c.Price, c.Discount, c.Cost,
                        c.UnitsAmount, c.UnitsSpent, c.UnitsAmount - (c.UnitsSpent ?? 0),
 c.GuestUnitsAmount, c.GuestLeft,
                        c.SolariumMinutes, c.MinutesLeft,
                        c.Length, (c.FinishDate.HasValue && c.FinishDate > DateTime.Today) ? (int)(c.FinishDate.Value - DateTime.Today).TotalDays : (int?)null,
                        c.Paid, c.LastPmt, c.LoanPlanDate, c.Cost - c.Paid, (c.IsActive) ? "да" : "нет", String.Join("; ", c.Status)
                        , c.Comment
                        , c.AdvertGroup
                        , c.AdvertType
                        , c.AdvertComment
                        , c.FromSite
                        , Decimal.Round(c.Bonuses, 2)
                        , c.Manager);
                }
                return res;
            }
        }
    }
}
