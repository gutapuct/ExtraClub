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
    [Description("Наименование")]
    [AvailableOperators(ClauseOperator.Contains)]
    public class StringParamSp1 : ClauseStringParameter<Spending>
    {
        protected override string StringFunction(Spending i)
        {
            return i.Name ?? "";
        }
    }
}
