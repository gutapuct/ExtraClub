using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TonusClub.ServiceModel;
using System.Windows.Media;
using System.ComponentModel;
using TonusClub.ServiceModel.Schedule;

namespace NewtonicServer.Server
{
    public class ClientInfo : INotifyPropertyChanged
    {
        public bool IsOnline
        {
            get
            {
                return HardwareId != Guid.Empty;
            }
        }

        public Brush BackgroundColor
        {
            get
            {
                if (!IsOnline) return Brushes.Salmon;
                if (CurrentPlan != null) return Brushes.LightGreen;
                return Brushes.Gold;
            }
        }

        public string DisplayName
        {
            get
            {
                if (Treatment.MaxCustomers > 1) return Treatment.DisplayName + "(" + Treatment.MacAddress + ")";
                return Treatment.DisplayName;
            }
        }

        private Guid _hardwareId = Guid.Empty;
        public Guid HardwareId
        {
            get
            {
                return _hardwareId;
            }
            set
            {
                _hardwareId = value;
                OnPropertyChanged("HardwareId");
                OnPropertyChanged("IsOnline");
                OnPropertyChanged("BackgroundColor");
                OnPropertyChanged("StateText");

            }
        }

        public string StateText
        {
            get
            {
                if (!IsOnline) return "Не подключен";
                if (CurrentPlan != null)
                {
                    return String.Format("Занятие до {0}, клиент {1}", CurrentPlan.VisitDate.AddMinutes(CurrentPlan.SerializedDuration), CurrentPlan.SerializedCustomerInfo);
                }
                else
                {
                    return "Свободен";
                }
            }
        }

        public Treatment Treatment { get; set; }

        private TreatmentEvent _currentPlan;
        public TreatmentEvent CurrentPlan { get
        {
            return _currentPlan;
        }
            set
            {
                _currentPlan = value;
                OnPropertyChanged("CurrentPlan");
                OnPropertyChanged("StateText");
                OnPropertyChanged("BackgroundColor");
            }
        }

        private Customer _customer;
        public Customer PendingCustomer
        {
            get
            {
                return DateTime.Now > (Timeout ?? DateTime.MinValue) ? null : _customer;
            }
            set
            {
                _customer = value;
            }
        }

        private TreatmentEvent _event;
        public TreatmentEvent PendingEvent
        {
            get
            {
                return DateTime.Now > (Timeout ?? DateTime.MinValue) ? null : _event;
            }
            set
            {
                _event = value;
            }
        }


        private HWProposal _pendingProposal;
        public HWProposal PendingProposal
        {
            get
            {
                return DateTime.Now > (Timeout ?? DateTime.MinValue) ? null : _pendingProposal; ;
            }
        }

        private DateTime? Timeout { get; set; }

        public ClientInfo(Treatment treatment)
        {
            Treatment = treatment;
        }

        public override string ToString()
        {
            if (Treatment != null) return Treatment.DisplayName;
            if (Treatment.MacAddress != null) return Treatment.MacAddress;
            return "Неизвестно - " + HardwareId.ToString();
            
        }

        internal void SetPendingConfirmation(Customer customer, TreatmentEvent plan, int timeout)
        {
            _pendingProposal = null;
            Timeout = DateTime.Now.AddSeconds(timeout);
            PendingCustomer = customer;
            _event = plan;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        internal void SetPendingProposalConfirmation(HWProposal proposal, Customer customer, int timeout)
        {
            _event = null;
            Timeout = DateTime.Now.AddSeconds(timeout);
            PendingCustomer = customer;
            _pendingProposal = proposal;
        }

        internal void ClearPendingProposal()
        {
            _pendingProposal = null;
        }

        internal BlockCardInfo BlockCardInfo { get; set; }
    }
}
