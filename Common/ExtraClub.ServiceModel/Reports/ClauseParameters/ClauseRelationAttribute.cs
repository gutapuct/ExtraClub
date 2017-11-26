using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ExtraClub.ServiceModel.Reports.ClauseParameters
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ClauseRelationAttribute : Attribute
    {
        public Type RelatedEntityType { get; set; }
        public ClauseRelationAttribute(Type relatedEntity)
        {
            RelatedEntityType = relatedEntity;
        }
    }

    public static class ClauseRegistry
    {
        public static IEnumerable<Type> GetRelatedAttributes(Type entityType)
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                var attr = type.GetCustomAttributes(typeof(ClauseRelationAttribute), true).FirstOrDefault() as ClauseRelationAttribute;
                if (attr != null)
                {
                    if (entityType == null || attr.RelatedEntityType == entityType) yield return type;
                }
            }

        }
    }
}
