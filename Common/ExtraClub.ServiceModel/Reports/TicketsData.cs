using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ExtraClub.ServiceModel.Reports
{
    [DataContract]
    public class TicketsData
    {
        [DataMember]
        public string MonthName { get; set; }

        [DataMember]
        public List<int> Amount { get; set; }
        [DataMember]
        public int NewCustomers { get; set; }
        [DataMember]
        public int OldCustomers { get; set; }
    }
}