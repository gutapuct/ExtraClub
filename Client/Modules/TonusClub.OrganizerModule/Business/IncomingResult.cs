using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TonusClub.OrganizerModule.Business
{
    public enum IncomingResult
    {
        NewCustomer,
        OldCustomer,
        NewCustomerScrenario,
        NotACustomer,
        Cancelled,
        SaveClicked
    }
}
