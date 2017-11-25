using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace TonusClub.ServiceModel.Reports.ClauseParameters.GoodSales
{
    [DataContract]
    [ClauseRelation(typeof(GoodSale))]
    [Description("Дата возврата")]
    public class DateParameterGS1 : ClauseDateParameter<GoodSale>
    {
        protected override DateTime? DateFunction(GoodSale entity)
        {
            return entity.ReturnDate;
        }
    }

    [DataContract]
    [ClauseRelation(typeof(GoodSale))]
    [Description("Дата покупки")]
    public class DateParameterGS2 : ClauseDateParameter<GoodSale>
    {
        protected override DateTime? DateFunction(GoodSale entity)
        {
            return entity.BarOrder.PurchaseDate;
        }
    }

}
