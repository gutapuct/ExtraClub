using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace TonusClub.ServiceModel.Reports.ClauseParameters.Spendings
{
    [DataContract]
    [ClauseRelation(typeof(Spending))]
    [Description("Дата")]
    public class DateParameterSp1 : ClauseDateParameter<Spending>
    {
        protected override DateTime? DateFunction(Spending entity)
        {
            return entity.CreatedOn;
        }
    }

}
