using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TonusClub.ServiceModel
{
    partial class EmployeeCategory : IInitable
    {
        public bool Helper { get; set; }

        [DataMember]
        public string SerializedJobsList { get; set; }

        [DataMember]
        public List<Guid> SerializedJobIds { get; set; }


        public void Init()
        {
            SerializedJobsList = "";
            SerializedJobIds = new List<Guid>();

            foreach (var i in Jobs)
            {
                SerializedJobIds.Add(i.Id);
                if (!String.IsNullOrEmpty(SerializedJobsList))
                {
                    SerializedJobsList += "; ";
                }
                SerializedJobsList += i.Name;
            }

        }

        public string ContractTypeText
        {
            get
            {
                if (IsPupilContract) return "Ученический";
                return "Трудовой";
            }
        }
    }
}
