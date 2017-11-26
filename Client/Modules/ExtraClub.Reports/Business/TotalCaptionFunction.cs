using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Windows.Data;

namespace ExtraClub.Reports.Business
{
    class TotalCaptionFunction : EnumerableSelectorAggregateFunction
    {
        //public override AggregateResult Calculate(GroupRecord targetGroup)
        //{
        //    return new AggregateResult(this);
        //}

        protected override string AggregateMethodName
        {
            get
            {
                return "StdDev";
            }
        }

        protected override Type ExtensionMethodsType
        {
            get
            {
                return typeof(Statistics);
            }
        }
    }
}
