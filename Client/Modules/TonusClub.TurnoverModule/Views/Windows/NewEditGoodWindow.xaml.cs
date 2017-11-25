using System;
using System.Windows;
using System.ComponentModel;
using TonusClub.Infrastructure.Interfaces;
using TonusClub.ServiceModel;
using TonusClub.UIControls;

namespace TonusClub.TurnoverModule.Views.Windows
{
    /// <summary>
    /// Interaction logic for NewEditGoodWindow.xaml
    /// </summary>
    public partial class NewEditGoodWindow
    {
        public Good Good { get; set; }

        public ICollectionView UnitTypes { get; private set; }
        public ICollectionView Manufacturers { get; private set; }
        public ICollectionView GoodCategories { get; private set; }
        public ICollectionView ProductTypes { get; private set; }
        public ICollectionView Users { get; private set; }
        public ICollectionView Divisions { get; private set; }

        public NewEditGoodWindow(IDictionaryManager dictManager, ClientContext context, Good good)
        {
            DataContext = this;
            UnitTypes = dictManager.GetViewSource("UnitTypes");
            GoodCategories = dictManager.GetViewSource("GoodsCategories");
            ProductTypes = dictManager.GetViewSource("ProductTypes");
            Manufacturers = dictManager.GetViewSource("Manufacturers");
            Users = dictManager.GetViewSource("Users");
            Divisions = dictManager.GetViewSource("Divisions");

            if (good.Id == Guid.Empty || good == null)
            {
                Good = new Good { Id = Guid.NewGuid(), IsVisible = true };
            }
            else
            {
                Good = good;
            }


            InitializeComponent();
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            _context.PostGood(Good);
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
