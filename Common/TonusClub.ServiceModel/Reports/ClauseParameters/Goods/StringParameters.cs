using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace TonusClub.ServiceModel.Reports.ClauseParameters.Goods
{
    [DataContract]
    [ClauseRelation(typeof(Good))]
    [Description("Наименование")]
    [AvailableOperators(ClauseOperator.Contains)]
    public class StringParamGo1 : ClauseStringParameter<Good>
    {
        protected override string StringFunction(Good i)
        {
            return i.Name ?? "";
        }
    }
}
