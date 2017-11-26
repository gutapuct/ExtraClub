using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ExtraClub.ServiceModel
{
    partial class Package:IInitable
    {
        [DataMember]
        public List<PackageLine> SerializedPackageLines { get; set; }

        public void Init()
        {
            SerializedPackageLines = PackageLines.ToList();
        }
    }
}
