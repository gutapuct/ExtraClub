using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using ExtraClub.ServiceModel;
using ExtraClub.Entities;
using System.Diagnostics;

using ExtraClub.ServiceModel.Schedule;
using System.Data.Objects;

namespace ExtraClub.ServerCore
{
    public static class ScheduleCore
    {
        public static void PostScheduleProposal(Guid customerId, Guid divisionId, Guid ticketId, ScheduleProposal scheduleProposal)
        {
            using(var context = new ExtraEntities())
            {
                //if(context.Companies.Count() > 1 && divisionId != Guid.Parse("12A4128D-8258-4341-89C4-1F49B13F068D"))
                //{
                //    throw new FaultException<string>("На центральном сервере запрещена запись на услуги.", "На центральном сервере запрещена запись на услуги.");
                //}
                context.ContextOptions.LazyLoadingEnabled = false;
                var ticket = context.Tickets.FirstOrDefault(i => i.Id == ticketId);

                var sa = new ScheduleAnalyzer(customerId, divisionId, ticketId, scheduleProposal.List.Min(i => i.StartTime));
                var fix = sa.FixSchedule(scheduleProposal.List.Select(i => new ScheduleProposalElement
                {
                    ConfigId = i.ConfigId,
                    EndTime = i.EndTime,
                    MovedByRules = i.MovedByRules,
                    StartTime = i.StartTime,
                    Treatment = new TreatmentProposal
                    {
                        ConfigId = i.Treatment.ConfigId,
                        Id = i.Treatment.Id,
                        Tag = i.Treatment.Tag
                    },
                    Price = i.Price
                }).ToList(), true);

                if(!CompareSchedules(scheduleProposal.List, fix))
                {
                    throw new FaultException<string>("Запись с такими условиями невозможна. Попробуйте сгенерировать расписание заново.", "Запись с такими условиями невозможна. Попробуйте сгенерировать расписание заново.");
                }


                if(ticket != null)
                {
                    Logger.Log("Ticket units: " + ticket.UnitsAmount);
                    var charges = context.UnitCharges.Where(i => i.TicketId == ticketId);
                    var spis = charges.Any() ? charges.Sum(i => i.Charge) : 0;
                    Logger.Log("Charged: " + spis);
                    var left = ticket.UnitsAmount - spis;

                    var planned = context.TreatmentEvents.Where(i => i.TicketId == ticketId && i.VisitStatus == 0).Select(i => i.TreatmentConfig.Price).ToArray();
                    if(planned.Any())
                    {
                        left -= planned.Sum();
                        Logger.Log("Planned: " + planned.Sum());
                    }
                    Logger.Log("Total left: " + left);

                    Logger.Log("Proposed: " + scheduleProposal.List.Sum(i =>
                    {
                        return context.TreatmentConfigs.Single(j => j.Id == i.ConfigId).Price;
                    }));


                    if(scheduleProposal.List.Sum(i =>
                    {
                        return context.TreatmentConfigs.Single(j => j.Id == i.ConfigId).Price;
                    }) > left)
                    {
                        throw new FaultException<string>(Localization.Resources.NoEnoughUnits,
                            Localization.Resources.NoEnoughUnits);
                    }

                }

                var user = UserManagement.GetUser(context);
                var div = context.Divisions.Single(i => i.Id == divisionId);
                if(div.OpenDate.HasValue && div.OpenDate.Value > scheduleProposal.List.Min(i => i.StartTime))
                {
                    throw new FaultException<string>(Localization.Resources.NoBookBefore,
                        Localization.Resources.NoBookBefore);
                }

                if(ticket != null)
                {
                    if(ticket.DivisionId != divisionId)
                    {
                        var ticketClubs = context.Companies.Where(i => i.CompanyId == div.CompanyId).Select(i => i.TicketsClubs).Single();
                        if(!ticketClubs)
                        {
                            throw new FaultException<string>(Localization.Resources.NoBookOther,
                               Localization.Resources.NoBookOther);
                        }
                    }
                }
                var customer = context.Customers.SingleOrDefault(i => i.Id == customerId);
                //bool flag = false;
                foreach(var spe in scheduleProposal.List)
                {
                    var te = new TreatmentEvent
                    {
                        CompanyId = customer.CompanyId,
                        AuthorId = user.UserId,
                        CreatedOn = DateTime.Now,
                        CustomerId = customerId,
                        Deleted = false,
                        DivisionId = divisionId,
                        Id = Guid.NewGuid(),
                        Modified = false,
                        TicketId = ticketId == Guid.Empty ? (Guid?)null : ticketId,
                        TreatmentId = spe.Treatment.Id,
                        VisitDate = spe.StartTime,
                        VisitStatus = 0,
                        TreatmentConfigId = spe.ConfigId,
                        ProgramId = scheduleProposal.ProgramId == Guid.Empty /*|| flag*/ ? (Guid?)null : scheduleProposal.ProgramId
                    };
                    //flag = true;
                    context.TreatmentEvents.AddObject(te);
                }
                var tn = "Резерв";
                if(ticket != null)
                {
                    tn = ticket.Number;
                }
                Logger.DBLog("Добавление записи для клиента {0} в клуб {1} на абонемент {2}, всего процедур {3}", customer.FullName, div.Name, tn, scheduleProposal.List.Count);

                context.SaveChanges();
            }
        }

        private static bool CompareSchedules(List<ScheduleProposalElement> list, List<ScheduleProposalElement> fix)
        {
            //Skip chech for parallel proposals
            for(int i = 0; i < list.Count; i++)
                for(int j = i + 1; j < list.Count; j++)
                {
                    if(list[i].StartTime < list[j].EndTime && list[j].StartTime < list[j].EndTime)
                        return true;
                }

            if(list.Count != fix.Count) return false;
            for(int i = 0; i < list.Count; i++)
            {
                if(list[i].StartTime != fix[i].StartTime) return false;
                if(list[i].ConfigId != fix[i].ConfigId) return false;
            }
            return true;
        }

        public static void CancelTreatmentEvents(List<Guid> events)
        {
            using(var context = new ExtraEntities())
            {
                context.ContextOptions.LazyLoadingEnabled = false;
                var user = UserManagement.GetUser(context, "Company");
                foreach(var evId in events)
                {
                    var ev = context.TreatmentEvents.Include("Treatment").SingleOrDefault(te => te.Id == evId);
                    if(ev != null && ev.VisitStatus == (short)TreatmentEventStatus.Planned && ev.VisitDate >= DateTime.Now.AddHours(user.Company.MaxCancellationPeriod))
                    {
                        ev.VisitStatus = (short)TreatmentEventStatus.Canceled;
                        ev.ProgramId = null;
                        ev.ModifiedBy = user.UserId;
                    }
                    var cus = context.Customers.Where(i => i.Id == ev.CustomerId).FirstOrDefault();
                    Logger.DBLog("Отмена записи на услугу. Дата: {0}, процедура: {1}. Клиент: {3} (телефон: {4}). Отменил: {2}.", ev.VisitDate, ev.Treatment.DisplayName, user.FullName, cus.LastName + " " + cus.FirstName, cus.Phone2);
                }
                context.SaveChanges();
            }
        }

        public static Guid? GetTreatmentProgramIdForCustomer(Guid customerId)
        {
            return null;
        }

        public static List<TreatmentsParalleling> GetParallelingRules()
        {
            using(var context = new ExtraEntities())
            {
                var res = new List<TreatmentsParalleling>();
                context.TreatmentTypes.Where(i => i.TreatmentTypes.Count > 0).ToList().ForEach(i =>
                {
                    foreach(var j in i.TreatmentTypes)
                    {
                        res.Add(new TreatmentsParalleling(i, j));
                    }
                }
                );
                return res;
            }
        }

        public static void DeleteParallelRule(TreatmentsParalleling rule)
        {
            using(var context = new ExtraEntities())
            {
                var tt = context.TreatmentTypes.Single(i => i.Id == rule.Type1Id);
                var tt1 = tt.TreatmentTypes.SingleOrDefault(i => i.Id == rule.Type2Id);
                if(tt1 != null)
                {
                    tt.TreatmentTypes.Remove(tt1);
                    context.SaveChanges();
                }
            }
        }

        public static void PostTreatmentsParalleling(TreatmentsParalleling old, TreatmentsParalleling rule)
        {
            using(var context = new ExtraEntities())
            {
                if(old != null && old.Type1Id != Guid.Empty)
                {
                    DeleteParallelRule(old);
                }

                var tt = context.TreatmentTypes.Single(i => i.Id == rule.Type1Id);
                var tt1 = tt.TreatmentTypes.SingleOrDefault(i => i.Id == rule.Type2Id);
                if(tt1 == null)
                {
                    tt.TreatmentTypes.Add(context.TreatmentTypes.Single(i => i.Id == rule.Type2Id));
                    context.SaveChanges();
                }
            }
        }

        public static void PostTreatmentBreakdown(Guid treatmentId)
        {
            using(var context = new ExtraEntities())
            {
                var user = UserManagement.GetUser(context);

                var treatment = context.Treatments.Single(i => i.Id == treatmentId);
                treatment.IsActive = false;

                var groups = context.TreatmentEvents
                    .Include("TreatmentConfig")
                    .Where(i => i.TreatmentId == treatmentId && i.VisitDate >= DateTime.Now && i.VisitStatus == 0)
                    .GroupBy(i => i.Customer)
                    .Select(i => new { CustomerId = i.Key.Id, Events = i })
                    .ToList();


                foreach(var group in groups)
                {
                    var s = Localization.Resources.TreatmentCancelling + ":\n\n";
                    foreach(var j in group.Events)
                    {
                        j.VisitStatus = (short)TreatmentEventStatus.Canceled;
                        s += String.Format(Localization.Resources.BookingCancelled + "\r\n", j.TreatmentConfig.Name, j.VisitDate);
                    }

                    var minDate = group.Events.Min(j => j.VisitDate);

                    var ne = new CustomerNotification
                    {
                        AuthorId = user.UserId,
                        CompanyId = treatment.CompanyId,
                        CreatedOn = DateTime.Now,
                        CustomerId = group.CustomerId,
                        Id = Guid.NewGuid(),
                        Message = s,
                        Subject = Localization.Resources.TreatmentCancelling,
                        ExpiryDate = minDate,
                        Priority = 1
                    };

                    if(minDate.Date == DateTime.Today)
                    {
                        context.BonusAccounts.AddObject(new BonusAccount
                        {
                            Amount = 100,
                            AuthorId = user.UserId,
                            CompanyId = user.CompanyId,
                            CreatedOn = DateTime.Now,
                            CustomerId = group.CustomerId,
                            Description = String.Format("Начисление в связи с поломкой оборудования {0}", treatment.Tag),
                            Id = Guid.NewGuid()
                        });
                    }

                    context.CustomerNotifications.AddObject(ne);
                }

                context.SaveChanges();
                Logger.DBLog("Поломка тренажера {0}", treatment.Tag);

            }
        }

        public static void PostClubClosing(Guid divisionId, DateTime start, DateTime end, string cause)
        {
            using(var context = new ExtraEntities())
            {
                var user = UserManagement.GetUser(context);

                var groups = context.TreatmentEvents
                    .Include("TreatmentConfig")
                    .Where(i => i.DivisionId == divisionId && i.VisitDate >= start && i.VisitDate <= end && i.VisitStatus == 0)
                    .GroupBy(i => i.Customer)
                    .Select(i => new { CustomerId = i.Key.Id, Events = i })
                    .ToList();

                foreach(var group in groups)
                {
                    var s = Localization.Resources.ClubClose + ":\n\n";
                    foreach(var j in group.Events)
                    {
                        j.VisitStatus = (short)TreatmentEventStatus.Canceled;
                        s += String.Format(Localization.Resources.BookingCancelled + "\r\n", j.TreatmentConfig.Name, j.VisitDate);
                    }
                    s += Localization.Resources.ClubCloseCause + ": " + cause;
                    var ne = new CustomerNotification
                    {
                        AuthorId = user.UserId,
                        CompanyId = user.CompanyId,
                        CreatedOn = DateTime.Now,
                        CustomerId = group.CustomerId,
                        Id = Guid.NewGuid(),
                        Message = s,
                        Subject = Localization.Resources.ClubClose,
                        ExpiryDate = group.Events.Min(j => j.VisitDate),
                        Priority = 1
                    };
                    context.CustomerNotifications.AddObject(ne);

                    context.BonusAccounts.AddObject(new BonusAccount
                    {
                        Amount = 150,
                        AuthorId = user.UserId,
                        CompanyId = user.CompanyId,
                        CreatedOn = DateTime.Now,
                        CustomerId = group.CustomerId,
                        Description = String.Format("Начисление в связи с закрытием клуба {0:dd.MM.yyyy} - {1:dd.MM.yyyy}", start, end),
                        Id = Guid.NewGuid()
                    });
                }

                context.SaveChanges();
            }
        }

        public static List<TreatmentEvent> GetDivisionTreatmetnsVisits(Guid divisionId, DateTime start, DateTime finish)
        {
            using(var context = new ExtraEntities())
            {
                return context.TreatmentEvents
                    .Include("Ticket")
                    .Include("TreatmentConfig")
                    .Include("Treatment")
                    .Include("Treatment.TreatmentType")
                    .Include("Customer")
                    .Include("Customer.CustomerCards")
                    .Include("TreatmentProgram")
                    .Where(i => i.DivisionId == divisionId && i.VisitDate >= start && i.VisitDate <= finish).Where(i => i.VisitStatus != 1).ToList().Init();
            }
        }

        public static TreatmentEvent GetTreatmentEventById(Guid treatmentEventId)
        {
            using(var context = new ExtraEntities())
            {
                var res = context.TreatmentEvents.Single(i => i.Id == treatmentEventId);
                res.Init();
                return res;
            }
        }

        public static void PostTreatmentEventChange(Guid treatmentEventId, DateTime newTime)
        {
            using(var context = new ExtraEntities())
            {
                var res = context.TreatmentEvents.Single(i => i.Id == treatmentEventId);
                res.Init();
                res.VisitDate = newTime;

                Logger.DBLog("Редактирование записи на процедуру {0} {1}, новое время {1}", res.VisitStatus, res.Treatment.NameWithTag, newTime);

                context.SaveChanges();
            }
        }

        public static void SetTreatmentEventColor(Guid eventId, int colorId)
        {
            using(var context = new ExtraEntities())
            {
                var res = context.TreatmentEvents.SingleOrDefault(i => i.Id == eventId);
                if(res == null) return;
                res.CustomColorId = colorId;
                context.SaveChanges();
            }
        }

        public static void MarkTreatmentsVisited(Guid[] visited)
        {
            using(var context = new ExtraEntities())
            {
                var user = UserManagement.GetUser(context);
                foreach(var eId in visited)
                {
                    var ev = context.TreatmentEvents.Include("Ticket").Single(i => i.Id == eId);
                    if(ev.TicketId.HasValue)
                    {
                        if(!ev.Ticket.IsActive && !ev.Ticket.StartDate.HasValue)
                        {
                            ev.Ticket.IsActive = true;
                            ev.Ticket.StartDate = DateTime.Today;
                        }

                        ev.VisitStatus = (short)TreatmentEventStatus.Completed;
                        if(!new ExtraEntities().UnitCharges.Any(i => i.EventId == ev.Id))
                        {
                            var x = new ExtraEntities().ExecuteStoreCommand(@"
                                Insert into UnitCharges
                                select {4},{2},{6},{1},0,{5},{3},{0},{7}
                                where not exists (select * from UnitCharges where EventId={7})
                                ", user.UserId,//0
                                (int)ev.TreatmentConfig.Price,
                                 ev.CompanyId,//2
                                DateTime.Now,
                                 Guid.NewGuid(),//4
                                 String.Format(Localization.Resources.TreVisit, ev.TreatmentConfig.Name, ev.VisitDate),
                                 ev.TicketId.Value,//6
                                 ev.Id);
                        }
                        context.SaveChanges();
                    }
                }
            }
        }

        public static List<ScheduleProposalElement> GetAvailableParallels(Guid treatmentEventId)
        {
            using(var context = new ExtraEntities())
            {
                var res = new List<ScheduleProposalElement>();
                var tr = context.TreatmentEvents.Single(i => i.Id == treatmentEventId);
                if(tr.VisitStatus != 0) return res;
                if(!tr.TicketId.HasValue) return res;
                tr.VisitStatus = 1;
                context.SaveChanges();
                try
                {
                    foreach(var tc in tr.TreatmentConfig.TreatmentType.TreatmentTypes.SelectMany(i => i.TreatmentConfigs).Where(i => i.IsActive))
                    {
                        try
                        {
                            var sa = new ScheduleAnalyzer(tr.CustomerId, tr.DivisionId, tr.TicketId.Value, tr.VisitDate);
                            var sp = sa.GetScheduleProposals(true, new List<Guid> { tr.TreatmentConfigId, tc.Id }, true);
                            if(sp.List.Count == 0) continue;
                            var spe = sp.List.First();
                            if(spe.List.Count != 2) continue;
                            var main = spe.List.Single(i => i.ConfigId == tr.TreatmentConfigId);
                            if(main.StartTime != tr.VisitDate) continue;
                            var adding = spe.List.Single(i => i != main);
                            if(main.StartTime <= adding.StartTime && main.EndTime >= adding.EndTime)
                            {
                                res.Add(adding);
                            }
                        }
                        catch(FaultException)
                        {
                        }
                    }
                }
                finally
                {
                    tr.VisitStatus = 0;
                    context.SaveChanges();
                }
                return res;
            }
        }

        public static Guid PostParallelSigning(Guid originalEventId, Guid configId, Guid treatmentId, DateTime startTime)
        {
            using(var context = new ExtraEntities())
            {
                var te = context.TreatmentEvents.Single(i => i.Id == originalEventId);
                var user = UserManagement.GetUser(context);

                ValudateAvailableUnits(context, te.Ticket, configId);

                var newTe = new TreatmentEvent
                {
                    AuthorId = user.UserId,
                    CompanyId = te.CompanyId,
                    CreatedOn = DateTime.Now,
                    CustomerId = te.CustomerId,
                    DivisionId = te.DivisionId,
                    Id = Guid.NewGuid(),
                    TicketId = te.TicketId,
                    TreatmentConfigId = configId,
                    TreatmentId = treatmentId,
                    VisitDate = startTime,
                    VisitStatus = 0
                };

                context.TreatmentEvents.AddObject(newTe);
                context.SaveChanges();

                return newTe.Id;
            }
        }

        private static void ValudateAvailableUnits(ExtraEntities context, Ticket ticket, Guid configId)
        {
            var charges = context.UnitCharges.Where(i => i.TicketId == ticket.Id);
            var spis = charges.Any() ? charges.Sum(i => i.Charge) : 0;

            var left = ticket.UnitsAmount - spis;

            var planned = ticket.TreatmentEvents.Where(i => i.VisitStatus == 0).Select(i => i.TreatmentConfig.Price).ToArray();
            if(planned.Any()) left -= planned.Sum();
            var tp = context.TreatmentConfigs.FirstOrDefault(i => i.Id == configId).Price;
            if(tp > left)
            {
                throw new FaultException<string>(Localization.Resources.NoEnoughUnits,
                    Localization.Resources.NoEnoughUnits);
            }
        }

        public static TreatmentEvent GetCustomerPlanningForTreatment(Guid customerId, Guid treatmentId)
        {
            using(var context = new ExtraEntities())
            {
                var now = DateTime.Now;
                var today = DateTime.Today;
                var te = context.TreatmentEvents
                    .Where(i => i.CustomerId == customerId && i.TreatmentId == treatmentId && i.VisitStatus == 0 && i.VisitDate <= now)
                    .ToList()
                    .Where(i => i.VisitDate.AddMinutes(i.TreatmentConfig.FullDuration) > now).FirstOrDefault();

                //Если на одну логическую единицу много физических, проверяем, чо как
                var t = context.Treatments.Single(i => i.Id == treatmentId);
                if(t.MaxCustomers > 1)
                {
                    te = context.TreatmentEvents
                        .Where(i => i.CustomerId == customerId && i.TreatmentId == treatmentId && i.VisitStatus == 2 && i.VisitDate <= now)
                        .ToList()
                        .Where(i => i.VisitDate.AddMinutes(i.TreatmentConfig.FullDuration) > now).FirstOrDefault();
                }

                if(te == null)
                {
                    //сейчас тренажер свободен и есть запись на ближайшее время
                    var tr = context.Treatments.Single(i => i.Id == treatmentId);
                    if(!tr.TreatmentEvents.Where(i => i.VisitDate >= today && i.VisitDate < now && i.VisitStatus != 1).ToList().Any(i => i.VisitDate.AddMinutes(i.TreatmentConfig.FullDuration) > now))
                    {
                        var plan = tr.TreatmentEvents.OrderBy(i => i.VisitDate).FirstOrDefault(i => i.VisitDate >= now && i.VisitStatus == 0);
                        if(plan != null && plan.CustomerId == customerId)
                        {
                            plan.Init();
                            return plan;
                        }
                    }
                }

                if(te != null) te.Init();
                return te;
            }
        }

        public static void PostTreatmentStart(Guid treatmentId, DateTime newTime)
        {
            using(var context = new ExtraEntities())
            {
                var user = UserManagement.GetUser(context);

                var ev = context.TreatmentEvents.Single(i => i.Id == treatmentId);
                ev.VisitStatus = (short)TreatmentEventStatus.Completed;
                ev.VisitDate = newTime;
                if(!context.UnitCharges.Any(i => i.EventId == ev.Id))
                {
                    var uc = new UnitCharge
                    {
                        AuthorId = user.UserId,
                        Charge = (int)ev.TreatmentConfig.Price,
                        CompanyId = ev.CompanyId,
                        Date = DateTime.Now,
                        Id = Guid.NewGuid(),
                        Reason = String.Format(Localization.Resources.TreVisit, ev.TreatmentConfig.Name, ev.VisitDate),
                        TicketId = ev.TicketId.Value,
                        EventId = ev.Id
                    };
                    context.UnitCharges.AddObject(uc);
                }
                context.SaveChanges();
            }
        }

        public static HWProposal GetHWProposal(Guid treatmentId, Guid customerId)
        {
            using(var context = new ExtraEntities())
            {
                var treatment = context.Treatments.SingleOrDefault(i => i.Id == treatmentId);
                if(treatment == null)
                {
                    return new HWProposal { ErrorMessage = "Тренажер не найден! Обратитесь к администратору." };
                }

                if(treatment.TreatmentType.TreatmentConfigs.Where(i => i.IsActive).Count() != 1)
                {
                    return new HWProposal { ErrorMessage = "На данном тренажере возможны разные занятия, обратитесь к администратору." };
                }

                var tc = treatment.TreatmentType.TreatmentConfigs.First();

                var customer = context.Customers.SingleOrDefault(i => i.Id == customerId);
                if(customer == null)
                {
                    return new HWProposal { ErrorMessage = "Карта не идентифицирована, обратитесь к администратору." };
                }

                customer.Init();

                var ticket = customer.Tickets.FirstOrDefault(i => i.IsActive && !i.TicketType.TreatmentTypes.Any(j => j.Id == treatment.TreatmentTypeId) && i.DivisionId == treatment.DivisionId);

                if(ticket == null)
                {
                    return new HWProposal { ErrorMessage = "Услуга недоступна для вашего абонемента." };
                }

                if(customer.ContraIndications.SelectMany(i => i.TreatmentTypes).Select(i => i.Id).Any(i => i == treatment.TreatmentTypeId))
                {
                    return new HWProposal { ErrorMessage = "Тренажер недоступен при указанных Вами противопоказаниях." };
                }

                //если пока тренажер свободен - записываем с текущего времени на полное занятие
                //если тренажер вот-вот будет занят, записываем на чуть пораньше, предлагаем усеченное занятие
                //если на сегодня уже есть запись на данный тритментконфиг, подставляем его в реплейсмент

                var now = DateTime.Now;
                var today = DateTime.Today;
                var tomorrow = DateTime.Today.AddDays(1);

                var rep = treatment.TreatmentEvents.FirstOrDefault(i => i.CustomerId == customer.Id && i.VisitDate >= now && i.VisitDate < tomorrow && i.VisitStatus == 0);
                var repId = rep == null ? (Guid?)null : rep.Id;

                if(treatment.TreatmentEvents
                    .Where(i => i.VisitDate >= today && i.VisitStatus != 1)
                    .ToList()
                    .Count(i => Core.DatesIntersects(now, now.AddMinutes(tc.FullDuration), i.VisitDate, i.VisitDate.AddMinutes(tc.FullDuration))) <= treatment.MaxCustomers - 1)
                {
                    return new HWProposal
                    {
                        TicketId = ticket.Id,
                        VisitDate = now,
                        ReplacingId = repId,
                        Message = String.Format("Занятие закончится в {0:H:mm}, будет снято {1:n0} единиц. Для подтверждения приложите карту еще раз.", now.AddMinutes(tc.FullDuration), tc.Price),
                        Line2 = String.Format("{0:H:mm} - {1:H:mm}", now, now.AddMinutes(tc.FullDuration))
                    };
                }
                else
                {
                    //проверка на занятость в текущий момент!
                    if(treatment.TreatmentEvents
                        .Where(i => i.VisitDate > today && i.VisitDate < now && i.VisitStatus != 1 && i.VisitStatus != 3 && i.VisitStatus != 1)
                        .ToList()
                        .Count(i => i.VisitDate.AddMinutes(tc.FullDuration) > now) > treatment.MaxCustomers - 1)
                    {
                        return new HWProposal { ErrorMessage = "Тренажер занят!" };
                    }
                    var t = BindTimeEx(tc, treatment.Division, now);
                    return new HWProposal
                    {
                        TicketId = ticket.Id,
                        VisitDate = t,
                        ReplacingId = repId,
                        Message = String.Format("Занятие продлится на {2:n0} минут меньше, закончится в {0:H:mm}, будет снято {1:n0} единиц. Для подтверждения приложите карту еще раз.", t.AddMinutes(tc.FullDuration), tc.Price, (now - t).TotalMinutes),
                        Line2 = String.Format("{0:H:mm} - {1:H:mm}", now, t.AddMinutes(tc.FullDuration))
                    };
                    //return new HWProposal { ErrorMessage = String.Format("Занятие было бы с {0:H:mm} по {1:H:mm}", t, t.AddMinutes(tc.FullDuration)) };
                }
            }
        }

        private static DateTime BindTimeEx(TreatmentConfig tc, Division division, DateTime dest)
        {
            var time = DateTime.Today.Add(division.OpenTime ?? new TimeSpan(0));
            while(true)
            {
                if((dest - time).TotalMinutes < tc.FullDuration) return time;
                time = time.AddMinutes(tc.TreatmentType.Duration);
            }
        }

        public static TreatmentEvent PostNewTreatmentEvent(Guid ticketId, Guid treatmentId, DateTime visitStart)
        {
            using(var context = new ExtraEntities())
            {
                var user = UserManagement.GetUser(context);
                var ticket = context.Tickets.SingleOrDefault(i => i.Id == ticketId);
                var treatment = context.Treatments.Single(i => i.Id == treatmentId);

                var tc = treatment.TreatmentType.TreatmentConfigs.FirstOrDefault();

                var te = new TreatmentEvent
                {
                    AuthorId = user.UserId,
                    CompanyId = treatment.CompanyId,
                    CreatedOn = DateTime.Now,
                    CustomerId = ticket.CustomerId,
                    DivisionId = treatment.DivisionId,
                    Id = Guid.NewGuid(),
                    TicketId = ticketId,
                    TreatmentConfigId = tc.Id,
                    TreatmentId = treatmentId,
                    VisitDate = visitStart,
                    VisitStatus = 0
                };

                context.TreatmentEvents.AddObject(te);

                context.SaveChanges();

                var res = new ExtraEntities().TreatmentEvents.Single(i => i.Id == te.Id);
                res.Init();
                return res;
            }
        }

        public static TreatmentEvent GetCurrentTreatmentEvent(Guid treatmentId)
        {
            using(var context = new ExtraEntities())
            {
                var day = DateTime.Today;
                return context.TreatmentEvents
                    .Where(i => i.TreatmentId == treatmentId && i.VisitStatus == 2 && i.VisitDate >= day)
                    .ToList().Init()
                    .FirstOrDefault(i => i.VisitDate < DateTime.Now && i.EndTime > DateTime.Now);
            }
        }

        public static int CorrectAvailableTreatmentLength(Guid eventId)
        {
            using(var context = new ExtraEntities())
            {
                var te = context.TreatmentEvents.Single(i => i.Id == eventId);

                te.VisitDate = DateTime.Now;

                var next = context.TreatmentEvents.OrderBy(i => i.VisitDate)
                    .FirstOrDefault(i => i.TreatmentId == te.TreatmentId && i.VisitDate > te.VisitDate);

                var res = te.TreatmentConfig.FullDuration;
                if(te.VisitDate.AddMinutes(te.TreatmentConfig.FullDuration) > next.VisitDate)
                {
                    res = (int)(next.VisitDate - DateTime.Now).TotalMinutes;
                }
                context.SaveChanges();
                return res;
            }
        }

        public static List<Ticket> GetTicketsForPlanning(Guid customerId, Guid divisionId)
        {
            using(var context = new ExtraEntities())
            {
                var tickets = context.Tickets.Where(i => i.CustomerId == customerId && (i.Division.Company.TicketsClubs || i.DivisionId == divisionId)).OrderBy(i => i.CreatedOn).ToList();
                tickets.ForEach(i => i.InitDetails());
                return tickets.Where(t => t.Status == TicketStatus.Available || t.Status == TicketStatus.Active || t.Status == TicketStatus.Unpaid || t.Status == TicketStatus.Freezed).ToList();
            }
        }

        public static void SetTreatmentAsVisited(Guid eventId)
        {
            using(var context = new ExtraEntities())
            {
                var t = context.TreatmentEvents.SingleOrDefault(i => i.Id == eventId && i.VisitStatus == 3);
                if(t != null)
                {
                    t.VisitStatus = 2;
                    context.SaveChanges();
                }
            }
        }

        public static List<ScheduleProposalElement> FixSchedule(Guid divisionId, Guid customerId, Guid ticketId, List<ScheduleProposalElement> list)
        {
            var sa = new ScheduleAnalyzer(customerId, divisionId, ticketId, list.Min(i => i.StartTime));
            return sa.FixSchedule(list, false);
        }

        public static ScheduleProposalResult GetScheduleProposals(Guid customerId, Guid divId, Guid ticketId, DateTime visitDate, bool isParallelAllowed, List<Guid> treatments, bool isOptimalAllowed)
        {
            var sa = new ScheduleAnalyzer(customerId, divId, ticketId, visitDate);
            return sa.GetScheduleProposals(isParallelAllowed, treatments, isOptimalAllowed);
        }

        public static string GetLastVisitText(Guid customerId)
        {
            using(var context = new ExtraEntities())
            {
                var lastVisit = context.CustomerVisits.OrderByDescending(i => i.InTime).Select(i => (DateTime?)i.InTime).FirstOrDefault();
                if(!lastVisit.HasValue)
                {
                    return "Клиент еще не посетил ни одной тренировки";
                }
                var date = lastVisit.Value.Date;
                var next = lastVisit.Value.Date.AddDays(1);
                var treatments = context.TreatmentEvents.Where(i => i.CustomerId == customerId && i.VisitStatus == 2 && i.VisitDate >= date && i.VisitDate < next)
                    .Select(i => new { i.TreatmentConfig.Name, i.VisitDate }).OrderBy(i => i.VisitDate).ToList();
                return String.Join("\n", treatments.Select(i => String.Format("{0: d.MM.yyyy HH:mm} - {1}", i.VisitDate, i.Name)));
            }
        }
    }
}