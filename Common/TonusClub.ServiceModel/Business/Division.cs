using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TonusClub.ServiceModel
{
    partial class Division : IInitable
    {
        [DataMember]
        public DateTime? FirstCustomerDate { get; set; }

        public void Init()
        {
            if (CustomerVisits.Any())
            {
                FirstCustomerDate = CustomerVisits.Min(i => i.InTime);
            }
        }

        public decimal BankCardReturnComissionP
        {
            get
            {
                return BankCardReturnComission * 100;
            }
            set
            {
                BankCardReturnComission = value / 100;
            }
        }

        private bool _Helper;
        public bool Helper
        {
            get
            {
                return _Helper;
            }
            set
            {
                _Helper = value;
                OnPropertyChanged("Helper");
            }
        }
    }
}
