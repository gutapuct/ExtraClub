using System;
using System.Windows;
using TonusClub.UIControls.Windows;

namespace TonusClub.TurnoverModule.Views.ContainedControls
{
    public partial class CashierBookControl
    {
        public CashierBookControl()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (!(DivPicker.SelectedValue is Guid))
            {
                TonusWindow.Alert("Ошибка", "Укажите клуб!");
                return;
            }
            ReportManager.ProcessPdfReport(()=>ClientContext.GenerateCashierPageReport((Guid)DivPicker.SelectedValue, GenPicker.SelectedDate ?? DateTime.Today));
        }
    }
}
