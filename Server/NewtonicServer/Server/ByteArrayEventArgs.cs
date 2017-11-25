using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NewtonicServer.Server
{
    class ByteArrayEventArgs : EventArgs
    {
        public byte[] Bytes { get; set; }
    }
}
