using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TonusClub.ServiceModel.HttpMethodsTypes
{
    [DataContract]
    [Serializable]
    public class ZReportLine
    {
        [DataMember]
        public Guid GoodId { get; set; }

        [DataMember]
        public string GoodName { get; set; }

        [DataMember]
        public int Amount { get; set; }

        [DataMember]
        public decimal Price { get; set; }

        [DataMember]
        public Decimal Cost { get; set; }
    }
}
