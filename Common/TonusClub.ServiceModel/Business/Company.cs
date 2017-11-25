using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TonusClub.ServiceModel
{
    partial class Company
    {
        public decimal? MaxFreezePercentP
        {
            get
            {
                return MaxFreezePercent * 100;
            }
            set
            {
                MaxFreezePercent = value / 100;
            }
        }

        public decimal TicketReturnPercentCommissionP
        {
            get
            {
                return TicketReturnPercentCommission * 100;
            }
            set
            {
                TicketReturnPercentCommission = value / 100;
            }
        }

        public decimal DepositComissionPercentP
        {
            get
            {
                return DepositComissionPercent * 100;
            }
            set
            {
                DepositComissionPercent = value / 100;
            }
        }

    }
}
