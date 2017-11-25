using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using TonusClub.ServiceModel;

namespace TonusClub.Clients.ViewModels
{
    public class ContraView : INotifyPropertyChanged
    {

        public bool IsChanged { get; set; }

        public ContraView(ContraIndication ind, bool has)
        {
            _hasContra = has;
            Indication = ind;
        }


        public ContraIndication Indication { get; private set; }

        private bool _hasContra;

        public bool HasContra
        {
            get
            {
                return _hasContra;
            }
            set
            {
                if (_hasContra != value)
                {
                    _hasContra = value;
                    IsChanged = true;
                    OnPropertyChanged("HasContra");
                }
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
