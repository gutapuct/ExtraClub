using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ExtraClub.ServiceModel
{
    partial class CustomerCard
    {

        [DataMember]
        public CustomerCardType SerializedCustomerCardType { get; set; }

        public void InitDetails()
        {
            SerializedCustomerCardType = CustomerCardType;
        }

        public decimal Cost
        {
            get
            {
                return Price;
            }
        }

        public decimal PriceWoDiscount
        {
            get
            {
                if (Discount == 1) return 0;
                return Price/(1-Discount);
            }
        }

        
    }
}
