using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TonusClub.ServiceModel
{
    [DataContract]
    public class CumulativeDiscountInfo
    {
        [DataMember]
        public decimal DiscountPercent { get; set; }
        [DataMember]
        public decimal TicketsAmount { get; set; }
        [DataMember]
        public int TicketsCount { get; set; }
        [DataMember]
        public decimal GoodsAmount { get; set; }

        public decimal Amount
        {
            get
            {
                return TicketsAmount + GoodsAmount;
            }
        }
        [DataMember]
        public decimal NextRub { get; set; }
        [DataMember]
        public decimal NextRubPercent { get; set; }
        [DataMember]
        public decimal NextTicketsPercent { get; set; }
        [DataMember]
        public decimal NextTickets { get; set; }
    }
}
