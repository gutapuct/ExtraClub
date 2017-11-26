using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ExtraClub.ServiceModel
{
    partial class CustomerCardType : IDataErrorInfo
    {
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
                    case "Name":
                        if (String.IsNullOrWhiteSpace(Name)) return "!";
                        break;
                }
                return null;
            }
        }

        public double _Price
        {
            get
            {
                return (double)Price;
            }
            set
            {
                Price = (decimal)value;
                OnPropertyChanged("_Price");
            }
        }

        public double _LostPenalty
        {
            get
            {
                return (double)LostPenalty;
            }
            set
            {
                LostPenalty = (decimal)value;
                OnPropertyChanged("_LostPenalty");
            }
        }
        public double _Bonus
        {
            get
            {
                return (double)Bonus;
            }
            set
            {
                Bonus = (decimal)value;
                OnPropertyChanged("_Bonus");
            }
        }
        public double _BonusPercent
        {
            get
            {
                return (double)BonusPercent;
            }
            set
            {
                BonusPercent = (decimal)value;
                OnPropertyChanged("_BonusPercent");
            }
        }
        public double _DiscountBar
        {
            get
            {
                return (double)DiscountBar;
            }
            set
            {
                DiscountBar = (decimal)value;
                OnPropertyChanged("_DiscountBar");
            }
        }
        public double _ChildrenCost
        {
            get
            {
                return (double)ChildrenCost;
            }
            set
            {
                ChildrenCost = (decimal)value;
                OnPropertyChanged("_ChildrenCost");
            }
        }

        [DataMember]
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

        [XmlIgnore]
        public string TextDescription
        {
            get
            {
                var res = new StringBuilder();
                res.Append(Name);
                if (IsGuest) res.Append(" (гостевая)");
                if (IsVisit) res.Append(" (обзорная)");
                if (!String.IsNullOrWhiteSpace(Description))
                {
                    res.Append("\n");
                    res.Append(Description);
                }
                if (Price > 0)
                {
                    res.AppendFormat("\nЦена: {0:c}", Price);
                }
                if (Bonus > 0)
                {
                    res.AppendFormat("\nБонусы за покупку: {0:n0}", Bonus);
                }
                if (LostPenalty > 0)
                {
                    res.AppendFormat("\nШтраф за утерю: {0:c}", LostPenalty);
                }
                if (BonusPercent > 0)
                {
                    res.AppendFormat("\nБонусный процент за пополнение депозита: {0:n2}%", BonusPercent);
                }
                if (FreezePriceCoeff != 1)
                {
                    res.AppendFormat("\nКоэффициент стоимости заморозки К2: {0:n2}", FreezePriceCoeff);
                }
                return res.ToString();
            }
        }
    }
}
