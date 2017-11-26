using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ExtraClub.ServiceModel.Schedule
{
    [DataContract]
    public class ScheduleProposalResult
    {
        [DataMember]
        public List<ScheduleProposal> List { get; set; }

        [DataMember]
        public string Result { get; set; }
    }
}
