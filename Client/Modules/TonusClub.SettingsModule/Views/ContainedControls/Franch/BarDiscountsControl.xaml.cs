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
using Microsoft.Practices.Unity;
using TonusClub.ServiceModel;
using TonusClub.SettingsModule.ViewModels;
using TonusClub.SettingsModule.Views.ContainedControls.Franch.Windows;

namespace TonusClub.SettingsModule.Views.ContainedControls.Franch
{
    public partial class BarDiscountsControl
    {
        private SettingsLargeViewModel Model
        {
            get
            {
                return (SettingsLargeViewModel)DataContext;
            }
        }

        public BarDiscountsControl()
        {
            InitializeComponent();
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditBarDiscountWindow>(() => Model.RefreshBarDiscounts());
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.BarDiscountsView.CurrentItem == null) return;
            ProcessUserDialog<NewEditBarDiscountWindow>(() => Model.RefreshBarDiscounts(), new ParameterOverride("discount", Model.BarDiscountsView.CurrentItem));
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.BarDiscountsView.CurrentItem == null) return;
            ClientContext.DeleteObject("BarDiscounts", ((BarDiscount)Model.BarDiscountsView.CurrentItem).Id);
            Model.RefreshBarDiscounts();
        }

        private void Grid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e))
            {
                EditButton_Click(null, null);
            }
        }
    }
}
