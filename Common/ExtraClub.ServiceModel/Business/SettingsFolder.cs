﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtraClub.ServiceModel
{
    partial class SettingsFolder : IFolder
    {

        object IFolder.SettingsFolders1
        {
            get { return this.SettingsFolders1; }
        }
    }
}
