using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TonusClub.ServiceModel
{
    public partial class Treatment : IInitable
    {
        [DataMember]
        public TreatmentType SerializedTreatmentType { get; set; }

        [DataMember]
        public Division SerializedDivision { get; private set; }

        public void Init()
        {
            SerializedTreatmentType = TreatmentType;
            SerializedDivision = Division;
        }

        public string DisplayName
        {
            get
            {
                if (Tag != null) return Tag;
                if (TreatmentType != null) return TreatmentType.Name;
                if (SerializedTreatmentType != null) return SerializedTreatmentType.Name;
                return "(не указано)";
            }
        }

        public string NameWithTag
        {
            get
            {
                return SerializedTreatmentType.Name + " (" + DisplayName + ")";
            }
        }

        public bool Helper { get; set; }
    }
}
