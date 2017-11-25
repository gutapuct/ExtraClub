using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TonusClub.ServiceModel.Organizer
{
    public enum CallResult : int
    {
        Cancelled = 0,
        OldCustomer = 1,
        NotACustomer = 2,
        NewCustomer = 3,
        Screnario = 4,
        OK = 5
    }
}
