using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace ExtraClub.ServiceModel
{
    public class DictionaryPair : INotifyPropertyChanged, IComparable
    {
        public DictionaryPair()
        {
            Key = Guid.NewGuid();
        }

        public DictionaryPair(Guid key, string value)
        {
            this.Key = key;
            this.Value = value;

            Modified = false;
        }

        public Guid Key { get; set; }

        string _value;

        public string Value {
            get
            {
                return _value;
            }
            set
            {
                OnPropertyChanged("Value");
                _value = value;
            }
        }

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            Modified = false;
        }

        public bool Modified { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        //private Guid guid;
        //private string p;

        protected void OnPropertyChanged(string propertyName)
        {
            Modified = true;

            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (!(obj is DictionaryPair) || Value == null) return 0;
            return Value.CompareTo(((DictionaryPair)obj).Value);
        }

        #endregion
    }
}
