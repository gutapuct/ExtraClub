using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ExtraClub.ServiceModel
{
    partial class Provider : IInitable
    {
        [DataMember]
        public string SerializedOrganizationTypeText { get; set; }

        public void Init()
        {
            if (OrganizationTypeId.HasValue)
            {
                SerializedOrganizationTypeText = OrganizationType.Name;
            }
        }
    }
}
