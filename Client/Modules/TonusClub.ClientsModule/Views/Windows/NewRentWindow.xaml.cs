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
using TonusClub.UIControls.Windows;
using Telerik.Windows.Controls;
using TonusClub.Infrastructure.Extensions;
using TonusClub.UIControls;

namespace TonusClub.Clients.Views.Windows
{
    /// <summary>
    /// Interaction logic for NewRentWindow.xaml
    /// </summary>
    public partial class NewRentWindow
    {
        CashRegisterManager _cashMan;

        public Customer Customer { get; set; }

        public Rent Rent { get; set; }
        public List<BarPointGood> Goods { get; set; }

        private BarPointGood _SelectedGood;
        public BarPointGood SelectedGood
        {
            get
            {
                return _SelectedGood;
            }
            set
            {
                _SelectedGood = value;
                Rent.Price = _SelectedGood.RentPrice.Value;
                Rent.GoodId = _SelectedGood.GoodId;
                Rent.StorehouseId = _SelectedGood.StorehouseId;
            }
        }

        public NewRentWindow(ClientContext context, CashRegisterManager cashMan, Customer customer)
            : base(context)
        {
            _cashMan = cashMan;
            InitializeComponent();
            Customer = customer;

            Rent = new Rent
            {
                AuthorId = context.CurrentUser.UserId,
                CompanyId = context.CurrentCompany.CompanyId,
                CreatedOn = DateTime.Now,
                CustomerId = customer.Id,
                Id = Guid.NewGuid(),
                ReturnDate = DateTime.Today.AddDays(3)
            };

            ReturnPicker.SelectableDateStart = DateTime.Today;

            Goods = context.GetGoodsPresence().Where(i => i.RentPrice.HasValue).OrderBy(i => i.Name).ToList();

            DataContext = this;
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (Rent.GoodId == Guid.Empty) return;
            _context.PostRent(Rent);

            var txt = String.Format(UIControls.Localization.Resources.RentDocument,
                Customer.FullName, Customer.ActiveCard != null ? Customer.ActiveCard.CardBarcode : UIControls.Localization.Resources.NoCard,
                _SelectedGood.Name, Rent.ReturnDate).SplitByLen(35).ToList();
            txt.Add(UIControls.Localization.Resources.Signature + ": _________________");

            _cashMan.PrintText(txt);

            if (PaymentBox.IsChecked ?? false)
            {
                _cashMan.ProcessPayment(new RentPayment(Rent, SelectedGood.Name), Customer, pd =>
                {
                    if (!pd.Success)
                    {
                        TonusWindow.Alert(UIControls.Localization.Resources.Error,
                            UIControls.Localization.Resources.UnableToProcessPmt
                        );
                    }
                    else
                    {
                        DialogResult = true;
                        Close();
                    }
                });
            }
            else
            {
                DialogResult = true;
                Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
