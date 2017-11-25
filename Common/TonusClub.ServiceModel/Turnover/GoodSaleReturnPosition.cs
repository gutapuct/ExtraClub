using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TonusClub.ServiceModel.Turnover
{
    [DataContract]
    public class GoodSaleReturnPosition : PayableItem
    {
        [DataMember]
        public Guid GoodSaleId { get; set; }
    }
}
