using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TonusClub.ServiceModel
{
    partial class Instalment
    {
        private bool _Helper;
        [DataMember]
        public bool Helper
        {
            get
            {
                return _Helper;
            }
            set
            {
                _Helper = value;
                OnPropertyChanged("Helper");
            }
        }
    }
}
