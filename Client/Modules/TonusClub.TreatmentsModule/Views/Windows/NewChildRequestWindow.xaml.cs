using System;
using System.Windows;
using TonusClub.CashRegisterModule;
using TonusClub.UIControls;
using TonusClub.ServiceModel;
using TonusClub.ServiceModel.Turnover;
using TonusClub.UIControls.Interfaces;

namespace TonusClub.TreatmentsModule.Views.Windows
{
    /// <summary>
    /// Interaction logic for NewChildRequestWindow.xaml
    /// </summary>
    public partial class NewChildRequestWindow
    {
        private Customer _customer;
        public Customer Customer
        {
            get
            {
                return _customer;
            }
            set
            {
                _customer = value;
                Cost = Customer.ActiveCard.SerializedCustomerCardType.ChildrenCost;
                OnPropertyChanged("Customer");
            }
        }


        public bool IsProcessEnabled => Customer != null && !String.IsNullOrEmpty(_childName) && !String.IsNullOrEmpty(_healthStatus);


        private bool _printPdf = true;
        // ReSharper disable once InconsistentNaming
        public bool PrintPDF
        {
            get
            {
                return _printPdf;
            }
            set
            {
                _printPdf = value;
                OnPropertyChanged("PrintPDF");
            }
        }


        private decimal _cost;
        public decimal Cost
        {
            get
            {
                return _cost;
            }
            set
            {
                _cost = value;
                OnPropertyChanged("Cost");
            }
        }


        private string _healthStatus;
        public string HealthStatus
        {
            get
            {
                return _healthStatus;
            }
            set
            {
                _healthStatus = value;
                OnPropertyChanged("HealthStatus");
                OnPropertyChanged("IsProcessEnabled");
            }
        }


        private string _childName;
        public string ChildName
        {
            get
            {
                return _childName;
            }
            set
            {
                _childName = value;
                OnPropertyChanged("ChildName");
                OnPropertyChanged("IsProcessEnabled");
            }
        }

        private readonly IReportManager _repMan;
        private readonly CashRegisterManager _cashMan;

        public NewChildRequestWindow(Guid customerId, IReportManager repMan, CashRegisterManager cashMan)
        {
            InitializeComponent();
            _repMan = repMan;
            _cashMan = cashMan;
            DataContext = this;
            if(customerId!=Guid.Empty)
            {
                CustomerSearch.IsEnabled = false;
                Customer = _context.GetCustomer(customerId);
            }
        }

        private void RadButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }



        private void CommitButtonClick(object sender, RoutedEventArgs e)
        {
            _cashMan.ProcessPayment(new ChildrenRoomGood(ChildName, HealthStatus, Cost), Customer, res =>
            {
                if (res.Success)
                {
                    if (PrintPDF)
                    {
                        _repMan.ProcessPdfReport(() => _context.GenerateChildRequestReport(res.Parameter));
                    }
                    DialogResult = true;
                    Close();
                }
            });
            
        }

        private void CustomerSearch_SelectedClientChanged(object sender, GuidEventArgs e)
        {
            Customer = _context.GetCustomer(e.Guid);
        }
    }
}
