using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ExtraClub.ServiceModel
{
    partial class ChildrenRoom : IInitable
    {
        [DataMember]
        public string SerializedDivisionName { get; private set; }

        [DataMember]
        public string SerializedSdal { get; private set; }

        [DataMember]
        public string SerializedOut{ get; private set; }

        [DataMember]
        public string SerializedCustomer { get; private set; }

        [DataMember]
        public string SerializedCustomerCard { get; private set; }

        public void Init()
        {
            SerializedDivisionName = Division.Name;
            SerializedSdal = CreatedBy.FullName;
            if(OutById.HasValue)
            {
                SerializedOut = OutBy.FullName;
            }
            Customer.InitEssentials();
            SerializedCustomer = Customer.FullName;
            if (Customer.ActiveCard != null)
            {
                SerializedCustomerCard = Customer.ActiveCard.CardBarcode;
            }
        }
    }
}
