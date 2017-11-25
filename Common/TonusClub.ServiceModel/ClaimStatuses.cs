using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TonusClub.ServiceModel
{
    public enum ClaimStatuses:int
    {
        WaitingForSync = 0,
        WaitingForFtm = 1,
        InProgress = 2
    }
}
