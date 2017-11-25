using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TonusClub.ServiceModel.Reports.ClauseParameters
{
    public class AvailableOperatorsAttribute : Attribute
    {
        public ClauseOperator[] Operators { get; private set; }

        public AvailableOperatorsAttribute(params ClauseOperator[] operators)
        {
            Operators = operators;
        }
    }
}
