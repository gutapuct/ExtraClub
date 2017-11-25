using System;
using System.Collections.Generic;
using System.Windows;
using TonusClub.UIControls;
using TonusClub.Infrastructure.Interfaces;
using TonusClub.ServiceModel;

namespace TonusClub.TurnoverModule.Views
{
    public partial class NewEditPriceWindow : WindowBase
    {
        public bool IsAddingNew { get; set; }

        #region DataContext

        public GoodPrice GoodPrice { get; set; }

        public List<Good> Goods { get; private set; }

        public double CommonPrice
        {
            get
            {
                return (double)GoodPrice.CommonPrice;
            }
            set
            {
                GoodPrice.CommonPrice = (decimal)value;
            }
        }

        public double? EmployeePrice
        {
            get
            {
                return (double?)GoodPrice.EmployeePrice;
            }
            set
            {
                GoodPrice.EmployeePrice = (decimal?)value;
            }
        }

        public double? BonusPrice
        {
            get
            {
                return (double?)GoodPrice.BonusPrice;
            }
            set
            {
                GoodPrice.BonusPrice = (decimal?)value;
            }
        }

        public double? RentPrice
        {
            get
            {
                return (double?)GoodPrice.RentPrice;
            }
            set
            {
                GoodPrice.RentPrice = (decimal?)value;
            }
        }

        public double? RentFine
        {
            get
            {
                return (double?)GoodPrice.RentFine;
            }
            set
            {
                GoodPrice.RentFine = (decimal?)value;
            }
        }
        #endregion

        public NewEditPriceWindow(ClientContext context, GoodPrice goodPrice, IDictionaryManager dictMan, bool isAddingNew = true)
            : base(context)
        {
            IsAddingNew = isAddingNew;
            if (isAddingNew)
            {
                GoodPrice = new GoodPrice { DivisionId = context.CurrentDivision.Id, InPricelist = true };
            }
            else
            {
                GoodPrice = goodPrice;
            }
#if BEAUTINIKA
            Goods = _context.GetAllGoods()/*.Where(i => !i.MaterialTypeId.HasValue)*/.ToList();
#else
            Goods = _context.GetAllGoods();
#endif


            this.DataContext = this;
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (GoodPrice.DivisionId == Guid.Empty) return;
            if (GoodPrice.GoodId == Guid.Empty) return;

            if (GoodPrice.EmployeePrice == 0) GoodPrice.EmployeePrice = null;
            if (GoodPrice.BonusPrice == 0) GoodPrice.BonusPrice = null;
            if (GoodPrice.RentPrice == 0) GoodPrice.RentPrice = null;
            if (GoodPrice.RentFine == 0) GoodPrice.RentFine = null;

            _context.PostGoodPrice(GoodPrice);

            DialogResult = true;
            Close();
        }
    }
}
