using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ExtraClub.ServiceModel.Reports.ClauseParameters
{
    [ValueType(typeof(Date))]
    [AvailableOperators(ClauseOperator.Larger, ClauseOperator.LargerOrEqual,
        ClauseOperator.Smaller, ClauseOperator.SmallerOrEqual,
        ClauseOperator.Equals, ClauseOperator.NotEquals,
        ClauseOperator.IsNull, ClauseOperator.IsNotNull)]
    public abstract class ClauseDateParameter<T> : ClauseParameter
    {
        protected abstract DateTime? DateFunction(T entity);

        public override sealed Func<object, bool> QueryFunction
        {
            get
            {
                switch (Operator)
                {
                    case ClauseOperator.Equals:
                        return new Func<object, bool>(i =>
                        {
                            return DateFunction((T)i) == DateValue;
                        });
                    case ClauseOperator.NotEquals:
                        return new Func<object, bool>(i =>
                        {
                            return DateFunction((T)i) != DateValue;
                        });
                    case ClauseOperator.Larger:
                        return new Func<object, bool>(i =>
                        {
                            return DateFunction((T)i) > DateValue;
                        });
                    case ClauseOperator.LargerOrEqual:
                        return new Func<object, bool>(i =>
                        {
                            return DateFunction((T)i) >= DateValue;
                        });
                    case ClauseOperator.Smaller:
                        return new Func<object, bool>(i =>
                        {
                            return DateFunction((T)i) < DateValue;
                        });
                    case ClauseOperator.SmallerOrEqual:
                        return new Func<object, bool>(i =>
                        {
                            return DateFunction((T)i) <= DateValue;
                        });
                    case ClauseOperator.IsNull:
                        return new Func<object, bool>(i =>
                        {
                            return !DateFunction((T)i).HasValue;
                        });
                    case ClauseOperator.IsNotNull:
                        return new Func<object, bool>(i =>
                        {
                            return DateFunction((T)i).HasValue;
                        });


                }
                throw new Exception("Illegal operator for clause!");
            }
        }

        private DateTime? DateValue
        {
            get
            {
                DateTime date;
                if (DateTime.TryParse((Value ?? new object()).ToString(), out date))
                    return date;
                else return null;
            }
        }

    }
}
