using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ExtraClub.ServiceModel
{
    partial class ContraIndication : IInitable
    {
        [DataMember]
        public List<TreatmentType> SerializedTreatmentTypes { get; set; }

        public void Init()
        {
            SerializedTreatmentTypes = TreatmentTypes.ToList();
        }
    }
}
