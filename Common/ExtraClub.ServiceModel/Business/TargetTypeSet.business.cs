using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ExtraClub.ServiceModel
{
    partial class TargetTypeSet : IInitable
    {
        [DataMember]
        public string SerializedName { get; set; }
        [DataMember]
        public string SerializedConfigs { get; set; }

        public void Init()
        {
            SerializedName = CustomerTargetType.Name;
        }
        public void InitConfigs(List<TreatmentConfig> configs)
        {
            SerializedConfigs = String.Join(", ", (this.TreatmentConfigIds ?? "").Split(',')
                .Where(i => !String.IsNullOrEmpty(i))
                .Select(i => Guid.Parse(i))
                .Select(i => configs.Where(j => j.Id == i).Select(j => j.Name).FirstOrDefault())
                .Distinct().ToArray());
        }
    }
}
