using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace ExtraClub.ServiceModel
{
    [DataContract]
    public class ScheduleProposalElement
    {
        [DataMember]
        public TreatmentProposal Treatment { get; set; }
        [DataMember]
        public DateTime StartTime { get; set; }
        [DataMember]
        public DateTime EndTime { get; set; }
        [DataMember]
        public int Price { get; set; }
        [DataMember]
        public Guid ConfigId { get; set; }
        [DataMember]
        public bool MovedByRules { get; set; }

        public string TimeText
        {
            get
            {
                return String.Format("{0:H:mm} — {1:H:mm}", StartTime, EndTime);
            }
        }

        public Brush BackColor { get; set; }
    }
}
