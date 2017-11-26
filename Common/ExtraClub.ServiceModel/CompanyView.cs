using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace ExtraClub.ServiceModel
{
    [DataContract]
    [Serializable]
    public class CompanyView : INotifyPropertyChanged
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        bool _helper;
        [DataMember]
        public bool Helper
        {
            get
            {
                return _helper;
            }
            set
            {
                _helper = value;
                OnPropertyChanged("Helper");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }
}
