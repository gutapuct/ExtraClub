using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtraClub.Infrastructure.Extensions
{
    public static class ObjectExtensions
    {
            public static object GetValue(this object obj, string propertyName)
            {
                if (obj == null || String.IsNullOrEmpty(propertyName)) return null;
                var pi = obj.GetType().GetProperty(propertyName);
                if (pi == null) return null;
                return pi.GetValue(obj, null);
            }

            public static object SetValue(this object obj, string propertyName, object value)
            {
                if (obj == null || String.IsNullOrEmpty(propertyName)) return null;
                var pi = obj.GetType().GetProperty(propertyName);
                if (pi == null) return null;
                if (value == DBNull.Value) value = null;

                if (value is double && pi.PropertyType == typeof(decimal))
                {
                    value = Convert.ToDecimal(value);
                }

                pi.SetValue(obj, value, null);
                return value;
            }
        }
}
