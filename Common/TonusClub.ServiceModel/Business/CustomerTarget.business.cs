using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TonusClub.ServiceModel
{
    partial class CustomerTarget : IInitable
    {
        [DataMember]
        public string SerializedTypeName { get; set; }

        public string Status
        {
            get
            {
                if (TargetDate.Date > DateTime.Today && !(TargetComplete??false)) return "В процессе";
                if (!TargetComplete.HasValue) return "Требуется указать";
                if (TargetComplete.Value) return "Достигнута";
                return "Не достигнута";
            }
        }

        public void Init()
        {
            SerializedTypeName = CustomerTargetType.Name;
        }
    }
}
