using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TonusClub.ServiceModel.Reports
{
    [DataContract]
    public class ReportInfoInt
    {
        [DataMember]
        public string Key { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public ReportType Type { get; set; }

        [DataMember]
        public string ReportComments { get; set; }

        public bool HasDatePeriod { get; set; }

        public string TypeText
        {
            get
            {
                return Type.ToString();
            }
        }

        [DataMember]
        public List<ReportParamInt> Parameters { get; set; }

        [DataMember]
        public List<string> CustomFields { get; set; }

        [DataMember]
        public string BaseTypeName { get; set; }

        [DataMember]
        public byte[] XmlClause { get; set; }

        [DataMember]
        public Guid[] RoleIds { get; set; }

        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public bool IsCommon { get; set; }
    }

    public enum ReportType : int
    {
        Code = 0,
        CodeParams = 1,
        Configured = 2,
        ConfiguredParams = 3,
        Stored = 4,
        StoredParams = 5
    }
}