using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ExtraClub.ServiceModel
{
    partial class CustomerTargetType : IInitable
    {
        [DataMember]
        public Guid[] SerializedTreatmentConfigIds { get; set; }
        public void Init()
        {
            SerializedTreatmentConfigIds = TreatmentConfigs.Select(i => i.Id).ToArray();
        }
    }
}
