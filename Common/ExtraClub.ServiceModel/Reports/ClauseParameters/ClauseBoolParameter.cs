using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtraClub.ServiceModel.Reports.ClauseParameters
{
    [ValueType(typeof(bool))]
    public abstract class ClauseBoolParameter<T> : ClauseParameter
    {
        protected abstract bool? BoolFunction(T entity);

        public override sealed Func<object, bool> QueryFunction
        {
            get
            {
                switch (Operator)
                {
                    case ClauseOperator.True:
                        return new Func<object, bool>(i => BoolFunction((T)i).HasValue && BoolFunction((T)i).Value);
                    case ClauseOperator.False:
                        return new Func<object, bool>(i => BoolFunction((T)i).HasValue && !BoolFunction((T)i).Value);
                    case ClauseOperator.IsNull:
                        return new Func<object, bool>(i => !BoolFunction((T)i).HasValue);
                    case ClauseOperator.IsNotNull:
                        return new Func<object, bool>(i => BoolFunction((T)i).HasValue);
                }
                throw new Exception("Illegal operator for clause!");
            }
        }

    }
}
