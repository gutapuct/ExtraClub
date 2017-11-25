using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NewtonicServer.Server
{
    class CardEventArgs : EventArgs
    {
        public string CardNumber { get; set; }
        public Guid TreatmentId { get; set; }
    }
}
