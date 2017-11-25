using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using Telerik.Windows.Controls;
using TonusClub.CashRegisterModule;
using TonusClub.ServiceModel;
using TonusClub.UIControls;
using TonusClub.UIControls.Interfaces;
using TonusClub.UIControls.Windows;

namespace TonusClub.Clients.Views.Windows.CustomerAndCards
{
    /// <summary>
    /// Interaction logic for NewCustomerCard.xaml
    /// </summary>
    public partial class NewCustomerCard : IDataErrorInfo
    {
        #region DataContext

        public Company Company { get; set; }
        public Division Division { get; set; }

        public bool IsPostEnabled => String.IsNullOrEmpty(Error);

        public bool IsReplacing { get; set; }

        public string AddrCity
        {
            get
            {
                return Customer.AddrCity;
            }
            set
            {
                Customer.AddrCity = value;
                OnPropertyChanged("IsPostEnabled");
            }
        }

        public string AddrStreet
        {
            get
            {
                return Customer.AddrStreet;
            }
            set
            {
                Customer.AddrStreet = value;
                OnPropertyChanged("IsPostEnabled");
            }
        }

        public string AddrOther
        {
            get
            {
                return Customer.AddrOther;
            }
            set
            {
                Customer.AddrOther = value;
                OnPropertyChanged("IsPostEnabled");
            }
        }

        public Customer Customer { get; set; }

        public string Comment { get; set; }

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
                int ncn;
                if (int.TryParse(_newCardNumber, out ncn))
                {
                    var c = _context.GetCustomerByCard(ncn, false);
                    IsReplacing = c != null && c.Id != Customer.Id;
                }
                OnPropertyChanged("NewCardNumber");
                OnPropertyChanged("IsPostEnabled");
                OnPropertyChanged("IsReplacing");
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
                OnPropertyChanged("IsPostEnabled");
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
            }
        }

        public decimal? Cost
        {
            get
            {
                if (!_discountPercent.HasValue || CustomerCardType == null) return null;
                return CustomerCardType.Price * (1 - DiscountPercent);
            }
        }

        public Dictionary<decimal, string> Discounts { get; set; }

        public ICollectionView Cities { get; set; }
        public ICollectionView Streets { get; set; }
        public ICollectionView Metros { get; set; }

        #endregion

        readonly CashRegisterManager _cashRegister;
        readonly IReportManager _repMan;

        public NewCustomerCard(ClientContext context, Customer customer, CashRegisterManager cashRegister, IReportManager repMan)
        {
            Customer = customer;

            _cashRegister = cashRegister;
            _repMan = repMan;

            var cardTypes = context.GetCustomerCardTypes(true);
            CustomerCardTypes = CollectionViewSource.GetDefaultView(cardTypes);
            CustomerCardType = cardTypes.FirstOrDefault();

            InitializeComponent();

            Discounts = context.GetDiscountsForCurrentUser(DiscountTypes.CardSale);
            DiscountPercent = 0;

            var tmp = _context.GetAddressLists();

            Metros = CollectionViewSource.GetDefaultView(tmp[1]);
            Streets = CollectionViewSource.GetDefaultView(tmp[2]);
            Cities = CollectionViewSource.GetDefaultView(tmp[0]);

            Company = _context.CurrentCompany;
            Division = _context.CurrentDivision;

            DataContext = this;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            TonusWindow.Confirm(UIControls.Localization.Resources.Cancel,
                UIControls.Localization.Resources.CardSaleCancelMessage,
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
            _context.PostCustomerAddress(Customer);
            if (CustomerCardType != null && !String.IsNullOrEmpty(NewCardNumber))
            {
                _cashRegister.ProcessPayment(new CustomerCardGood(CustomerCardType.Id, DiscountPercent ?? 0, Cost ?? 0, Comment, NewCardNumber), Customer, pd =>
                {
                    if (pd.Success)
                    {
                        if (PrintPDF.IsChecked ?? false)
                        {
                            _repMan.ProcessPdfReport(()=>_context.GenerateCardContractReport(NewCardNumber));
                        }
                        DialogResult = true;
                        Close();
                    }
                });
            }
        }

        private void CustomerSearch_SelectedClientChanged(object sender, GuidEventArgs e)
        {
            var res = _context.UpdateInvitor(Customer.Id, e.Guid);
            if (!String.IsNullOrEmpty(res))
            {
                TonusWindow.Alert(new DialogParameters
                {
                    Header = UIControls.Localization.Resources.Error,
                    Content = res,
                    OkButtonContent = UIControls.Localization.Resources.Ok,
                    Owner = this
                });
                return;
            }
            Customer.InvitorId = e.Guid;
            OnPropertyChanged("IsPostEnabled");
        }

        public string Error
        {
            get
            {
                if (this["CustomerCardType"] != null || this["NewCardNumber"] != null) return "!";
#if BEAUTINIKA
#else
                if (CustomerCardType.IsGuest && !Customer.InvitorId.HasValue) return "!";
#endif

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
                    case "CustomerCardType":
                        if (CustomerCardType == null) return "!";
                        break;
                    case "NewCardNumber":
                        if (String.IsNullOrEmpty(NewCardNumber)) return "!";
                        break;
                    case "AddrOther":
                        if (String.IsNullOrEmpty(AddrOther)) return "!";
                        break;
                    case "AddrStreet":
                        if (String.IsNullOrEmpty(AddrStreet)) return "!";
                        break;
                    case "AddrCity":
                        if (String.IsNullOrEmpty(AddrCity)) return "!";
                        break;
                }
                return null;
            }
        }
    }
}
