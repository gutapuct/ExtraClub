using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ExtraClub.ServiceModel.Employees
{
    public class EmployeeScheduleProposalElement
    {
        public EmployeeScheduleProposalElement()
        {
            Id = Guid.NewGuid();
        }

        [DataMember]
        public Guid Id { get; private set; }

        [DataMember]
        public Guid EmployeeId { get; set; }

        [DataMember]
        public string EmployeeName { get; set; }

        [DataMember]
        public string EmployeeJob { get; set; }
        
        [DataMember]
        public DateTime Start { get; set; }
        
        [DataMember]
        public DateTime Finish { get; set; }

        [DataMember]
        public string Unit { get; set; }

        public int Length
        {
            get
            {
                return (int)Math.Ceiling((Finish - Start).TotalDays) + 1;
            }
        }
    }
}
