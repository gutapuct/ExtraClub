using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NewtonicServer.Server
{
    public class GuidEventArgs : EventArgs
    {
        public Guid Id { get; set; }
    }
}
