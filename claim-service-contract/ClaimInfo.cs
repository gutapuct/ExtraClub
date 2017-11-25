using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ClaimServiceContract
{
    [DataContract]
    public class ClaimInfo
    {
        [DataMember]
        public int? StatusId { get; set; }
        [DataMember]
        public string StatusDescription { get; set; }
        [DataMember]
        public int KindId { get; set; }
        [DataMember]
        public DateTime? FinishDate { get; set; }
        [DataMember]
        public string FinishedByName { get; set; }
        [DataMember]
        public Guid? FinishedByFtmId { get; set; }
        [DataMember]
        public string FinishDescription { get; set; }
    }
}
