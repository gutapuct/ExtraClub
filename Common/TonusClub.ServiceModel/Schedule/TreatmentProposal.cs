using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TonusClub.ServiceModel
{
    [DataContract]
    public class TreatmentProposal
    {
        [DataMember]
        public string Tag { get; set; }
        [DataMember]
        public Guid Id { get; set; }

        public Guid ConfigId { get; set; }
    }
}
