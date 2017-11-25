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
    [Description("Возврат")]
    [AvailableOperators(ClauseOperator.True, ClauseOperator.False)]
    public class BoolParameterGs1 : ClauseBoolParameter<GoodSale>
    {
        protected override bool? BoolFunction(GoodSale entity)
        {
            return entity.ReturnDate.HasValue;
        }
    }
}
