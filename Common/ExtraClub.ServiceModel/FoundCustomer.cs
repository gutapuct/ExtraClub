using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ExtraClub.ServiceModel
{
    [DataContract]
    public class FoundCustomer
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string FullName { get; set; }

        [DataMember]
        public string CardNumber { get; set; }

        public override string ToString()
        {
            return FullName;
        }
    }
}
