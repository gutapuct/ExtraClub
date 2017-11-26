using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtraClub.ServiceModel
{
    partial class ProviderFolder
    {
        private List<ProviderFolder> _children = new List<ProviderFolder>();
        public List<ProviderFolder> Children
        {
            get
            {
                if (_children == null) _children = new List<ProviderFolder>();
                return _children;
            }
        }
    }
}
