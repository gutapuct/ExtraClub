using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace ExtraClub.ServiceModel
{
    [DataContract]
    public class PaymentDetails : INotifyPropertyChanged
    {
        public PaymentDetails(Guid customerId, decimal requestedAmount, bool cashless)
        {
            CustomerId = customerId;
            RequestedAmount = requestedAmount;
            Cashless = cashless;
            SectionNumber = 1;
        }

        [DataMember]
        public bool Cashless { get; set; }

        public bool Cash
        {
            get
            {
                return !Cashless;
            }
        }

        [DataMember]
        public Guid DivisionId { get; set; }
        [DataMember]
        public Guid CustomerId { get; set; }
        [DataMember]
        public decimal RequestedAmount { get; set; }
        [DataMember]
        public decimal? RequestedBonusAmount { get; set; }
        [DataMember]
        public Guid? CertificateId { get; set; }
        [DataMember]
        public Guid? ProviderId { get; set; }

        public decimal RequestedAmountTotal
        {
            get
            {
                var res = RequestedAmount - (CertificateDicsount ?? 0);
                if (res < 0) return 0;
                return res;
            }
        }

        [DataMember]
        public DateTime PurchaseDate { get; set; }
        [DataMember]
        public int OrderNumber { get; set; }
        [DataMember]
        public decimal? BonusPayment { get; set; }

        decimal? disc = null;
        [DataMember]
        public decimal? CertificateDicsount
        {
            get
            {
                return disc;
            }
            set
            {
                if (disc != value)
                {
                    disc = value;
                    if (CashPayment > 0) CashPayment = RequestedAmountTotal;
                    if (CardPayment > 0) CardPayment = RequestedAmountTotal;
                    if (DepositPayment > 0) DepositPayment = RequestedAmountTotal;
                    OnPropertyChanged("CertificateDicsount");
                    OnPropertyChanged("RequestedAmountTotal");
                }
            }
        }

        decimal _CashPayment;
        [DataMember]
        public decimal CashPayment
        {
            get
            {
                return _CashPayment;
            }
            set
            {
                _CashPayment = value;
                OnPropertyChanged("CashPayment");
                OnPropertyChanged("Change");
                OnPropertyChanged("IsPaymentAllowed");
            }
        }

        decimal _DepositPayment;
        [DataMember]
        public decimal DepositPayment
        {
            get
            {
                return _DepositPayment;
            }
            set
            {
                _DepositPayment = value;
                OnPropertyChanged("DepositPayment");
                OnPropertyChanged("Change");
                OnPropertyChanged("IsPaymentAllowed");
            }
        }

        decimal _CardPayment;
        [DataMember]
        public decimal CardPayment
        {
            get
            {
                return _CardPayment;
            }
            set
            {
                _CardPayment = value;
                OnPropertyChanged("CardPayment");
                OnPropertyChanged("Change");
                OnPropertyChanged("IsPaymentAllowed");
            }
        }

        [DataMember]
        public bool Success { get; set; }
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Вспомогательная хрень, должна записываться на сервере исходя из токена
        /// </summary>
        public Guid UserId { get; set; }

        public decimal Change
        {
            get
            {
                if (CashPayment > 0)
                {
                    return CashPayment - RequestedAmountTotal;
                }
                else
                {
                    return 0;
                }
            }
        }


        public bool IsPaymentAllowed
        {
            get
            {

                return RequestedAmountTotal <= (decimal)CashPayment + (decimal)CardPayment + (decimal)DepositPayment;
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        [DataMember]
        public string CardNumber { get; set; }

        [DataMember]
        public string CardAuth { get; set; }

        [DataMember]
        public Guid Parameter { get; set; }

        [DataMember]
        public int SectionNumber { get; set; }
    }
}
