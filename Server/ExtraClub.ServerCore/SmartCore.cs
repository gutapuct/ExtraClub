using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using ExtraClub.Entities;
using ExtraClub.ServiceModel;
using ExtraClub.ServiceModel.Schedule;

namespace ExtraClub.ServerCore
{
    public static class SmartCore
    {
        public static int MaxUnitsPerDay = 8;
        public static ScheduleProposalResult GetSmartProposals(Guid customerId, Guid divisionId, Guid ticketId, DateTime visitDate, Guid targetId, bool allowParallel)
        {
            using(var context = new ExtraEntities())
            {
                var w = Stopwatch.StartNew();
                var visitDate1 = visitDate.Date;

                var cust = context.Customers.Where(i => i.Id == customerId)
                    .Select(i => new { i.Birthday, i.CompanyId, TargetIds = i.CustomerTargets.Where(j => !(j.TargetComplete ?? false)).Select(j => j.TargetTypeId), i.NoContraIndications })
                    .FirstOrDefault();
                if(cust == null)
                {
                    return null;
                }
                if(!cust.TargetIds.Any())
                {
                    return new ScheduleProposalResult { Result = "У клиента не указано ни одной цели для смарт-тренировок!" };
                }
                if(!cust.Birthday.HasValue)
                {
                    return new ScheduleProposalResult { Result = "У клиента не указан день рождения!" };
                }

                if(!cust.NoContraIndications.HasValue)
                {
                    return new ScheduleProposalResult { Result = "У клиента не заполнены противопоказания!" };
                }

                var customerAge = Math.Floor((DateTime.Now - cust.Birthday.Value).TotalDays / 365);
                var companyFolderIds = context.SettingsFolders.Where(i => i.AccessingCompanies.Any(j => j.CompanyId == cust.CompanyId)).Select(i => i.Id).ToArray();
                var treatments = context.Treatments.Where(i => i.IsActive && i.DivisionId == divisionId).Select(i => new { i.Id, i.TreatmentTypeId }).ToArray();
                var ttIds = treatments.Select(i => i.TreatmentTypeId).Distinct().ToArray();
                var tcIds = context.TreatmentConfigs.Where(i => ttIds.Contains(i.TreatmentTypeId) && i.IsActive).Select(i => i.Id).ToArray();

                var confs = new List<List<Guid>>();

                foreach(var stringset in context.TargetTypeSets.Where(i => i.TargetTypeId == targetId).Select(i => i.TreatmentConfigIds))
                {
                    var set = (stringset.Split(',').Select(i => Guid.Parse(i)).ToList());
                    if(set.All(i => tcIds.Contains(i)))
                    {
                        confs.Add(set);
                    }
                }

                try
                {
                    ScheduleProposalResult res = null;
                    var sa = new ScheduleAnalyzer(customerId, divisionId, ticketId, visitDate);
                    foreach(var conf in confs)
                    {
                        try
                        {
                            var sp = sa.GetScheduleProposals(allowParallel, conf, false);
                            if(sp.List.All(i => i.List.All(j => j.StartTime.Date == visitDate.Date)))
                            {
                                if(res == null)
                                {
                                    res = sp;
                                }
                                else
                                {
                                    res.List.AddRange(sp.List);
                                    if(string.IsNullOrEmpty(sp.Result) && !String.IsNullOrEmpty(res.Result))
                                    {
                                        res.Result = null;
                                    }
                                }
                            }
                        }
                        catch(FaultException)
                        {
                        }
                    }
                    if(res != null && res.List != null && res.List.Count > 1)
                    {
                        res.List.Sort((l, r) => { return Comparer<int>.Default.Compare(r.Duration.Value, l.Duration.Value); });
                    }
                    return res;
                }
                finally
                {
                    Debug.WriteLine("{0} ms total", w.ElapsedMilliseconds);
                }
            }
        }
    }
}
