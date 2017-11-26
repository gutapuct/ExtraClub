using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using ExtraClub.CashRegisterModule;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls;
using ExtraClub.UIControls.Interfaces;
using ExtraClub.UIControls.Windows;

namespace ExtraClub.Clients.Views
{
    /// <summary>
    /// Interaction logic for UpgradeCustomerCardWindow.xaml
    /// </summary>
    public partial class UpgradeCustomerCardWindow : IDataErrorInfo
    {
        readonly CashRegisterManager _cashRegister;
        readonly IReportManager _repMan;

        #region DataContext
        public Customer Customer { get; set; }

        private string _newCardNumber;
        public string NewCardNumber
        {
            get
            {
                return _newCardNumber;
            }
            set
            {
                _newCardNumber = value;
                OnPropertyChanged("NewCardNumber");
                OnPropertyChanged("IsPostEnabled");
            }
        }

        private CustomerCardType _customerCardType;
        public CustomerCardType CustomerCardType
        {
            get
            {
                return _customerCardType;
            }
            set
            {
                _customerCardType = value;
                OnPropertyChanged("CustomerCardType");
                OnPropertyChanged("DiscountText");
                OnPropertyChanged("Cost");
                OnPropertyChanged("ChangePrice");
                OnPropertyChanged("BonusToAdd");
                OnPropertyChanged("IsPostEnabled");
            }
        }

        public decimal BonusToAdd
        {
            get
            {
                if (CustomerCardType == null) return 0;
                return CustomerCardType.Bonus - Customer.ActiveCard.SerializedCustomerCardType.Bonus;
            }

        }

        public ICollectionView CustomerCardTypes { get; set; }

        private decimal? _discountPercent;
        public decimal? DiscountPercent
        {
            get
            {
                return _discountPercent;
            }
            set
            {
                _discountPercent = value;
                OnPropertyChanged("DiscountPercent");
                OnPropertyChanged("Cost");
                OnPropertyChanged("IsPostEnabled");
            }
        }

        public decimal? ChangePrice
        {
            get
            {
                return CustomerCardType?.Price - Customer.ActiveCard.SerializedCustomerCardType.Price;
            }
        }

        public decimal? Cost
        {
            get
            {
                if (!_discountPercent.HasValue || CustomerCardType == null) return null;
                if (IsChange)
                {
                    return (CustomerCardType.Price - Customer.ActiveCard.SerializedCustomerCardType.Price) * (1 - DiscountPercent);
                }
                return CustomerCardType.LostPenalty * (1 - DiscountPercent);
            }
        }

        public Dictionary<decimal, string> Discounts { get; set; }

        private bool _isChange;

        public bool IsChange
        {
            get
            {
                return _isChange;
            }
            set
            {
                _isChange = value;
                OnPropertyChanged("IsChange");
                OnPropertyChanged("IsLost");
            }
        }

        public bool IsLost
        {
            get
            {
                return !_isChange;
            }
            set
            {
                _isChange = !value;
                OnPropertyChanged("IsChange");
                OnPropertyChanged("IsLost");
            }
        }


        public bool IsPostEnabled => String.IsNullOrEmpty(Error);

        #endregion


        public UpgradeCustomerCardWindow(ClientContext context, CashRegisterManager cashRegister, Customer customer, bool isLost, IReportManager repMan)
        {
            IEnumerable<CustomerCardType> cardTypes;
            InitializeComponent();
            Customer = customer;

            _isChange = !isLost;
            _cashRegister = cashRegister;
            _repMan = repMan;

            if (IsChange)
            {
                cardTypes = context.GetCustomerCardTypes(true).Where(t => !t.IsGuest && !t.IsVisit && t.Price >= customer.ActiveCard.SerializedCustomerCardType.Price && t.Id != customer.ActiveCard.SerializedCustomerCardType.Id).OrderBy(c => c.Name);
                //CustomerCardType = _cardTypes.FirstOrDefault();
            }
            else
            {
                cardTypes = new[] { customer.ActiveCard.SerializedCustomerCardType }.ToList();
                CustomerCardType = cardTypes.First();
            }
            CustomerCardTypes = CollectionViewSource.GetDefaultView(cardTypes);

            InitializeComponent();

            Discounts = context.GetDiscountsForCurrentUser(DiscountTypes.CardSale);
            DiscountPercent = 0;

            DataContext = this;
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ExtraWindow.Confirm(UIControls.Localization.Resources.Cancel,
                 UIControls.Localization.Resources.UpgradeCardCancelWarning,
                e1 =>
                {
                    if ((e1.DialogResult ?? false))
                    {
                        Close();
                    }
                });
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (CustomerCardType != null && !String.IsNullOrEmpty(NewCardNumber))
            {
                _cashRegister.ProcessPayment(new CustomerCardGood(CustomerCardType.Id, DiscountPercent ?? 0, Cost ?? 0, String.Empty, NewCardNumber), Customer, pd =>
                {
                    if (pd.Success)
                    {
                        if (PrintDogovorPDF.IsChecked ?? false)
                        {
                            _repMan.ProcessPdfReport(() => _context.GenerateCardUpgradeReport(NewCardNumber));
                        }

                        DialogResult = true;
                        Close();
                    }
                });
            }
        }

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
                    case "NewCardNumber":
                        if (String.IsNullOrEmpty(NewCardNumber)) return "!";
                        break;
                    case "CustomerCardType":
                        if (CustomerCardType == null) return "!";
                        break;
                    case "DiscountPercent":
                        if (!DiscountPercent.HasValue) return "!";
                        break;
                }
                return String.Empty;
            }
        }

        private void PrintLostPDF_Click(object sender, RoutedEventArgs e)
        {
            _repMan.ProcessPdfReport(() => _context.GenerateCardLostReport(Customer.ActiveCard.CardBarcode));
        }
    }
}
