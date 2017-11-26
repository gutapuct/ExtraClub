using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using ExtraClub.CashRegisterModule;
using ExtraClub.Clients.ViewModels;
using ExtraClub.Clients.Views.Windows;
using ExtraClub.Infrastructure;
using ExtraClub.ServiceModel;

namespace ExtraClub.Clients.Views.ContainedControls
{
    public partial class GoodsReserve
    {
        private ClientLargeViewModel Model
        {
            get
            {
                return DataContext as ClientLargeViewModel;
            }
        }

        public GoodsReserve()
        {
            InitializeComponent();
        }

        private void RadGridView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e))
            {
                GoodOutClick(null, null);
            }
        }

        private void GoodOutClick(object sender, RoutedEventArgs e)
        {
            if (Model.GoodsReserveView.CurrentItem != null)
            {
                ProcessUserDialog<GoodOutWindow>(w =>
                    {
                        if (w.DialogResult ?? false)
                        {
                            var item = Model.GoodsReserveView.CurrentItem as GoodReserve;
                            if (ClientContext.GiveGoodToCustomer(Model.CurrentCustomer.Id, item.GoodId))
                            {
                                var ls = new List<string>();
                                ls.Add(UIControls.Localization.Resources.Visitor);
                                ls.Add(Model.CurrentCustomer.FullName);
                                if (Model.CurrentCustomer.ActiveCard != null)
                                {
                                    ls.Add(String.Format("{1}: {0}", Model.CurrentCustomer.ActiveCard.CardBarcode, UIControls.Localization.Resources.CardNumber));
                                }
                                ls.Add("-----------------------------------");
                                ls.Add("Выдача товара " + item.GoodName);
                                ls.Add("Товар получил:_____________________");

                                if (w.PrintFR.IsChecked ?? false)
                                {
                                    ApplicationDispatcher.UnityContainer.Resolve<CashRegisterManager>().PrintText(ls);
                                }
                                if (w.PrintPDF.IsChecked ?? false)
                                {
                                    ReportManager.PrintTextToPdf(ls);
                                }

                            }

                            Model.RefreshGoodsReserve();
                        }
                    });
            }
        }
    }
}
