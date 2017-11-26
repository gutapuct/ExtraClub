using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ExtraClub.ServiceModel.Schedule
{
    [DataContract]
    public class HWProposal
    {
        [DataMember]
        public string ErrorMessage { get; set; }

        [DataMember]
        public Guid? ReplacingId { get; set; }

        [DataMember]
        public Guid TicketId { get; set; }

        [DataMember]
        public DateTime VisitDate { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string Line2 { get; set; }
    }

}
