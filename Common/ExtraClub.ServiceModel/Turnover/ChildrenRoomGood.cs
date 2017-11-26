using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ExtraClub.ServiceModel.Turnover
{
    [DataContractAttribute]
    public sealed class ChildrenRoomGood : PayableItem
    {
        [DataMember]
        public string ChildName { get; set; }
        [DataMember]
        public string HealthStatus { get; set; }

        private ChildrenRoomGood(){}

        public ChildrenRoomGood(string childName, string healthStatus, decimal cost)
        {
            ChildName = childName;
            HealthStatus = healthStatus;
            Price = cost;
            InBasket = 1;
            Name = "Оплата детской комнаты";
        }
    }
}
