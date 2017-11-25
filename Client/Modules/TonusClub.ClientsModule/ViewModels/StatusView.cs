using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TonusClub.ServiceModel;
using System.ComponentModel;

namespace TonusClub.Clients.ViewModels
{
    public class StatusView : INotifyPropertyChanged
    {
        public bool IsChanged { get; set; }

        public StatusView(CustomerStatus stat, bool has)
        {
            _hasStatus = has;
            Status = stat;
        }


        public CustomerStatus Status { get; private set; }

        private bool _hasStatus;

        public bool HasStatus
        {
            get
            {
                return _hasStatus;
            }
            set
            {
                if (_hasStatus != value)
                {
                    _hasStatus = value;
                    IsChanged = true;
                    OnPropertyChanged("HasStatus");
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
