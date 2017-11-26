using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtraClub.ServiceModel.Sync
{
    [Serializable]
    public class RelationInfo
    {
        public string RelationName { get; set; }
        public string LeftName { get; set; }
        public string RightName { get; set; }
        public Guid Left { get; set; }
        public Guid Right { get; set; }
    }
}
