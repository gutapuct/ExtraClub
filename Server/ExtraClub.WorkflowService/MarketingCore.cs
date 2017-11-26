using System;
using System.Collections.Generic;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Data.Objects.SqlClient;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using ExtraClub.Entities;
using ExtraClub.ServiceModel;

namespace ExtraClub.WorkflowService
{
    class MarketingCore : IDisposable
    {
        ExtraEntities context = new ExtraEntities() { CommandTimeout = 600 };

        public void DoSmsGen(ExtraClub.ServerCore.CancelThreadInfo cancelInfo)
        {
            using(var context = new ExtraEntities())
            {
                foreach(var item in context.sp_GetSmsMarketing().ToArray())
                {
                    context.SmsMessages.AddObject(
                        new SmsMessage
                        {
                            CustomerId = item.CustomerId,
                            Id = Guid.NewGuid(),
                            Phone = SmsCore.FormatPhone(item.phone2),
                            ToSendFrom = DateTime.Today.AddDays(1).AddHours(15 - item.Utc),
                            Text = "У Вас заканчивается абонемент в ТОНУС-КЛУБ, заполните анкету и получите скидку 7% на следующий абонемент www.ExtraClub.ru/ank",
                            SkipCheck = true
                        });

                    context.ExecuteFunction("sp_MarkTicketMarketing", new ObjectParameter("ticketId", item.TicketId));

                    context.SaveChanges();
                    if(cancelInfo.Cancel) return;

                }
            }
        }

        public void Dispose()
        {
            context.Dispose();
        }

    }
}
