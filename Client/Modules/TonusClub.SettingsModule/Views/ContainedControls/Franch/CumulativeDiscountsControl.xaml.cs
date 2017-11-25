using Microsoft.Practices.Unity;
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
using TonusClub.SettingsModule.Views.ContainedControls.Franch.Windows;

namespace TonusClub.SettingsModule.Views.ContainedControls.Franch
{
    public partial class CumulativeDiscountsControl
    {
        private SettingsLargeViewModel Model
        {
            get
            {
                return (SettingsLargeViewModel)DataContext;
            }
        }

        public CumulativeDiscountsControl()
        {
            InitializeComponent();
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditCumulativeWindow>(() => Model.RefreshCumulativeDiscounts());
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.CumulativesView.CurrentItem == null) return;
            ProcessUserDialog<NewEditCumulativeWindow>(() => Model.RefreshCumulativeDiscounts(), new ParameterOverride("cumulative", Model.CumulativesView.CurrentItem));
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.CumulativesView.CurrentItem == null) return;
            ClientContext.DeleteObject("CumulativeDiscounts", ((CumulativeDiscount)Model.CumulativesView.CurrentItem).Id);
            Model.RefreshCumulativeDiscounts();
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
