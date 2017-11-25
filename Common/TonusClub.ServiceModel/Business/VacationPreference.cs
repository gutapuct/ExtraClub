using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TonusClub.ServiceModel
{
    partial class VacationPreference
    {
        public string PrefTypeText
        {
            get
            {
                if (PrefType == 0) return "Отпуск";
                if (PrefType == 1) return "Командировка";
                return "Отгул";
            }
        }

        public int Length
        {
            get
            {
                return (int)Math.Ceiling((EndDate - StartDate).TotalDays) + 1;
            }
        }
    }
}
