using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Objects;
using System.Data;

public static class Extensions
{
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

}