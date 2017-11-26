using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ExtraClub.ServiceModel
{
    partial class IncomingCallForm:IInitable
    {
        [DataMember]
        public List<IncomingCallFormButton> SerializedIncomingCallFormButtons { get; set; }

        public void Init()
        {
            SerializedIncomingCallFormButtons = IncomingCallFormButtons.OrderBy(i=>i.ButtonText).ToList();
        }
    }
}
