using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TonusClub.ServiceModel
{
    partial class CustomReport : IInitable
    {
        [DataMember]
        public Guid[] SerializedRoleIds { get; set; }

        public void Init()
        {
            SerializedRoleIds = Roles.Select(i => i.RoleId).ToArray();
        }
    }
}
