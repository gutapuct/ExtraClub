using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;
using TonusClub.ServiceModel;

namespace TonusClub.ServiceModel
{
    [DataContract]
    public class BarPointGood : PayableItem, IComparable
    {
        [DataMember]
        private string _Category;
        public string Category
        {
            get
            {
                return _Category;
            }
            set
            {
                _Category = value;
                OnPropertyChanged("Category");
            }
        }

        [DataMember]
        private Guid _StorehouseId;
        public Guid StorehouseId
        {
            get
            {
                return _StorehouseId;
            }
            set
            {
                _StorehouseId = value;
                OnPropertyChanged("StorehouseId");
            }
        }

        [DataMember]
        private string _StorehouseName;
        public string StorehouseName
        {
            get
            {
                return _StorehouseName;
            }
            set
            {
                _StorehouseName = value;
                OnPropertyChanged("StorehouseName");
            }
        }

        [DataMember]
        private decimal? _RentPrice;
        public decimal? RentPrice
        {
            get
            {
                return _RentPrice;
            }
            set
            {
                _RentPrice = value;
                OnPropertyChanged("RentPrice");
            }
        }

        [DataMember]
        private decimal? _RentFine;
        public decimal? RentFine
        {
            get
            {
                return _RentFine;
            }
            set
            {
                _RentFine = value;
                OnPropertyChanged("RentFine");
            }
        }

        [DataMember]
        public virtual System.Double Amount
        {
            get { return _amount; }
            set
            {
                if (_amount != value)
                {
                    _amount = value;
                    OnPropertyChanged("Amount");
                }
            }
        }
        private System.Double _amount;
        
        
        [DataMember]
        public virtual System.Guid GoodId
        {
            get { return _goodId; }
            set
            {
                if (_goodId != value)
                {
                    _goodId = value;
                    OnPropertyChanged("GoodId");
                }
            }
        }
        private System.Guid _goodId;

       
        private bool _deleted;

        [DataMember]
        public bool Deleted
        {
            get { return _deleted; }
            set
            {
                if (value != _deleted) Modified = true;
                _deleted = value;
            }
        }
        
        //public virtual System.String BackColor
        //{
        //    get { return _backColor; }
        //    set
        //    {
        //        if (_backColor != value)
        //        {
        //            _backColor = value;
        //            OnPropertyChanged("BackColor");
        //        }
        //    }
        //}
        //private System.String _backColor;

        [DataMember]
        public bool IsInPricelist { get; set; }


        public virtual System.String ClearVisible
        {
            get { return _clearVisible; }
            set
            {
                if (_clearVisible != value)
                {
                    _clearVisible = value;
                    OnPropertyChanged("ClearVisible");
                }
            }
        }
        private System.String _clearVisible;

       
        [DataMember]
        public virtual decimal EmployeePrice
        {
            get { return _employeePrice; }
            set
            {
                if (_employeePrice != value)
                {
                    _employeePrice = value;
                    OnPropertyChanged("EmployeePrice");
                }
            }
        }
        private decimal _employeePrice;



        [DataMember]
        public virtual System.Double? BonusPrice
        {
            get { return _bonusPrice; }
            set
            {
                if (_bonusPrice != value)
                {
                    _bonusPrice = value;
                    OnPropertyChanged("BonusPrice");
                }
            }
        }
        private System.Double? _bonusPrice;

        public string HasBonusPrice
        {
            get
            {
                return BonusPrice.HasValue ? "Visible" : "Collapsed";
            }
        }

        public int CompareTo(object obj)
        {
            if (!(obj is BarPointGood)) return 0;
            if (Name == null) return 0;
            return Name.CompareTo(((BarPointGood)obj).Name);
        }
    }
}
