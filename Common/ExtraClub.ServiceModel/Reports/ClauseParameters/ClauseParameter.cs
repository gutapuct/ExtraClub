using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace ExtraClub.ServiceModel.Reports.ClauseParameters
{
    public abstract class ClauseParameter
    {
        [XmlIgnore]
        public virtual Func<IExtraService, Dictionary<object, string>> GetValuesFunction
        {
            get
            {
                return null;
            }
        }

        [XmlIgnore]
        public abstract Func<object, bool> QueryFunction { get; }

        [DataMember]
        public ClauseOperator Operator { get; set; }

        [DataMember]
        public object Value { get; set; }

        [XmlIgnore]
        public string Name
        {
            get
            {
                return ((DescriptionAttribute)this.GetType().GetCustomAttributes(typeof(DescriptionAttribute), true)[0]).Description;
            }
        }

        [XmlIgnore]
        public ClauseOperator[] Operators
        {
            get
            {
                return ((AvailableOperatorsAttribute)this.GetType().GetCustomAttributes(typeof(AvailableOperatorsAttribute), true)[0]).Operators;
            }
        }
    }
}
