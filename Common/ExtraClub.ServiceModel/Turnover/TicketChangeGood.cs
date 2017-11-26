using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ExtraClub.ServiceModel
{
    [DataContract]
    public class TicketChangeGood : PayableItem
    {
        [DataMember]
        public Guid OldTicketId { get; set; }

        [XmlIgnore]
        [DataMember]
        public TicketType NewTicketType { get; set; }

        [XmlIgnore]
        [DataMember]
        public Instalment Instalment { get; set; }

        [DataMember]
        public decimal Discount { get; set; }

        private TicketChangeGood() { }

        public TicketChangeGood(Guid oldTicketId, TicketType newTicketType, Instalment instalment, decimal price, decimal discount)
        {
            this.OldTicketId = oldTicketId;
            this.NewTicketType = newTicketType;
            this.Instalment = instalment;
            UnitName = "шт.";
            if (Instalment == null || Instalment.Id == Guid.Empty)
            {
                Name =  "Замена абонемента на " + NewTicketType.Name;
            }
            else
            {
                Name = "Первый взнос за замену абонемента на " + NewTicketType.Name;
            }
            Price = price;
            InBasket = 1;
            Discount = discount;
        }
    }
}
