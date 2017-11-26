using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using ExtraClub.Entities;
using ExtraClub.ServiceModel;
using ExtraClub.ServiceModel.Schedule;

namespace ExtraClub.ServerCore
{
    internal class ScheduleAnalyzer
    {
        private DateTime VisitTime;
        private DateTime VisitDate;
        private Guid CustomerId;
        private Guid DivisionId;
        private Guid TicketId;

        private int MaxTreatmentReserve;
        private Division Division;
        private Customer Customer;
        private Ticket Ticket;
        private TreatmentEvent[] TicketEvents;
        private Dictionary<Guid, List<Treatment>> TreatmentGroups;
        private Dictionary<Guid, List<TreatementEventInfo>> DivisionTreatmentEvents;
        private Dictionary<Guid, HashSet<DateTime>> DivisionTreatmentSchedule;
        private Dictionary<Guid, List<TreatmentEvent>> CustomerEvents;
        private TreatmentEvent[] CustomerEventsRaw;

        private static TreatmentSeqRest[] TreatmentSeqRests;
        private static Dictionary<Guid, TreatmentConfig> AllTreatmentConfigs;
        private static List<TreatmentSeqRest> DurationRestsList;
        private static Dictionary<Guid, List<TreatmentSeqRest>> DurationRests;
        private static List<TreatmentSeqRest> SerRests;
        private static Dictionary<Guid, int> TreatmentTypeDurations;
        private static Dictionary<Guid, int> TreatmentConfigFullDurations;


        public ScheduleAnalyzer(Guid customerId, Guid divisionId, Guid ticketId, DateTime visitTime)
        {
            VisitTime = visitTime;
            VisitDate = VisitTime.Date;
            CustomerId = customerId;
            DivisionId = divisionId;
            TicketId = ticketId;

            var context = new ExtraEntities();

            if(ticketId != Guid.Empty)
            {
                Ticket = context.Tickets
                    .Include("TicketType")
                    .Include("TicketType.TreatmentTypes")
                    .Include("TicketType.TicketTypeLimits")
                    .Include("Instalment")
                    .SingleOrDefault(i => i.Id == ticketId && (i.DivisionId == divisionId || i.Division.Company.TicketsClubs));
            }

            if(Ticket != null)
            {
                Ticket.InitDetails();

                if(Ticket.TicketType.TicketTypeLimits.Any())
                {
                    TicketEvents = context.TreatmentEvents.Where(i => i.TicketId == ticketId && i.VisitStatus != 1).ToArray();
                }
            }

            MaxTreatmentReserve = context.Users.Where(u => u.UserName.ToLower() == OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name).Select(i => i.Company.MaxTreatmentReserve).FirstOrDefault();

            Customer = context.Customers.Include("ContraIndications").First(c => c.Id == customerId);
            Division = context.Divisions.Single(i => i.Id == divisionId);

            ValidateTicketTime(VisitTime, VisitTime);

            TreatmentGroups = GenerateTreatmentGroups(context);
            DivisionTreatmentEvents = GetDivisionTreatmentEvents(context);
            DivisionTreatmentSchedule = GenerateDivisionSchedule();
            CustomerEventsRaw = context.TreatmentEvents
                .Where(t => t.CustomerId == customerId && EntityFunctions.TruncateTime(t.VisitDate) == VisitDate && t.VisitStatus != 1).ToArray();


            if(TreatmentSeqRests == null)
            {
                TreatmentSeqRests = context.TreatmentSeqRests.Include("Treatment1Type").Include("Treatment2Type").Where(ts => ts.Amount.HasValue).ToArray();
            }
            if(AllTreatmentConfigs == null)
            {
                AllTreatmentConfigs = context.TreatmentConfigs.ToDictionary(t => t.Id, t => t);
            }
            if(DurationRestsList == null)
            {
                DurationRestsList = context.TreatmentSeqRests.Where(ts => ts.Interval.HasValue).ToList();
            }
            if(DurationRests == null)
            {
                DurationRests = GenerateDurationRests();
            }
            if(SerRests == null)
            {
                SerRests = context.TreatmentSeqRests.Where(ts => !ts.Interval.HasValue && !ts.Amount.HasValue).ToList();
            }
            if(TreatmentTypeDurations == null)
            {
                TreatmentTypeDurations = context.TreatmentTypes.ToDictionary(i => i.Id, i => i.Duration);
            }
            if(TreatmentConfigFullDurations == null)
            {
                TreatmentConfigFullDurations = context.TreatmentConfigs.ToDictionary(i => i.Id, i => i.LengthCoeff * i.TreatmentType.Duration);
            }

            CustomerEvents = GenerateCustomerEvents(context);
        }

        public ScheduleProposalResult GetScheduleProposals(bool allowParalleling, List<Guid> treatmentConfigIds, bool isOptimalAllowed)
        {
            if(treatmentConfigIds.Count > 9) isOptimalAllowed = false;

            if(Ticket == null || TicketId == Guid.Empty)
            {
                if(treatmentConfigIds.Count > MaxTreatmentReserve)
                {
                    var str = String.Format("Максимум процедур без абонемента: " + MaxTreatmentReserve);
                    throw new FaultException<string>(str, str);
                }
            }

            var res = new List<ScheduleProposal>();
            var sw = Stopwatch.StartNew();
            string msg = null;

            var treatmentTypeIds = AllTreatmentConfigs.Values.Where(j => treatmentConfigIds.Contains(j.Id)).Select(i => i.TreatmentTypeId).ToArray();


            ValidateAvail(treatmentConfigIds, treatmentTypeIds);
            ValidateLimits(treatmentConfigIds);

            var treatmentConfigsList = treatmentConfigIds.Select(i => AllTreatmentConfigs[i]).ToList();

            ValidateInstalment(treatmentConfigsList);

            var gen = isOptimalAllowed ? GenerateRotatingItemsEx(treatmentConfigsList) : GenerateSingleItem(treatmentConfigsList);

            var mappingOfConfigsToTypes = treatmentConfigsList.Distinct().ToDictionary(i => i.Id, i => i.TreatmentTypeId);

            foreach(var list in gen)
            {
                if(!ValidateSequence(list)) continue;
                DateTime time = VisitTime;
                var proposal = new ScheduleProposal();
                foreach(var config in list)
                {
                    bool moved;
                    do
                    {
                        time = BindTime(config, time, proposal, allowParalleling, mappingOfConfigsToTypes, out moved);
                    } while(IsCustomerBusyForTime(ref time, TreatmentConfigFullDurations[config.Id]));

                    var treatment = GetFreeTreatmentByType(config, ref time);

                    if(treatment == null) continue;

                    proposal.List.Add(new ScheduleProposalElement
                    {
                        Treatment = new TreatmentProposal { Id = treatment.Id, Tag = config.TreatmentType.Name + " " + treatment.Tag, ConfigId = config.Id },
                        StartTime = time,
                        EndTime = time.AddMinutes(config.FullDuration),
                        ConfigId = config.Id,
                        MovedByRules = moved,
                        Price = (int)Math.Round(config.Price)
                    });
                    if(!allowParalleling)
                    {
                        time = time.AddMinutes(config.FullDuration);
                    }
                }
                res.Add(proposal);
            }
            try
            {
                if(res.Any() && res.First().List.Any())
                {
                    var endTime = GetCloseTime(res.First().List.First().StartTime.Date);

                    foreach(var j in res.ToList())
                    {
                        if(j.List.Max(i => i.EndTime.TimeOfDay) > endTime)
                        {
                            res.Remove(j);
                            msg = "Некоторые результаты не подошли по времени действия абонемента!";
                        }
                    }
                }
            }
            catch { }
            sw.Stop();

            Debug.WriteLine(String.Format("Generation proposals for {0} treatments took {1} ms", treatmentConfigIds.Count, sw.ElapsedMilliseconds));
            if(res.Count > 0) res[0].Prefer = true;
            res.Sort();
            if(res.Count > 10)
            {
                res = res.Take(10).ToList();
            }
            if(res.Any(i => i.List.Any(j => j.StartTime.Date > VisitTime.Date)))
            {
                msg = String.Format("Некоторые процедуры будут запланированы позднее {0:d MMMM}!", VisitTime);
            }
            return new ScheduleProposalResult { List = res, Result = msg };
        }

        private void ValidateInstalment(List<TreatmentConfig> treatmentConfigsList)
        {
            if(Ticket == null || !Ticket.InstalmentId.HasValue) return;

            var sum = treatmentConfigsList.Sum(i => (decimal?)i.Price) ?? 0;
            decimal spent = 0;
            if(TicketEvents != null && TicketEvents.Length > 0)
            {
                spent = TicketEvents.Sum(i => (decimal?)AllTreatmentConfigs[i.TreatmentConfigId].Price) ?? 0;
            }
            if(Ticket.Loan > 0 && ((spent + sum) / Ticket.UnitsAmount) > Ticket.Instalment.AvailableUnitsPercent)
            {
                var str = String.Format("По неполностью оплаченному абонементу возможно использование не более {0:p1} единиц!", Ticket.Instalment.AvailableUnitsPercent);
                throw new FaultException<string>(str, str);
            }

        }

        private Dictionary<Guid, List<TreatementEventInfo>> GetDivisionTreatmentEvents(ExtraEntities context)
        {
            var d2 = VisitDate.AddDays(1);
            return context.TreatmentEvents.Where(v => v.VisitDate >= VisitDate && v.VisitDate < d2 && v.VisitStatus != (short)TreatmentEventStatus.Canceled)
                .GroupBy(i => i.TreatmentId)
                .ToDictionary(i => i.Key, j => j.Select(i => new TreatementEventInfo
                {
                    LengthCoeff = i.TreatmentConfig.LengthCoeff,
                    Duration = i.TreatmentConfig.TreatmentType.Duration,
                    VisitDate = i.VisitDate
                }).ToList());
        }

        private TimeSpan GetCloseTime(DateTime visitDate)
        {
            var ticekttime = Ticket == null ? TimeSpan.MaxValue : TimeSpan.Parse("" + Ticket.TicketType.VisitEnd[0] + Ticket.TicketType.VisitEnd[1] + ":" + Ticket.TicketType.VisitEnd[2] + Ticket.TicketType.VisitEnd[3]);

            var day = visitDate.DayOfWeek;

            var closetime = Division.CloseTime;
            if(day == DayOfWeek.Tuesday && Division.CloseTime2.HasValue) closetime = Division.CloseTime2;
            if(day == DayOfWeek.Wednesday && Division.CloseTime3.HasValue) closetime = Division.CloseTime3;
            if(day == DayOfWeek.Thursday && Division.CloseTime4.HasValue) closetime = Division.CloseTime4;
            if(day == DayOfWeek.Friday && Division.CloseTime5.HasValue) closetime = Division.CloseTime5;
            if(day == DayOfWeek.Saturday && Division.CloseTime6.HasValue) closetime = Division.CloseTime6;
            if(day == DayOfWeek.Sunday && Division.CloseTime7.HasValue) closetime = Division.CloseTime7;

            if(closetime.HasValue && closetime < ticekttime)
            {
                return closetime.Value;
            }
            return ticekttime;
        }

        private void ValidateTicketTime(DateTime scheduleStartTime, DateTime scheduleEndTime)
        {
            if(Ticket != null)
            {
                var startTime = TimeSpan.Parse("" + Ticket.TicketType.VisitStart[0] + Ticket.TicketType.VisitStart[1] + ":" + Ticket.TicketType.VisitStart[2] + Ticket.TicketType.VisitStart[3]);
                if(scheduleStartTime.TimeOfDay < startTime)
                {
                    var str = Localization.Resources.UnavailTicketTimeError;
                    throw new FaultException<string>(str, str);
                }

                var endTime = TimeSpan.Parse("" + Ticket.TicketType.VisitEnd[0] + Ticket.TicketType.VisitEnd[1] + ":" + Ticket.TicketType.VisitEnd[2] + Ticket.TicketType.VisitEnd[3]);
                if(scheduleEndTime.TimeOfDay > endTime)
                {
                    var str = Localization.Resources.UnavailTicketTimeError;
                    throw new FaultException<string>(str, str);
                }
            }
            var day = VisitTime.DayOfWeek;

            var closetime = Division.CloseTime;
            if(day == DayOfWeek.Tuesday && Division.CloseTime2.HasValue) closetime = Division.CloseTime2;
            if(day == DayOfWeek.Wednesday && Division.CloseTime3.HasValue) closetime = Division.CloseTime3;
            if(day == DayOfWeek.Thursday && Division.CloseTime4.HasValue) closetime = Division.CloseTime4;
            if(day == DayOfWeek.Friday && Division.CloseTime5.HasValue) closetime = Division.CloseTime5;
            if(day == DayOfWeek.Saturday && Division.CloseTime6.HasValue) closetime = Division.CloseTime6;
            if(day == DayOfWeek.Sunday && Division.CloseTime7.HasValue) closetime = Division.CloseTime7;

            if(closetime.HasValue)
            {
                if(VisitTime.TimeOfDay > closetime)
                {
                    var str = "В желаемое время посещения клуб не работает";
                    throw new FaultException<string>(str, str);
                }
            }

            var opentime = Division.OpenTime;
            if(day == DayOfWeek.Tuesday && Division.OpenTime2.HasValue) opentime = Division.OpenTime2;
            if(day == DayOfWeek.Wednesday && Division.OpenTime3.HasValue) opentime = Division.OpenTime3;
            if(day == DayOfWeek.Thursday && Division.OpenTime4.HasValue) opentime = Division.OpenTime4;
            if(day == DayOfWeek.Friday && Division.OpenTime5.HasValue) opentime = Division.OpenTime5;
            if(day == DayOfWeek.Saturday && Division.OpenTime6.HasValue) opentime = Division.OpenTime6;
            if(day == DayOfWeek.Sunday && Division.OpenTime7.HasValue) opentime = Division.OpenTime7;

            if(opentime.HasValue)
            {
                if(VisitTime.TimeOfDay < opentime)
                {
                    var str = "В желаемое время посещения клуб не работает";
                    throw new FaultException<string>(str, str);
                }
            }
        }

        private void ValidateLimits(List<Guid> treatmentConfigs)
        {
            if(Ticket == null) return;
            if(!Ticket.TicketType.TicketTypeLimits.Any()) return;

            foreach(var tc in treatmentConfigs)
            {
                var lim = Ticket.TicketType.TicketTypeLimits.SingleOrDefault(i => i.TreatmentConfigId == tc);
                if(lim == null)
                {
                    var c = AllTreatmentConfigs[tc];
                    var str = String.Format(Localization.Resources.TreatmentUnavail, c.Name);
                    throw new FaultException<string>(str, str);
                }
                var vis = 0;
                if(TicketEvents != null)
                {
                    TicketEvents.Count(i => i.TreatmentConfigId == tc);
                }
                if(vis + (treatmentConfigs.Count(i => i == lim.TreatmentConfigId)) - 1 >= lim.Amount)
                {
                    var c = AllTreatmentConfigs[tc];
                    var str = String.Format(Localization.Resources.TreatmentUp, c.Name, lim.Amount);
                    throw new FaultException<string>(str, str);
                }
            }
        }

        private static ComparableList<ComparableList<TreatmentConfig>> GenerateRotatingItemsEx(IEnumerable<TreatmentConfig> treatmentConfigsList)
        {
            var res = GenerateRotatingItems(treatmentConfigsList);

            res.Insert(0, GenerateSingleItem(treatmentConfigsList)[0]);

            if(res.Count == 0) return res;

            for(int i = 1; i < res.Count; i++)
            {
                if(res[i].CompareTo(res[0]) == 0)
                {
                    res.RemoveAt(i);
                    break;
                }
            }

            return res;
        }

        private static ComparableList<ComparableList<TreatmentConfig>> GenerateSingleItem(IEnumerable<TreatmentConfig> treatmentConfigsList)
        {
            var res = new ComparableList<ComparableList<TreatmentConfig>> { new ComparableList<TreatmentConfig>() };
            res[0].AddRange(treatmentConfigsList);
            return res;
        }

        private bool IsCustomerBusyForTime(ref DateTime time, int duration)
        {
            var t = time;
            if(CustomerEventsRaw.Any(ev => Core.DatesIntersects(ev.VisitDate, ev.VisitDate.AddMinutes(TreatmentConfigFullDurations[ev.TreatmentConfigId]), t, t.AddMinutes(duration))))
            {
                time = time.AddMinutes(1);
                return true;
            }

            return false;
        }

        private Dictionary<Guid, List<TreatmentEvent>> GenerateCustomerEvents(ExtraEntities context)
        {
            var res = new Dictionary<Guid, List<TreatmentEvent>>();
            var dateS = VisitDate.AddDays(-2);
            var dateF = VisitDate.AddDays(1);
            var tsrc = context.TreatmentEvents.Where(te => te.CustomerId == CustomerId
                    && EntityFunctions.TruncateTime(te.VisitDate) >= dateS
                    && te.VisitDate < dateF
                    && te.VisitStatus != (short)TreatmentEventStatus.Canceled).ToArray();
            foreach(var tsr in DurationRestsList)
            {
                if(res.ContainsKey(tsr.TreatmentType1Id)) continue;
                res.Add(tsr.TreatmentType1Id, new List<TreatmentEvent>());
                res[tsr.TreatmentType1Id].AddRange(tsrc.Where(te => te.Treatment.TreatmentTypeId == tsr.TreatmentType1Id));
            }

            return res;
        }

        private Dictionary<Guid, HashSet<DateTime>> GenerateDivisionSchedule()
        {
            var res = new Dictionary<Guid, HashSet<DateTime>>();
            var toRemove = new Dictionary<KeyValuePair<Guid, DateTime>, int>();
            foreach(var l in TreatmentGroups.Keys)
            {
                foreach(var t in TreatmentGroups[l])
                {
                    res.Add(t.Id, new HashSet<DateTime>());
                    if(DivisionTreatmentEvents.ContainsKey(t.Id))
                    {
                        foreach(var other in DivisionTreatmentEvents[t.Id])
                        {
                            if(t.MaxCustomers == 1)
                            {
                                for(int j = 0; j < other.LengthCoeff; j++)
                                {
                                    res[t.Id].Add(other.VisitDate.AddMinutes(j * other.Duration));
                                }
                            }
                            else
                            {
                                for(int j = 0; j < other.LengthCoeff; j++)
                                {
                                    var pair = new KeyValuePair<Guid, DateTime>(t.Id, other.VisitDate.AddMinutes(j * other.Duration));
                                    if(toRemove.ContainsKey(pair))
                                    {
                                        if(++toRemove[pair] >= t.MaxCustomers)
                                        {
                                            res[t.Id].Add(other.VisitDate.AddMinutes(j * other.Duration));
                                        }
                                    }
                                    else
                                    {
                                        toRemove.Add(pair, 1);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// сопоставление TreatmentType и Treatment
        /// </summary>
        private Dictionary<Guid, List<Treatment>> GenerateTreatmentGroups(ExtraEntities context)
        {
            var res = context.Treatments.Where(i => i.DivisionId == DivisionId && i.IsActive).GroupBy(i => i.TreatmentTypeId).ToDictionary(i => i.Key, i => i.ToList());

            //*Strange if
            if(res.Count == 0)
            {
                var str = String.Format(Localization.Resources.NoTreatmentInClub);
                throw new FaultException<string>(str, str);
            }
            return res;
        }

        private Dictionary<Guid, List<TreatmentSeqRest>> GenerateDurationRests()
        {
            var res = new Dictionary<Guid, List<TreatmentSeqRest>>();
            foreach(var ts in DurationRestsList)
            {
                if(ts.TreatmentType2Id != null)
                {
                    if(!res.ContainsKey(ts.TreatmentType2Id.Value))
                    {
                        res.Add(ts.TreatmentType2Id.Value, new List<TreatmentSeqRest>());
                    }
                    res[ts.TreatmentType2Id.Value].Add(ts);
                }
            }
            return res;
        }

        private void ValidateAvail(List<Guid> treatmentConfigs, Guid[] treatmentTypeIds)
        {
            foreach(var rest in TreatmentSeqRests)
            {

                if(treatmentTypeIds.Count(t => t == rest.TreatmentType1Id) * rest.Treatment1Type.Duration > rest.Amount)
                {
                    var str = String.Format(Localization.Resources.MaxTreaLength, rest.Treatment1Type.Name, rest.Amount);
                    throw new FaultException<string>(str, str);
                }
            }
            if(Ticket != null)
            {
                foreach(var trest in Ticket.TicketType.TreatmentTypes)
                {
                    if(treatmentTypeIds.Contains(trest.Id))
                    {
                        var str = String.Format(Localization.Resources.TickUnavTre, trest.Name);
                        throw new FaultException<string>(str, str);
                    }
                }
            }

            if(!Customer.NoContraIndications.HasValue)
            {
                var str = Localization.Resources.NoContrasErr;
                throw new FaultException<string>(str, str);
            }

            foreach(var contra in Customer.ContraIndications)
            {
                foreach(TreatmentType tt in contra.TreatmentTypes)
                {
                    if(treatmentTypeIds.Contains(tt.Id))
                    {
                        var str = String.Format(Localization.Resources.ContraConfl, contra.Name, tt.Name);
                        throw new FaultException<string>(str, str);
                    }
                }
            }
        }

        private bool ValidateSequence(List<TreatmentConfig> listConf)
        {
            var list = listConf.Select(i => i.TreatmentType).ToList();
            foreach(var sr in SerRests)
            {
                if(list.Contains(sr.Treatment1Type) && list.Contains(sr.Treatment2Type) && list.IndexOf(sr.Treatment1Type) < list.IndexOf(sr.Treatment2Type)) return false;
            }
            return true;
        }

        private DateTime BindTime(TreatmentConfig config, DateTime time, ScheduleProposal prop,
            bool allowParalleling, Dictionary<Guid, Guid> mappingOfConfigsToTypes,
            out bool moved)
        {
            moved = false;
            var fd = TreatmentConfigFullDurations[config.Id];

            //Корректируем входное время с учетом ограничений на промежуток между процедурами
            if(DurationRests.ContainsKey(config.TreatmentTypeId))
            {
                foreach(var rest in DurationRests[config.TreatmentTypeId])
                {
                    //По уже запланированным датам
                    var compTime = time.AddMinutes(-rest.Interval.Value);
                    var md = CustomerEvents[rest.TreatmentType1Id]
                        .Where(te => te.VisitDate.AddMinutes(TreatmentConfigFullDurations[te.TreatmentConfigId]) > compTime)
                        .Max(te => (DateTime?)te.VisitDate.AddMinutes(TreatmentConfigFullDurations[te.TreatmentConfigId]).AddMinutes(rest.Interval.Value)) ?? DateTime.MinValue;
                    if(md > time)
                    {
                        moved = true;
                        time = md;
                    }
                    //По текущему расписанию
                    md = prop.List
                        .Where(spe => mappingOfConfigsToTypes[spe.Treatment.ConfigId] == rest.TreatmentType1Id && spe.EndTime >= time.AddMinutes(-rest.Interval.Value))
                        .Max(spe => (DateTime?)spe.EndTime.AddMinutes(rest.Interval.Value)) ?? md;
                    if(md > time)
                    {
                        moved = true;
                        time = md;
                    }
                }
            }
            //Проверка, не пересечется ли с уже имеющимся, делается отдельно, сейчас нас не интересует.
            //Проверяем, не пересечется ли с уже запланированным - только для параллелинга.

            //Привязываем время к сетке посещений
            var delay = (time - time.Date).TotalMinutes;
            var g = (delay % config.TreatmentType.Duration);
            if(g != 0)
            {
                var d = TreatmentTypeDurations[config.TreatmentTypeId];
                time = time.Date.AddMinutes((Math.Floor((double)(delay - g) / d) + 1) * d);
            }

            if(allowParalleling)
            {
                foreach(var spe in prop.List)
                {
                    //Если имеющийся полностью включит в себя текущий и они параллелятся - совмещаем
                    if(DatesContains(time, time.AddMinutes(fd), spe.StartTime, spe.EndTime)
                        && config.TreatmentType.TreatmentTypes1.Any(tt => tt.Id == mappingOfConfigsToTypes[spe.Treatment.ConfigId]))
                    {
                        //Только здесь еще необходима проверка, не пересекается ли данная процедура с чем-то неподходящим
                        var flag = true;
                        foreach(var j in prop.List)
                        {
                            if(j != spe)
                            {
                                if(Core.DatesIntersects(time, time.AddMinutes(fd), j.StartTime, j.EndTime) && !config.TreatmentType.TreatmentTypes1.Any(tt => tt.Id == mappingOfConfigsToTypes[j.Treatment.ConfigId]))
                                {
                                    flag = false;
                                    break;
                                }
                            }
                        }
                        if(flag) break;
                    }

                    if(Core.DatesIntersects(time, time.AddMinutes(fd), spe.StartTime, spe.EndTime))
                    {
                        if(spe.EndTime > time)
                        {
                            if(config.TreatmentType.TreatmentTypes1.Any(tt => tt.Id == mappingOfConfigsToTypes[spe.Treatment.ConfigId]))
                            {
                                //А если совместимы, просто по времени никак
                                time = time.AddMinutes(fd);
                            }
                            else
                            {
                                //Если несовместимы
                                time = spe.EndTime;
                            }
                        }
                    }
                }
            }

            //Привязываем время к сетке посещений
            delay = (time - time.Date).TotalMinutes;
            var pd = TreatmentTypeDurations[config.TreatmentTypeId];
            g = (delay % pd);
            if(g == 0) return time;
            return time.Date.AddMinutes((Math.Floor((double)(delay - g) / pd) + 1) * pd);
        }

        /// <summary>
        /// true if 1 fits in 2
        /// </summary>
        private static bool DatesContains(DateTime start1, DateTime end1, DateTime start2, DateTime end2)
        {
            return (start1 >= start2 && end1 <= end2);
        }

        private Treatment GetFreeTreatmentByType(TreatmentConfig config, ref DateTime time)
        {
            if(!TreatmentGroups.ContainsKey(config.TreatmentTypeId)) return null;
            var duration = TreatmentTypeDurations[config.TreatmentTypeId];
            var trs = TreatmentGroups[config.TreatmentTypeId];
            while(true)
            {
                var t = time;
                foreach(var tr in trs)
                {
                    bool flag = true;
                    for(int i = 0; i < config.LengthCoeff; i++)
                    {
                        if(DivisionTreatmentSchedule.ContainsKey(tr.Id) && DivisionTreatmentSchedule[tr.Id].Contains(t.AddMinutes(duration * i)))
                        {
                            flag = false;
                            break;
                        }
                    }
                    if(!flag) continue;
                    return tr;
                }
                time = time.AddMinutes(duration);
            }
        }

        private static ComparableList<ComparableList<TreatmentConfig>> GenerateRotatingItems(IEnumerable<TreatmentConfig> treatmentConfigs)
        {
            if(treatmentConfigs.Count() == 0)
            {
                var r = new ComparableList<ComparableList<TreatmentConfig>>();
                r.Add(new ComparableList<TreatmentConfig>());
                return r;
            }
            var res = new ComparableList<ComparableList<TreatmentConfig>>();
            for(int i = 0; i < treatmentConfigs.Count(); i++)
            {
                var first = treatmentConfigs.ElementAt(i);
                var other = treatmentConfigs.ToList();
                other.RemoveAt(i);
                foreach(var x in GenerateRotatingItems(other))
                {
                    x.Insert(0, first);
                    res.Add(x);
                }
            }
            res.Sort();
            var index = 0;
            while(index < res.Count - 1)
            {
                if(res[index].Equals(res[index + 1]))
                    res.RemoveAt(index);
                else
                    index++;
            }
            return res;
        }

        public List<ScheduleProposalElement> FixSchedule(List<ScheduleProposalElement> list, bool allowParalleling = false)
        {
            try
            {
                if(Ticket == null || TicketId == Guid.Empty)
                {
                    if(list.Count > MaxTreatmentReserve)
                    {
                        throw new Exception(String.Format("Максимум процедур без абонемента: " + MaxTreatmentReserve));
                    }
                }

                //bool allowParalleling = true;
                if (!allowParalleling)
                {
                    for (int n = 0; n < list.Count; n++)
                    {
                        for (int m = n + 1; m < list.Count; m++)
                        {
                            if (Core.DatesIntersects(list[n].StartTime, list[n].EndTime, list[m].StartTime, list[m].EndTime))
                            {
                                var len = list[m].EndTime - list[m].StartTime;
                                list[m].StartTime = list[n].EndTime;
                                list[m].EndTime = list[m].StartTime.Add(len);
                            }
                        }
                    }
                }
                var treatmentConfigsList = list.Select(i => AllTreatmentConfigs[i.ConfigId]).ToList();
                var treatmentConfigIds = list.Select(i => i.ConfigId).ToList();
                var mappingOfConfigsToTypes = treatmentConfigsList.Distinct().ToDictionary(i => i.Id, i => i.TreatmentTypeId);
                var startTime = list.First().StartTime.Date;
                var treatmentTypeIds = AllTreatmentConfigs.Values.Where(j => treatmentConfigIds.Contains(j.Id)).Select(i => i.TreatmentTypeId).ToArray();

                ValidateAvail(treatmentConfigIds, treatmentTypeIds);
                ValidateLimits(treatmentConfigIds.Distinct().ToList());
                if(!ValidateSequence(treatmentConfigsList))
                {
                    throw new Exception(Localization.Resources.IncorrectProg);
                }
                var sched = new ScheduleProposal();
                var currTime = list[0].StartTime;
                foreach(var i in list)
                {
                    var config = AllTreatmentConfigs[i.ConfigId];
                    if(currTime < i.StartTime) currTime = i.StartTime;
                    i.Treatment.ConfigId = i.ConfigId;

                    var time = currTime;
                    do
                    {
                        bool moved;
                        do
                        {
                            time = BindTime(config, time, sched, allowParalleling, mappingOfConfigsToTypes, out moved);
                        } while(IsCustomerBusyForTime(ref time, config.FullDuration));
                        i.MovedByRules = moved;
                        var tre = GetFreeTreatmentByType(config, ref time);
                        if(tre != null)
                        {
                            i.Treatment.Id = tre.Id;
                            break;
                        }

                    } while(true);

                    i.StartTime = time;
                    var et = allowParalleling ? i.StartTime : i.StartTime.AddMinutes(config.FullDuration);
                    sched.List.Add(new ScheduleProposalElement { StartTime = i.StartTime, EndTime = et, ConfigId = i.ConfigId, Treatment = i.Treatment });
                    currTime = et;
                }
                ValidateTicketTime(sched.List.Min(i => i.StartTime), sched.List.Max(i => i.EndTime));
                return sched.List;
            }
            catch(Exception ex)
            {
                throw new FaultException<string>(ex.Message, ex.Message);
            }
        }

    }
}
