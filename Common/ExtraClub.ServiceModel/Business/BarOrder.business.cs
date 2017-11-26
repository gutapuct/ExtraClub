using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using ExtraClub.ServiceModel.Turnover;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace ExtraClub.ServiceModel
{
    partial class BarOrder : IInitable
    {
        private static XmlSerializer x;

        public IEnumerable<PayableItem> GetContent()
        {
            if (Content == null) return new PayableItem[0];
            if (x == null)
            {
                x = new XmlSerializer(typeof(PayableItem[]), new Type[] { typeof(BarPointGood), typeof(ChildrenRoomGood), typeof(CloseRentPayment), typeof(CustomerCardGood), typeof(GoodSaleReturnPosition), typeof(RentPayment), typeof(SolariumGood), typeof(TicketChangeGood), typeof(TicketFreezeGood), typeof(TicketGood), typeof(TicketPaymentPosition), typeof(TicketRebillGood), typeof(TicketReturnPosition), typeof(DepositGood) });
            }
            MemoryStream ms = new MemoryStream(Content);
            var res = x.Deserialize(ms);
            return res as IEnumerable<PayableItem>;
        }

        [DataMember]
        public string SerializedCardBarcode { get; set; }

        [DataMember]
        public string SerializedCustomerName { get; set; }

        [DataMember]
        public string SerializedProviderName { get; set; }

        [DataMember]
        public string SerializedCreatedBy { get; set; }

        [DataMember]
        public bool NeedClosure { get; set; }

        public decimal Payment
        {
            get
            {
                return CashPayment + DepositPayment + CardPayment;
            }
        }

        public string PaymentType
        {
            get
            {
                if (ProviderId.HasValue)
                {
                    if (PaymentDate.HasValue) return "Безнал (" + PaymentDate.Value.ToString("d") + ")";
                    else return "Безнал (не оплачен)";
                }
                if (DepositPayment > 0) return "Депозит";
                if (CardPayment > 0)
                {
                    if (PaymentDate.HasValue) return "Банк. карта (" + PaymentDate.Value.ToString("d") + ")";
                    else return "Банк. карта (нет возврата)";
                }
                return "Наличные";
            }
        }

        public void Init()
        {
            Customer.InitActiveCard();
            SerializedCustomerName = Customer.FullName;
            if (Customer.ActiveCard != null)
            {
                SerializedCardBarcode = Customer.ActiveCard.CardBarcode;
            }
            SerializedCreatedBy = CreatedBy.FullName;
            if (ProviderId.HasValue)
            {
                SerializedProviderName = this.Provider.FullName;
            }
            NeedClosure = CardPayment > 0 && !PaymentDate.HasValue;
        }

        private string _contentString = null;
        public string ContentString
        {
            get
            {
                if (_contentString == null)
                {
                    var res = new StringBuilder();
                    foreach (var i in GetContent())
                    {
                        if (res.Length != 0)
                        {
                            res.Append("; ");
                        }
                        res.Append(i.Name);
                    }
                    _contentString = res.ToString();
                }
                return _contentString;
            }
        }

        public short PmtTypeId
        {
            get
            {
                if (CashPayment > 0) return 0;
                if (DepositPayment > 0) return 1;
                if (CardPayment > 0) return 2;
                if (BonusPayment > 0) return 4;
                if (ProviderId.HasValue) return 3;
                return 0;
            }
        }

        public int TotalPos
        {
            get
            {
                return GetContent().Count();
            }
        }
    }
}
