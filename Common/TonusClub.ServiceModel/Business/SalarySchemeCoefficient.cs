using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TonusClub.ServiceModel.Employees;
using System.Runtime.Serialization;

namespace TonusClub.ServiceModel
{
    partial class SalarySchemeCoefficient : IInitable
    {
        [DataMember]
        public List<SalaryRateTable> SerializedRateTable { get; set; }

        public string TypeText
        {
            get
            {
                if (CoefficientTypes.TypeNames.ContainsKey(CoeffTypeId)) return CoefficientTypes.TypeNames[CoeffTypeId];
                else return "Не указано";
            }
        }

        public void Init()
        {
            SerializedRateTable = SalaryRateTables.ToList();
        }
    }
}
