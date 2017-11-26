using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ExtraClub.ServiceModel
{
    public class EntityObject
    {
        public bool Modified { get; set; }

        [OnDeserialized()]
        internal virtual void OnDeserializedMethod(StreamingContext context)
        {
            Modified = false;
        }
    }
}
