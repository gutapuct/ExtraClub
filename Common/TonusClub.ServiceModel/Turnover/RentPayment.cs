using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TonusClub.ServiceModel.Turnover
{
    [DataContract]
    public class RentPayment : PayableItem
    {
        private RentPayment() { }
        public RentPayment(Rent rent, string goodName)
        {
            RentId = rent.Id;
            InBasket = rent.Length;
            UnitName = "дн.";
            Price = rent.Price;
            Name = "Прокат " + goodName;
        }

        [DataMember]
        public Guid RentId { get; set; }
    }
}
