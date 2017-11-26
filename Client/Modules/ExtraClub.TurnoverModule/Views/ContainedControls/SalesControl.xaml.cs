using System.Linq;
using System.Windows;
using System.Windows.Input;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls;
using ExtraClub.UIControls.Interfaces;

namespace ExtraClub.TurnoverModule.Views.ContainedControls
{
    public partial class SalesControl
    {
        public SalesControl()
        {
            InitializeComponent();
        }

        private void ExportExcel(object sender, RoutedEventArgs e)
        {
            ReportManager.ExportObjectToExcel($"Продажи из бара по клубу {ClientContext.CurrentDivision.Name}",
            GoodSalesGrid.Items.Cast<GoodSale>().ToList(),
            new ColumnInfo<GoodSale>("Заказ", i => i.SerializedOrderNumber.ToString()),
            new ColumnInfo<GoodSale>("Дата", i => i.SerializedOrderDate.ToString("d")),
            new ColumnInfo<GoodSale>("Наименование", i => i.SerializedGoodName),
            new ColumnInfo<GoodSale>("Количество", i => i.Amount),
            new ColumnInfo<GoodSale>("Ед.изм.", i => i.SerializedUnitType),
            new ColumnInfo<GoodSale>("Склад", i => i.SerializedStorehouseName),
            new ColumnInfo<GoodSale>("Цена", i => (i.PriceMoney ?? 0)),
            new ColumnInfo<GoodSale>("Стоимость", i => i.Cost),
            new ColumnInfo<GoodSale>("Способ оплаты", i => i.PaymentType),
            new ColumnInfo<GoodSale>("Покупатель", i => i.SerializedCustomer),
            new ColumnInfo<GoodSale>("Тип покупателя", i => i.SerializedCustomerType),
            new ColumnInfo<GoodSale>("Карта", i => i.SerializedCustomerCard),
            new ColumnInfo<GoodSale>("Кассир", i => i.SerializedCreatedBy),
            new ColumnInfo<GoodSale>("Был ли возрат", i => i.IsReturned ? "Да" : "Нет")
            );
        }

        private void GoodSalesGrid_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e))
            {
                var item = GoodSalesGrid.SelectedItem as GoodSale;
                if (item != null)
                {
                    NavigationManager.MakeClientRequest(item.SerializedCustomerId);
                }
            }
        }
    }
}
