using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TonusClub.CashRegisterModule;
using TonusClub.Infrastructure.Interfaces;
using TonusClub.ServiceModel;
using TonusClub.ServiceModel.Turnover;

namespace TonusClub.Clients.Views.Windows
{
    /// <summary>
    /// Interaction logic for AddDepositPayment.xaml
    /// </summary>
    public partial class AddDepositPayment
    {
        public Customer Customer { get; set; }

        private decimal _Amount;
        public decimal Amount
        {
            get
            {
                return _Amount;
            }
            set
            {
                _Amount = value;
                OnPropertyChanged("Amount");
                OnPropertyChanged("IsPostEnabled");
                OnPropertyChanged("DiscountBarText");
            }
        }

        public string DiscountBarText
        {
            get
            {
                var discount =
                    _discounts.Where(i => i.ValueFrom <= Amount && i.ValueTo >= Amount).Max(i => (decimal?)i.DiscountPercent) ?? 0;
                return String.Format("При пополнении на данную сумму в баре возможна скидка {0:p}", discount/100);
            }
        }

        private bool _IsBank;
        public bool IsBank
        {
            get
            {
                return _IsBank;
            }
            set
            {
                _IsBank = value;
                OnPropertyChanged("IsBank");
                OnPropertyChanged("IsPostEnabled");
            }
        }


        private string _Description;
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                _Description = value;
                OnPropertyChanged("Description");
                OnPropertyChanged("IsPostEnabled");
            }
        }

        public bool IsPostEnabled
        {
            get
            {
                return Amount > 0 && ((!_IsBank) || (IsBank && !String.IsNullOrWhiteSpace(Description)));
            }
        }

        CashRegisterManager _cashMan;

        private List<BarDiscount> _discounts;

        public AddDepositPayment(Customer customer, CashRegisterManager cashMan)
        {
            _cashMan = cashMan;
            _discounts = _context.GetBarDiscounts();
            InitializeComponent();

            Customer = customer;

            DataContext = this;
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsBank)
            {
                _context.PostDepositAdd(Customer.Id, Amount, Description);
                DialogResult = true;
                Close();
            }
            else
            {
                _cashMan.ProcessPayment(new DepositGood(Amount), Customer, pd =>
                {
                    if (pd.Success)
                    {
                        DialogResult = true;
                        Close();
                    }
                }, false);
            }

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
