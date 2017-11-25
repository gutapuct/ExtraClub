using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TonusClub.ServiceModel
{
    [DataContract]
    public class CustomerCardGood : PayableItem
    {

        [DataMember]
        public Guid CardTypeId { get; set; }

        [DataMember]
        public decimal DiscountPercent { get; set; }

        [DataMember]
        public string Comment { get; set; }

        [DataMember]
        public string CardNumber { get; set; }

        private CustomerCardGood() { }

        public CustomerCardGood(Guid cardTypeId, decimal discountPercent, decimal cost, string comment, string cardNumber)
        {
            CardTypeId = cardTypeId;
            DiscountPercent = discountPercent;
            Price = cost;
            Comment = comment;
            CardNumber = cardNumber;
            InBasket = 1;
            Name = "Членская карта №" + CardNumber;
            UnitName = "шт.";
        }
    }
}
