using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ExtraClub.ServiceModel
{
    partial class News
    {
        [DataMember]
        public Guid PrirodaId { get; set; }

        [DataMember]
        public int PrirodaLevel { get; set; }
    }
}
