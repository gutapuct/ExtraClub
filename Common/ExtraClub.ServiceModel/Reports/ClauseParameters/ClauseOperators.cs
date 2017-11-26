using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtraClub.ServiceModel.Reports.ClauseParameters
{
    public enum ClauseOperator
    {
        IsNull,
        IsNotNull,
        Equals,
        NotEquals,
        Larger,
        LargerOrEqual,
        Smaller,
        SmallerOrEqual,
        Contains,
        NotContains,
        True,
        False
    }
}
