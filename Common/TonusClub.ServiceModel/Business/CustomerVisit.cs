using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TonusClub.ServiceModel
{
    partial class CustomerVisit : IInitable
    {
        [DataMember]
        public string SerializedDivisionName { get; set; }

        public void Init()
        {
            SerializedDivisionName = Division.Name;
        }

        public int Number { get; set; }
    }
}
