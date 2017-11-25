﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TonusClub.ServiceModel
{
    partial class SalarySheet:IInitable
    {
        [DataMember]
        public string SerializedCreatedBy { get; set; }

        public void Init()
        {
            SerializedCreatedBy = CreatedBy.FullName;
        }
    }
}
