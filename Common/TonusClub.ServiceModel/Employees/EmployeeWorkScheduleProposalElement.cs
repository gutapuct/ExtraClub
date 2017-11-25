using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TonusClub.ServiceModel.Employees
{
    public class EmployeeWorkScheduleProposalElement
    {
        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public List<Guid> JobPlacementIds { get; set; }

        public List<JobPlacement> JobPlacements { get; set; }
    }
}
