using Sync.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace Sync.Code
{
    public static class Locker
    {
        static object _lockObj = new object();
        public static DateTime? IsLocked(Guid companyId)
        {
            lock (_lockObj)
            {
                using (var context = new SyncMetadataEntities())
                {
                    var lockCount = context.Locks.Count(i => i.ReleaseTime >= DateTime.Now);
                    if(lockCount > 25)
                    {
                        throw new FaultException<string>("Служба синхронизации перегружена. Попробуйте позднее!", "Служба синхронизации перегружена. Попробуйте позднее!");
                    }

                    var c = context.Locks.FirstOrDefault(i => i.CompanyId == companyId);
                    if (c == null || c.ReleaseTime < DateTime.Now) return null;
                    return c.ReleaseTime;
                }
            }
        }
        public static void SetLock(Guid companyId)
        {
            lock (_lockObj)
            {
                using (var context = new SyncMetadataEntities())
                {
                    var c = context.Locks.FirstOrDefault(i => i.CompanyId == companyId);
                    if (c != null && c.ReleaseTime >= DateTime.Now) throw new Exception("Блокировка уже установлена!");

                    context.Locks.AddObject(new Locks
                    {
                        Id = Guid.NewGuid(),
                        CompanyId = companyId,
                        LockTime = DateTime.Now,
                        ReleaseTime = DateTime.Now.AddMinutes(15)
                    });
                    context.SaveChanges();
                }
            }
        }
        public static void Unlock(Guid companyId)
        {
            lock (_lockObj)
            {
                using (var context = new SyncMetadataEntities())
                {
                    var ca = context.Locks.Where(i => i.CompanyId == companyId).ToList();
                    ca.ForEach(c =>
                    {
                        context.DeleteObject(c);
                        context.SaveChanges();
                    });
                }
            }
        }
    }
}