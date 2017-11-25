using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TonusClub.ServiceModel
{
    partial class VacationListItem
    {
        public int Length
        {
            get
            {
                return (int)(FinishDate - StartDate).TotalDays;
            }
        }
    }
}
