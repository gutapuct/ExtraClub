using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ExtraClub.ServiceModel
{
    [DataContract]
    public class TicketGood : PayableItem
    {
        private TicketGood() { }

        public TicketGood(Ticket ticket, decimal paidAmt = 0m, decimal? partialPayment = null)
        {
            Ticket = ticket;
            Instalment = ticket.Instalment;
            TicketType = ticket.TicketType;
            DiscountPercent = ticket.DiscountPercent;
            VatAmount = ticket.VatAmount;
            CreditInitialPayment = ticket.CreditInitialPayment;
            PartialPayment = partialPayment;
            CreditComment = ticket.CreditComment;
            if (partialPayment.HasValue && partialPayment > 0)
            {
                Price = partialPayment.Value;
            }
            else if (paidAmt == 0m)
            {
                Price = Ticket.FirstPayment;
            }
            else
            {
                Price = 0;
            }
            InBasket = 1;
            UnitName = "шт.";
            if (Instalment == null || Instalment.Id == Guid.Empty)
            {
                Name = "Абонемент " + TicketType.Name;
            }
            else
            {
                Name = "Первый взнос за абонемент " + TicketType.Name;
            }
        }

        [XmlIgnore]
        [DataMember]
        public Ticket Ticket { get; set; }

        [XmlIgnore]
        [DataMember]
        public Instalment Instalment { get; private set; }

        [XmlIgnore]
        [DataMember]
        public TicketType TicketType { get; private set; }

        [DataMember]
        public decimal DiscountPercent { get; set; }

        [DataMember]
        public Guid TicketId { get; set; }

        [DataMember]
        public decimal? VatAmount { get; set; }

        [DataMember]
        public decimal? CreditInitialPayment { get; set; }

        [DataMember]
        public decimal? PartialPayment { get; set; }

        [DataMember]
        public string CreditComment { get; set; }
    }
}
