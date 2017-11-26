using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace ExtraClub.ServiceModel
{
    partial class Certificate : IInitable, IDataErrorInfo
    {
        [DataMember]
        public string SerializedBuyerName { get; private set; }
        [DataMember]
        public DateTime? SerializedUseDate { get; private set; }
        [DataMember]
        public string SerializedCategoryName { get; private set; }
        

        public string _PriceMoney
        {
            get
            {
                if (PriceMoney == null) return "";
                return PriceMoney.Value.ToString("n2");
            }
            set
            {
                decimal mon;
                if (Decimal.TryParse(value.Replace(" ", ""), out mon))
                {
                    PriceMoney = mon;
                }
                else
                {
                    PriceMoney = null;
                }
                OnPropertyChanged("_PriceMoney");
                OnPropertyChanged("_PriceBonus");
            }
        }


        public string _Amount
        {
            get
            {
                return Amount.ToString("n2");
            }
            set
            {
                decimal mon;
                if (Decimal.TryParse(value.Replace(" ", ""), out mon))
                {
                    Amount = mon;
                }
                else
                {
                    Amount = 0;
                }
                OnPropertyChanged("_Amount");
            }
        }

        public string _PriceBonus
        {
            get
            {
                if (PriceBonus == null) return "";
                return PriceBonus.Value.ToString("n0");
            }
            set
            {
                int bon;
                if (Int32.TryParse(value.Replace(" ", ""), out bon))
                {
                    PriceBonus = bon;
                }
                else
                {
                    PriceBonus = null;
                }
                OnPropertyChanged("_PriceMoney");
                OnPropertyChanged("_PriceBonus");
            }
        }


        public void Init()
        {
            if (BuyerId.HasValue)
            {
                SerializedBuyerName = Customer.FullName + " " + SellDate.Value.ToString("d");
            }
            if (UsedOrderId.HasValue)
            {
                SerializedUseDate = UsedInOrder.PurchaseDate;
            }
            if (CategoryId.HasValue)
            {
                SerializedCategoryName = GoodsCategory.Name;
            }
        }

        public string Error
        {
            get
            {
                StringBuilder error = new StringBuilder();

                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(this);
                foreach (PropertyDescriptor prop in props)
                {
                    string propertyError = this[prop.Name];
                    if (!String.IsNullOrEmpty(propertyError))
                    {
                        error.Append((error.Length != 0 ? ", " : "") + propertyError);
                    }
                }

                return error.ToString();
            }
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "Amount":
                        if (Amount == 0) return "!";
                        break;
                    case "PriceBonus":
                        if ((PriceBonus ?? 0) == 0 && (PriceMoney ?? 0) == 0) return "!";
                        break;
                    case "PriceMoney":
                        if ((PriceBonus ?? 0) == 0 && (PriceMoney ?? 0) == 0) return "!";
                        break;
                    case "_PriceBonus":
                        if ((PriceBonus ?? 0) == 0 && (PriceMoney ?? 0) == 0) return "!";
                        break;
                    case "_PriceMoney":
                        if ((PriceBonus ?? 0) == 0 && (PriceMoney ?? 0) == 0) return "!";
                        break;
                    case "BarCode":
                        if (String.IsNullOrEmpty(BarCode)) return "!";
                        break;
                }
                return null;
            }
        }
    }
}
