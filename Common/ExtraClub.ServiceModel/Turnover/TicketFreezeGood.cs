using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ExtraClub.ServiceModel
{
    [DataContract]
    public class TicketFreezeGood : PayableItem
    {
        private Ticket Ticket;

        [DataMember]
        public DateTime StartDate { get; set; }
        [DataMember]
        public DateTime EndDate { get; set; }
        [DataMember]
        public Guid ReasonId { get; set; }
        [DataMember]
        public Guid TicketId { get; set; }
        [DataMember]
        public string Comment { get; set; }

        private TicketFreezeGood() { }

        public TicketFreezeGood(Ticket ticket, DateTime startDate, DateTime endDate, Guid reasonId, string comment, decimal price)
        {
            Ticket = ticket;
            StartDate = startDate;
            EndDate = endDate;
            ReasonId = reasonId;
            TicketId = ticket.Id;
            Comment = comment;
            Price = price;
            InBasket = 1;
            Name = "Заморозка абонемента" + Ticket.Number;
            UnitName = "шт.";
        }

    }
}