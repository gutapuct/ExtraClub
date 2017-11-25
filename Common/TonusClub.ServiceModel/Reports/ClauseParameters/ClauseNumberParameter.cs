using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TonusClub.ServiceModel.Reports.ClauseParameters
{
    [ValueType(typeof(decimal))]
    [AvailableOperators(ClauseOperator.Larger, ClauseOperator.LargerOrEqual,
        ClauseOperator.Smaller, ClauseOperator.SmallerOrEqual,
        ClauseOperator.Equals, ClauseOperator.NotEquals,
        ClauseOperator.IsNull, ClauseOperator.IsNotNull)]
    public abstract class ClauseNumberParameter<T>:ClauseParameter
    {
        protected abstract decimal? NumberFunction(T entity);

        public override sealed Func<object, bool> QueryFunction
        {
            get
            {
                switch (Operator)
                {
                    case ClauseOperator.Equals:
                        return new Func<object, bool>(i =>
                        {
                            return NumberFunction((T)i) == DecimalValue;
                        });
                    case ClauseOperator.NotEquals:
                        return new Func<object, bool>(i =>
                        {
                            return NumberFunction((T)i) != DecimalValue;
                        });
                    case ClauseOperator.Larger:
                        return new Func<object, bool>(i =>
                        {
                            return NumberFunction((T)i) > DecimalValue;
                        });
                    case ClauseOperator.LargerOrEqual:
                        return new Func<object, bool>(i =>
                        {
                            return NumberFunction((T)i) >= DecimalValue;
                        });
                    case ClauseOperator.Smaller:
                        return new Func<object, bool>(i =>
                        {
                            return NumberFunction((T)i) < DecimalValue;
                        });
                    case ClauseOperator.SmallerOrEqual:
                        return new Func<object, bool>(i =>
                        {
                            return NumberFunction((T)i) <= DecimalValue;
                        });
                    case ClauseOperator.IsNull:
                        return new Func<object, bool>(i =>
                        {
                            return !NumberFunction((T)i).HasValue;
                        });
                    case ClauseOperator.IsNotNull:
                        return new Func<object, bool>(i =>
                        {
                            return NumberFunction((T)i).HasValue;
                        });
                }
                throw new Exception("Illegal operator for clause!");
            }
        }

        private decimal? DecimalValue
        {
            get
            {
                decimal res;
                if (Decimal.TryParse((Value ?? new object()).ToString(), out res))
                    return res;
                else return null;
            }
        }
    }
}
