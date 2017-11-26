using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ExtraClub.ServiceModel.Organizer
{
    [DataContract]
    public class CustomerNotificationInfo
    {
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public string FullName { get; set; }
        [DataMember]
        public string Card { get; set; }
        [DataMember]
        public string Phone2 { get; set; }
    }
}
