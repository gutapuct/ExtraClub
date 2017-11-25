using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TonusClub.ServiceModel
{
    partial class GoodPrice : IInitable
    {
        [DataMember]
        public string SerializedGoodName { get; set; }
        [DataMember]
        public string SerializedUnitType { get; set; }
        [DataMember]
        public string SerializedCategory { get; set; }

        public GoodPrice()
        {
            InPricelist = true;
        }

        public void Init()
        {
            SerializedGoodName = Good.Name;
            SerializedCategory = Good.GoodsCategoryId.HasValue ? Good.GoodsCategory.Name : "";
            SerializedUnitType = Good.UnitTypeId.HasValue ? Good.UnitType.Name : "";
        }
    }
}