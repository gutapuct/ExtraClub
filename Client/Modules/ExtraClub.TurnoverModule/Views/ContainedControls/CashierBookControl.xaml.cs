using System;
using System.Windows;
using ExtraClub.UIControls.Windows;

namespace ExtraClub.TurnoverModule.Views.ContainedControls
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
                ExtraWindow.Alert("Ошибка", "Укажите клуб!");
                return;
            }
            ReportManager.ProcessPdfReport(()=>ClientContext.GenerateCashierPageReport((Guid)DivPicker.SelectedValue, GenPicker.SelectedDate ?? DateTime.Today));
        }
    }
}
