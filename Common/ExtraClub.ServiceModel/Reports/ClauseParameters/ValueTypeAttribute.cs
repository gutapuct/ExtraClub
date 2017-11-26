using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtraClub.ServiceModel.Reports.ClauseParameters
{
    public class ValueTypeAttribute : Attribute
    {
        public Type Type { get; private set; }
        public ValueTypeAttribute(Type attrType)
        {
            Type = attrType;
        }
    }
}
