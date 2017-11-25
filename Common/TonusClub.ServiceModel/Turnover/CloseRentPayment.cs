using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TonusClub.ServiceModel.Turnover
{
    [DataContract]
    public class CloseRentPayment : PayableItem
    {
        [DataMember]
        public Guid RentId { get; set; }
        [DataMember]
        public decimal? LostFine { get; set; }
        [DataMember]
        public decimal? OverdueFine { get; set; }
        [DataMember]
        public bool IsManualAmount { get; set; }

        private CloseRentPayment() { }

        public CloseRentPayment(Rent rent)
        {
            RentId = rent.Id;
            LostFine = rent.LostFine;
            OverdueFine = rent.OverdueFine;
            IsManualAmount = rent.IsManualAmount;

            InBasket = 1;
            Price = rent.AmountToPay;
            Name = "Прокат " + rent.SerializedGoodName;
        }
    }
}
