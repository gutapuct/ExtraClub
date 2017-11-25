using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TonusClub.ServiceModel
{
    partial class GoodSale : IInitable
    {
        [DataMember]
        public string SerializedGoodName { get; set; }

        [DataMember]
        public int SerializedOrderNumber { get; set; }

        [DataMember]
        public DateTime SerializedOrderDate { get; set; }

        [DataMember]
        public string SerializedUnitType { get; set; }

        [DataMember]
        public string SerializedStorehouseName { get; set; }

        [DataMember]
        public string SerializedCustomer { get; set; }

        [DataMember]
        public string SerializedCustomerType { get; set; }

        [DataMember]
        public string SerializedCustomerCard { get; set; }

        [DataMember]
        public string SerializedCreatedBy { get; set; }

        [DataMember]
        public string SerializedPaymentWay { get; set; }

        [DataMember]
        public Guid SerializedCustomerId { get; set; }

        public bool IsReturned
        {
            get
            {
                return ReturnById.HasValue;
            }
        }

        public string IsReturnedText
        {
            get
            {
                return ReturnById.HasValue ? "Да" : "";
            }
        }

        public void Init()
        {
            SerializedGoodName = Good.Name;
            SerializedOrderNumber = BarOrder.OrderNumber;
            SerializedOrderDate = BarOrder.PurchaseDate;
            if (Good.UnitTypeId.HasValue)
            {
                SerializedUnitType = Good.UnitType.Name;
            }
            SerializedStorehouseName = Storehouse.Name;
            SerializedCustomer = BarOrder.Customer.FullName;
            SerializedCustomerId = BarOrder.Customer.Id;
            BarOrder.Customer.InitActiveCard();
            if (BarOrder.Customer.ActiveCard != null)
            {
                SerializedCustomerCard = BarOrder.Customer.ActiveCard.CardBarcode;
            }
            SerializedCustomerType = BarOrder.Customer.Employees.Any() ? "Сотрудник" : "Клиент";
            SerializedCreatedBy = BarOrder.CreatedBy.FullName;
            InitPaymentWay();
        }

        public void InitPaymentWay()
        {
            if (BarOrder.BonusPayment.HasValue)
            {
                SerializedPaymentWay = "Бонусы";
            }
            else if (BarOrder.DepositPayment > 0)
            {
                SerializedPaymentWay = "Депозит";
            }
            else if (BarOrder.CardPayment > 0)
            {
                SerializedPaymentWay = "Карта";
            }
            else
            {
                SerializedPaymentWay = "Наличные";
            }
        }

        public decimal Cost
        {
            get
            {
                return (decimal)Amount * (PriceMoney ?? 0);
            }
        }

        public string PaymentType
        {
            get
            {
                if (PriceMoney.HasValue) return "Деньги";
                return "Бонусы";
            }
        }

    }
}
