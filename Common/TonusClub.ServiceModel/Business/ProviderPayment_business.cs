using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TonusClub.ServiceModel
{
    partial class ProviderPayment : IInitable
    {
        [DataMember]
        public string SerializedProviderName { get; set; }
        [DataMember]
        public string SerializedUserName { get; set; }
        [DataMember]
        public string SerializedOrderText { get; set; }


        public void Init()
        {
            SerializedProviderName = Provider.Name;
            SerializedUserName = CreatedBy.FullName;
            SerializedOrderText = String.Format("№{0} от {1:d}", Order.Number, Order.Date);
        }
    }
}
