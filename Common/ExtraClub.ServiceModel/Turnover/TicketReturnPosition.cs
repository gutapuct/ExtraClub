using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ExtraClub.ServiceModel
{
    [DataContract]
    public class TicketReturnPosition: PayableItem
    {

        [DataMember]
        public virtual Guid TicketId
        {
            get { return _ticketId; }
            set
            {
                if (_ticketId != value)
                {
                    _ticketId = value;
                    OnPropertyChanged("TicketId");
                }
            }
        }
        private System.Guid _ticketId;
    }
}
