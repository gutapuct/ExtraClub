using System;
using System.Runtime.Serialization;

namespace ClaimServiceContract
{
    [DataContract]
    public class TaskInfo
    {
        [DataMember]
        public string ManagerExecutorEmail { get; set; }
        [DataMember]
        public string ManagerCheckingEmail { get; set; }
        [DataMember]
        public Guid? CrmEntityId { get; set; } 
        [DataMember]
        public string CrmUrl { get; set; }//Customer, Club, Person
        [DataMember]
        public string CrmDescription { get; set; }
        [DataMember]
        public DateTime? DateDeadline { get; set; }
        [DataMember]
        public string Subject { get; set; }
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public int MaxScore { get; set; }
        [DataMember]
        public string Result { get; set; }
        [DataMember]
        public bool IsNormative { get; set; }
    }
}
