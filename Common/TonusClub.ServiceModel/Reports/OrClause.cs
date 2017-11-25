using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace TonusClub.ServiceModel.Reports
{
    [DataContract]
    public class OrClause : Clause
    {
        [XmlIgnore]
        public override bool IsFinite
        {
            get { return false; }
        }

        [XmlIgnore]
        public override string Name
        {
            get { return "или"; }
        }
    }
}
