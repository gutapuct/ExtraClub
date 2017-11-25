using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TonusClub.ServiceModel
{
    partial class TreatmentSeqRest : IInitable
    {
        [DataMember]
        public TreatmentType SerializedTreatment1Type { get; set; }
        [DataMember]
        public TreatmentType SerializedTreatment2Type { get; set; }

        public void Init()
        {
            SerializedTreatment1Type = Treatment1Type;
            SerializedTreatment2Type = Treatment2Type;
        }
    }
}
