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
using TonusClub.UIControls.Windows;
using System.ServiceModel;
using TonusClub.ServiceModel;
using Microsoft.Practices.Unity;
using TonusClub.UIControls;

namespace TonusClub.TurnoverModule.Views.ContainedControls.Windows
{
    public partial class MarkdownWindow
    {
        Guid GoodId;
        Guid StoreId;

        public string GoodName { get; set; }
        public decimal Price { get; set; }
        public int Amount { get; set; }

        public string NewName { get; set; }
        public decimal NewPrice { get; set; }
        public int NewAmount { get; set; }

        public Guid? ProviderId { get; set; }

        public IUnityContainer UnityContainer { get; set; }

        public MarkdownWindow(IUnityContainer container, ClientContext context, GoodPrice price)
        {
            UnityContainer = container;
            Price = NewPrice = price.CommonPrice;
            GoodId = price.GoodId;
            GoodName = price.SerializedGoodName;
            NewName = context.GetMarkdownName(GoodId);
            var stores = context.GetStorehouses();
            var amount = context.GetGoodsPresence().FirstOrDefault(i => i.GoodId == GoodId && stores.Any(j => j.Id == i.StorehouseId && j.BarSale));
            if (amount != null)
            {
                Amount = (int)amount.Amount;
                StoreId = amount.StorehouseId;
            }

            InitializeComponent();
            DataContext = this;
        }

        private void AssetButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(NewName))
            {
                TonusWindow.Alert(UIControls.Localization.Resources.Error,
                    UIControls.Localization.Resources.NewPriceNeeded);
                return;
            }
            if (NewPrice <= 0)
            {
                TonusWindow.Alert(UIControls.Localization.Resources.Error,
                    UIControls.Localization.Resources.PriceNonNegative);
                return;
            }
            if (NewAmount <= 0 || NewAmount > Amount)
            {
                TonusWindow.Alert(UIControls.Localization.Resources.Error,
                    UIControls.Localization.Resources.AmountWarning);
                return;
            }
            if (!ProviderId.HasValue)
            {
                TonusWindow.Alert(UIControls.Localization.Resources.Error,
                    UIControls.Localization.Resources.ProviderNeeded);
                return;
            }
            try
            {
                _context.PostMarkdown(StoreId, GoodId, NewName.Trim(), NewPrice, NewAmount, ProviderId.Value);

                DialogResult = true;
                Close();
            }
            catch(FaultException ex)
            {
                TonusWindow.Alert(UIControls.Localization.Resources.Error, ex.Message);
            }
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
