using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TonusClub.ServiceModel
{
    partial class AnketTreatment
    {
        [DataMember]
        public string SerializedName { get; set; }
    }
}
