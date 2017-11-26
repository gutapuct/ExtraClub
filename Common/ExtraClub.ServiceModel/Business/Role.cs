using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ExtraClub.ServiceModel
{
    partial class Role : IInitable
    {
        [DataMember]
        public Guid[] SerializedPermissions { get; set; }

        [DataMember]
        public string UsersInRole { get; set; }

        public void Init()
        {
            SerializedPermissions = Permissions.Select(i => i.PermissionId).ToArray();
            var sb = new StringBuilder();
            Users.OrderBy(i => i.FullName).ToList().ForEach(i =>
            {
                if (sb.Length > 0) sb.Append("; ");
                sb.Append(i.FullName);
            });
            UsersInRole = sb.ToString();
        }

        public bool Helper { get; set; }

        [DataMember]
        public string CreatedByName { get; set; }
        [DataMember]
        public string ModifiedByName { get; set; }

    }
}
