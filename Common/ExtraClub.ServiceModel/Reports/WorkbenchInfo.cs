using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ExtraClub.ServiceModel.Reports
{
    [DataContract]
    public class WorkbenchInfo
    {
        [DataMember]
        public List<VisitInfo> CustomerVisits { get; set; }

        [DataMember]
        public List<CallInfo> CustomerCalls { get; set; }

        [DataMember]
        public List<OrganizerItem> CustomerTasks { get; set; }

        [DataMember]
        public decimal SalesPlan { get; set; }

        [DataMember]
        public decimal TotalSales { get; set; }
    }

    [DataContract]
    public class VisitInfo
    {
        [DataMember]
        public Guid CustomerId { get; set; }
        [DataMember]
        public string VisitTime { get; set; }
        [DataMember]
        public string FullName { get; set; }
        [DataMember]
        public string Phone { get; set; }
        [DataMember]
        public string TreatmnetNames { get; set; }
        [DataMember]
        public bool IsInClub { get; set; }

        public bool IsNotInClub
        {
            get
            {
                return !IsInClub;
            }
        }

    }

    [DataContract]
    public class CallInfo
    {
        [DataMember]
        public Guid CustomerId { get; set; }
        [DataMember]
        public string FullName { get; set; }
        [DataMember]
        public string Phone { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public DateTime Deadline { get; set; }
        [DataMember]
        public Guid Id { get; set; }
    }
}
