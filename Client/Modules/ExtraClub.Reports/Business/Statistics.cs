using System;
using System.Collections.Generic;
using System.Linq;

namespace ExtraClub.Reports.Business
{
        public static class Statistics
        {
            public static object StdDev<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, TResult> selector)
            {
                return "Итого " + source.GetType().GetProperty("Key").GetValue(source, null);
            }
    }
}
