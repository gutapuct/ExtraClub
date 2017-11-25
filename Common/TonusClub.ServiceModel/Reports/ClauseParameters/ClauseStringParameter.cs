using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TonusClub.ServiceModel.Reports.ClauseParameters
{
    [ValueType(typeof(string))]
    public abstract class ClauseStringParameter<T> : ClauseParameter
    {
        protected abstract string StringFunction(T entity);

        public override Func<object, bool> QueryFunction
        {
            get
            {
                switch (Operator)
                {
                    case ClauseOperator.Contains:
                        return new Func<object, bool>(i =>
                        {
                            return StringFunction((T)i).ToLower().Contains(((string)Value).ToLower());
                        });
                    case ClauseOperator.NotContains:
                        return new Func<object, bool>(i =>
                        {
                            return !StringFunction((T)i).ToLower().Contains(((string)Value).ToLower());
                        });
                    case ClauseOperator.Equals:
                        return new Func<object, bool>(i =>
                        {
                            return !StringFunction((T)i).ToLower().Equals(((string)Value).ToLower());
                        });
                }
                throw new Exception("Illegal operator for clause!");
            }
        }
    }
}
