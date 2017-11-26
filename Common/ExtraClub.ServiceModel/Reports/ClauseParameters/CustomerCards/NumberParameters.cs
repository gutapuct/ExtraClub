using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace ExtraClub.ServiceModel.Reports.ClauseParameters.CustomerCards
{
    [DataContract]
    [ClauseRelation(typeof(CustomerCard))]
    [Description("Цена")]
    public class NumberParameterC1 : ClauseNumberParameter<CustomerCard>
    {
        protected override decimal? NumberFunction(CustomerCard entity)
        {
            return entity.Price;
        }
    }
}
