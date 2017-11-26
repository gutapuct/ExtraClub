using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ExtraClub.ServiceModel
{
    partial class Spending : IInitable
    {
        [DataMember]
        public string SerializedSpendingTypeName { get; set; }

        [DataMember]
        public string SerializedCreatedBy { get; set; }

        [DataMember]
        public string DivisionName { get; set; }

        public void Init()
        {
            SerializedSpendingTypeName = SpendingType.Name;
            SerializedCreatedBy = CreatedBy.FullName;
            DivisionName = DivisionId.HasValue ? Division.Name : "Общепроектный";
        }
    }
}
