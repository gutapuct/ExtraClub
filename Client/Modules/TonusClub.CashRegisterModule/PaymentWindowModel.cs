using System;
using System.Collections.Generic;
using TonusClub.ServiceModel;
using TonusClub.Infrastructure.Interfaces;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using TonusClub.UIControls;

namespace TonusClub.CashRegisterModule
{
    public class PaymentWindowModel : INotifyPropertyChanged
    {
        public readonly ClientContext Context;

        public bool IsEnoughDeposit { get; set; }

        public bool PayWithFR { get; set; }

        public Dictionary<int, string> Sections { get; set; }

        public Visibility SectionSelectionVisible { get; set; }


        private string _cardNumber = String.Empty;
        public string CardNumber
        {
            get
            {
                return _cardNumber;
            }
            set
            {
                if (_cardNumber != value)
                {
                    _cardNumber = value;
                    int id;
                    if (Int32.TryParse(_cardNumber, out id))
                    {
                        var cert = Context.GetCertificateByNumber(id);
                        if (cert != null)
                        {
                            PaymentDetails.CertificateId = cert.Id;
                            PaymentDetails.CertificateDicsount = cert.Amount;
                            return;
                        }
                    }
                    _cardNumber = string.Empty;
                    PaymentDetails.CertificateDicsount = null;
                    PaymentDetails.CertificateId = null;
                    OnPropertyChanged("CardNumber");
                }
            }
        }

        public IEnumerable GoodsList { get; private set; }

        public List<Provider> Providers { get; set; }

        public PaymentWindowModel(ClientContext context, IDictionaryManager dictManager, PaymentDetails details, IEnumerable goodsList, Customer customer = null)
        {
            if (details.Cashless)
            {
                Providers = context.GetAllProviders();
            }
            PayWithFR = true;
            PaymentDetails = details;
            Context = context;
            if (customer != null) Customer = customer;
            else Customer = Context.GetCustomer(PaymentDetails.CustomerId);
            IsEnoughDeposit = Customer.RurDepositValue >= PaymentDetails.RequestedAmount;
            PaymentDetails.OrderNumber = Context.GetMaxPaymentNumber() + 1;
            this.GoodsList = goodsList;
        }

        //private PaymentDetails _details;

        public Customer Customer { get; set; }

        public PaymentDetails PaymentDetails { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
