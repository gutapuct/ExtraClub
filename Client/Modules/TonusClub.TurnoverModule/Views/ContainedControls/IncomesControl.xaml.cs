using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TonusClub.TurnoverModule.ViewModels;
using Microsoft.Practices.Unity;
using TonusClub.TurnoverModule.Views.ContainedControls.Windows;
using TonusClub.UIControls.Windows;
using TonusClub.ServiceModel;
using TonusClub.UIControls.BaseClasses;
using TonusClub.UIControls.Interfaces;

namespace TonusClub.TurnoverModule.Views.ContainedControls
{
    public partial class IncomesControl
    {
        private TurnoverLargeViewModel Model => (TurnoverLargeViewModel)DataContext;

        public IncomesControl()
        {
            InitializeComponent();
        }

        private void AddIncomeClick(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewIncomeWindow>(() => Model.RefreshIncomes());
        }

        private void EditIncomeClick(object sender, RoutedEventArgs e)
        {
            if (Model.IncomesView.CurrentItem == null) return;
            ProcessUserDialog<NewIncomeWindow>(() => Model.RefreshIncomes(), new ParameterOverride("income", Model.IncomesView.CurrentItem));
        }

        private void RemoveIncomeClick(object sender, RoutedEventArgs e)
        {
            if (Model.IncomesView.CurrentItem == null) return;
            TonusWindow.Confirm(UIControls.Localization.Resources.Deletion,
                 UIControls.Localization.Resources.DeleteLineMessage,
                e1 =>
                {
                    if ((e1.DialogResult ?? false))
                    {
                        ClientContext.DeleteObject("Incomes", ((Income)Model.IncomesView.CurrentItem).Id);
                        Model.RefreshIncomes();
                    }
                });
        }

        private void IncomesGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e) && ClientContext.CheckPermission("EditIncomeButton"))
            {
                EditIncomeClick(null, null);
            }
        }

        
        private void ExportExcel(object sender, RoutedEventArgs e)
        {
            ReportManager.ExportObjectToExcel(
                $"Доходы за период {Model.IncomesStart:dd.MM.yyyy}—{Model.IncomesEnd:dd.MM.yyyy}",
            IncomesGrid.Items.Cast<Income>().ToList(),
            new ColumnInfo<Income>("Номер", i => i.Number.ToString()),
            new ColumnInfo<Income>("Дата", i => i.CreatedOn.ToString("dd.MM.yyyy")),
            new ColumnInfo<Income>("Клуб", i => i.DivisionName),
            new ColumnInfo<Income>("Название", i => i.Name),
            new ColumnInfo<Income>("Категория", i => i.SerializedIncomeTypeName),
            new ColumnInfo<Income>("Сумма", i => i.Amount),
            new ColumnInfo<Income>("Тип оплаты", i => i.PaymentType),
            new ColumnInfo<Income>("Сотрудник", i => i.SerializedCreatedBy)
            );
        }

        private void AddIncomeByCopyClick(object sender, RoutedEventArgs e)
        {
            if (Model.IncomesView.CurrentItem == null) return;
            var item = ViewModelBase.Clone<Income>(Model.IncomesView.CurrentItem);
            item.Id = Guid.NewGuid();
            item.CreatedOn = DateTime.Now;
            item.Number = -1;
            item.Amount = 0;
            ProcessUserDialog<NewIncomeWindow>(() => Model.RefreshIncomes(), new ParameterOverride("income", item));
        }

    }
}
