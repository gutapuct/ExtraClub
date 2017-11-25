using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TonusClub.ServiceModel
{
    partial class Income : IInitable
    {
        [DataMember]
        public string SerializedIncomeTypeName { get; set; }

        [DataMember]
        public string SerializedCreatedBy { get; set; }

        [DataMember]
        public string DivisionName { get; set; }


        public void Init()
        {
            SerializedIncomeTypeName = IncomeType.Name;
            SerializedCreatedBy = CreatedBy.FullName;
            DivisionName = DivisionId.HasValue ? Division.Name : "Общепроектный";
        }
    }
}
