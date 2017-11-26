using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtraClub.ServiceModel
{
    public class IncludeAttribute : Attribute
    {
        public string[] Includes { get; set; }
        public IncludeAttribute(params string[] includes)
        {
            Includes = includes;
        }
    }
}
