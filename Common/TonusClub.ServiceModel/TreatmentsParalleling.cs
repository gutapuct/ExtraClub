using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TonusClub.ServiceModel
{
    [DataContract]
    public class TreatmentsParalleling
    {
        [DataMember]
        public string Type1 { get; set; }
        [DataMember]
        public string Type2 { get; set; }
        [DataMember]
        public Guid Type1Id { get; set; }
        [DataMember]
        public Guid Type2Id { get; set; }

        public TreatmentsParalleling(TreatmentType large, TreatmentType small)
        {
            Type1 = large.Name;
            Type2 = small.Name;

            Type1Id = large.Id;
            Type2Id = small.Id;
        }

        public TreatmentsParalleling()
        {
        }
    }
}
