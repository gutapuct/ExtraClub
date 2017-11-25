using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TonusClub.ServiceModel
{
    partial class SshFile
    {
        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public float LengthF { get; set; }

        [DataMember]
        public bool Avail { get; set; }
    }
}
