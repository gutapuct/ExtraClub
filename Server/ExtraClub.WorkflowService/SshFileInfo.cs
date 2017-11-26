using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtraClub.WorkflowService
{
    class SshFileInfo
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public long Length { get; set; }
        public DateTime Modified { get; set; }
    }
}
