using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace TonusClub.ServiceModel.Reports.ReportColumns
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ReportColumnsAttribute : Attribute
    {
        public Type RelatedEntityType { get; set; }
        public ReportColumnsAttribute(Type relatedEntity)
        {
            RelatedEntityType = relatedEntity;
        }
    }

    public static class ReportColumnsRegistry
    {
        public static Dictionary<string, string> GetColumns(Type entityType)
        {
            var res = new Dictionary<string, string>();
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                var attr = type.GetCustomAttributes(typeof(ReportColumnsAttribute), true).FirstOrDefault() as ReportColumnsAttribute;
                if (attr != null && attr.RelatedEntityType == entityType)
                {
                    foreach(var prop in type.GetProperties())
                    {
                        var desc = prop.GetCustomAttributes(typeof(DescriptionAttribute), false);
                        if (desc != null && desc.Any())
                        {
                            res.Add(prop.Name, ((DescriptionAttribute)desc.First()).Description);
                        }
                    }
                }
            }
            return res;
        }

        public static object GetColumnMapper(Type entityType, object entity)
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                var attr = type.GetCustomAttributes(typeof(ReportColumnsAttribute), true).FirstOrDefault() as ReportColumnsAttribute;
                if (attr != null && attr.RelatedEntityType == entityType)
                {
                    return Activator.CreateInstance(type, new object[] { entity });
                }
            }
            return new object();
        }

        public static Type GetColumnMapperType(Type baseType)
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                var attr = type.GetCustomAttributes(typeof(ReportColumnsAttribute), true).FirstOrDefault() as ReportColumnsAttribute;
                if (attr != null && attr.RelatedEntityType == baseType)
                {
                    return type;
                }
            }
            return null;
        }
    }
}
