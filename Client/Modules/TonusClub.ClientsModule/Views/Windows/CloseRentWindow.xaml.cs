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
using TonusClub.Infrastructure.BaseClasses;
using TonusClub.ServiceModel.Turnover;
using TonusClub.UIControls.BaseClasses;

namespace TonusClub.Clients.Views.Windows
{
    /// <summary>
    /// Interaction logic for CloseRentWindow.xaml
    /// </summary>
    public partial class CloseRentWindow
    {
        CashRegisterManager _cashMan;

        public Rent Rent { get; set; }
        public Customer Customer { get; set; }
        public GoodPrice Price { get; set; }

        bool isLost = false;
        public bool IsLost
        {
            get
            {
                return isLost;
            }
            set
            {
                isLost = value;
                LostFineBox.IsEnabled = isLost;
                Rent.LostFine = isLost ? (Price != null ? Price.CommonPrice : 0) : (decimal?)null;
            }
        }

        public CloseRentWindow(CashRegisterManager cashMan, Customer customer, Rent rent)
        {
            _cashMan = cashMan;

            Rent = ViewModelBase.Clone<Rent>(rent);
            Customer = customer;
            Price = _context.GetGoodPrices().Single(i => i.GoodId == rent.GoodId);
            InitializeComponent();
            DataContext = this;

            if (Price != null)
            {
                if (Rent.ReturnDate.Date < DateTime.Today)
                {
                    Rent.OverdueFine = ((int)(DateTime.Today - Rent.ReturnDate.Date).TotalDays) * Price.RentFine ?? 0;
                }
                else
                {
                    RentFineBox.IsEnabled = false;
                }
            }
            Rent.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Rent_PropertyChanged);
        }

        void Rent_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LostFine" || e.PropertyName == "OverdueFine")
            {
                Rent.IsManualAmount = (Price != null && Rent.LostFine != Price.CommonPrice) || Rent.OverdueFine != (((int)(DateTime.Today - Rent.ReturnDate.Date).TotalDays) * Price.RentFine ?? 0);
            }
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            _cashMan.ProcessPayment(new CloseRentPayment(Rent), Customer, pd => {
                if (pd.Success)
                {
                    DialogResult = true;
                    Close();
                }
            });
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
