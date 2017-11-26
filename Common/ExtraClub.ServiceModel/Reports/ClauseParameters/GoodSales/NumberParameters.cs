using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace ExtraClub.ServiceModel.Reports.ClauseParameters.GoodSales
{
    [DataContract]
    [ClauseRelation(typeof(GoodSale))]
    [Description("Количество")]
    public class NumberParameterGS1 : ClauseNumberParameter<GoodSale>
    {
        protected override decimal? NumberFunction(GoodSale entity)
        {
            return (decimal)entity.Amount;
        }
    }
    [DataContract]
    [ClauseRelation(typeof(GoodSale))]
    [Description("Стоимость 1 ед.")]
    public class NumberParameterGS2 : ClauseNumberParameter<GoodSale>
    {
        protected override decimal? NumberFunction(GoodSale entity)
        {
            return entity.PriceMoney;
        }
    }
    [DataContract]
    [ClauseRelation(typeof(GoodSale))]
    [Description("Стоимость 1 ед., бонусы")]
    public class NumberParameterGS3 : ClauseNumberParameter<GoodSale>
    {
        protected override decimal? NumberFunction(GoodSale entity)
        {
            return entity.PriceBonus;
        }
    }

}
