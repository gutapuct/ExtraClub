using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace ExtraClub.ServiceModel.Reports.ClauseParameters.Spendings
{
    [DataContract]
    [ClauseRelation(typeof(Spending))]
    [Description("Сумма")]
    public class NumberParameterSp1 : ClauseNumberParameter<Spending>
    {
        protected override decimal? NumberFunction(Spending entity)
        {
            return entity.Amount;
        }
    }

}
