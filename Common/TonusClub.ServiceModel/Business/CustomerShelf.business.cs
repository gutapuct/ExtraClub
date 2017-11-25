using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TonusClub.ServiceModel
{
    partial class CustomerShelf : IInitable
    {
        [DataMember]
        public string SerializedDivisionName { get; private set; }

        [DataMember]
        public string SerializedVydal { get; private set; }

        [DataMember]
        public string SerializedReturnBy { get; private set; }

        [DataMember]
        public string SerializedCustomerName { get; private set; }

        public void Init()
        {
            SerializedDivisionName = Division.Name;
            SerializedVydal = CreatedBy.FullName;
            if (ReturnById.HasValue)
            {
                SerializedReturnBy = ReturnBy.FullName;
            }
            Customer.InitActiveCard();
            if (Customer.ActiveCard != null)
            {
                SerializedCustomerName = "[" + Customer.ActiveCard.CardBarcode + "]" + Customer.FullName;
            }
            else
            {
                SerializedCustomerName = Customer.FullName;
            }
        }
    }
}
