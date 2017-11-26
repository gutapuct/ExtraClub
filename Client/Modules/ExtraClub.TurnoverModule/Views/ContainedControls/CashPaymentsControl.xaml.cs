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
using ExtraClub.TurnoverModule.ViewModels;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls.Windows;
using ExtraClub.UIControls;
using ExtraClub.UIControls.Interfaces;

namespace ExtraClub.TurnoverModule.Views.ContainedControls
{

    public partial class CashPaymentsControl
    {
        private TurnoverLargeViewModel Model
        {
            get
            {
                return (TurnoverLargeViewModel)DataContext;
            }
        }
        public CashPaymentsControl()
        {
            InitializeComponent();
        }


        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.CashPaymentsView.CurrentItem == null) return;
            var bo = Model.CashPaymentsView.CurrentItem as BarOrder;
            if (!bo.NeedClosure) return;

            ExtraWindow.Confirm(UIControls.Localization.Resources.PayReturn,
                String.Format(UIControls.Localization.Resources.PayReturnMessage, bo.Payment, bo.Payment * (1 - ClientContext.CurrentDivision.BankCardReturnComission)),
            e1=>
            {
                if (e1.DialogResult ?? false)
                {
                    ClientContext.ProcessBankReturn(bo.Id);
                    Model.RefreshCashPayments();
                }
            });
        }

        private void CashPaymentsGrid_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e))
            {
                if (Model.CashPaymentsView.CurrentItem == null) return;
                var bo = Model.CashPaymentsView.CurrentItem as BarOrder;
                NavigationManager.MakeClientRequest(bo.CustomerId);
            }
        }

        private void Excel_Click(object sender, RoutedEventArgs e)
        {
            ReportManager.ExportObjectToExcel(String.Format("Оплаты заказов за {0:dd.MM.yyyy} — {1:dd.MM.yyyy} по клубу {2}", Model.CashPaymentsStart, Model.CashPaymentsEnd, ClientContext.CurrentDivision.Name),
                Model._cashPayments,
                new ColumnInfo<BarOrder>("№", i => i.OrderNumber.ToString()),
                new ColumnInfo<BarOrder>("Дата", i => i.PurchaseDate.ToString("dd.MM.yyyy HH:mm")),
                new ColumnInfo<BarOrder>("Карта", i => i.SerializedCardBarcode),
                new ColumnInfo<BarOrder>("Клиент", i => i.SerializedCustomerName),
                new ColumnInfo<BarOrder>("Содержимое", i => i.ContentString),
                new ColumnInfo<BarOrder>("Сумма", i => i.Payment),
                new ColumnInfo<BarOrder>("Способ", i => i.PaymentType),
                new ColumnInfo<BarOrder>("Сотрудник", i => i.SerializedCreatedBy)
                );
        }
    }
}
