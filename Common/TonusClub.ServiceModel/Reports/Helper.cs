using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TonusClub.ServiceModel.Reports.ClauseParameters;

namespace TonusClub.ServiceModel.Reports
{
    public static class Helper
    {
        static Dictionary<ClauseOperator, string> OperatorNames;
        static Helper()
        {
            OperatorNames = new Dictionary<ClauseOperator, string>();
            OperatorNames.Add(ClauseOperator.Contains, "Содержит");
            OperatorNames.Add(ClauseOperator.Equals, "Равно");
            OperatorNames.Add(ClauseOperator.False, "Ложно");
            OperatorNames.Add(ClauseOperator.IsNotNull, "Не пусто");
            OperatorNames.Add(ClauseOperator.IsNull, "Пусто");
            OperatorNames.Add(ClauseOperator.Larger, "Больше");
            OperatorNames.Add(ClauseOperator.LargerOrEqual, "Больше или равно");
            OperatorNames.Add(ClauseOperator.NotContains, "Не содержит");
            OperatorNames.Add(ClauseOperator.NotEquals, "Не равно");
            OperatorNames.Add(ClauseOperator.Smaller, "Меньше");
            OperatorNames.Add(ClauseOperator.SmallerOrEqual, "Меньше или равно");
            OperatorNames.Add(ClauseOperator.True, "Истинно");
        }

        public static string GetText(ClauseOperator i)
        {
            return OperatorNames[i];
        }
    }
}
