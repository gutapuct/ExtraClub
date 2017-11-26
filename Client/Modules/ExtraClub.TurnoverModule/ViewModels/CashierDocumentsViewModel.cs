using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using ExtraClub.Infrastructure.BaseClasses;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls.BaseClasses;

namespace ExtraClub.TurnoverModule.ViewModels
{
    public class CashierDocumentsViewModel : ViewModelBase
    {
        private List<Division> _Divisions = new List<Division>();
        public ICollectionView Divisions { get; set; }

        private List<CashInOrder> _CashInOrders = new List<CashInOrder>();
        public ICollectionView CashInOrders { get; set; }

        private DateTime cioStart = DateTime.Today.AddMonths(-1);
        public DateTime CioStart
        {
            get
            {
                return cioStart;
            }
            set
            {
                cioStart = value;
                if (cioStart > CioEnd)
                {
                    cioStart = CioEnd;
                }
                OnPropertyChanged("CioStart");
                RefreshCashInOrders();
            }
        }

        private DateTime cioEnd = DateTime.Today.AddDays(1);
        public DateTime CioEnd
        {
            get
            {
                return cioEnd;
            }
            set
            {
                cioEnd = value;
                if (CioStart > cioEnd)
                {
                    cioEnd = CioStart;
                }
                OnPropertyChanged("CioEnd");
                RefreshCashInOrders();
            }
        }

        public decimal Amount { get; set; }

        private List<CashOutOrder> _CashOutOrders = new List<CashOutOrder>();
        public ICollectionView CashOutOrders { get; set; }

        private DateTime cooStart = DateTime.Today.AddMonths(-1);
        public DateTime CooStart
        {
            get
            {
                return cooStart;
            }
            set
            {
                cooStart = value;
                if (CooStart > CooEnd)
                {
                    cooStart = CooEnd;
                }
                OnPropertyChanged("CooStart");
                RefreshCashOutOrders();
            }
        }

        private DateTime cooEnd = DateTime.Today.AddDays(1);
        public DateTime CooEnd
        {
            get
            {
                return cooEnd;
            }
            set
            {
                cooEnd = value;
                if (CooStart > CooEnd)
                {
                    cooEnd = CooStart;
                }
                OnPropertyChanged("CooEnd");
                RefreshCashOutOrders();
            }
        }

        public CashierDocumentsViewModel(IUnityContainer container)
            : base()
        {
            CashInOrders = CollectionViewSource.GetDefaultView(_CashInOrders);
            CashOutOrders = CollectionViewSource.GetDefaultView(_CashOutOrders);
            Divisions = CollectionViewSource.GetDefaultView(_Divisions);
        }

        protected override void RefreshDataInternal()
        {
            _Divisions.Clear();
            _Divisions.AddRange(ClientContext.GetDivisions());
            _CashInOrders.Clear();
            _CashInOrders.AddRange(ClientContext.GetCashInOrders(cioStart, cioEnd));
            _CashOutOrders.Clear();
            _CashOutOrders.AddRange(ClientContext.GetCashOutOrders(cooStart, cooEnd));

            Amount = ClientContext.GetCashTodaysAmount();
            OnPropertyChanged("Amount");
        }

        protected override void RefreshFinished()
        {
            base.RefreshFinished();
            CashInOrders.Refresh();
            CashOutOrders.Refresh();
            Divisions.Refresh();
        }

        public void RefreshCashInOrders()
        {
            _CashInOrders.Clear();
            _CashInOrders.AddRange(ClientContext.GetCashInOrders(cioStart, cioEnd));
            CashInOrders.Refresh();
            Amount = ClientContext.GetCashTodaysAmount();
            OnPropertyChanged("Amount");
        }

        public void RefreshCashOutOrders()
        {
            _CashOutOrders.Clear();
            _CashOutOrders.AddRange(ClientContext.GetCashOutOrders(cooStart, cooEnd));
            CashOutOrders.Refresh();
            Amount = ClientContext.GetCashTodaysAmount();
            OnPropertyChanged("Amount");
        }

    }
}
