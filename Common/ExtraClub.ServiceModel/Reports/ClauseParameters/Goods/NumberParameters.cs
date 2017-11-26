using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace ExtraClub.ServiceModel.Reports.ClauseParameters.Goods
{
    [DataContract]
    [ClauseRelation(typeof(Good))]
    [Description("Продаж всего")]
    public class NumberParameterGo1 : ClauseNumberParameter<Good>
    {
        protected override decimal? NumberFunction(Good entity)
        {
            if (!entity.GoodSales.Any()) return 0;
            return (decimal)entity.GoodSales.Sum(i => i.Amount);
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Good))]
    [Description("Поставок всего")]
    public class NumberParameterGo2 : ClauseNumberParameter<Good>
    {
        protected override decimal? NumberFunction(Good entity)
        {
            if (!entity.ConsignmentLines.Any(i => i.Consignment.ProviderId.HasValue && i.Consignment.IsAsset)) return 0;
            return (decimal)entity.ConsignmentLines.Where(i => i.Consignment.ProviderId.HasValue && i.Consignment.IsAsset).Sum(i => i.Quantity);
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Good))]
    [Description("Списаний всего")]
    public class NumberParameterGo3 : ClauseNumberParameter<Good>
    {
        protected override decimal? NumberFunction(Good entity)
        {
            if (!entity.ConsignmentLines.Any(i => !i.Consignment.ProviderId.HasValue && !i.Consignment.DestinationStorehouseId.HasValue && i.Consignment.IsAsset)) return 0;
            return (decimal)entity.ConsignmentLines.Where(i => !i.Consignment.ProviderId.HasValue && !i.Consignment.DestinationStorehouseId.HasValue && i.Consignment.IsAsset).Sum(i => i.Quantity);
        }
    }
    [DataContract]
    [ClauseRelation(typeof(Good))]
    [Description("Остаток на всех складах")]
    public class NumberParameterGo4 : ClauseNumberParameter<Good>
    {
        protected override decimal? NumberFunction(Good entity)
        {
            return entity.TotalLeft;
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Good))]
    [Description("Максимальная цена")]
    public class NumberParameterGo5 : ClauseNumberParameter<Good>
    {
        protected override decimal? NumberFunction(Good entity)
        {
            return entity.MaxPrice;
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Good))]
    [Description("Минимальная цена")]
    public class NumberParameterGo6 : ClauseNumberParameter<Good>
    {
        protected override decimal? NumberFunction(Good entity)
        {
            return entity.MinPrice;
        }
    }
}
