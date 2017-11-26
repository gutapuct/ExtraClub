using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ExtraClub.Entities;
using ExtraClub.ServiceModel;

namespace ExtraClub.WorkflowService
{
    public class EventsProcessor
    {
        public void Execute(List<Division> divs)
        {
            using (var context = new ExtraEntities())
            {
                var tcsDurations = context.TreatmentConfigs.ToDictionary(i => i.Id, i => i.LengthCoeff * i.TreatmentType.Duration);

                var w = Stopwatch.StartNew();
                var InternalUserId = WorkflowCore.InternalUserId;
                var intUser = context.Users.SingleOrDefault(i => i.CompanyId == InternalUserId);
                if (intUser == null)
                {
                    InternalUserId = context.Users.First().UserId;
                }
                var divIds = divs.Select(i => i.Id).ToList();
                var events = context.TreatmentEvents.Where(i => divIds.Contains(i.DivisionId))
                    .Where(e => (e.VisitDate <= DateTime.Now) && e.VisitStatus == 0)
                    .Select(i => new
                    {
                        Id = i.Id,
                        CustomerId = i.CustomerId,
                        TreatmentConfigId = i.TreatmentConfigId,
                        TreatmentId = i.TreatmentId,
                        VisitDate = i.VisitDate,
                        TicketId = i.TicketId,
                        DivisionId = i.DivisionId,
                        Price = i.TreatmentConfig.Price,
                        CompanyId = i.CompanyId,
                        TreatmentConfigName = i.TreatmentConfig.Name,
                        VisitStatus = i.VisitStatus
                    })
                    .ToArray();

                Debug.WriteLine("ProcessEvents count: " + events.Length);
                Debug.WriteLine("Getting took: " + w.ElapsedMilliseconds);

                var treatmentsOnline = divs.SelectMany(i => i.Treatments.Where(j => j.IsOnline && j.UseController)).Select(i => i.Id).ToList();

                var cIds = events.Select(i => i.CustomerId).Distinct().ToArray();
                var customerPresence = context.CustomerVisits
                    .Where(i => !i.OutTime.HasValue)
                    .GroupBy(i => i.CustomerId)
                    .ToDictionary(i => i.Key, i => i.FirstOrDefault().DivisionId);

                foreach (var ev in events)
                {
                    if (ev.VisitDate.AddMinutes(tcsDurations[ev.TreatmentConfigId]) > DateTime.Now && ev.TicketId.HasValue) continue;
                    if (treatmentsOnline.Contains(ev.TreatmentId))
                    {
                        continue;
                    }
                    if (divIds.Contains(ev.DivisionId))
                    {
                        using (var cwoll = new ExtraEntities())
                        {
                            cwoll.ContextOptions.LazyLoadingEnabled = false;
                            var cev = cwoll.TreatmentEvents.Single(i => i.Id == ev.Id);
                            if (cev.VisitStatus == 2 || cev.VisitStatus == 3) continue;
                            var hasCharges = cwoll.UnitCharges.Any(i => i.EventId == ev.Id);

                            if (customerPresence.ContainsKey(ev.CustomerId)
                                && customerPresence[ev.CustomerId] == ev.DivisionId && ev.TicketId.HasValue)
                            {
                                cev.VisitStatus = (short)TreatmentEventStatus.Completed;
                            }
                            else
                            {
                                cev.VisitStatus = (short)TreatmentEventStatus.Missed;
                            }


                            if (ev.TicketId.HasValue && !hasCharges)
                            {
                                Guid newId = Guid.NewGuid();
                                var res = new ExtraEntities().ExecuteStoreCommand(@"
                                    Insert into UnitCharges
                                    select {4},{2},{6},{1},0,{5},{3},{0},{7}
                                    where not exists (select * from UnitCharges where EventId={7})
                                    ", InternalUserId,//0
                                        (int)ev.Price,
                                         ev.CompanyId,//2
                                        DateTime.Now,
                                         newId,//4
                                         String.Format("Посещение услуги {0} от {1:dd.MM.yyyy HH:mm}", ev.TreatmentConfigName, ev.VisitDate),
                                         ev.TicketId.Value,//6
                                         ev.Id);
                                if (res > 0)
                                {
                                    var uc = cwoll.UnitCharges.Single(i => i.Id == newId);
                                    if (ev.VisitStatus == (short)TreatmentEventStatus.Missed) uc.Reason += " (Штраф)";
                                }
                            }
                            cwoll.SaveChanges();
                        }
                    }
                }
                w.Stop();
                Debug.WriteLine("ProcessEvents took " + w.ElapsedMilliseconds);
            }
        }
    }
}
