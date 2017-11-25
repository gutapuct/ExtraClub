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
using TonusClub.ServiceModel;
using TonusClub.SettingsModule.ViewModels;
using TonusClub.UIControls.Windows;

namespace TonusClub.SettingsModule.Views.ContainedControls.Franch
{
    public partial class CustomerStatusesControl
    {
        private SettingsLargeViewModel Model
        {
            get
            {
                return (SettingsLargeViewModel)DataContext;
            }
        }

        public CustomerStatusesControl()
        {
            InitializeComponent();
        }


        private void AddElementButton_Click(object sender, RoutedEventArgs e)
        {
            TonusWindow.Prompt("Новый статус", "Введите название статуса", "", res =>
                {
                    if (!String.IsNullOrWhiteSpace(res.TextResult))
                    {
                        ClientContext.PostCustomerStatus(Guid.NewGuid(), res.TextResult.Trim());
                        Model.RefreshCustomerStatuses();
                    }
                });
        }

        private void EditElementButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.FrCustomerStatusesView.CurrentItem == null) return;

            TonusWindow.Prompt("Редактирование статуса", "Введите название статуса", ((CustomerStatus)Model.FrCustomerStatusesView.CurrentItem).Name, res =>
            {
                if (!String.IsNullOrWhiteSpace(res.TextResult))
                {
                    ClientContext.PostCustomerStatus(((CustomerStatus)Model.FrCustomerStatusesView.CurrentItem).Id, res.TextResult.Trim());
                    Model.RefreshCustomerStatuses();
                }
            });
        }

        private void RemoveElementButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.FrCustomerStatusesView.CurrentItem == null) return;
            TonusWindow.Confirm("Удаление статуса", "Удалить выделенный статус?", w =>
            {
                if (w.DialogResult ?? false)
                {
                    ClientContext.PostCustomerStatusDelete(((CustomerStatus)Model.FrCustomerStatusesView.CurrentItem).Id);
                    Model.RefreshCustomerStatuses();
                }
            });
        }

        private void Grid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditElementButton_Click(null, null);
        }
    }
}
