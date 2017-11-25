using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TonusClub.ServiceModel
{
    [DataContract]
    public class CustomerEventView
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public bool IsCall { get; set; }

        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public string Employee { get; set; }

        [DataMember]
        public string Result { get; set; }

        [DataMember]
        public string Comments { get; set; }
        
        [DataMember]
        public string TypeText { get; set; }
    }
}
