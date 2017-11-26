using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace ExtraClub.ServiceModel.Reports.ClauseParameters.Treatments
{
    [DataContract]
    [ClauseRelation(typeof(Treatment))]
    [Description("Название")]
    [AvailableOperators(ClauseOperator.Contains)]
    public class StringParamT1 : ClauseStringParameter<Treatment>
    {
        protected override string StringFunction(Treatment i)
        {
            return i.DisplayName ?? "";
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Treatment))]
    [Description("Договор поставки")]
    [AvailableOperators(ClauseOperator.Contains)]
    public class StringParamT2 : ClauseStringParameter<Treatment>
    {
        protected override string StringFunction(Treatment i)
        {
            return i.DogNumber ?? "";
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Treatment))]
    [Description("Серийный номер")]
    [AvailableOperators(ClauseOperator.Contains)]
    public class StringParamT3 : ClauseStringParameter<Treatment>
    {
        protected override string StringFunction(Treatment i)
        {
            return i.SerialNumber ?? "";
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Treatment))]
    [Description("Дата поставки")]
    [AvailableOperators(ClauseOperator.Contains, ClauseOperator.Equals)]
    public class StringParamT4 : ClauseStringParameter<Treatment>
    {
        protected override string StringFunction(Treatment i)
        {
            return i.Delivery ?? "";
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Treatment))]
    [Description("Истечение гарантии")]
    [AvailableOperators(ClauseOperator.Contains, ClauseOperator.Equals)]
    public class StringParamT5 : ClauseStringParameter<Treatment>
    {
        protected override string StringFunction(Treatment i)
        {
            return i.GuaranteeExp ?? "";
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Treatment))]
    [Description("Истечение срока службы")]
    [AvailableOperators(ClauseOperator.Contains, ClauseOperator.Equals)]
    public class StringParamT6 : ClauseStringParameter<Treatment>
    {
        protected override string StringFunction(Treatment i)
        {
            return i.UseExp ?? "";
        }
    }

    [DataContract]
    [ClauseRelation(typeof(Treatment))]
    [Description("Комментарий")]
    [AvailableOperators(ClauseOperator.Contains)]
    public class StringParamT7 : ClauseStringParameter<Treatment>
    {
        protected override string StringFunction(Treatment i)
        {
            return i.Comment ?? "";
        }
    }


}
