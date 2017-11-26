using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtraClub.ServiceModel
{
    partial class EmployeeTrip
    {
        public int TotalDays
        {
            get
            {
                return (int)(EndDate - BeginDate).TotalDays;
            }
        }
    }
}
