using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtraClub.ServiceModel
{
    partial class CumulativeDiscount
    {
        public string TypeText
        {
            get
            {
                return IsCountDisc ? "По количеству аб-в" : "По сумме продаж";
            }
        }

        public bool IsNotCountDisc
        {
            get
            {
                return !IsCountDisc;
            }
        }
    }
}
