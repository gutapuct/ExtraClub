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
using TonusClub.SettingsModule.ViewModels;
using TonusClub.SettingsModule.Views.ContainedControls.Network.Windows;

namespace TonusClub.SettingsModule.Views.ContainedControls.Network
{
    public partial class FranchControl
    {
        private SettingsLargeViewModel Model
        {
            get
            {
                return (SettingsLargeViewModel)DataContext;
            }
        }

        public FranchControl()
        {
            InitializeComponent();
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewCompanyWindow>(() => Model.RefreshCompanies());
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void OrgCompanies_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e))
            {
                if (!ClientContext.CheckPermission("NetCompaniesMgmt")) return;
                EditButton_Click(null, null);
            }
        }
    }
}
