using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ExtraClub.TurnoverModule.ViewModels;
using Microsoft.Practices.Unity;
using ExtraClub.TurnoverModule.Views.Windows;
using ExtraClub.UIControls.Windows;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls.BaseClasses;
using ExtraClub.UIControls.Interfaces;

namespace ExtraClub.TurnoverModule.Views.ContainedControls
{
    public partial class SpendingsControl
    {
        private TurnoverLargeViewModel Model => (TurnoverLargeViewModel)DataContext;

        public SpendingsControl()
        {
            InitializeComponent();
        }

        private void AddSpendingClick(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewSpendingWindow>(() => Model.RefreshSpendings());
        }

        private void EditSpendingClick(object sender, RoutedEventArgs e)
        {
            if (Model.SpendingsView.CurrentItem == null) return;
            ProcessUserDialog<NewSpendingWindow>(() => Model.RefreshSpendings(), new ParameterOverride("spending", Model.SpendingsView.CurrentItem));
        }

        private void DeleteSpendingClick(object sender, RoutedEventArgs e)
        {
            if (Model.SpendingsView.CurrentItem == null) return;
            ExtraWindow.Confirm(UIControls.Localization.Resources.Deletion,
                 UIControls.Localization.Resources.DeleteSpendingMessage,
                    e1 =>
                    {
                        if ((e1.DialogResult ?? false))
                        {
                            ClientContext.DeleteObject("Spendings", ((Spending)Model.SpendingsView.CurrentItem).Id);
                            Model.RefreshSpendings();
                        }
                    });
        }

        private void SpendingsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e) && ClientContext.CheckPermission("EditSpendingButton"))
            {
                EditSpendingClick(null, null);
            }
        }

        private void ExportExcel(object sender, RoutedEventArgs e)
        {
            ReportManager.ExportObjectToExcel(
                $"Расходы за период {Model.SpendingsStart:dd.MM.yyyy}—{Model.SpendingsEnd:dd.MM.yyyy}",
            SpendingsGrid.Items.Cast<Spending>().ToList(),
            new ColumnInfo<Spending>("Номер", i => i.Number.ToString()),
            new ColumnInfo<Spending>("Дата", i => i.CreatedOn.ToString("dd.MM.yyyy")),
            new ColumnInfo<Spending>("Клуб", i => i.DivisionName),
            new ColumnInfo<Spending>("Название", i => i.Name),
            new ColumnInfo<Spending>("Категория", i => i.SerializedSpendingTypeName),
            new ColumnInfo<Spending>("Сумма", i => i.Amount),
            new ColumnInfo<Spending>("Тип оплаты", i => i.PaymentType),
            new ColumnInfo<Spending>("Сотрудник", i => i.SerializedCreatedBy)
            );
        }

        private void AddSpendingByCopyClick(object sender, RoutedEventArgs e)
        {
            if (Model.SpendingsView.CurrentItem == null) return;

            var item = ViewModelBase.Clone<Spending>(Model.SpendingsView.CurrentItem);
            item.Id = Guid.NewGuid();
            item.CreatedOn = DateTime.Now;
            item.Number = -1;
            item.Amount = 0;
            ProcessUserDialog<NewSpendingWindow>(() => Model.RefreshSpendings(), new ParameterOverride("spending", item));
        }

    }
}
