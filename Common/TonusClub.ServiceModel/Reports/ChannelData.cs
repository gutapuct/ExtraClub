using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TonusClub.ServiceModel.Reports
{
    [DataContract]
    public class ChannelData
    {
        [DataMember]
        public string CatName { get; set; }
        [DataMember]
        public int Value { get; set; }
    }
}
