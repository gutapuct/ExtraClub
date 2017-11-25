using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TonusClub.ServiceModel
{
    partial class UnitCharge : IInitable
    {
        [DataMember]
        public string SerializedTicketNumber { get; set; }

#if BEAUTINIKA
        [DataMember]
        public string SerializedEmployee { get; set; }
#endif

        public void Init()
        {
            SerializedTicketNumber = Ticket.Number;
        }
    }
}
