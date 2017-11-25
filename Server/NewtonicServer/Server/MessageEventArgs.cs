using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NewtonicServer.Server
{
    public class MessageEventArgs:EventArgs
    {
        public Guid Id { get; set; }
        public byte[] Bytes { get; set; }
    }
}
