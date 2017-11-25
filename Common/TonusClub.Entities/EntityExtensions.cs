using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;

namespace TonusClub.Entities
{
    public static class EntityExtensions
    {
        public static void EnsureLoad<TEntity>(this EntityReference<TEntity> entityReference)
            where TEntity : EntityObject
        {
            if (!entityReference.IsLoaded)
            {
                entityReference.Load();
            }
        }

        public static void EnsureLoad<TEntity>(this EntityCollection<TEntity> entityCollection)
            where TEntity : EntityObject
        {
            if (!entityCollection.IsLoaded)
            {
                entityCollection.Load();
            }
        }

    }
}