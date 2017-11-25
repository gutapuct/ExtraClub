using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sync.Models;
using System.ServiceModel;
using System.Diagnostics;

namespace Sync
{
    public static class SyncLogger
    {
        public static void LogFormat(Guid metaCompanyId, string mesage, params object[] parameters)
        {
            using (var context = new SyncMetadataEntities())
            {
                var logItem = new LogItem
                {
                    CreatedOn = DateTime.Now,
                    Id = Guid.NewGuid(),
                    Message = String.Format(mesage, parameters),
                    MetaCompanyId = metaCompanyId,
                    MethodName = new StackTrace().GetFrame(1).GetMethod().Name,
                    SessionId = OperationContext.Current.SessionId ?? String.Empty
                };
                context.LogItems.AddObject(logItem);
                context.SaveChanges();
            }
        }
    }
}