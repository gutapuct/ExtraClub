using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TonusClub.ServiceModel.Reports
{
    public class SelectListItem
    {
        public Guid Key { get; set; }
        public string Value { get; set; }

        bool _b;
        public bool Helper
        {
            get
            {
                return _b;
            }
            set
            {
                _b = value;
                if (Changed != null) Changed(this, new EventArgs());
            }
        }

        public event EventHandler Changed;
    }
}
