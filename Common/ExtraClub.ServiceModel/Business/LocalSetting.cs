﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ExtraClub.ServiceModel
{
    partial class LocalSetting
    {
        [DataMember]
        public DateTime KeyValidTill { get; set; }
    }
}
