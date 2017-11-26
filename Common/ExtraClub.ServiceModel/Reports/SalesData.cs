using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ExtraClub.ServiceModel.Reports
{
    [DataContract]
    public class SalesData
    {
        [DataMember]
        public string MonthName { get; set; }

        [DataMember]
        public List<decimal?> Amount { get; set; }
        [DataMember]
        public decimal? Nal { get; set; }
        [DataMember]
        public decimal? Beznal { get; set; }
    }
}
