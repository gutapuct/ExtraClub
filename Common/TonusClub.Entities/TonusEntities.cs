using System;
using System.Collections.Generic;
using System.Data.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Reflection;

namespace TonusClub.Entities
{
    public partial class TonusEntities
    {
        public TonusEntities()
            : base(ConnectionString, ContainerName)
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            MetadataWorkspace.LoadFromAssembly(Assembly.GetAssembly(typeof(ServiceModel.AssemblyAnchor)));
            this.CommandTimeout = 600;
        }
    }
}
