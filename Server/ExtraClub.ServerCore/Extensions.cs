using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using ExtraClub.ServiceModel;
using System.Data.SqlClient;
using System.Data.Objects;
using System.Data;
using ExtraClub.Entities;

namespace ExtraClub.ServerCore
{
    public static class Extensions
    {
        public static DateTime Max(DateTime date1, DateTime date2)
        {
            return (date1 > date2) ? date1 : date2;
        }

        public static DateTime Min(DateTime date1, DateTime date2)
        {
            return (date1 < date2) ? date1 : date2;
        }

        public static decimal SafeSum<TSource>(this IQueryable<TSource> source, Func<TSource, decimal> func)
        {
            if (!source.Any()) return 0;
            return source.Sum(func);
        }

        public static string ToArrayString(this DateTime[] period, string format)
        {
            return period[0].ToString(format) + " - " + period[1].ToString(format);
        }

        public static TResult Max<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, TResult defaultValue)
        {
            if (source.Count() == 0) return defaultValue;
            return source.Max(selector);
        }

        public static List<T> Init<T>(this List<T> list) where T : IInitable
        {
            list.ForEach(i => i.Init());

            return list;
        }

        public static object[] ExecuteToArrayMethod(this object obj)
        {
            var mi = typeof(Enumerable).GetMethod("ToArray");
            if (mi == null) return null;
            if (mi.IsGenericMethod)
            {
                mi = mi.MakeGenericMethod(new Type[] { typeof(object) });
                return (object[])mi.Invoke(obj, new object[] { obj });
            }
            return (object[])mi.Invoke(obj, new object[0]);
        }

        public static object GetValue(this object obj, string propertyName)
        {
            if (obj == null || String.IsNullOrEmpty(propertyName)) return null;

            var attrs = propertyName.Split('.');

            if (attrs.Count() > 1)
            {
                foreach (var attr in attrs)
                {
                    obj = obj.GetValue(attr);
                    if (obj == null) return null;
                }
                return obj;
            }

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

        public static SqlCommand AddParameter<T>(this SqlCommand cmd, string paramName, T? value) where T : struct
        {

            var param = new SqlParameter(paramName, value ?? (object)DBNull.Value);

            cmd.Parameters.Add(param);

            return cmd;
        }

        public static SqlCommand AddParameter(this SqlCommand cmd, string paramName, string value)
        {

            var param = new SqlParameter(paramName, value ?? (object)DBNull.Value);

            cmd.Parameters.Add(param);

            return cmd;
        }

        public static object GetByKey(this ObjectContext context, string setName, string keyName, object keyValue)
        {
            object obj;
            context.TryGetObjectByKey(new EntityKey("ExtraEntities." + setName,
                                    new KeyValuePair<string, object>[] { new KeyValuePair<string, object>(keyName, keyValue) }), out obj);
            return obj;
        }

        public static DateTime UtcToday(this Company company)
        {
            return DateTime.Today.AddHours(new ExtraEntities().LocalSettings.Any() ? 0 : company.UtcCorr);
        }

        public static DateTime UtcNow(this Company company)
        {
            return DateTime.Now.AddHours(new ExtraEntities().LocalSettings.Any() ? 0 : company.UtcCorr);
        }
    }
}
