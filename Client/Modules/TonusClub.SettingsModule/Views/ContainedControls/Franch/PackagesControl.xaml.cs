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
    public partial class PackagesControl
    {
        private SettingsLargeViewModel Model
        {
            get
            {
                return (SettingsLargeViewModel)DataContext;
            }
        }

        public PackagesControl()
        {
            InitializeComponent();
        }

        private void Grid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e))
            {
                EditButton_Click(null, null);
            }
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewEditPackageWindow>(() => Model.RefreshPackages());
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.PackagesView.CurrentItem != null)
            {
                ProcessUserDialog<NewEditPackageWindow>(() => Model.RefreshPackages(), new ParameterOverride("package", Model.PackagesView.CurrentItem));
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if(Model.PackagesView.CurrentItem != null)
            {
                ClientContext.SetObjectActive("Packages", ((Package)Model.PackagesView.CurrentItem).Id, false);
                Model.RefreshPackages();
            }
        }
    }
}
