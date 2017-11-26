using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ExtraClub.ServiceModel
{
    partial class CashInOrder : IInitable
    {
        [DataMember]
        public string SerializedCreatedByName { get; set; }

        [DataMember]
        public string SerializedReceivedByName { get; set; }

        public int DivNumber { get; set; }

        public void Init()
        {
            SerializedCreatedByName = CreatedBy.FullName;
            SerializedReceivedByName = ReceivedBy.FullName;
        }
    }
}
