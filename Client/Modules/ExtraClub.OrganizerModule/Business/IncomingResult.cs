using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtraClub.OrganizerModule.Business
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
