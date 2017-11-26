using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ExtraClub.ServiceModel.Reports.ClauseParameters
{
    [ValueType(typeof(Guid))]
    public abstract class ClauseGuidParameter<T> : ClauseParameter
    {
        protected abstract Guid? GuidFunction(T entity);
        protected abstract IEnumerable<Guid> GuidsFunction(T entity);

        public override Func<object, bool> QueryFunction
        {
            get
            {
                switch (Operator)
                {
                    case ClauseOperator.Equals:
                        return new Func<object, bool>(i =>
                        {
                            return GuidFunction((T)i) == (Guid?)Value;
                        });
                    case ClauseOperator.NotEquals:
                        return new Func<object, bool>(i =>
                        {
                            return GuidFunction((T)i) != (Guid?)Value;
                        });
                    case ClauseOperator.IsNull:
                        return new Func<object, bool>(i =>
                        {
                            return GuidFunction((T)i) == null;
                        });
                    case ClauseOperator.IsNotNull:
                        return new Func<object, bool>(i =>
                        {
                            return GuidFunction((T)i) != null;
                        });
                    case ClauseOperator.Contains:
                        return new Func<object, bool>(i =>
                        {
                            if (Value == null) return false;
                            return GuidsFunction((T)i).Contains((Guid)Value);
                        });
                    case ClauseOperator.NotContains:
                        return new Func<object, bool>(i =>
                        {
                            if (Value == null) return false;
                            return !GuidsFunction((T)i).Contains((Guid)Value);
                        });
                }
                throw new Exception("Illegal operator for clause!");
            }
        }
    }
}
