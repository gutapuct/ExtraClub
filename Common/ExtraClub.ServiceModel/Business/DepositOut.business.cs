using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ExtraClub.ServiceModel
{
    partial class DepositOut:IInitable
    {
        [DataMember]
        public decimal Comission { get; set; }

        public decimal TotalAmount
        {
            get
            {
                return Amount - Comission;
            }
        }

        public void Init()
        {
            Comission = Amount * (Company.DepositComissionPercent) + Company.DepositComissionRub;
        }
    }
}
