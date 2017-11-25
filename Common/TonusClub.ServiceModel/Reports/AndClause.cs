using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace TonusClub.ServiceModel.Reports
{
    [DataContract]
    public class AndClause : Clause
    {
        [XmlIgnore]
        public override bool IsFinite
        {
            get { return false; }
        }

        [XmlIgnore]
        public override string Name
        {
            get { return "и"; }
        }
    }
}
