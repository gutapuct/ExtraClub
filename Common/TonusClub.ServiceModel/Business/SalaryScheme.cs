using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TonusClub.ServiceModel
{
    partial class SalaryScheme : IInitable
    {
        [DataMember]
        public List<SalarySchemeCoefficient> SerializedSalarySchemeCoefficients { get; set; }

        public void Init()
        {
            SerializedSalarySchemeCoefficients = SalarySchemeCoefficients.ToList();
            SerializedSalarySchemeCoefficients.ForEach(i => i.Init());
        }
    }
}
