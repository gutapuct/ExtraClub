using System;
using System.Windows;
using Microsoft.Practices.Unity;
using TonusClub.CashRegisterModule;
using TonusClub.Clients.ViewModels;
using TonusClub.Infrastructure;
using TonusClub.ServiceModel;
using TonusClub.ServiceModel.Turnover;
using TonusClub.UIControls;
using TonusClub.UIControls.Windows;

namespace TonusClub.Clients.Views.ContainedControls
{
    /// <summary>
    /// Interaction logic for BarSalesControl.xaml
    /// </summary>
    public partial class BarSalesControl
    {
        private ClientLargeViewModel Model => DataContext as ClientLargeViewModel;

        public BarSalesControl()
        {
            InitializeComponent();
        }

        private void ReturnStatementClick(object sender, RoutedEventArgs e)
        {
            if (Model.SalesView.CurrentItem != null)
            {
                ReportManager.ProcessPdfReport(() => ClientContext.GenerateGoodReturnStatementReport(((GoodSale)Model.SalesView.CurrentItem).Id));
            }
        }

        private void ProcessReturnClick(object sender, RoutedEventArgs e)
        {
            if ((GoodSale)Model.SalesView.CurrentItem == null) return;
            var gs = (GoodSale)Model.SalesView.CurrentItem;
            if (gs.Cost <= 0) return;
            if (gs.IsReturned) return;
            var warn = "";
            if (gs.SerializedPaymentWay == UIControls.Localization.Resources.Deposit) warn = ClientContext.CurrentCompany.DepositWarning;
            else if (gs.SerializedPaymentWay == UIControls.Localization.Resources.Card) warn = ClientContext.CurrentCompany.CardWarning;
            else if (gs.SerializedPaymentWay == UIControls.Localization.Resources.Cash) warn = ClientContext.CurrentCompany.CashWarning;
            var msg = UIControls.Localization.Resources.GoodReturnInfoMessage;
            if (!String.IsNullOrWhiteSpace(warn))
            {
                msg += "\n" + warn;
            }
            TonusWindow.Confirm(UIControls.Localization.Resources.GoodReturn,
                 msg, w =>
            {
                if (w.DialogResult ?? false)
                {
                    var pmt = new PaymentDetails(Model.CurrentCustomer.Id, -gs.Cost, false);
                    pmt = ApplicationDispatcher.UnityContainer.Resolve<CashRegisterManager>().ProcessReturn(pmt, new[] { new GoodSaleReturnPosition{
                Name = String.Format(UIControls.Localization.Resources.GoodReturn+": {0}", gs.SerializedGoodName),
                Price = -gs.Cost,
                GoodSaleId = gs.Id
                } });
                    Model.RefreshSales();
                    if (pmt.Success)
                    {
                        ReportManager.ProcessPdfReport(() => ClientContext.GenerateRkoForGoodReturn(gs.Id));
                    }
                }
            });
        }

        private void MoneyGotClick(object sender, RoutedEventArgs e)
        {
            if ((GoodSale)Model.SalesView.CurrentItem == null) return;
            var gs = (GoodSale)Model.SalesView.CurrentItem;
            if (!gs.IsReturned) return;
            ReportManager.ProcessPdfReport(() => ClientContext.GenerateRkoForGoodReturn(gs.Id));
        }

        private void NewSaleClick(object sender, RoutedEventArgs e)
        {
            if (Model.CurrentCustomer != null)
            {
                NavigationManager.MakeBarRequest(Model.CurrentCustomer.Id);
            }
        }
    }
}
