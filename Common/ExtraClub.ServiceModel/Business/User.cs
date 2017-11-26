using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ExtraClub.ServiceModel
{
    partial class User : IInitable
    {
        [DataMember]
        public List<Guid> SerializedRoleIds { get; set; }

        [DataMember]
        public string SerializedRoles { get; set; }

        public void Init()
        {
            SerializedRoleIds = Roles.Select(i => i.RoleId).ToList();
            SerializedRoles = GetRoles();
            PasswordHash = null;
        }

        private string GetRoles()
        {
            var sb = new StringBuilder();
            foreach (var role in Roles)
            {
                if (sb.Length > 0) sb.Append("; ");
                sb.Append(role.RoleName);
            }
            return sb.ToString();
        }
    }
}
