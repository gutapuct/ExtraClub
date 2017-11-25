using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace TonusClub.ServiceModel
{
    [DataContract]
    public class TicketPaymentPosition : PayableItem
    {
        [DataMember]
        [XmlIgnore]
        public Ticket Ticket { get; set; }

        private TicketPaymentPosition() { }

        public TicketPaymentPosition(Ticket ticket, decimal amount)
            : base()
        {
            Ticket = ticket;
            InBasket = 1;
            Price = amount;
            Name = "Оплата абонемента №" + Ticket.Number;
        }
    }
}
