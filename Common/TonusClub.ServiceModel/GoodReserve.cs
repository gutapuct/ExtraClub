using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TonusClub.ServiceModel
{
    [DataContract]
    public class GoodReserve
    {
        [DataMember]
        public Guid GoodId { get; set; }
        [DataMember]
        public string GoodName { get; set; }
        [DataMember]
        public int Amount { get; set; }
    }
}
