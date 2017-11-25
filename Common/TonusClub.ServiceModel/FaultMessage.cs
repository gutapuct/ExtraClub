using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TonusClub.ServiceModel
{
    [DataContract]
    public class FaultMessage
    {
        private string _message;

        [DataMember]
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }
    }
}
