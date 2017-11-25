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
using TonusClub.TurnoverModule.ViewModels;
using TonusClub.TurnoverModule.Views.Windows;

namespace TonusClub.TurnoverModule.Views.ContainedControls
{
    public partial class SalesPlanControl
    {
        private TurnoverLargeViewModel Model
        {
            get
            {
                return this.DataContext as TurnoverLargeViewModel;
            }
        }

        public SalesPlanControl()
        {
            InitializeComponent();
        }

        private void SalesPlanGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e))
            {
                if (!ClientContext.CheckPermission("SalesPlanMgmt")) return;
                EditButton_Click(null, null);
            }
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditSalesPlanWindow>(() => Model.RefreshSalesPlan());
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.SalesPlanView.CurrentItem == null) return;
            ProcessUserDialog<NewEditSalesPlanWindow>(() => Model.RefreshSalesPlan(), new ResolverOverride[] { new ParameterOverride("plan", Model.SalesPlanView.CurrentItem) });
        }
    }
}
