using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ExtraClub.ServiceModel
{
    partial class BonusAccount : IInitable
    {

        [DataMember]
        public string SerializedCreatedBy { get; set; }

        public void Init()
        {
            SerializedCreatedBy = CreatedBy.FullName;
        }
    }
}
