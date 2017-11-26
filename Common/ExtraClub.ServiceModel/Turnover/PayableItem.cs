using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace ExtraClub.ServiceModel
{
    [DataContract]
    public class PayableItem : INotifyPropertyChanged
    {

        [DataMember]
        public System.String Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }
        }
        private System.String _name;


        [DataMember]
        public System.String UnitName
        {
            get { return _unitName; }
            set
            {
                if (_unitName != value)
                {
                    _unitName = value;
                    OnPropertyChanged("UnitName");
                }
            }
        }
        private System.String _unitName;

        [DataMember]
        public decimal Price
        {
            get { return _price; }
            set
            {
                if (_price != value)
                {
                    _price = value;
                    OnPropertyChanged("Price");
                }
            }
        }
        private decimal _price;

        [DataMember]
        public int InBasket
        {
            get { return _inBasket; }
            set
            {
                if (_inBasket != value)
                {
                    _inBasket = value;
                    OnPropertyChanged("InBasket");
                }
            }
        }
        private int _inBasket;
  

        public decimal Cost
        {
            get
            {
                return Price * InBasket;
            }
        }

        public string InBasketText
        {
            get
            {
                return String.Format("{0} {1}", InBasket, UnitName);
            }
        }

        public bool Modified { get; set; }

        [OnDeserialized()]
        private void OnDeserializedMethod(StreamingContext context)
        {
            Modified = false;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            Modified = true;

            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
