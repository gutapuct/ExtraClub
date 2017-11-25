using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TonusClub.ServiceModel
{
    public enum DiscountTypes : short
    {
        CardSale = 1, TicketSale = 2
    }

    public enum DocumentTypes : short
    {
        JobApply = 1,
        JobChange = 2,
        JobFire = 3,
        Vacation = 4,
        Ill = 5,
        Miss = 6,
        Trip = 7,
        CategoryChange = 8
    }
}
