using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TonusClub.ServiceModel
{
    partial class EmployeeVacation
    {
        public int VacationLength
        {
            get
            {
                return (int)Math.Ceiling((EndDate - BeginDate).TotalDays) + 1;
            }
        }
    }
}
