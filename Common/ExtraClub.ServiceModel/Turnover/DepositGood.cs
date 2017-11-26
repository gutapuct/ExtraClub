using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ExtraClub.ServiceModel.Turnover
{
    [DataContract]
    public class DepositGood:PayableItem
    {
        [DataMember]
        public decimal DepositAmount { get; set; }

        private DepositGood() { }
        public DepositGood(decimal amount)
        {
            DepositAmount = amount;
            this.InBasket = 1;
            this.Name = "Пополнение депозита";
            this.Price = amount;
            this.UnitName = "ед.";
        }
    }
}
