using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TonusClub.Entities;
using TonusClub.ServerCore;
using TonusClub.ServiceModel;

namespace TonusClub.CustomerPortal.Business
{
    public static class SaleBL
    {
        internal static Guid CreateTicket(Guid customerId, Guid ticketTypeId)
        {
            var context = new TonusEntities();
            var customer = context.Customers.Single(i => i.Id == customerId);
            var user = context.Users.First(i => i.CompanyId == customer.CompanyId);
            var tt = context.TicketTypes.Single(i => i.Id == ticketTypeId);

            var ticket = new Ticket
            {
                AuthorId = user.UserId,
                CompanyId = customer.CompanyId,
                CreatedOn = DateTime.Now,
                CustomerId = customerId,
                Deleted = false,
                DiscountPercent = 0,
                DivisionId = customer.ClubId.Value,
                GuestUnitsAmount = tt.GuestUnits,
                Id = Guid.NewGuid(),
                IsActive = false,
                Length = tt.Length,
                Number = Core.GetTicketNumber(context, customer.Company),
                Price = tt.Price,
                TicketTypeId = tt.Id,
                UnitsAmount = tt.Units,
                FreezesAmount = PaymentCore.GetFreezesAmount(context, customer.CompanyId, tt.Id),
                SolariumMinutes = tt.SolariumMinutes,
                FirstPmtTypeId = 1,
                InvoiceNumber = null,
                VatAmount = null
            };
            context.Tickets.AddObject(ticket);
            context.SaveChanges();
            return ticket.Id;
        }
    }
}