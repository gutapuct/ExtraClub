using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TonusClub.ServiceModel
{
    [DataContract]
    public class TicketRebillGood : PayableItem
    {
        private Ticket Ticket;

        [DataMember]
        public Guid TicketId { get; set; }

        [DataMember]
        public Guid NewCustomerId { get; set; }

        private TicketRebillGood() { }

        public TicketRebillGood(Ticket ticket, Guid newCustomerId, decimal price)
        {
            Ticket = ticket;
            NewCustomerId = newCustomerId;
            TicketId = ticket.Id;
            Price = price;
            InBasket = 1;
            Name = "Комиссия за переоформление абонемента №" + Ticket.Number;
        }
    }
}
