using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ExtraClub.ServiceModel
{
    partial class UnitCharge : IInitable
    {
        [DataMember]
        public string SerializedTicketNumber { get; set; }

        public void Init()
        {
            SerializedTicketNumber = Ticket.Number;
        }
    }
}
